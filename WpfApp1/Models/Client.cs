using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Models
{
    class Client
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public DateTime RegistrationDate { get; set; }

        public decimal Balance { get; set; }

        public int BonusPoints { get; set; }

        public string Description { get; set; }

        public virtual ICollection<Sale> Sales { get; set; }
    }
}
