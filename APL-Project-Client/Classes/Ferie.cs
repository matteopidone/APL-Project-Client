using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APL_Project_Client.Classes;
// Queste classi definiscono lo stato di una richiesta di ferie (Accettata, Rifiutata, In attesa).
// Viene utilizzato il pattern "State".

// Interfaccia IFerieState.
public interface IFerieState
{
    void HolidayApproved(Ferie ferie);
    void HolidayRefused(Ferie ferie);
    HolidayType getType();
}

// Stato "Richiesta di ferie in attesa di essere accettata".
public class FerieRichieste : IFerieState
{
    public void HolidayApproved(Ferie ferie)
    {
        ferie.Stato = new FerieAccettate();
    }

    public void HolidayRefused(Ferie ferie)
    {
        ferie.Stato = new FerieRifiutate();
    }
    public HolidayType getType()
    {
        return HolidayType.Pending;
    }
}

// Stato "Richiesta di ferie accettata".
public class FerieAccettate : IFerieState
{
    public void HolidayApproved(Ferie ferie)
    {
        // Non si può accettare una richiesta già accettata.
        throw new InvalidOperationException("Le ferie sono già state accettate");
    }

    public void HolidayRefused(Ferie ferie)
    {
        ferie.Stato = new FerieRifiutate();
    }
    public HolidayType getType()
    {
        return HolidayType.Accepted;
    }
}

// Stato "Richiesta di ferie rifiutata".
public class FerieRifiutate : IFerieState
{
    public void HolidayApproved(Ferie ferie)
    {
        ferie.Stato = new FerieAccettate();
    }

    public void HolidayRefused(Ferie ferie)
    {
        // Non si può rifiutare una richiesta già rifiutata.
        throw new InvalidOperationException("Le ferie sono già state rifiutate");
    }
    public HolidayType getType()
    {
        return  HolidayType.Refused;
    }
}

// Classe Ferie.
public class Ferie
{
    // Stato della richiesta.
    private IFerieState stato;

    // Data della richiesta.
    public DateTime date { get; set; }

    // Motivazione.
    public string motivatione { get; set; }

    // Proprietà che serve al componente del form Home (DataGridView)
    // che mette in tabella le richieste in attesa e rifiutate.
    public string Esito
    {
        get
        {
            if (stato.getType() is HolidayType.Pending)
            {
                return ("IN ATTESA");
            }
            else if (stato.getType() is HolidayType.Accepted)
            {
                return ("ACCETTATA");
            }
            else if (stato.getType() is HolidayType.Refused)
            {
                return ("RIFIUTATA");
            }
            return ("null");
        }
    }

    // Costruttore.
    public Ferie(int day, int month, int year, string motivatione)
    {
        stato = new FerieRichieste();
        date = new DateTime(year, month, day);
        this.motivatione = motivatione;
    }

    // Setter dello stato.
    public IFerieState Stato
    {
        set { stato = value; }
    }

    // Approva richiesta.
    public void HolidayApproved()
    {
        stato.HolidayApproved(this);
    }

    // Rifiuta richiesta.
    public void HolidayRefused()
    {
        stato.HolidayRefused(this);
    }

}


