using APL_Project_Client.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace APL_Project_Client
{
    public partial class Home : Form
    {
        Dipendente d;
        private DateTime dateSelected;
        SemaphoreSlim semaphoreSendRequest = new SemaphoreSlim(1);
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
            this.progressBar1.Visible = false;
            this.monthCalendar1.Visible = true;
        }
        private void RequestHolidaysUpdatedHandler(object sender, List<Ferie> e)
        {
            dataGridView1.DataSource = e;
            dataGridView1.Visible = true;
        }
        private async void Home_Load(object sender, EventArgs e)
        {
            d.HolidaysAcceptedReceived += this.HolidaysReceiveHandler;
            d.HolidaysPendingUpdated += this.RequestHolidaysUpdatedHandler;
            var boolean = await d.fetchHolidays();

        }

        private void monthCalendar1_DateChanged(object sender, DateRangeEventArgs e)
        {
            DateTime date = e.Start;
            if(semaphoreSendRequest.CurrentCount == 1)
            {
                if( d.RequestContainsDate(date) )
                {
                    this.label3.Text = "Hai già effettuato la richiesta per giorno " + date.ToString("d");
                    this.label3.Visible = true;
                }
                else if( ! d.isGiornoFerie(date) && ! IsWeekend(date) && date > DateTime.Now )
                {
                    dateSelected = date;
                    this.label3.Text = "Vuoi procedere alla richiesta per giorno " + dateSelected.ToString("d") + "?";
                    this.button2.Visible = true;
                    this.label3.Visible = true;

                } else {
                    this.label3.Text = "";
                    this.button2.Visible = false;
                    this.label3.Visible = false;
                }

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

        private async void button2_Click(object sender, EventArgs e)
        {
            if(this.dateSelected != null)
            {
                try
                {
                    //Inlobare queste cose in un metodo, o inglobare i due compoenenti in un compoenente, non lo sos
                    this.label3.Visible = false;
                    this.button2.Visible = false;
                    this.progressBar2.Visible = true;
                    await semaphoreSendRequest.WaitAsync();
                    bool response = await d.sendHolidayRequest(this.dateSelected);
                    //Inserire una progress bar
                    //Richiesta http per richiedere il giorno di ferie
                    if (response)
                    {
                        this.label3.Text = "Richiesta di ferie inoltrata con successo!";
                        this.progressBar2.Visible = false;
                        this.label3.Visible = true;
                        //Posso invocare un evento che vada ad aggiornare il listato di ferie preso (ordinato per data) e magari riutilizzare logiche e meccanismi di load di questo "componente"
                        //Se faccio un componente custom, posso omagari passare al load una lista di ferie, e quando la richiamo magari passo la vecchia lista in add col valore nuovo
                        //Disaccoppiando il render del componente alle logiche di poolamento che ci stanno dietro
                        //UpdateListFerie(response)
                    }
                    else
                    {
                        this.label3.Text = "Impossibile inoltrare la richiesta.";
                        this.progressBar2.Visible = false;
                        this.label3.Visible = true;
                        //RequestError()

                    }
                }
                finally
                {
                    semaphoreSendRequest.Release();
                }

            }
            else
            {
                this.label3.Text = "Impossibile inoltrare la richiesta.";
                this.progressBar2.Visible = false;
                this.label3.Visible = true;
                //Invocare un metodo che è invocato sia nel caso di fallimento della chiamata http, sia se ce'è un'inconsistenza
                //RequestError()
            }
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void ferieBindingSource_CurrentChanged(object sender, EventArgs e)
        {

        }
    }
}
