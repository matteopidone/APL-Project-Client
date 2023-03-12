using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APL_Project_Client.Model;
public enum StatoFerie
{
    Richieste,
    Accettate,
    Rifiutate
}
public interface IFerieState
{
    void ApprovaFerie(Ferie ferie);
    void RifiutaFerie(Ferie ferie);
    StatoFerie GetStato();
}

public class FerieRichieste : IFerieState
{
    public void ApprovaFerie(Ferie ferie)
    {
        ferie.Stato = new FerieAccettate();
    }

    public void RifiutaFerie(Ferie ferie)
    {
        ferie.Stato = new FerieRifiutate();
    }
    public StatoFerie GetStato()
    {
        return StatoFerie.Richieste;
    }
}

public class FerieAccettate : IFerieState
{
    public void ApprovaFerie(Ferie ferie)
    {
        throw new InvalidOperationException("Le ferie sono già state accettate");
    }

    public void RifiutaFerie(Ferie ferie)
    {
        ferie.Stato = new FerieRifiutate();
    }
    public StatoFerie GetStato()
    {
        return StatoFerie.Accettate;
    }
}

public class FerieRifiutate : IFerieState
{
    public void ApprovaFerie(Ferie ferie)
    {
        ferie.Stato = new FerieAccettate();
    }

    public void RifiutaFerie(Ferie ferie)
    {
        throw new InvalidOperationException("Le ferie sono già state rifiutate");
    }
    public StatoFerie GetStato()
    {
        return StatoFerie.Rifiutate;
    }
}

public class Ferie
{
    private IFerieState stato;
    public DateTime date { get; set; }
    public string motivation { get; set; }
    public static int ferieRimanenti;
    public static int ferieUtilizzate;
    //Proprietà che serve al componente che mette in tabella le richieste pendenti e rifiutate
    public string Esito
    {
        get
        {
            if (stato.GetStato() is StatoFerie.Richieste)
            {
                return ("RICHIESTA");
            }
            else if (stato.GetStato() is StatoFerie.Accettate)
            {
                return ("ACCETTATA");
            }
            else if (stato.GetStato() is StatoFerie.Rifiutate)
            {
                return ("RIFIUTATA");
            }
            return ("null");
        }
    }


    public Ferie(int day, int month, int year, string motivation)
    {
        stato = new FerieRichieste();
        date = new DateTime(year, month, day);
        this.motivation = motivation;
    }

    public void ApprovaFerie()
    {
        stato.ApprovaFerie(this);
    }

    public void RifiutaFerie()
    {
        stato.RifiutaFerie(this);
    }

    public IFerieState Stato
    {
        set { stato = value; }
    }
}


