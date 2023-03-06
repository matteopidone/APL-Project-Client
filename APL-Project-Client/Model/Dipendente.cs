using System;

namespace APL_Project_Client.Model;
public class Dipendente
{ //Farlo come classe astratta, poi dividere in Utente e Collega
    public string nome;
    public string cognome;
    public string email;
    private List<Ferie> listFerie;
    public event EventHandler<List<DateTime>> HolidaysReceived; 
    public Dipendente(string nome, string cognome, string email)
	{
        this.nome = nome;
        this.cognome = cognome; 
        this.email = email;

	}

    private List<DateTime> getHolidaysDays()
    {
        List<DateTime> d = new List<DateTime>();
        if(listFerie != null) 
        {   
            foreach( Ferie holiday in listFerie )
            {
                d.Add(holiday.date);
            }
        
        }
        return d;
    
    } 

    public async Task<bool> fetchHolidays()
    {
        if( listFerie == null )
        {
            //chiamata http che valorizza e ritorna, poi aggiorno la lista di ferie
            //this.listFerie = ecc
            List<DateTime> d = new List<DateTime>();
            d.Add(new DateTime(2023, 3, 11));
            d.Add(new DateTime(2023, 3, 14));
            d.Add(new DateTime(2023, 3, 17));

            await Task.Delay(3000);
            if( HolidaysReceived != null)
            {
                HolidaysReceived(this, d);
            }
            return true;
        } else
        {
            await Task.Delay(3000);
            //List<DateTime> = this.getHolidaysDays();
            List<DateTime> d = new List<DateTime>();
            d.Add(new DateTime(2023, 3, 11));
            d.Add(new DateTime(2023, 3, 14));
            d.Add(new DateTime(2023, 3, 17));
            if (HolidaysReceived != null)
            {
                HolidaysReceived(this, d);
            }
            return true;
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
