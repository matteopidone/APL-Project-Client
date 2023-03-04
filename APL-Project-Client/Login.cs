using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using System.Text;

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
            String nomeUtente = this.textBox1.Text;
            String password = this.textBox2.Text;
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] hashedBytes = new SHA256Managed().ComputeHash(passwordBytes);
            string hashedPassword = Convert.ToBase64String(hashedBytes);

            bool isValidEmail = Regex.IsMatch(nomeUtente, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");

            if (nomeUtente.Length != 0 && password.Length != 0 && isValidEmail)
            {

                progressBar1.Style = ProgressBarStyle.Marquee;
                progressBar1.MarqueeAnimationSpeed = 30;
                progressBar1.Visible = true;
                

                using (var client = new HttpClient())
                {
                    //var parameters = new Dictionary<string, string> { { "nome_utente", nomeUtente }, { "password", hashedPassword } };
                    //var content = new FormUrlEncodedContent(parameters);
                    //var response = await client.PostAsync("https://api.example.com/login", content);
                    //var responseString = await response.Content.ReadAsStringAsync();
                    var responseString = "true";
                    if (responseString == "true")
                    {
                        // Apri il form di login completato
                        Home homeForm = new Home();
                        homeForm.Show();
                         this.Hide();
                    }
                    else
                    {
                        // Visualizza un messaggio di errore
                        MessageBox.Show("Username o Password errati, riprova o contatta il tuo datore di lavoro", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        progressBar1.Value = 0;
                        progressBar1.Visible = false;


                    }
                }

            } else
            {
                MessageBox.Show("Inserisci tutti i dati o inserisci una mail valida", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                progressBar1.Value = 0;
                progressBar1.Visible = false;

            }
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