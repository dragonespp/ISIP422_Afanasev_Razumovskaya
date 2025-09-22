using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISIP422_Afanasev_Razumovskaya.Models
{
    public class SalesHistory
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public int QuantitySold { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime SaleDate { get; set; }
        public Category Category { get; set; }

        public SalesHistory(Product product, int quantity)
        {
            ProductCode = product.Code;
            ProductName = product.Name;
            QuantitySold = quantity;
            UnitPrice = product.Price;
            TotalAmount = quantity * product.Price;
            SaleDate = DateTime.Now;
            Category = product.Category;
        }
    }
}
