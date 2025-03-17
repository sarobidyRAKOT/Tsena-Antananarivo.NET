using System.Data.OleDb;
using Tsena_Antananarivo.NET.Data;

namespace Tsena_Antananarivo.NET.Models.paiements;

public class Tarif
{

    int id_marchee;
    int mois;
    int annees;
    public double tarif {get; private set;} = 0;

    private Tarif (int id_marchee, int mois, int annees) {
        this.id_marchee = id_marchee;
        this.mois = mois;
        this.annees = annees;
    }


    public static Tarif get_tarif (int id_marchee, int mois, int annees, Util_DB udb) {

        Tarif tarif = new Tarif (id_marchee, mois, annees);
        string requete = $"""
            SELECT 
                IIf(tarif IS NULL, 0, tarif) AS t
            FROM tarif
            WHERE id_marchee = {id_marchee}
            AND DateSerial({annees}, {mois}, 15) 
            BETWEEN DateSerial(Year(debut_periode), Month(debut_periode), 1) 
            AND DateSerial(Year(fin_periode), Month(fin_periode) + 1, 0);
        """;
        
        udb.Connect ();
        try {
            OleDbDataReader reader = udb.read_data (requete);

            while (reader.Read ()) {
                tarif.tarif = reader.GetDouble (reader.GetOrdinal("tarif"));
                break;
            }
            reader.Close ();
        }
        catch (System.Exception) { 
            throw;
        }

        return tarif;
    }

    public double get_montant (double montant) {
        double tarif_calcule = (montant * this.tarif) / 100;
        return montant + tarif_calcule;
    }
}
