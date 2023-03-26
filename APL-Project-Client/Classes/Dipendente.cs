using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;

namespace APL_Project_Client.Classes;
public class Dipendente
{ 
    public string nome;
    public string cognome;
    public string descrizione;
    public string email;
    public string token;

    // Lista di richieste di ferie accettate.
    private List<Ferie> listHolidaysAccepted;
    
    // Lista di richieste di ferie in attesa.
    private List<Ferie> listRequestPending;
    
    // Lista di richiese di ferie rifiutate.
    private List<Ferie> listHolidaysRefused;

    // Evento "Le date delle richieste ACCETTATE sono state aggiornate".
    public event EventHandler<List<DateTime>> HolidaysAcceptedReceived;

    // Evento "Le richieste IN ATTESA e RIFIUTATE sono state aggiornate".
    public event EventHandler<List<Ferie>> HolidaysPendingUpdated;
    
    public Dipendente(string nome, string cognome, string email, string descrizione, string token)
	{
        this.nome = nome;
        this.cognome = cognome; 
        this.email = email;
        this.descrizione = descrizione;
        this.token = token;
        listRequestPending = new List<Ferie>();
        listHolidaysAccepted= new List<Ferie>();
        listHolidaysRefused = new List<Ferie>();

    }

    // Metodo che interroga il server per il login dell'utente.
    public static async Task<LoginAPIResult> loginUser(string email, string password)
    {
        // Stuttura che conterrà la risposta del server.
        LoginAPIResult r;
        
        using ( HttpClient client = new HttpClient() )
        {
            // Inserisco i parametri della richiesta.
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "email", email }, { "password", password }, { "role", "0" } };
            string jsonRequest = JsonConvert.SerializeObject(parameters);
            HttpContent content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");
        
            // Invio la richiesta.
            var response = await client.PostAsync("http://localhost:9000/api/login", content);

