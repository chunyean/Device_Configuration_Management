using System;
using Microsoft.Data.SqlClient;
using Spectre.Console;

// 1. Load the environment variables from the .env file
DotNetEnv.Env.Load();

// 2. Retrieve the connection string 
// .NET 10 treats missing env vars with string handling patterns smoothly
string? connectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING");

if (string.IsNullOrWhiteSpace(connectionString))
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("Error: SQL_CONNECTION_STRING environment variable is missing.");
    Console.ResetColor();
    return;
}

using var connection = new SqlConnection(connectionString);

// use try catch to catch any return error
try {
    Console.WriteLine("Targeting [.NET 10.0] Runtime Environment...");
    Console.WriteLine("Opening connection...");
    
    connection.Open();
    
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Successfully connected!");
    Console.ResetColor();

        if(args.Length == 0){

            string sqlQuery = "SELECT * FROM t_DeviceCfg";

            using var command = new SqlCommand(sqlQuery, connection);

            using var result = command.ExecuteReader();

            int rowCount = 0;

            bool null_error = false;
            bool duplicate_error = false;
            bool slot_error = false;
            List<int> task_id_list = new List<int>();
            List<int> task_id_duplicate = new List<int>();

            var table = new Table();
            var table2 = new Table();
            var table3 = new Table();

            table.AddColumn("Equipment ID");
            table.AddColumn("Task ID");
            table.AddColumn("Equipment Type");
            table.AddColumn("Equipment Name");
            table.AddColumn("IP Address");
            table.AddColumn("Port");
            table.AddColumn("Slot Index");
            table.AddColumn("Enable");
            table.AddColumn("Version");
            table.AddColumn("Trigger");
            table.AddColumn("Scan Rate");
            table.AddColumn("Last Update");

            //clone error table
            foreach (var column in table.Columns) {
                table2.AddColumn(column);
                table3.AddColumn(column);
            }
            
            while (result.Read()){

                rowCount++;

                //null value will return N/A
                int equipID = result.GetInt32(0);
                int taskID = result.GetInt32(1);
                string type = result.IsDBNull(2) ? "N/A" : result.GetString(2);
                string name = result.IsDBNull(3) ? "N/A" : result.GetString(3);
                string ipAdd = result.IsDBNull(4) ? "N/A" : result.GetString(4);
                int port = result.GetInt32(5);
                int slot = result.GetInt32(6);
                int enable = result.GetInt16(7);
                double version = result.GetDouble(8);
                string triggerVal = result.IsDBNull(9) ? "N/A" : result.GetString(9); 
                int scanRate = result.GetInt32(10);
                string lastUpdate = result.IsDBNull(11) ? "N/A" : result.GetDateTime(11).ToString("yyyy-MM-dd HH:mm:ss.fff");

                if(type == "N/A" || name == "N/A" || ipAdd == "N/A" || triggerVal == "N/A" || lastUpdate == "N/A") {

                    null_error = true;

                    table2.AddRow(
                    equipID.ToString(), 
                    taskID.ToString(), 
                    type, 
                    name, 
                    ipAdd, 
                    port.ToString(), 
                    slot.ToString(), 
                    enable.ToString(), 
                    version.ToString("0.00"), 
                    triggerVal, 
                    scanRate.ToString(), 
                    lastUpdate);
                };

                if(task_id_list.Contains(taskID)) {
                    duplicate_error = true;

                    if(!task_id_duplicate.Contains(taskID)) {
                        task_id_duplicate.Add(taskID);
                    }

                } else {
                    task_id_list.Add(taskID);
                }

                if(slot < 0) {

                    slot_error = true;

                    table3.AddRow(
                    equipID.ToString(), 
                    taskID.ToString(), 
                    type, 
                    name, 
                    ipAdd, 
                    port.ToString(), 
                    slot.ToString(), 
                    enable.ToString(), 
                    version.ToString("0.00"), 
                    triggerVal, 
                    scanRate.ToString(), 
                    lastUpdate);

                }

                table.AddRow(
                    equipID.ToString(), 
                    taskID.ToString(), 
                    type, 
                    name, 
                    ipAdd, 
                    port.ToString(), 
                    slot.ToString(), 
                    enable.ToString(), 
                    version.ToString("0.00"), 
                    triggerVal, 
                    scanRate.ToString(), 
                    lastUpdate
                );

            };

            if(rowCount > 0){

                AnsiConsole.Write(table);

                //check for null value
                if(null_error){

                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Discover null value represent as N/A");
                    AnsiConsole.Write(table2);
                    Console.ResetColor();

                }

                //check for duplciate task id
                if(duplicate_error) {

                    Console.WriteLine();
                    string task_ids = string.Join(", ", task_id_duplicate);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Discover duplicate value for Task ID");
                    Console.WriteLine("Task ID : " + task_ids);
                    Console.ResetColor();

                }

                //check for slot index error
                if(slot_error) {

                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Discover Slot Index issue");
                    AnsiConsole.Write(table3);
                    Console.ResetColor();

                }

            } else {

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("No data found.");
                Console.ResetColor();

            }

        } else if(args.Length == 1 && !string.IsNullOrWhiteSpace(args[0])) {

            //check second argument is number
            if (!int.TryParse(args[0], out int res)) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Value Error: The value of '{args[0]}' is not a valid number.");
                Console.ResetColor();
                return; 
            }

            string equip_id = args[0];

            string sqlQuery = $"SELECT * FROM t_DeviceCfg WHERE EquipId = '{equip_id}'";

            using var command = new SqlCommand(sqlQuery, connection);

            using var result = command.ExecuteReader();

            int rowCount = 0;

                var table = new Table();

                table.AddColumn("Equipment ID");
                table.AddColumn("Task ID");
                table.AddColumn("Equipment Type");
                table.AddColumn("Equipment Name");
                table.AddColumn("IP Address");
                table.AddColumn("Port");
                table.AddColumn("Slot Index");
                table.AddColumn("Enable");
                table.AddColumn("Version");
                table.AddColumn("Trigger");
                table.AddColumn("Scan Rate");
                table.AddColumn("Last Update");
                
                while (result.Read()){

                    rowCount++;

                    int equipID = result.GetInt32(0);
                    int taskID = result.GetInt32(1);
                    string type = result.IsDBNull(2) ? "N/A" : result.GetString(2);
                    string name = result.IsDBNull(3) ? "N/A" : result.GetString(3);
                    string ipAdd = result.IsDBNull(4) ? "N/A" : result.GetString(4);
                    int port = result.GetInt32(5);
                    int slot = result.GetInt32(6);
                    int enable = result.GetInt16(7);
                    double version = result.GetDouble(8);
                    string triggerVal = result.IsDBNull(9) ? "N/A" : result.GetString(9); 
                    int scanRate = result.GetInt32(10);
                    string lastUpdate = result.IsDBNull(11) ? "N/A" : result.GetDateTime(11).ToString("yyyy-MM-dd HH:mm:ss.fff");

                    table.AddRow(
                        equipID.ToString(), 
                        taskID.ToString(), 
                        type, 
                        name, 
                        ipAdd, 
                        port.ToString(), 
                        slot.ToString(), 
                        enable.ToString(), 
                        version.ToString("0.00"), 
                        triggerVal, 
                        scanRate.ToString(), 
                        lastUpdate
                    );
                };


            if(rowCount > 0) {

                AnsiConsole.Write(table);

            } else {

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("This Equipment ID doesn't exist");
                Console.ResetColor();

                return;

            }

        } else {
            // handle not valid or not correct value

            if (args.Length == 1) {

                if (!int.TryParse(args[0], out int result))
                {

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Both value is not correct! second value is not a valid number!");
                    Console.ResetColor();

                }
            }

        }
    
}

catch (SqlException ex) {

    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"Database Error occurred: {ex.Message}");
    Console.ResetColor();
}

catch (Exception ex) {

    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"Unexpected System Error: {ex.Message}");
    Console.ResetColor();

}
