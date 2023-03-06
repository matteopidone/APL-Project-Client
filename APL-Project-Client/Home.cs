using APL_Project_Client.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace APL_Project_Client
{
    public partial class Home : Form
    {
        Dipendente d;
        public Home(Dipendente d1)
        {
            InitializeComponent();
            d = d1;
            label1.Text = "Benvenuto " + d.nome + " " + d.cognome;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool response = true;
            if (response)
            {
                Login loginForm = new Login();
                loginForm.Show();
                this.Hide();
            }
        }
        
        private void HolidaysReceiveHandler(object sender, List<DateTime> e)
        {
            this.ColorizeDates(e, Color.Red);
        }
        private async void Home_Load(object sender, EventArgs e)
        {
            d.HolidaysReceived += this.HolidaysReceiveHandler;
            var boolean = await d.fetchHolidays();

        }

        private void monthCalendar1_DateChanged(object sender, DateRangeEventArgs e)
        {
            DateTime date = e.Start;
            if( ! d.isGiornoFerie(date) && ! IsWeekend(date) )
            {
                this.label3.Text = "Vuoi procedere alla richiesta per giorno " + date.ToString("d") + "?";
                this.button2.Visible = true;
                this.label3.Visible = true;

            } else {
                this.label3.Text = "";
                this.button2.Visible = false;
                this.label3.Visible = false;
            }
        }

        private bool IsWeekend(DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
        }
        private void ColorizeDates(List<DateTime> dates, Color color)
        {
            foreach (DateTime date in dates)
            {
            monthCalendar1.AddBoldedDate(date);
            monthCalendar1.UpdateBoldedDates();

            monthCalendar1.TitleForeColor = color;

            monthCalendar1.RemoveAnnuallyBoldedDate(date);
            monthCalendar1.AddAnnuallyBoldedDate(date);
            monthCalendar1.UpdateBoldedDates();
            monthCalendar1.Update();

            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}