            if( response.StatusCode == System.Net.HttpStatusCode.BadRequest ) 
            {
                //Se lo status code è 400, login fallito.
                r = new LoginAPIResult();
                r.found = false;
                return r;
            }
            if (!response.IsSuccessStatusCode)
            {
                // Se lo status code è diverso da 200.
                throw new HttpRequestException("Errore, contatta il tuo datore.");
            }
            // Prendo il contenuto della risposta e lo torno al Form.
            string result = await response.Content.ReadAsStringAsync();
            r = JsonConvert.DeserializeObject<LoginAPIResult>(result);
            return r;
        }
        

    }
    // Metodo che ricerca tutte le ferie (pendenti, accettate, rifiutate e torna al chiamante un boooleano.
    public async Task<bool> fetchHolidays()
    {
        HttpResponseMessage response;
        using ( HttpClient client = new HttpClient() )
        {
            // Costruisco la richiesta, inserendo il token per autenticarla.
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            UriBuilder uriBuilder = new UriBuilder("http://localhost:9000/api/getHolidays");
            uriBuilder.Query = "email=" + email;
            response = await client.GetAsync(uriBuilder.ToString());

            // Se la richiesta non è andata a buon fine.
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException("Errore nel caricamento delle tue ferie, contatta il tuo datore.");
            }

            // Leggo il contenuto della risposta.
            string content = await response.Content.ReadAsStringAsync();

            //Leggo il JSON di risposta deserializzandolo nel tipo appropriato.
            List<getHolidaysAPIResult> listHolidaysReceived = JsonConvert.DeserializeObject<List<getHolidaysAPIResult>>(content);
        
            // Utilizzo la risposta del server per costruire rispettivamente il listato di ferie Richieste/Pendenti, Accettate e Rifiutate. 
            if(listHolidaysReceived != null)
            {
                foreach(getHolidaysAPIResult holiday in listHolidaysReceived)
                {
                    Ferie f = new Ferie(holiday.day, holiday.month, holiday.year, holiday.message);
                
                    switch (holiday.type)
                    {
                        case HolidayType.Pending :
                            // Aggiungo nel listato di ferie Richieste/Pendenti.
                            listRequestPending.Add(f);
                            break;

                        case HolidayType.Accepted :
                            // Aggiungo nel listato di ferie Accettate.
                            f.HolidayApproved();
                            listHolidaysAccepted.Add(f);
                            break;

                        case HolidayType.Refused :
                            // Aggiungo nel listato di ferie Rifiutate.
                            f.HolidayRefused();
                            listHolidaysRefused.Add(f);
                            break;

                    }

                }
            }
            // Se gli event handler sono valorizzati, lancio gli eventi.
            if( HolidaysAcceptedReceived != null)
            {
                // Evento che notifica l'aggiornamento delle ferie Accettate.
                HolidaysAcceptedReceived(this, getDateHolidaysAccepted());
            }
            if (HolidaysPendingUpdated != null)
            {
                // Evento che notifica l'aggiornamento delle ferie Richieste/Pendenti e Rifiutate.
                HolidaysPendingUpdated(this, getHolidaysPendingAndRefused());
            }
            return true;
        }
       
    }

    // Metodo che permette di inviare una richiesta di ferie (Pendente).
    public async Task<bool> sendHolidayRequest(DateTime date, string motivation)
    {
        HttpResponseMessage response;
        using ( HttpClient client = new HttpClient() )
        {
            // Inserisco il token per autenticare la richiesta.
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Inserisco i parametri della richiesta.
            Dictionary<string, object> parameters = new Dictionary<string, object> { { "email", email }, { "year", date.Year }, { "month", date.Month }, { "day", date.Day }, { "message", motivation } };
            string jsonRequest = JsonConvert.SerializeObject(parameters);
            HttpContent content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");
        
            // Invio la richiesta.
            response = await client.PostAsync("http://localhost:9000/api/insertHoliday", content);
        
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException("Errore nell'invio della richiesta, contatta il tuo datore.");
            }

            string Resultcontent = await response.Content.ReadAsStringAsync();
        
            //Leggo il JSON di risposta deserializzandolo nel tipo appropriato.
            insertHolidayAPIResult json = JsonConvert.DeserializeObject<insertHolidayAPIResult>(Resultcontent);
        
            // Se l'invio della richiesta ha successo, lo inserisco nel listato esistente
            // e notifico gli handler associati (nel nostro caso il form Home).
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
    }

    // Metodo che torna una lista con i giorni di ferie accettati.
    private List<DateTime> getDateHolidaysAccepted()
    {
        List<DateTime> d = new List<DateTime>();
        if (listHolidaysAccepted != null)
        {
            foreach (Ferie holiday in listHolidaysAccepted)
            {
                d.Add(holiday.date);
            }

        }
        return d;

    }

    // Metodo che indica se per quella data sono in ferie (Richiesta Accettata).
    public bool isHolidayAccepted(DateTime date)
    {
        if( listHolidaysAccepted.Count != 0)
        {
        return listHolidaysAccepted.Any(holiday => holiday.date.Year == holiday.date.Year && holiday.date.Month == holiday.date.Month && holiday.date.Day == date.Day);
        }
        return false;
    }
    // Metodo che indica se per quella data ho fatto richiesta di ferie (Richiesta Pendente).
    public bool isHolidayPending(DateTime date)
    {
        if (listRequestPending.Count != 0)
        {
           return listRequestPending.Any(holiday => holiday.date.Year == holiday.date.Year && holiday.date.Month == holiday.date.Month && holiday.date.Day == date.Day);
        }
        return false;
    }
    // Metodo che torna una lista di ferie Richieste/Pendenti e Rifiutate.
    private List<Ferie> getHolidaysPendingAndRefused()
    {
        return listRequestPending.OrderBy(f => f.date).Concat(listHolidaysRefused.OrderBy(f => f.date)).ToList();
    }
}
