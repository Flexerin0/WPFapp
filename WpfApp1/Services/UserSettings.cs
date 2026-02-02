using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Services
{
    public class UserSettings
    {
        public string SavedLogin { get; set; } = "";
        public string SavedPassword { get; set; } = "";
        public bool RememberMe { get; set; } = false;
    }
}
