# Payroll Engine Admin Application
👉 This application is part of the [Payroll Engine](https://github.com/Payroll-Engine/PayrollEngine/wiki).

## Features
The functions of the admin app are divided into assets:

| Asset            | Base asses                | Setup | Client Setup |
|:--|:--|:--|:--|
| Web App          | Backend                   |   ✔️  |              |
| Backend          |                           |   ✔️  |              |
| Backend Remote   |                           |       |    ✔️        |
| Console          | Backend or Remote Backend |   ✔️  |    ✔️        |
| Tests            | Console                   |   ✔️  |    ✔️        |
| Examples         | Console                   |   ✔️  |    ✔️        |

<sup>1)</sup> Only as Windows system administrator.<br/>


## User settings

Environment variables
| Variable                | Description                                | Required  | Used by assets     |
|:--|:--|:--|:--|
| `PayrollDatabaseConnection` | The database connection string         | yes       | Backend            |
| `PayrollApiConnection`      | The backend connection string          | yes       | Web App, Console   |
| `PayrollWebAppConnection`   | The web app connection string          | no        | <sup>1)</sup>      |
| `PayrollApiKey`             | The payroll API key                    | no        | Backend            |

<sup>1)</sup> Used to start the web app server within the admin application.<br/>


## Assets
### Backend Local
- Add/Edit database connection
- Setup (Create/Update) database
- Add/Edit webserver connection
- Start webserver
- Browse backend API

Backend asset configuration in `Backend/asset.json`:
```json
{
  "WebserverName": "Payroll Engine - Backend Server",
  "WebserverExec": "PayrollEngine.Backend.Server.dll",
  "Database": {
    "MinVersion": "0.9.0",
    "CurrentVersion": "1.1.0",
    "InitScripts": [
      "Database/ModelCreate.sql"
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

- `WebserverName`: webserver name
- `WebserverExec`: webserver executable name

T-SQL Scripts:
- Any script is executed within a database transaction
- no `USE [Database]` in scripts

Versioning:
- `MinVersion`: first supported version
- `CurrentVersion`: current application database version
- `CurrentVersion` >= `MinVersion`
- Update requires for any version change an individual update script
- no gaps between updates allowed
- `ToVersion` of then newest update script must be equals `CurrentVersion`

### Backend Remote
- Add/Edit webserver connection
- Browse backend API

### Web App
- Add/Edit web app webserver connection
- Start webserver
- Start Web App login

Web App asset configuration in `WebApp/asset.json`:
```json
{
  "WebserverName": "Payroll Engine - Web App Server"
  "WebserverExec": "PayrollEngine.WebApp.Server.dll"
}
```

- `WebserverName`: webserver name
- `WebserverExec`: webserver executable name

### Console
- Register/Unregister file types (Windows admin only)

Console asset configuration in `Console/asset.json`:
```json
{
  "Executable": "PayrollEngine.PayrollConsole.exe",
  "FileTypeName": "Payroll Engine Console",
  "FileTypeExtension": ".pecmd"
}
```

- `Executable`: console executable name
- `FileTypeName`: registration name for the file type
- `FileTypeExtension`: file type extension


## Application Configuration
The application configuration file `appsetings.json` contains the following settings:

| Setting                 | Description                                   | Type     | Default                        |
|:--|:--|:--|:--|
| `Culture`               | The user interface culture                    | string   | System culture                 |
| `DarkMode`              | Dark application theme                        | bool     | Operating system setting       |
| `HelpUrl`               | The help url                                  | string   | Payroll Engine Wiki            |
| `FileAssetsPath`        | Path to file assets                           | string   | `..`                           |
| `DatabaseCollation`     | Database character treating                   | string   | `SQL_Latin1_General_CP1_CS_AS` |
| `DatabaseConnectTimeout`| Timeout for database requests (seconds)       | int      | 5                              |
| `HttpConnectTimeout`    | Timeout for http requests (seconds)           | int      | 5                              |
| `AutoRefreshTimeout`    | Application auto refresh timeout (seconds) <sup>1)</sup> | int      | 120                 |

<sup>1)</sup> Minimum=5, maximum=3600, Off=0<br />

## Third party components
- UI with [MudBlazor](https://github.com/MudBlazor/MudBlazor/) - license `MIT`
