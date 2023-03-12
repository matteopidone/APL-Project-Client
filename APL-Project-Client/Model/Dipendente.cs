using Microsoft.VisualBasic;
using Newtonsoft.Json;
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
        try
        {
            var client = new HttpClient();
            var uriBuilder = new UriBuilder("http://151.97.115.169:9000/api/getHolidays");
            uriBuilder.Query = "email=" + email;
            var response = await client.GetAsync(uriBuilder.ToString());
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                dynamic json = JsonConvert.DeserializeObject(content);

                foreach(var holiday in json)
                {
                    if (holiday.type == StatoFerie.Richieste)
                    {
                        int day = holiday.day;
                        int month = holiday.month;
                        int year = holiday.year;
                        string motivation = holiday.message;
                        Ferie f = new Ferie(day, month, year, motivation);
                        listRequest.Add(f);
                    } 
                    else if(holiday.type == StatoFerie.Accettate)
                    {
                        int day = holiday.day;
                        int month = holiday.month;
                        int year = holiday.year;
                        string motivation = holiday.message;
                        Ferie f = new Ferie(day, month, year, motivation);
                        f.ApprovaFerie();
                        listFerie.Add(f);
                    }
                    else if (holiday.type == StatoFerie.Rifiutate)
                    {
                        int day = holiday.day;
                        int month = holiday.month;
                        int year = holiday.year;
                        string motivation = holiday.message;
                        Ferie f = new Ferie(day, month, year, motivation);
                        f.RifiutaFerie();
                        listRequestRefused.Add(f);
                    }
                }

            }
            else
            {

            }

        }
        catch (HttpRequestException ex)
        {
            MessageBox.Show("Errore nella richiesta: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Errore generico: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        if( HolidaysReceived != null)
        {
            HolidaysReceived(this, getHolidaysDays());
        }
        if (RequestHolidaysUpdated != null)
        {
            RequestHolidaysUpdated(this, getHolidaysRequestedAndRefused());
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
