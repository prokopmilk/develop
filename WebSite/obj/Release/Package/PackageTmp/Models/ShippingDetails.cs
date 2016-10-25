using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebSite.Models
{
    public class ShippingDetails
    {
        [Required(ErrorMessage = "Укажите имя и фамилию")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Заполните адрес доставки")]
        public string Line1 { get; set; }
        
        [Required(ErrorMessage ="Укажите контактный телефон")]
        public string Phone { get; set; }

        
        public string Mail { get; set; }

        [Required(ErrorMessage = "Укажите город")]
        public string City { get; set; }

        public bool SelfDeliv { get; set; }

    }
}