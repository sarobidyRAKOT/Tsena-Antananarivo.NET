using System.Data.OleDb;
using System.Drawing.Drawing2D;
using Tsena_Antananarivo.NET.Data;
using Tsena_Antananarivo.NET.Models.Contrats;
using Tsena_Antananarivo.NET.Models.paiements;


namespace Tsena_Antananarivo.NET.Models;


public class Locataire {

    public int id_locataire;
    public string nom { get; private set; }
    public List <Box> boxs = new List<Box> ();
    public double trosa {get; private set;} = 0;

    public Locataire (int id_locataire, string nom) {
        this.id_locataire = id_locataire;
        this.nom = nom;
    }
    // public Locataire () {}


    public static List <Locataire> get_All (Util_DB udb) {
        
        udb.Connect();
        List <Locataire> list = new List<Locataire> ();
        string requete = "SELECT * FROM locataire";
        Contrat contrat = new Contrat();


        try {        
            OleDbDataReader reader = udb.read_data (requete);
            while (reader.Read()) {
                int id = reader.GetInt32(0); // id_locataire
                string nom = reader.GetString(1); // nom locataire

                Locataire locataire = new Locataire(id, nom);
                locataire.boxs = contrat.get_boxLocataire(id_locataire:id, udb:udb);

                list.Add(locataire);
                // Console.WriteLine($"ato ____ {id} {nom}");
            }
            reader.Close();
        }
        catch (Exception ex) {
            Console.WriteLine($"{ex.Message}");
        }

        return list;
    }

    public static List <Locataire> get_trosa (int mois, int annees, Util_DB udb) {

        List <Locataire> liste = new List<Locataire> ();
        string  requete = $"""
            SELECT
                l.*,
                lp.reste AS reste
            FROM locataire AS l
            LEFT JOIN (
                SELECT
                    pd.id_locataire AS id_locataire,
                    sum (pd.reste) AS reste,
                    sum (pd.payee) AS payee
                FROM paiement_detail AS pd
                WHERE DateSerial(pd.annee, pd.mois, 1) <= DateSerial({annees}, {mois}, 1)
                GROUP BY pd.id_locataire
            ) AS lp ON l.id_locataire = lp.id_locataire
        """;

        udb.Connect ();
        try {
            OleDbDataReader reader = udb.read_data (requete);

            while (reader.Read()) {
                int id_locataire = reader.GetInt32 (reader.GetOrdinal("id_locataire"));
                string nom = reader.GetString (reader.GetOrdinal("nom"));
                double reste = reader.IsDBNull(reader.GetOrdinal("reste")) ? 0 : reader.GetDouble(reader.GetOrdinal("reste"));

                Locataire locataire = new Locataire (id_locataire, nom);
                locataire.trosa = reste;
                    
                liste.Add (locataire);
            }
            reader.Close ();
        }
        catch (System.Exception) {
            throw;
        }

        Paiement paiement = new Paiement (udb);
        foreach (Locataire l in liste) {
            l.trosa += paiement.montantDu_parLoc(l.id_locataire, mois, annees);
        }
        
        return liste;
    }
    

}
