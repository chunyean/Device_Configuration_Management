This project is use .Net 10 and SQL Server 2025 Express, 

## Setup
To set up and run this practical task project
1. Clone the project
2. Create a .env file in the project root
3. Add the SQL Server connection string using the following variable:
SQL_CONNECTION_STRING=your_sql_server_connection_string
4. Open terminal or Powershell
5. Navigate to the project folder.
6. Restore the project dependencies:
dotnet restore

## Setup run DeviceCfg.exe (run command at terminal)
1. Build the project 
After cloning the repository, open a terminal in the project root directory and run:

dotnet build .\DeviceCfg.csproj

This generates DeviceCfg.exe in the bin\Debug\net10.0 directory.

2. Check the DeviceCfg.exe location

Get-ChildItem -Recurse -Filter DeviceCfg.exe

This command displays the location of the compiled executable.

3.  Set the temporary PATH environment variable

$env:PATH += ";$(Resolve-Path .\bin\Debug\net10.0)"

Replace .\bin\Debug\net10.0 with the correct directory if your executable is located elsewhere.

## Running the Application
Run:
dotnet run OR DeviceCfg.exe
* Display out all the information from database. 
* Display data issues identified during validation

