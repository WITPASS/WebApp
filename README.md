# WebApp
A complete sample web application set using docker, postgres, web api, razor pages and razor components

### Prerequisites:
 1. Visual Studio 2019 with the ASP.NET and web development workload
 2. .NET Core SDK 3.0 Preview
 
 ### Running the Admin Panel
 * Set Api Project as Startup Project, by right clicking on it in solution explorer
 * Make sure the postgres server is running at `localhost:5432`
 * Enter `"DB_USER": "your username", "DB_PASS": "your pass"`, in `appsettings.Development.json` for postgres
 * Run migrations in Api Project `update-database` using package manager console
 * Open terminal in Api Directory and run `dotnet watch run`
 * Open terminal in Admin Directory and run `dotnet watch run`
 * Open Admin panel `http://localhost:9002`

#### To work on Razor Components project in Visual Studio:
 * Install the latest .NET Core 3.0 Preview SDK release.
 * Enable Visual Studio to use preview SDKs:
 * Open Tools > Options in the menu bar.
 * Open the Projects and Solutions node. Open the .NET Core tab.
 * Check the box for Use previews of the .NET Core SDK. Select OK.
 
 ... more coming soon ...
