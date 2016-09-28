using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Linq;
using System.Web;

namespace WebSite.Models
{
    public class New
    {
        [Key]
        public int id { get; set; }

        public string Header { get; set; }
        public string Description  { get; set; }

        [DataType(DataType.MultilineText)]
        public string Text { get; set; }

        [Column(TypeName = "image")]
        public byte[] Picture{ get; set; }

        public DateTime DataPub { get; set; }

    }
    public class NewPost
    {
        public New news { get; set; }
        public IEnumerable<New> News { get; set; }
    }
}