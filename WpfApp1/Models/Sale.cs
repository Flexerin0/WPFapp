using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Models
{
    class Sale
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public int ClientId { get; set; }

        public int Quantity { get; set; }

        public decimal PricePerUnit { get; set; }

        public decimal Discount { get; set; }

        public decimal TotalAmount { get; set; }

        public DateTime SaleDateTime { get; set; }

        public string CheckNumber { get; set; }

        public virtual Product Product { get; set; }
        public virtual Client Client { get; set; }
    }
}
