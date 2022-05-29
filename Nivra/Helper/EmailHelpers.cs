using Microsoft.Extensions.Options;
using Nivara.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Nivara.Common.Helpers
{
    public  class EmailHelpers
    {

        #region MyRegion
        private readonly IOptions<AppSettingsModel> options;
        #endregion

        ConfigHelper configMgr;
        string _toAddress = string.Empty;
        string _fromAddress = string.Empty;
        string _subject = string.Empty;
        string _body = string.Empty;
        string _host = string.Empty;
        string _userName = string.Empty;
        string _password = string.Empty;
        bool _enableSSL = false;
        string _ccAddress = string.Empty;
        List<Attachment> attachements;

        int _port = 0;
        public EmailHelpers(IOptions<AppSettingsModel> _options)
        {

            options = _options;
            configMgr = new ConfigHelper(options);
            _host = configMgr.MailHost;
            _userName = configMgr.MailUserName;
            _password = configMgr.MailPassword;
            _enableSSL = configMgr.MailEnableSSL;
            _port = configMgr.MailPortNumber;
            attachements = new List<Attachment>();
        }
        public EmailHelpers To(string address) { _toAddress = address; return this; }
        public EmailHelpers From(string address) { _fromAddress = address; return this; }
        public EmailHelpers CC(string address) { _ccAddress = address; return this; }
        public EmailHelpers Subject(string subject) { _subject = subject; return this; }
        public EmailHelpers Body(string body) { _body = body; return this; }
        public EmailHelpers Attachment(Byte[] bytes, string fileName, string mediaType)
        {
            Attachment att = new Attachment(new MemoryStream(bytes), fileName, mediaType);
            attachements.Add(att);
            return this;
        }
        public EmailHelpers AttachmentByPath(List<string> paths)
        {
            foreach (var item in paths)
            {
                Attachment att = new Attachment(item);
                attachements.Add(att);
            }
            return this;
        }
        public bool Send()
        {
            MailMessage mailMessage = new MailMessage()
            {
                Subject = _subject,
                Body = _body,

                IsBodyHtml = true
            };
            if (string.IsNullOrEmpty(_fromAddress))
            {
                _fromAddress = configMgr.MailUserName;
            }
            mailMessage.From = new MailAddress(_fromAddress, configMgr.MailFromName);

            if (!string.IsNullOrEmpty(_toAddress))
            {
                var toAddresses = _toAddress.Split(',');
                if (toAddresses.Length > 0)
                {
                    foreach (var toAddress in toAddresses)
                    {
                        if (!string.IsNullOrEmpty(toAddress))
                            mailMessage.To.Add(new MailAddress(toAddress));
                    }
                }
            }
            if (!string.IsNullOrEmpty(_ccAddress))
            {
                var ccAddresses = _ccAddress.Split(',');
                if (ccAddresses.Length > 0)
                {
                    foreach (var ccAddress in ccAddresses)
                    {
                        if (!string.IsNullOrEmpty(ccAddress))
                            mailMessage.CC.Add(new MailAddress(ccAddress));
                    }
                }
            }
            if (attachements.Count > 0)
            {
                foreach (var attachment in attachements)
                    mailMessage.Attachments.Add(attachment);
            }
            return Send(mailMessage);
        }
        public bool Send(MailMessage Message)
        {
            bool mailSent = false;
            try
            {
                // Add Logo image attachment from local disk
                byte[] iconBytes = Convert.FromBase64String(@"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");
                MemoryStream iconBitmap = new MemoryStream(iconBytes);
                LinkedResource iconResource = new LinkedResource(iconBitmap, MediaTypeNames.Image.Jpeg);
                iconResource.ContentId = "nfx_logo";

                AlternateView alternativeView = AlternateView.CreateAlternateViewFromString(Message.Body, null, MediaTypeNames.Text.Html);
                alternativeView.LinkedResources.Add(iconResource);
                Message.AlternateViews.Add(alternativeView);

                SmtpClient client = new SmtpClient();
                client.Host = _host;
                client.Port = _port;
                client.Credentials = new NetworkCredential(_userName, _password);
                client.EnableSsl = _enableSSL;
                client.Send(Message);
                mailSent = true;
            }
            catch (Exception ex)
            {

            }
            return mailSent;
        }

    }
}
