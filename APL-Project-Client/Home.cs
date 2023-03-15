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
            addHolidaysToCalendar(e);
            progressBar1.Visible = false;
            monthCalendar1.Visible = true;
        }
        private void RequestHolidaysUpdatedHandler(object sender, List<Ferie> e)
        {
            dataGridView1.DataSource = e;
            dataGridView1.Visible = true;
        }
        private async void fetchAllHolidays()
        {
            // Definisco associo gli handler agli eventi esposti per popolare la Home.
            d.HolidaysAcceptedReceived += HolidaysReceiveHandler;
            d.HolidaysPendingUpdated += RequestHolidaysUpdatedHandler;
            try
            {
                await d.fetchHolidays();

            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show("Errore nella richiesta: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (InvalidOperationException ex )
            {
                MessageBox.Show("Errore :" + ex.Message +"\nContattare il tuo datore di lavoro", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errore generico: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

        }

        private async void Home_Load(object sender, EventArgs e)
        {
            fetchAllHolidays();
        }

        private void monthCalendar1_DateChanged(object sender, DateRangeEventArgs e)
        {
            DateTime date = e.Start;
            if(semaphoreSendRequest.CurrentCount == 1)
            {
                if( d.isHolidayPending(date) )
                {
                    label3.Text = "Hai già effettuato la richiesta per giorno " + date.ToString("d");
                    label3.Visible = true;
                }
                else if( ! d.isHolidayAccepted(date) && ! IsWeekend(date) && date > DateTime.Now )
                {
                    dateSelected = date;
                    label3.Text = "Vuoi procedere alla richiesta per giorno " + dateSelected.ToString("d") + "?";
                    button2.Visible = true;
                    label3.Visible = true;
                    textBox1.Visible = true;

                } else {
                    label3.Text = "";
                    button2.Visible = false;
                    textBox1.Text = "";
                    textBox1.Visible = false;
                    label3.Visible = false;
                }

            }

        }

        private bool IsWeekend(DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
        }
        private void addHolidaysToCalendar(List<DateTime> dates)
        {
            foreach (DateTime date in dates)
            {
                monthCalendar1.AddBoldedDate(date);
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
        private async void sendHolidayRequest(string motivation)
        {
            try
            {
                label3.Visible = false;
                button2.Visible = false;
                textBox1.Visible= false;
                textBox1.Text = "";
                progressBar2.Visible = true;
                await semaphoreSendRequest.WaitAsync();
                bool response = false;
                try
                {
                    response = await d.sendHolidayRequest(dateSelected, motivation);
                }
                catch (HttpRequestException ex)
                {
                    MessageBox.Show("Errore nella richiesta: " + ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Errore generico: " + ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                //Inserire una progress bar
                if (response)
                {
                    label3.Text = "Richiesta di ferie inoltrata con successo!";
                    progressBar2.Visible = false;
                    label3.Visible = true;
                }
                else
                {
                    label3.Text = "Impossibile inoltrare la richiesta.";
                    progressBar2.Visible = false;
                    label3.Visible = true;

                }
            }
            finally
            {
                semaphoreSendRequest.Release();
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            if(dateSelected != null)
            {
                string motivation = textBox1.Text;
                sendHolidayRequest(motivation);

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
