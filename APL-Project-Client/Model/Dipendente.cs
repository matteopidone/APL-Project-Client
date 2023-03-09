using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;

namespace APL_Project_Client.Model;
public class Dipendente
{ //Farlo come classe astratta, poi dividere in Utente e Collega
    public string nome;
    public string cognome;
    public string email;

    private List<Ferie> listFerie;
    private List<Ferie> listRequest;
    private List<Ferie> listRequestRefused;

    public event EventHandler<List<DateTime>> HolidaysReceived;
    public event EventHandler<List<Ferie>> RequestHolidaysUpdated;
    public Dipendente(string nome, string cognome, string email)
	{
        this.nome = nome;
        this.cognome = cognome; 
        this.email = email;
        this.listRequest = new List<Ferie>();
        this.listFerie= new List<Ferie>();
        this.listRequestRefused = new List<Ferie>();

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
        //chiamata http che valorizza e ritorna, poi aggiorno la lista di ferie
        //Sulla base dello stato, li metto nella Lista corretta, successivamente richiamo i 3 eventi
        /*
        for (ogni elemento ricevuto)
        {
            int stato;
            if(stato == StatoFerie.Richieste)
            {
                Ferie f = new Ferie()
                listRequest.Add(f);
            } else if(stato == StatoFerie.Accettate)
            {
                Ferie f = new Ferie();
                f.ApprovaFerie()
                listFerie.Add(f);
            }
            else if (0 == StatoFerie.Rifiutate)
            {
                Ferie f = new Ferie();
                f.RifiutaFerie()
                listRefused.Add(f);s

            }

        }*/
        this.listFerie.Add(new Ferie(11, 3, 2023, "mot"));
        this.listFerie.Add(new Ferie(14, 3, 2023, "mot"));
        this.listFerie.Add(new Ferie(17, 3, 2023, "mot"));
        Ferie f1 = new Ferie(1, 3, 2023, "mot");
        f1.RifiutaFerie();
        Ferie f2 = new Ferie(1, 3, 2023, "mot");
        f2.RifiutaFerie();
        Ferie f3 = new Ferie(1, 3, 2023, "mot");
        f3.RifiutaFerie();
        this.listRequestRefused.Add(f1);
        this.listRequestRefused.Add(f2);
        this.listRequestRefused.Add(f3);

        this.listRequest.Add(new Ferie(21, 3, 2023, "mot"));
        this.listRequest.Add(new Ferie(22, 3, 2023, "mot"));
        this.listRequest.Add(new Ferie(23, 3, 2023, "mot"));

        await Task.Delay(3000);
        if( HolidaysReceived != null)
        {
            HolidaysReceived(this, getHolidaysDays());
        }
        //Togliere, solo test evento
        if (RequestHolidaysUpdated != null)
        {
            //Vengono unite la lista di ferie ancora richieste e quelle rifiutate, ordinate per data.
            RequestHolidaysUpdated(this, getHolidaysRequestedAndRefused());
            //RequestHolidaysUpdated(this, new List<Ferie>());
        }
        return true;
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
                RequestHolidaysUpdated(this, getHolidaysRequestedAndRefused());
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
    //Simile a quello sopra
    public bool RequestContainsDate(DateTime date)
    {
        if (listRequest.Count != 0)
        {
            return this.listRequest.Any(holiday => holiday.date.Year == holiday.date.Year && holiday.date.Month == holiday.date.Month && holiday.date.Day == date.Day);
        }
        return false;
    }

    public List<Dipendente> getDipendentiPresenti(DateTime date)
    {
        return new List<Dipendente>();
    }

    private List<Ferie> getHolidaysRequestedAndRefused()
    {
        return listRequest.OrderBy(f => f.date).Concat(listRequestRefused.OrderBy(f => f.date)).ToList();
    }
}
