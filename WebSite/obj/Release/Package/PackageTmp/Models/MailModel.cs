using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSite.Models
{
    public class MailModel
    {
        public string text { get; set; }
        public IEnumerable<New> News { get; set; }
    }
}