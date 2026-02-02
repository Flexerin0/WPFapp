using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Models
{
    internal class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SName { get; set; }
        public string Patronomic { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public DateTime BirthDate { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Note { get; set; }

        public override string ToString()
        {
            return $"{Name} {SName} | {Note}";
        }
    }
}
