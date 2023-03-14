using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APL_Project_Client.Model
{
    public struct LoginAPIResult
    {
        public bool found { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string email { get; set; }
    }
    public struct getHolidaysAPIResult
    {
        public StatoFerie type { get; set; }
        public int day { get; set; }
        public int month { get; set; }
        public int year { get; set; }
        public string message { get; set; }
    }
}
