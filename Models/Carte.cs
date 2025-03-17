using Tsena_Antananarivo.NET.Data;

namespace Tsena_Antananarivo.NET.Models;

public class Carte
{
    
    public List <Marchee> marchees {get; private set; }
    public List <Box> boxs {get; private set;}
    public List <Locataire> locataires {get; private set;}
    public string? mois {get; set;}
    public int annees {get; set;}

    public Carte () {
        this.boxs = new List<Box> ();
        this.marchees = new List<Marchee> ();
        this.locataires = new List<Locataire> ();
    }

    public void get_carte (int mois, int annees, Util_DB udb) {

        // NETTOYER _____________
        this.marchees.Clear();
        this.locataires.Clear();
        this.boxs.Clear();

        boxs = Box.get_all(mois, annees, udb);
        locataires = Locataire.get_trosa (mois, annees, udb);
        marchees = Marchee.get_all(udb);

    }


}
