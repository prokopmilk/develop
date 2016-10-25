using ActionMailer.Net.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebSite.Models;

namespace WebSite.Controllers
{
    public class MailController : MailerBase

    {
        
        public EmailResult Subscription(MailModel mModel, string email, string sub)
        {
            
            To.Add(email);
            Subject = sub;
            MessageEncoding = Encoding.UTF8;
            return Email("Subscription", mModel);
        }

        public EmailResult AdminMail(MailModel mModel, string email, string sub)
        {

            To.Add(email);
            Subject = sub;
            MessageEncoding = Encoding.UTF8;
            return Email("AdminMail", mModel);
        }
        public EmailResult Account(MailModel mModel, string email, string sub)
        {

            To.Add(email);
            Subject = sub;
            MessageEncoding = Encoding.UTF8;
            return Email("Account", mModel);
        }
    }

}