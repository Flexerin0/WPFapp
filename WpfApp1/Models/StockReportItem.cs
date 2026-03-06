using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Models
{
    internal class StockReportItem
    {
        public string ProductName { get; set; }
        public int TotalReceived { get; set; }
        public int TotalSold { get; set; }
        public int StockBalance { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal TotalStockValue { get; set; }
        public string CategoryName { get; set; }
    }
}
