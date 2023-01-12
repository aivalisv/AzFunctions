#r "Newtonsoft.Json"
#r "SendGrid"

using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System.Data.SqlClient;
using SendGrid.Helpers.Mail;
using System;
using System.Text;

[FunctionName("SendEmail")]
public static SendGridMessage  Run(TimerInfo myTimer,  ILogger log)
{
    log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
                       SendGridMessage message = new SendGridMessage()
    {
        Subject = $"Your Security Report"
    };
                
    string connectionString = "Server=tcp:*******.database.windows.net,1433;Initial Catalog=*******;Persist Security Info=False;User ID=*******;Password=*******;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
    var dates = new List<DateTime>();

             try{
                using(SqlConnection connection = new SqlConnection(connectionString)){
                    // Opening a connection
                    connection.Open();
 
                    // Prepare the SQL Query
                    var query = $"select DATEADD(MINUTE, DATEDIFF(MINUTE, '2000', add_date) / 10 * 10, '2000') as click From security_check_log where cast(add_date as date) = dateadd(day,0, cast(getdate() as date)) GROUP BY DATEADD(MINUTE, DATEDIFF(MINUTE, '2000', add_date) / 10 * 10, '2000')";
 
                    // Prepare the SQL command and execute query
                    SqlCommand command = new SqlCommand(query,connection);
 
                    var reader = command.ExecuteReader();  
                    while (reader.Read())  
                    {  
                       dates.Add( (DateTime)reader["click"]);  
                    } 
                }

 


            }
            catch(Exception e){
            
                log.LogError(e.ToString());
            }

StringBuilder sb = new StringBuilder();
    sb.Append("<TABLE>\n");
    foreach (var item in dates)
    {
        sb.Append("<TR>");
        sb.Append("<TD>");
        sb.Append(item);
        sb.Append("</TD>");
        sb.Append("</TR>");
    }
    sb.Append("</TABLE>");
              

      log.LogInformation($"Done at: {DateTime.Now}");
    message.AddContent("text/html", sb.ToString());
    return message;
}
