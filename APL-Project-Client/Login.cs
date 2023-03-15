using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using System.Text;
using Newtonsoft.Json;
using APL_Project_Client.Model;

namespace APL_Project_Client
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string email = this.textBox1.Text;
            string password = this.textBox2.Text;
            sendLoginRequest(email, password);
        }
        private async void sendLoginRequest(string email, string password)
        {
            //byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            //byte[] hashedBytes = new SHA256Managed().ComputeHash(passwordBytes);
            //string hashedPassword = Convert.ToBase64String(hashedBytes);
            string hashedPassword = password;

            bool isValidEmail = Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");

            if (email.Length != 0 && password.Length != 0 && isValidEmail)
            {
                showProgressBarLogin();
                LoginAPIResult result;

                try
                {
                    result = await Dipendente.loginUser(email, password);
                }
                catch (HttpRequestException ex)
                {
                    MessageBox.Show("Errore nella richiesta: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    hideProgressBarLogin();
                    return;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Errore generico: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    hideProgressBarLogin();
                    return;
                }

                if (result.found)
                {
                    Home homeForm = new Home(new Dipendente(result.name, result.surname, result.email));
                    homeForm.Show();
                    Hide();
                }
                else
                {
                    MessageBox.Show("Username o Password errati, riprova o contatta il tuo datore di lavoro", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    hideProgressBarLogin();
                    return;
                }

            }
            else
            {
                MessageBox.Show("Inserisci tutti i dati o inserisci una mail valida", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                hideProgressBarLogin();
                return;

            }
        }
        private void showProgressBarLogin()
        {
            progressBar1.Style = ProgressBarStyle.Marquee;
            progressBar1.MarqueeAnimationSpeed = 30;
            progressBar1.Visible = true;
        }
        private void hideProgressBarLogin()
        {
            progressBar1.Value = 0;
            progressBar1.Visible = false;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void maskedTextBox1_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}