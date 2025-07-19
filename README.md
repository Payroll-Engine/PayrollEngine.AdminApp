# Payroll Engine Admin Application
👉 This application is part of the [Payroll Engine](https://github.com/Payroll-Engine/PayrollEngine/wiki).

The Admin Application is used to install and operate the Payroll Engine. It supports both local installation (OnPremises) and configuration of remote applications (Client Setup).

## Features
The application supports the following tasks
- Backend administration with database and API web server
- Web application web server management
- Registration of file types
- Opening tests and samples

The functions of the admin app are divided into assets:

| Asset            | Required asset            | Setup | Client Setup |
|:--|:--|:--:|:--:|
| Web App          | Backend                   |   ✔️  |              |
| Backend          |                           |   ✔️  |              |
| Backend Remote   |                           |       |    ✔️        |
| Console          | Backend or Remote Backend |   ✔️  |    ✔️        |
| Tests            | Console                   |   ✔️  |    ✔️        |
| Examples         | Console                   |   ✔️  |    ✔️        |

## Usage
### Local Installation
The following steps are required for local use of the Payroll Engine:
1. Deploy the database server (local [installation](https://www.microsoft.com/en-us/download/details.aspx?id=104781) or in the cloud) (`Add` command).
2. Add the database connection and install the database (`Add` and `Create` commands).
3. Add a Backend API server connection (`Add` command).
4. Start the Backend API server (Start command).
5. Test the Backend API (command `API` > Swagger)
6. [Registration](#filetype-registration) of Payroll Console file types
7. Run tests (batch file `Tests/Test.All.pecmd`)
8. Add a Web Application Server connection (`Add` command)
9. Start the Web Application Server (`Start` command)
10. Login to the Web Application (`Login` command)

### Remote Installation
The following steps are required to install the Payroll Console:
1. Add the Backend API server connection (`Add` command)
2. Test the Backend API (Command `API` > Swagger)
3. [Registration](#filetype-registration) of Payroll Console file types
4. Run tests (command file `Tests/Test.All.pecmd`)

### Filetype Registration
To automate the Payroll Engine, the file type `.pecmd` must be registered for the Payroll Console.

The following command files are located in the installation start folder and must be run as **administrator**:
- `FileType.Reg.cmd` - Add Registration
- `FileType.Unreg.cmd` - Remove Registration

> This registry only works on Windows. On other operating systems this step must be done manually.

## Data Storage
All configuration data is stored in the operating system's environment variables:

| Variable                    | Description                                   | Required  | Read by | Set by                  |
|:--|:--|:--:|:--|:--|
| `PayrollApiKey`             | The backend API access key                    | no        | Backend | Backend                 |
| `PayrollDatabaseConnection` | The database connection string <sup>1)</sup>  | yes       | Backend | Backend                 |
| `PayrollApiConnection`      | The backend connection string <sup>2)</sup>   | yes       | Backend | Web App, Console        |
| `PayrollWebAppConnection`   | The Web App connection string <sup>3)</sup>   | no        | Web App | Admin App <sup>4)</sup> |

<sup>1)</sup> Example: `Server=localhost; Database=PayrollEngine; Integrated Security=true; TrustServerCertificate=true; Timeout=30;`.<br />
<sup>2)</sup> Example: `BaseUrl=https://localhost; Port=44354; Timeout=02:46:40; ApiKey=MyApiKey;`.<br/>
<sup>3)</sup> Example: `BaseUrl=https://localhost; Port=7179;`.<br/>
<sup>4)</sup> Used to start the web application server within the admin application.<br/>

The `PayrollApiKey` parameter forces a key when accessing the Backend API. This value must be set to restrict access to the API server. Once the API key has been defined, it must be included in the client connection string.


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

## Asset Configuration
The parameters of an Asset are defined in the `asset.json` configuration file.

### Backend Asset Configuration
The configuration of the Backend asset contains information about the database and the web server.

Example of a Backend asset:
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

The following example shows the configuration of the version 1.1.0, which supports the update capability up to version 0.9.0:
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

The structure contains
- `MinVersion`: the first supported version.
- `CurrentVersion`: the current version of the application database.
- `CurrentVersion` must be greater than or equal to `MinVersion`.
- Multiple T-SQL scripts can be executed per step, the database transaction is per script.
- No `USE [Database]` in T-SQL scripts.

There are additional rules for update versions:
Update versions must be consecutive, starting with `MinVersion` and ending with `CurrentVersion`.
The start `FromVersion` of the lowest update version must be `MinVersion`.
The end `ToVersion` of the highest update version must be `CurrentVersion`.

### Web Application asset configuration
Example of a web application asset:
```json
{
  "WebserverName": "Payroll Engine - Web App Server"
  "WebserverExec": "PayrollEngine.WebApp.Server.dll"
}
```

## Solution projects
The.NET Core application consists of the following projects:

| Name                                           | Type             | Description                                       |
|:--|:--|:--|
| `PayrollEngine.AdminApp.Core`                  | Library          | Core types, assets and services                   |
| `PayrollEngine.AdminApp.Persistence.SqlServer` | Library          | Database service for SQL Server                   |
| `PayrollEngine.AdminApp.Presentation`          | Razor Library    | The web application connection string             |
| `PayrollEngine.AdminApp.Windows`               | Wpf/Razor WinExe | Windows WPF application hosting the Razor WebView |

## Third party components
- UI with [MudBlazor](https://github.com/MudBlazor/MudBlazor/) - license `MIT`
