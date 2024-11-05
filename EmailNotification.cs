using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AflinxFunction
{
    public class EmailNotification
    {
        [FunctionName("EmailFunction")]
        public void Run([ServiceBusTrigger("email", Connection = "ServiceBusConnection")]string myQueueItem, ILogger log)
        {

            dynamic data = JsonConvert.DeserializeObject(myQueueItem);
            string NotificationAPIUrl = Environment.GetEnvironmentVariable("NotificationAPIUrl");
            foreach (var item in data.users)
            {
                data.toemail = Convert.ToString(item);

                UriBuilder builder = new UriBuilder(NotificationAPIUrl + "api/notificationque/sendemail");


                var request = new HttpRequestMessage(HttpMethod.Post, builder.Uri);
                if (data != null)
                {
                    var content = GetContentstring(data);
                  
                    request.Content = content;

                }

                //request.Headers.Add("Content-Type", "application/json");

                var httpClient = new HttpClient();
                HttpResponseMessage ResponseMessage = httpClient.Send(request);

                log.LogInformation($"Email: {Convert.ToString(ResponseMessage.Content)}");
            }
            //log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
        }

        public static StringContent GetContentstring(object data)
        {
           

            return new StringContent(data.ToString(), Encoding.UTF8, "application/json");
        }
       
    }
}
