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
            label4.Text = d.descrizione;
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
        private void showTableHolidays()
        {
            progressBar3.Visible = false;
            dataGridView1.Visible = true;
        }
        private void hideTableHolidays()
        {
            progressBar3.Visible = true;
            dataGridView1.Visible = false;
        }
        private void showFormSendHolidayRequest(string date)
        {
            label3.Text = "Vuoi procedere alla richiesta per giorno " + date + "?";
            button2.Visible = true;
            label3.Visible = true;
            label6.Visible = true;
            textBox1.Visible = true;
        }

        private void hideFormSendHolidayRequest()
        {
            label3.Text = "";
            button2.Visible = false;
            textBox1.Text = "";
            textBox1.Visible = false;
            label6.Visible= false;
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
            progressBar3.Visible = false;
            // Inserisco gli elementi nella tabella.
            dataGridView1.DataSource = e;
            showTableHolidays();
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
            progressBar3.Visible = false;
        }
        private void showMessageRequestSendFailed()
        {
            label3.Text = "Impossibile inoltrare la richiesta.";
            progressBar2.Visible = false;
            label3.Visible = true;
            progressBar3.Visible = false;
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
                showCalendar();
                // Inserisco una tabella vuota.
                dataGridView1.DataSource = new List<Ferie>();
                showTableHolidays();
            }
            catch (InvalidOperationException ex )
            {
                MessageBox.Show("Errore :" + ex.Message +"\nContattare il tuo datore di lavoro", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                showCalendar();
                // Inserisco una tabella vuota.
                dataGridView1.DataSource = new List<Ferie>();
                showTableHolidays();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errore generico: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                showCalendar();
                // Inserisco una tabella vuota.
                dataGridView1.DataSource = new List<Ferie>();
                showTableHolidays();
            }
        }

        private void Home_Load(object sender, EventArgs e)
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

        private async void sendHolidayRequest(string motivation)
        {
            hideFormSendHolidayRequest();
            hideTableHolidays();
            showHolidaysProgressBar();
            progressBar3.Visible = true;
            bool response = false;
            try
            {
                await semaphoreSendRequest.WaitAsync();
                response = await d.sendHolidayRequest(dateSelected, motivation);
                if (response)
                {
                    showMessageRequestSendSuccess();
                }
                else
                {
                    showMessageRequestSendFailed();
                }
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show("Errore nella richiesta: " + ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                showMessageRequestSendFailed();
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errore generico: " + ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                showMessageRequestSendFailed();
                return;
            }
            finally
            {
                semaphoreSendRequest.Release();
                showTableHolidays();
            }
        }

        private void button2_Click(object sender, EventArgs e)
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

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if(e.ColumnIndex == 2)
            {
                if (dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == "RIFIUTATA")
                {
                    e.CellStyle.ForeColor = Color.Red;
                }

            }
        }
    }
}
