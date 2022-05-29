using Microsoft.Extensions.Options;
using Nivara.Common.Helpers;
using Nivara.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nivara.Web.Helper
{
    public static class CommonHelper
    {
        public static EmailModel<bool> SendEmail(IOptions<AppSettingsModel> options, string toEmail, string subject, string body, List<string> attachment)
        {
            EmailModel<bool> result = new EmailModel<bool>();
            try
            {
                bool isSent = new EmailHelpers(options)
                                   .Subject(subject)
                                   .Body(body)
                                   .To(toEmail)
                                   .Send();
            }
            catch (Exception ex)
            {

                throw;
            }

            //result.EmailSuccess = isSent;
            result.EmailMessage = "";
            return result;
        }
    }

    public class EmailModel<T>
    {
        public int StatusCode { get; set; } = 200;
        public bool EmailSuccess { get; set; }
        public string EmailMessage { get; set; }
        public T Data { get; set; }
    }
}
