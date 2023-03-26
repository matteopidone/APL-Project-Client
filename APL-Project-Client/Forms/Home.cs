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
        // Istanza del dipendente utilizzata nel form.
        Dipendente d;
        
        // Data selezionata dall'utente per inviare la richiesta.
        private DateTime dateSelected;

        // Istanza di un semaforo con una dimensione dello slot pari a 1.
        SemaphoreSlim semaphoreSendRequest = new SemaphoreSlim(1);

        // Costruttore.
        public Home(Dipendente d1)
        {
            InitializeComponent();

            // Associo l'istanza del dipendente.
            d = d1;
            
            // Valorizzo il messaggio di benvenuto e la descrizione del dipendente.
            label1.Text = "Benvenuto " + d.nome + " " + d.cognome;
            label4.Text = d.descrizione;
        }

        // Al load del form, invoco l'api che ritorna le informazioni
        // su tutte le richieste (ACCETTATE, IN ATTESA, RIFIUTATE).
        private void Home_Load(object sender, EventArgs e)
        {
            fetchAllHolidays();
        }

        private async void fetchAllHolidays()
        {
            // Definisco gli handler agli eventi esposti per popolare la Home.
            d.HolidaysAcceptedReceived += HolidaysReceiveHandler;
            d.HolidaysPendingUpdated += RequestHolidaysUpdatedHandler;
            try
            {
                // Metodo per la rierca di richieste.
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
            catch (InvalidOperationException ex)
            {
                MessageBox.Show("Errore :" + ex.Message + "\nContattare il tuo datore di lavoro", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        // Metodo che inoltra una richiesta di ferie.
        private async void sendHolidayRequest(string motivation)
        {
            hideFormSendHolidayRequest();
            hideTableHolidays();
            showHolidaysProgressBar();
            progressBar3.Visible = true;
            bool response = false;
            try
            {
                // Acquisico lo slot del semaforo.
                await semaphoreSendRequest.WaitAsync();
                // Inoltro la richiesta.
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
                // Rilascio lo slot.
                semaphoreSendRequest.Release();
                showTableHolidays();
            }
        }

        //Handler che, una volta che le richieste accettate sono state ricevute dal server, popola il calendario.
        private void HolidaysReceiveHandler(object sender, List<DateTime> e)
        {
            // Inserisco le date nel calendario e lo mostro. 
            addHolidaysToCalendar(e);
            showCalendar();
        }

        //Handler che, una volta che le richieste (pendenti e rifiutate) sono state ricevute dal server, popola il calendario.
        private void RequestHolidaysUpdatedHandler(object sender, List<Ferie> e)
        {
            progressBar3.Visible = false;
            // Inserisco gli elementi nella tabella e lo mostro.
            dataGridView1.DataSource = e;
            showTableHolidays();
        }

        private void monthCalendar1_DateChanged(object sender, DateRangeEventArgs e)
        {
            DateTime date = e.Start;
            // Se nessuno ha acquisito lo slot del semaforo,
            // ovvero se non si sta aspettando l'esito di un invio di una richiesta,
            // permetto di inserire una nuova richiesta.
            if (semaphoreSendRequest.CurrentCount == 1)
            {
                // Se ho già fatto richiesta per quel giorno e sono in attesa di esito,
                // mostro il messaggio "Hai già fatto richiesta".
                if (d.isHolidayPending(date))
                {
                    hideFormSendHolidayRequest();
                    showAlreadyRequestedMessage(date.ToString("d"));

                }
                // Se il dipendente non è in ferie quel giorno e se la data è valida
                // mostro il form di invio della richiesta.
                else if (!d.isHolidayAccepted(date) && !IsWeekend(date) && date > DateTime.Now)
                {
                    // Imposto la data selezionata.
                    dateSelected = date;
                    showFormSendHolidayRequest(dateSelected.ToString("d"));
                }
                else
                {
                    hideFormSendHolidayRequest();
                }

            }

        }

        // Gestisco la chiusura della Home e mostro un form di Login.
        private void button1_Click(object sender, EventArgs e)
        {
            bool response = true;
            if (response)
            {
                Login loginForm = new Login();
                loginForm.Show();
                Close();
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
            label6.Visible = false;
            label3.Visible = false;
        }
        private void showHolidaysProgressBar()
        {
            progressBar2.Visible = true;
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

        private bool IsWeekend(in DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
        }
        // Data in input una lista di date, valorizzo nel calendario le richieste ACCETTATE.
        private void addHolidaysToCalendar(in List<DateTime> dates)
        {
            foreach (DateTime date in dates)
            {
                monthCalendar1.AddBoldedDate(date);
                monthCalendar1.UpdateBoldedDates();
                monthCalendar1.Update();
            }
        }

        //Se la data è stata selezionata, inoltro la richiesta.
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
