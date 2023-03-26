using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APL_Project_Client.Classes;
public interface IFerieState
{
    void HolidayApproved(Ferie ferie);
    void HolidayRefused(Ferie ferie);
    HolidayType getType();
}

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

public class FerieAccettate : IFerieState
{
    public void HolidayApproved(Ferie ferie)
    {
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

public class FerieRifiutate : IFerieState
{
    public void HolidayApproved(Ferie ferie)
    {
        ferie.Stato = new FerieAccettate();
    }

    public void HolidayRefused(Ferie ferie)
    {
        throw new InvalidOperationException("Le ferie sono già state rifiutate");
    }
    public HolidayType getType()
    {
        return  HolidayType.Refused;
    }
}

public class Ferie
{
    // Stato della richiesta.
    private IFerieState stato;
    public DateTime date { get; set; }
    public string motivatione { get; set; }
    //Proprietà che serve al componente che mette in tabella le richieste pendenti e rifiutate
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
    public void HolidayApproved()
    {
        stato.HolidayApproved(this);
    }

    public void HolidayRefused()
    {
        stato.HolidayRefused(this);
    }

}


