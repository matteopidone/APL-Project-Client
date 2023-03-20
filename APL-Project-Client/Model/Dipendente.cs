using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;

namespace APL_Project_Client.Model;
public class Dipendente
{ //Farlo come classe astratta, poi dividere in Utente e Collega
    public string nome;
    public string cognome;
    public string email;
    public string token;
    // Lista di richieste di ferie accettate.
    private List<Ferie> listHolidaysAccepted;
    // Lista di richieste di ferie in attesa.
    private List<Ferie> listRequestPending;
    // Lista di richiese di ferie rifiutate.
    private List<Ferie> listHolidaysRefused;

    public event EventHandler<List<DateTime>> HolidaysAcceptedReceived;
    public event EventHandler<List<Ferie>> HolidaysPendingUpdated;
    public Dipendente(string nome, string cognome, string email, string token)
	{
        this.nome = nome;
        this.cognome = cognome; 
        this.email = email;
        this.token = token;
        listRequestPending = new List<Ferie>();
        listHolidaysAccepted= new List<Ferie>();
        listHolidaysRefused = new List<Ferie>();

    }

    private List<DateTime> getDateHolidaysAccepted()
    {
        List<DateTime> d = new List<DateTime>();
        if(listHolidaysAccepted != null) 
        {   
            foreach( Ferie holiday in listHolidaysAccepted )
            {
                d.Add(holiday.date);
            }
        
        }
        return d;
    
    }
    public static async Task<LoginAPIResult> loginUser(string email, string password)
    {
        LoginAPIResult r;
        

        HttpClient client = new HttpClient();
        Dictionary<string, string> parameters = new Dictionary<string, string> { { "email", email }, { "password", password } };
        string jsonRequest = JsonConvert.SerializeObject(parameters);
        HttpContent content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");
        var response = await client.PostAsync("http://localhost:9000/api/login", content);
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException("Errore, contatta il tuo datore.");
        }

        string result = await response.Content.ReadAsStringAsync();
        r = JsonConvert.DeserializeObject<LoginAPIResult>(result);
        return r;

    }

    public async Task<bool> fetchHolidays()
    {
        HttpResponseMessage response;
        HttpClient client = new HttpClient();

        // Inserisco il token per autenticare la richiesta.
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        UriBuilder uriBuilder = new UriBuilder("http://localhost:9000/api/getHolidays");
        uriBuilder.Query = "email=" + email;
        response = await client.GetAsync(uriBuilder.ToString());

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException("Errore nel caricamento delle tue ferie, contatta il tuo datore.");
        }
        string content = await response.Content.ReadAsStringAsync(); 
        List<getHolidaysAPIResult> listHolidaysReceived = JsonConvert.DeserializeObject<List<getHolidaysAPIResult>>(content);

        if(listHolidaysReceived != null)
        {
            foreach(getHolidaysAPIResult holiday in listHolidaysReceived)
            {
                Ferie f = new Ferie(holiday.day, holiday.month, holiday.year, holiday.message);
                
                switch (holiday.type)
                {
                    case StatoFerie.Richieste :
                        listRequestPending.Add(f);
                        break;

                    case StatoFerie.Accettate :
                        f.HolidayApproved();
                        listHolidaysAccepted.Add(f);
                        break;

                    case StatoFerie.Rifiutate :
                        f.HolidayRefused();
                        listHolidaysRefused.Add(f);
                        break;

                }

            }
        } 
        if( HolidaysAcceptedReceived != null)
        {
            HolidaysAcceptedReceived(this, getDateHolidaysAccepted());
        }
        if (HolidaysPendingUpdated != null)
        {
            HolidaysPendingUpdated(this, getHolidaysPendingAndRefused());
        }
        return true;
    }

    public async Task<bool> sendHolidayRequest(DateTime date, string motivation)
    {
        HttpResponseMessage response;
        HttpClient client = new HttpClient();
        // Inserisco il token per autenticare la richiesta.
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        Dictionary<string, object> parameters = new Dictionary<string, object> { { "email", email }, { "year", date.Year }, { "month", date.Month }, { "day", date.Day }, { "message", motivation } };
        string jsonRequest = JsonConvert.SerializeObject(parameters);
        HttpContent content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");
        response = await client.PostAsync("http://localhost:9000/api/insertHoliday", content);
        
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException("Errore nell'invio della richiesta, contatta il tuo datore.");
        }

        string Resultcontent = await response.Content.ReadAsStringAsync();
        insertHolidayAPIResult json = JsonConvert.DeserializeObject<insertHolidayAPIResult>(Resultcontent);
        if (json.result)
        {
            Ferie f = new Ferie(date.Day, date.Month, date.Year, motivation);
            listRequestPending.Add(f);
            if (HolidaysPendingUpdated != null)
            {
                HolidaysPendingUpdated(this, getHolidaysPendingAndRefused());
            }
            return true;
        }
        return false;
    }

    public bool isHolidayAccepted(DateTime date)
    {
        if( listHolidaysAccepted.Count != 0)
        {
        return listHolidaysAccepted.Any(holiday => holiday.date.Year == holiday.date.Year && holiday.date.Month == holiday.date.Month && holiday.date.Day == date.Day);
        }
        return false;
    }
    public bool isHolidayPending(DateTime date)
    {
        if (listRequestPending.Count != 0)
        {
            return listRequestPending.Any(holiday => holiday.date.Year == holiday.date.Year && holiday.date.Month == holiday.date.Month && holiday.date.Day == date.Day);
        }
        return false;
    }

    public List<Dipendente> getDipendentiPresenti(DateTime date)
    {
        return new List<Dipendente>();
    }

    private List<Ferie> getHolidaysPendingAndRefused()
    {
        return listRequestPending.OrderBy(f => f.date).Concat(listHolidaysRefused.OrderBy(f => f.date)).ToList();
    }
}
