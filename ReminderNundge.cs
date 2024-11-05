using System;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace AflinxFunction
{
    public class ReminderNundge
    {
        [FunctionName("ReminderNundge")]
        public void Run([TimerTrigger("0 30 8 * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            string NotificationAPIUrl = Environment.GetEnvironmentVariable("NotificationAPIUrl");

            try
            {
                UriBuilder Licensebuilder = new UriBuilder(NotificationAPIUrl + "api/AflinxUser/Licenseexpiredreminder");
                var Licenserequest = new HttpRequestMessage(HttpMethod.Get, Licensebuilder.Uri);
                var LicensehttpClient = new HttpClient();
                HttpResponseMessage LicenseResponseMessage = LicensehttpClient.Send(Licenserequest);
            }
            catch (Exception ex)
            {

               
            }
            try
            {
                UriBuilder Certificatebuilder = new UriBuilder(NotificationAPIUrl + "api/AflinxUser/Certificateexpiredreminder");
                var Certificaterequest = new HttpRequestMessage(HttpMethod.Get, Certificatebuilder.Uri);
                var CertificatehttpClient = new HttpClient();
                HttpResponseMessage CertificateResponseMessage = CertificatehttpClient.Send(Certificaterequest);
            }
            catch (Exception ex)
            {


            }
            try
            {
                UriBuilder Designationbuilder = new UriBuilder(NotificationAPIUrl + "api/AflinxUser/Designationexpiredreminder");
                var Designationrequest = new HttpRequestMessage(HttpMethod.Get, Designationbuilder.Uri);
                var DesignationhttpClient = new HttpClient();
                HttpResponseMessage DesignationResponseMessage = DesignationhttpClient.Send(Designationrequest);
            }
            catch (Exception ex)
            {


            }
           

           

           

            UriBuilder Trainingexpiredreminderbuilder = new UriBuilder(NotificationAPIUrl + "api/AflinxUser/Trainingexpiredreminderexpiredreminder");
            var Trainingexpiredreminderrequest = new HttpRequestMessage(HttpMethod.Get, Trainingexpiredreminderbuilder.Uri);
            var TrainingexpiredreminderhttpClient = new HttpClient();
            HttpResponseMessage TrainingexpiredreminderResponseMessage = TrainingexpiredreminderhttpClient.Send(Trainingexpiredreminderrequest);
        }
    }
}
