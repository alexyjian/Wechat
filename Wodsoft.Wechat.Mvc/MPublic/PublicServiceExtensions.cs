﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Xml;

namespace Wodsoft.Wechat.MPublic
{
    public static class PublicServiceExtensions
    {
        public static ActionResult SignIn(this Controller controller, PublicService service, string returnUrl)
        {
            var state = new Random().Next(100000000, 1000000000);
            controller.Session["wechatState"] = state;
            string url = service.GetAuthUrlWithInfo(returnUrl, state.ToString());
            return new RedirectResult(url);
        }

        public static async Task<ActionResult> HandleMessage(this Controller controller, PublicService service)
        {
            IReplyMessage message;
            if (controller.Request.QueryString.AllKeys.Contains("msg_signature"))
                message = await service.HandleEncryptedMessage(controller.Request.InputStream, controller.Request.QueryString["timestamp"], controller.Request.QueryString["nonce"], controller.Request.QueryString["msg_signature"]);
            else
                message = await service.HandleMessage(controller.Request.InputStream);
            if (message == null)
                return new ContentResult() { Content = "success" };
            MemoryStream stream = new MemoryStream();
            message.WriteResponseText(stream);
            if (controller.Request.QueryString.AllKeys.Contains("msg_signature"))
            {
                var encryptedMessage = service.EncryptMessage(Encoding.UTF8.GetString(stream.ToArray()));
                var timestamp = service.GetTimestamp().ToString();
                var nonce = Guid.NewGuid().ToString().Replace("-", "");

                List<string> data = new List<string> { service.AppToken, timestamp, nonce, encryptedMessage };
                data.Sort();

                string signature;
                using (SHA1 sha = SHA1.Create())
                {
                    signature = string.Concat(sha.ComputeHash(Encoding.ASCII.GetBytes(string.Concat(data))).SelectMany(t => t.ToString("x2")));
                }

                XmlDocument doc = new XmlDocument();
                var xml = doc.CreateElement("xml");
                var xmlEncrypt = doc.CreateElement("Encrypt");
                xmlEncrypt.InnerText = encryptedMessage;
                var xmlSignature = doc.CreateElement("MsgSignature");
                xmlSignature.InnerText = signature;
                var xmlTimeStamp = doc.CreateElement("TimeStamp");
                xmlTimeStamp.InnerText = timestamp;
                var xmlNonce = doc.CreateElement("Nonce");
                xmlNonce.InnerText = nonce;
                xml.AppendChild(xmlEncrypt);
                xml.AppendChild(xmlSignature);
                xml.AppendChild(xmlTimeStamp);
                xml.AppendChild(xmlNonce);
                stream.Position = 0;
                doc.Save(stream);
            }
            stream.Position = 0;
            return new FileStreamResult(stream, "text/xml");
        }
    }
}
