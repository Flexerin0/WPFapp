using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace WpfApp1.Models
{
    internal class EmployeeImage
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public BitmapImage Image { get; set; }
    }
}
