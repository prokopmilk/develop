using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSite.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public bool StatusOrder { get; set; }
        public double TotalSum { get; set; }
        public DateTime PurchaseDate { get; set; }
        public bool SolveOrder { get; set; }

        // Ссылка на покупателя
        public Customer Customer { get; set; }

       
        //Ссылка на позиции заказа
        public virtual List<OrderItem> OrderItems { get; set; }
       
    }
}