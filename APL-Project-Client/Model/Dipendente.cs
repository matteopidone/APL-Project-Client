using System;

namespace APL_Project_Client.Model;
public class Dipendente
{ //Farlo come classe astratta, poi dividere in Utente e Collega
    public string nome;
    public string cognome;
    public string email;
    private List<Ferie> listFerie;
    public Dipendente(string nome, string cognome, string email)
	{
        this.nome = nome;
        this.cognome = cognome; 
        this.email = email;

	}

    public List<DateTime> getGiorniFerie()
    {
        if( listFerie != null )
        {
            //Thread.Sleep(3000);
            return new List<DateTime>();
            //chiamata http che valorizza e ritorna
        } else
        {
            //Thread.Sleep(3000);
            /*foreach (Ferie ferie in listFerie)
            {            
            }*/
            List<DateTime> d = new List<DateTime>();
            d.Add(new DateTime(2023, 3, 11));
            d.Add(new DateTime(2023, 3, 14));
            d.Add(new DateTime(2023, 3, 16));
            return d;
        }
    }

    public bool isGiornoFerie(DateTime date)
    {
        if( listFerie != null)
        {
        return !this.listFerie.Any(holiday => holiday.date.Year == holiday.date.Year && holiday.date.Month == holiday.date.Month && holiday.date.Day == date.Day);
        }
        return false;
    }

    public List<Dipendente> getDipendentiPresenti(DateTime date)
    {
        return new List<Dipendente>();
    } 

}
