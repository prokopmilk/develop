using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using WebSite.Models.Repository;

namespace WebSite.Models
{
    public class Cart
    {
        
        public Cart() { }
        private List<CartLine> lineCollection = new List<CartLine>();
        //private EFDbContext context = new EFDbContext(ConfigurationManager.ConnectionStrings[0].ConnectionString);
        public void AddItem(Tovar tovar, int quantity)
        {
            
            CartLine line = lineCollection
                .Where(g => g.tovar.Tovar_ID == tovar.Tovar_ID)
                .FirstOrDefault();

           
            if (line == null)
            {
                lineCollection.Add(new CartLine
                {
                    tovar = tovar,
                    
                    Quantity = quantity
                });
            }
            else
            {
                line.Quantity += quantity;
            }
        }
        public void ChangeCountTovar(int ID, int Quantity)
        {
            CartLine line = lineCollection
                .Where(g => g.tovar.Tovar_ID == ID)
                .FirstOrDefault();
            if (line != null)
            {
               line.Quantity = Quantity;
            }
        }
        public int GetCountTovar()
        {
            return lineCollection.Sum(t => t.Quantity);
        }
        public void RemoveLine(Tovar tovar)
        {
            lineCollection.RemoveAll(l => l.tovar.Tovar_ID == tovar.Tovar_ID);
        }

        public double ComputeTotalValue()
        {
            
            return lineCollection.Sum(e =>e.tovar.Price*e.Quantity);

        }
        public void Clear()
        {
            lineCollection.Clear();
        }

        public IEnumerable<CartLine> Lines
        {
            get { return lineCollection; }
        }
    }

    public class CartLine
    {
        public Tovar tovar { get; set; }
        public Category category{ get; set; }
        public int Quantity { get; set; }

      
        public string toString()
        {
            string s = tovar.Category+" "+tovar.Volume+" л(кг)";
            return s;
        }
    }

}
