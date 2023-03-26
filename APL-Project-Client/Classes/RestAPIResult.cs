using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APL_Project_Client.Classes
{
    // Queste struct rappresentano lo schema di risposta delle varie api del server.
    // Da queste struct si estrapolano le informazioni tornate dal server.

    // Struct api "/login".
    public struct LoginAPIResult
    {
        public bool found { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string email { get; set; }
        public string description { get; set; }
        public string token { get; set; }
    }

    // Struct api "/getHolidays".
    public struct getHolidaysAPIResult
    {
        public HolidayType type { get; set; }
        public int day { get; set; }
        public int month { get; set; }
        public int year { get; set; }
        public string message { get; set; }
    }

    //Struct api "/insertHoliday".
    public struct insertHolidayAPIResult
    {
        public bool result { get; set; }
        
    }
    
    // Enum che descrive il campo type tornato dal server.
    public enum HolidayType
    {
        Pending = 0,
        Accepted = 1,
        Refused = 2
    }
}
