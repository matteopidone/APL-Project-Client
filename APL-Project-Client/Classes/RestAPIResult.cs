﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APL_Project_Client.Classes
{
    public struct LoginAPIResult
    {
        public bool found { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string email { get; set; }
        public string description { get; set; }
        public string token { get; set; }
    }
    public struct getHolidaysAPIResult
    {
        public StatoFerie type { get; set; }
        public int day { get; set; }
        public int month { get; set; }
        public int year { get; set; }
        public string message { get; set; }
    }
    public struct insertHolidayAPIResult
    {
        public bool result { get; set; }
        
    }
}
