using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSite.Models
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }
        public int Quantity { get; set; }
        public int Tovar_ID { get; set; }

        //Ссылка на заказ
        public virtual Order Order { get; set; }


        public virtual Tovar tovar { get; set; }

        public OrderItem() { }
    }
}