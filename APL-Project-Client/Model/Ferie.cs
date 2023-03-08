using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APL_Project_Client.Model;
public class Ferie
{
    public DateTime date { get; set; }
    public string motivation { get; set; }
    public Ferie(int day, int month, int year, string motivation)
    {
        date = new DateTime(year, month, day);  
        this.motivation = motivation;

    }
}

