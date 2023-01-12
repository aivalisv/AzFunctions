#r "Newtonsoft.Json"

using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System.Data.SqlClient;


public static async Task<IActionResult> Run(HttpRequest req, ILogger log)
{
    log.LogInformation("C# HTTP trigger function processed a request.");

    string voltage = req.Query["voltage"];

    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
    dynamic data = JsonConvert.DeserializeObject(requestBody);
    voltage = voltage ?? data?.voltage;


  string connectionString = "Server=tcp:*******.database.windows.net,1433;Initial Catalog=******;Persist Security Info=False;User ID=*******;Password=*******;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
 
            // Using the connection string to open a connection
            try{
                using(SqlConnection connection = new SqlConnection(connectionString)){
                    // Opening a connection
                    connection.Open();
 
                    // Defining the log message and Create Date
                    var voltforDB = voltage;
                    var createDate = DateTime.UtcNow;
 
                    // Prepare the SQL Query
                    var query = $"INSERT INTO [rpi_voltage] ([Voltage],[date]) VALUES('{voltforDB}', '{createDate}')";
 
                    // Prepare the SQL command and execute query
                    SqlCommand command = new SqlCommand(query,connection);
 
                    // Open the connection, execute and close connection
                    if(command.Connection.State == System.Data.ConnectionState.Open){
                        command.Connection.Close();
                    }
                    command.Connection.Open();
                    command.ExecuteNonQuery();
 
                }
            }
            catch(Exception e){
                log.LogError(e.ToString());
            }



    string responseMessage = string.IsNullOrEmpty(voltage)
        ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
}
