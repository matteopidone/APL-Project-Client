using APL_Project_Client.Classes;
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
        private void showCalendar()
        {
            progressBar1.Visible = false;
            monthCalendar1.Visible = true;
        }
        private void showTableHolidays(List<Ferie> f)
        {
            dataGridView1.DataSource = f;
            dataGridView1.Visible = true;
        }
        private void showFormSendHolidayRequest(string date)
        {
            label3.Text = "Vuoi procedere alla richiesta per giorno " + date + "?";
            button2.Visible = true;
            label3.Visible = true;
            textBox1.Visible = true;
        }

        private void hideFormSendHolidayRequest()
        {
            label3.Text = "";
            button2.Visible = false;
            textBox1.Text = "";
            textBox1.Visible = false;
            label3.Visible = false;
        }
        private void showHolidaysProgressBar()
        {
            progressBar2.Visible = true;
        }
        private void HolidaysReceiveHandler(object sender, List<DateTime> e)
        {
            addHolidaysToCalendar(e);
            showCalendar();
        }
        private void RequestHolidaysUpdatedHandler(object sender, List<Ferie> e)
        {
            showTableHolidays(e);
        }
        private void showAlreadyRequestedMessage(string day)
        {
            label3.Text = "Hai già effettuato la richiesta per giorno " + day;
            label3.Visible = true;
        }
        private void showMessageRequestSendSuccess()
        {
            label3.Text = "Richiesta di ferie inoltrata con successo!";
            progressBar2.Visible = false;
            label3.Visible = true;
        }
        private void showMessageRequestSendFailed()
        {
            label3.Text = "Impossibile inoltrare la richiesta.";
            progressBar2.Visible = false;
            label3.Visible = true;
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
                    showAlreadyRequestedMessage(date.ToString("d"));

                }
                else if( ! d.isHolidayAccepted(date) && ! IsWeekend(date) && date > DateTime.Now )
                {
                    dateSelected = date;
                    showFormSendHolidayRequest(dateSelected.ToString("d"));

                } else {
                    hideFormSendHolidayRequest();
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
                hideFormSendHolidayRequest();
                showHolidaysProgressBar();

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
                    showMessageRequestSendSuccess();
                }
                else
                {
                    showMessageRequestSendFailed();
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
                showMessageRequestSendFailed();
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
