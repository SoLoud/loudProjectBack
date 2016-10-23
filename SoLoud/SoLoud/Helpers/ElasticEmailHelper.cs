using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SoLoud.Helpers
{
    public class ApiResponse
    {
        public bool success { get; set; }
        public string error { get; set; }
    }

    public class ApiResponse<T> : ApiResponse
    {
        public T data { get; set; }
    }
    public class SendResponse
    {
        public string transactionid { get; set; }
        public string messageid { get; set; }
    }
    public class Email
    {
        public string FromAddress { get; set; }
        public string FromName { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
    public class ElasticEmailHelper
    {
        /* Properties */
        private string elasticMailBaseUrl = ConfigurationManager.AppSettings["elasticMailBaseUrl"].ToString();
        private string _elasticMailBaseApiKey = ConfigurationManager.AppSettings["elasticMailBaseApiKey"].ToString();
        public IRestClient client { get; set; }
        public string elasticMailApiKey
        {
            get
            {
                return
                    _elasticMailBaseApiKey;
            }
        }

        /* Constructors */
        public ElasticEmailHelper()
        {
            this.client = new RestClient(elasticMailBaseUrl);
        }

        public ElasticEmailHelper(IRestClient iClient)
        {
            this.client = iClient;
        }


        /* Methods */
        public void Send(Email Email, List<string> Addresses)
        {
            if (Email == null || Addresses == null)
            {
                throw new ArgumentNullException((Email == null ? "Email" : " ") + (Addresses == null ? "Addresses" : " "));
            }
            if (Addresses.Count == 0)
                throw new ArgumentException("Addresses List can not be empty");

            var req = new RestRequest("email/send", Method.POST);
            req.AddParameter("apikey", elasticMailApiKey);

            req.AddParameter("subject", Email.Subject);
            req.AddParameter("bodyHtml", Email.Body);

            req.AddParameter("replyTo", Email.FromAddress);
            req.AddParameter("replyToName", Email.FromName);
            req.AddParameter("from", Email.FromAddress);
            req.AddParameter("fromName", Email.FromName);

            var to = String.Join(",", Addresses);
            req.AddParameter("to", to);

            //Send request
            IRestResponse<ApiResponse<SendResponse>> resp = client.Execute<ApiResponse<SendResponse>>(req);
            bool wasSuccess = resp.Data.success;

            if (!wasSuccess)
                throw new Exception("Something Went wrong during campaign send from elasticEmail. CaSeEr!#" + resp.Data.error);
        }

        internal Task Send(Microsoft.AspNet.Identity.IdentityMessage message)
        {
            //if (message == null || message.Subject == null)
            //{
            //    throw new ArgumentNullException((message == null ? "message" : " ") + (message == null ? "message" : " "));
            //}

            var req = new RestRequest("email/send", Method.POST);
            req.AddParameter("apikey", elasticMailApiKey);

            req.AddParameter("subject", message.Subject);
            req.AddParameter("bodyHtml", message.Body);

            req.AddParameter("replyTo", "soloudapp@gmail.com");
            req.AddParameter("replyToName", "SoLoud");
            req.AddParameter("from", "soloudapp@gmail.com");
            req.AddParameter("fromName", "SoLoud");

            req.AddParameter("to", message.Destination);

            //Send request
            IRestResponse<ApiResponse<SendResponse>> resp = client.Execute<ApiResponse<SendResponse>>(req);
            bool wasSuccess = resp.Data.success;

            if (!wasSuccess)
                throw new Exception("Something Went wrong during campaign send from elasticEmail. CaSeEr!#" + resp.Data.error);

            return Task.FromResult(0);
        }
    }
}