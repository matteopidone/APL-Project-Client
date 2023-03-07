using System;

namespace APL_Project_Client.Model;
public class Dipendente
{ //Farlo come classe astratta, poi dividere in Utente e Collega
    public string nome;
    public string cognome;
    public string email;

    private List<Ferie> listFerie;
    private List<Ferie> listRequest;

    public event EventHandler<List<DateTime>> HolidaysReceived;
    public event EventHandler<DateTime> RequestHolidaysUpdated;
    public Dipendente(string nome, string cognome, string email)
	{
        this.nome = nome;
        this.cognome = cognome; 
        this.email = email;
        this.listRequest = new List<Ferie>();// questa qui andrà sostituita da una fetch
        this.listFerie= new List<Ferie>();

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
        if( listFerie.Count == 0 )
        {
            //chiamata http che valorizza e ritorna, poi aggiorno la lista di ferie
            this.listFerie.Add(new Ferie(11, 3, 2023, "mot"));
            this.listFerie.Add(new Ferie(14, 3, 2023, "mot"));
            this.listFerie.Add(new Ferie(17, 3, 2023, "mot"));

            await Task.Delay(3000);
            if( HolidaysReceived != null)
            {
                HolidaysReceived(this, getHolidaysDays());
            }
            return true;
        } else
        {
            await Task.Delay(3000);
            this.listFerie.Add(new Ferie(11, 3, 2023, "mot"));
            this.listFerie.Add(new Ferie(14, 3, 2023, "mot"));
            this.listFerie.Add(new Ferie(17, 3, 2023, "mot"));
            if (HolidaysReceived != null)
            {
                HolidaysReceived(this, getHolidaysDays());
            }
            return true;
        }
    }

    public async Task<bool> sendHolidayRequest(DateTime date)
    {
        await  Task.Delay(4000);
        bool updated = true;
        if (updated)
        {
            Ferie f = new Ferie(date.Day, date.Month, date.Year, "Motivation");
            listRequest.Add(f);
            if (RequestHolidaysUpdated != null)
            {
                RequestHolidaysUpdated(this, date);
            }
            return true;
        }
        return false;
    }

    public bool isGiornoFerie(DateTime date)
    {
        if( listFerie.Count != 0)
        {
        return this.listFerie.Any(holiday => holiday.date.Year == holiday.date.Year && holiday.date.Month == holiday.date.Month && holiday.date.Day == date.Day);
        }
        return false;
    }

    public List<Dipendente> getDipendentiPresenti(DateTime date)
    {
        return new List<Dipendente>();
    } 

}
