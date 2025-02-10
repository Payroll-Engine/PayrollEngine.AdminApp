# Payroll Engine Admin Application
👉 This application is part of the [Payroll Engine](https://github.com/Payroll-Engine/PayrollEngine/wiki).

Die Admin Application dient zur Installation und Betreibung der Payroll Engine. Es untersützt die volle lokale Installation (OnPremises) sowie die Konfiguration von Remote Andwendungen (Client-Setup).

## Features
Mit dieser Anwendung werden folgende Arbeiten unterstützt:
- Administration des Backends mit der Datenbank und dem API Web-Server
- Administration vom Web App Web-Server
- Registrierung von Dateitypen
- Öffnen der Tests und Beispiele

The functions of the admin app are divided into assets:

| Asset            | Required asset            | Setup | Client Setup |
|:--|:--|:--:|:--:|
| Web App          | Backend                   |   ✔️  |              |
| Backend          |                           |   ✔️  |              |
| Backend Remote   |                           |       |    ✔️        |
| Console          | Backend or Remote Backend |   ✔️  |    ✔️        |
| Tests            | Console                   |   ✔️  |    ✔️        |
| Examples         | Console                   |   ✔️  |    ✔️        |

## Anwendung
### Lokale Installation
Folgende Schritte sind zur lokalen Nutzung der Payroll Engine notwendig:
1. Datenbank-Server zur Verfügung stellen (lokale [Installation](https://www.microsoft.com/en-us/download/details.aspx?id=104781) oder in der Cloud) (Kommando `Add`)
2. Datenbankverbindung hinzufügen und Datenbank installieren (Kommando `Add` und `Create`)
3. Backend API Serververbindung hinzufügen (Kommando `Add`)
4. Backend API Server starten (Kommando `Start`)
5. Backend API testen (Kommando `API` > Swagger)
6. [Registrierung](#filetype-registration) der Payroll Console Dateitypen
7. Ausführen der Tests (Kommandodatei `Tests/Test.All.pecmd`)
8. Web App Serververbindung hinzufügen (Kommando `Add`)
9. Web App Server starten (Kommando `Start`)
10. Web App Login (Kommando `Login`)



### Remote Installation
1. Backend API Serververbindung hinzufügen (Kommando `Add`)
2. Backend API testen (Kommando `API` > Swagger)
3. [Registrierung](#filetype-registration) der Payroll Console Dateitypen
4. Ausführen der Tests (Kommandodatei `Tests/Test.All.pecmd`)

### Filetype Registration
Zur Automatisierung der Payroll Engine muss für die Payroll Console der Dateityp `.pecmd` registriert werden.
Im Startordner der Installation befinden sich folgende Kommandodateien, welche als **Administrator** gestartet werden müssen:
- `FileType.Reg.cmd` - Registrierung hinzufügen
- `FileType.Unreg.cmd` - Registrierung entfernen

> Diese Registrierung funktioniert nur für Windows. In anderen Betriebssystem muss dieser Schritt manuell ausgeführt werden.

## Data Storage
Alle Einstellungsdaten werden in den User-Umgebungsvariablen des Betriebsystems gespeichert:

| Variable                    | Description                            | Required  | Used by            |
|:--|:--|:--:|:--|
| `PayrollDatabaseConnection` | The database connection string         | yes       | Backend            |
| `PayrollApiConnection`      | The backend connection string          | yes       | Web App, Console   |
| `PayrollWebAppConnection`   | The web app connection string          | no        | Admin App <sup>1)</sup> |
| `PayrollApiKey`             | The payroll API key                    | no        | Backend            |

<sup>1)</sup> Used to start the web app server within the admin application.<br/>

Die Einstellung `PayrollApiKey` erzwingt einen Schlüssel beim Zugriff auf die Backend API. Dieser Wert sollte gesetzt werden, sobald der Zugriff auf den API-Server eingeschränkt werden soll. SObald der API-Schlüssel definiert ist, könnnen nur Applikationen die API verwenden, welche diesen Schlüssel kennen.

## Application Configuration
The application configuration file `appsetings.json` contains the following settings:

| Setting                 | Description                                   | Type     | Default                        |
|:--|:--|:--|:--|
| `Culture`               | The user interface culture                    | string   | System culture                 |
| `DarkMode`              | Dark application theme                        | bool     | Operating system setting       |
| `AppUrl`                | The application url                           | string   | payrollengine.org              |
| `FileAssetsPath`        | Path to file assets                           | string   | Working path                   |
| `DatabaseCollation`     | Default database character treating           | string   | `SQL_Latin1_General_CP1_CS_AS` |
| `DatabaseConnectTimeout`| Timeout for database requests (seconds)       | int      | 5                              |
| `HttpConnectTimeout`    | Timeout for http requests (seconds)           | int      | 5                              |
| `AutoRefreshTimeout`    | Application auto refresh timeout (seconds)    | int      | 120 (Off=0, Min=5, Max=3600)   |



## Asset Konfiguration
Die Einstellungen eines wird in der Konfiguration `asset.json` bestimmt. Die folgenden Asset-Konfigurationen bestehen.

### Backend Asset Konfiguration
Die Konfiguration vom Backend Asset beinhaltet Informationen zu Datenbank und Webserver.

Das folgende Beispiel zeigt die Konfiguration der (fiktiven) Version 1.1.0 welche die Updatefähigkeit bis zur Version 0.9.0 unterstützt:
```json
{
  "WebserverName": "Payroll Engine - Backend Server",
  "WebserverExec": "PayrollEngine.Backend.Server.dll",
  "Database": {
    "MinVersion": "0.9.0",
    "CurrentVersion": "0.9.0",
    "InitScripts": [
      "Database/Model_090.sql"
    ]
  }
}
```

In der Asset-Konfiguration wird zwischen `InitScripts` und `UpdateScripts` unterschieden.
Das folgende Beispiel zeigt die Konfiguration der (fiktiven) Version 1.1.0 welche die Updatefähigkeit bis zur Version 0.9.0 unterstützt:
```json
{
  "WebserverName": "Payroll Engine - Backend Server",
  "WebserverExec": "PayrollEngine.Backend.Server.dll",
  "Database": {
    "MinVersion": "0.9.0",
    "CurrentVersion": "1.1.0",
    "InitScripts": [
      "Database/Model_110.sql"
    ],
    "UpdateScripts": [
      {
        "FromVersion": "0.9.0",
        "ToVersion": "1.0.0",
        "Scripts": [
          "Database/ModelUpdate_090-100.sql"
        ]
      },
      {
        "FromVersion": "1.0.0",
        "ToVersion": "1.1.0",
        "Scripts": [
          "Database/ModelUpdate_100-110.sql"
        ]
      }
    ]
  }
}
```

Der Aufbau beinhaltet:
- `MinVersion`: first supported version.
- `CurrentVersion`: current application database version.
- `CurrentVersion` muss gleich oder grösser als `MinVersion` sein.
- Es können pro Schritt mehrere T-SQL Scripts aufgeführt werden, die Datenbanktransaktion erfolgt pro Datei.
- No `USE [Database]` in T-SQL scripts.

Für Update-Versionen gelten zusätzliche Reglen:
- Die Update-Versionen müssen startend bei `CurrentVersion` und endend bei `CurrentVersion`, lückenlos vorhanden sein.
- Der Start `FromVersion` der niedrigsten Update-Version muss `MinVersion` sein.
- Das Ende `ToVersion` der höchsten Update-Version muss `CurrentVersion` sein.

### Web App Asset Konfiguration
Web App asset configuration in `WebApp/asset.json`:
```json
{
  "WebserverName": "Payroll Engine - Web App Server"
  "WebserverExec": "PayrollEngine.WebApp.Server.dll"
}
```

- `WebserverName`: webserver name
- `WebserverExec`: webserver executable name

## Third party components
- UI with [MudBlazor](https://github.com/MudBlazor/MudBlazor/) - license `MIT`
