using System.Data.OleDb;
using Tsena_Antananarivo.NET.Data;

namespace Tsena_Antananarivo.NET.Models.Contrats;

public class Contrat
{

    public int id_contrat, id_locataire, id_box;
    public DateTime debut {get;}
    public DateTime fin {get;}
    public bool reglee;

    public Contrat () {}

    public Contrat (int id_contrat, DateTime debut, DateTime fin, int id_locataire, int id_box, bool reglee) {
        
        this.id_contrat = id_contrat;
        this.debut = debut;
        this.fin = fin;
        this.id_locataire = id_locataire;
        this.id_box = id_box;
        this.reglee = reglee;
    }


    private static List <Contrat> constructs (OleDbDataReader reader) {

        List <Contrat> liste = new List<Contrat> ();
        while (reader.Read ()) {

            int id_contrat = reader.GetInt32 (reader.GetOrdinal("id_contrat"));
            DateTime debut = reader.GetDateTime (reader.GetOrdinal("debut"));
            DateTime fin = reader.GetDateTime (reader.GetOrdinal("fin"));
            int id_locataire = reader.GetInt32 (reader.GetOrdinal("id_locataire"));
            int id_box = reader.GetInt32 (reader.GetOrdinal("id_box"));
            bool reglee = reader.GetBoolean (reader.GetOrdinal("reglee"));

            Contrat cd = new Contrat (id_contrat, debut, fin, id_locataire, id_box, reglee);
            liste.Add (cd);
        }   
        reader.Close ();
        return liste;
    }

    private static Contrat? construct (OleDbDataReader reader) {

        Contrat? contrat = null;
        while (reader.Read ()) {

            int id_contrat = reader.GetInt32 (reader.GetOrdinal("id_contrat"));
            DateTime debut = reader.GetDateTime (reader.GetOrdinal("debut"));
            DateTime fin = reader.GetDateTime (reader.GetOrdinal("fin"));
            int id_locataire = reader.GetInt32 (reader.GetOrdinal("id_locataire"));
            int id_box = reader.GetInt32 (reader.GetOrdinal("id_box"));
            bool reglee = reader.GetBoolean (reader.GetOrdinal("reglee"));

            contrat = new Contrat (id_contrat, debut, fin, id_locataire, id_box, reglee);
        }   
        reader.Close ();
        return contrat;
    }



    public List <Box> get_boxLocataire (int id_locataire, Util_DB udb) {
        
        udb.Connect();
        List <Box> list = new List <Box> ();

        string requete = $@"
            SELECT DISTINCT
                b.*
            FROM contrats AS c
            INNER JOIN box AS b ON c.id_box = b.id_box
            WHERE c.id_locataire = {id_locataire}
        ";

        // Console.WriteLine(requete);

        try {
            OleDbDataReader reader = udb.read_data (requete);
            list = Box.constructs (reader);
            reader.Close();
        }
        catch (System.Exception) {
            throw;
        }

        return list;
    }




    public static Contrat? get_by (int id_locataire, int id_box, Util_DB udb) {
        
        // ENCIENT CONTRATS ...
        string  requete = $"""
            SELECT TOP 1
                c.*
            FROM contrats AS c
            WHERE c.id_locataire = {id_locataire} AND id_box = {id_box}
            AND reglee = False
            ORDER BY debut ASC
        """;

        
        udb.Connect ();
        try {
            OleDbDataReader reader = udb.read_data (requete);
            return construct(reader);
        }
        catch (System.Exception) {
            throw;
        }
    }

    public static bool cheack_premierContrat (int id_locataire, int mois, int annees, Util_DB udb) {

        string requete = $"""
            SELECT 
                c.*
            FROM contrats AS c
            WHERE c.id_locataire = {id_locataire}
            AND c.debut <= DateSerial({annees}, {mois} + 1, 0);
        """;

        udb.Connect ();
        bool misy = false;
        try {
            OleDbDataReader reader = udb.read_data (requete);
            while (reader.Read()) {
                misy = true;
                break;
            }
            reader.Close ();
        }
        catch (System.Exception) {
            throw;
        }
        return misy;
    }
    public static void update_reglee (int id_contrat, Util_DB udb) {

        string requete = $"UPDATE contrats SET reglee = True WHERE id_contrat = {id_contrat}";
        udb.Connect ();
        udb.CUD_data (requete);
    }

    public static bool cheack_contrat (Contrat_detail contrat, DateTime periode, Util_DB udb) {

        string requete = $"""
            SELECT 
                c.*
            FROM contrats AS c
            WHERE c.id_locataire = {contrat.id_locataire}
            AND c.id_box = {contrat.id_box}
            AND c.id_contrat = {contrat.id_contrat}
            AND DateSerial({periode.Year}, {periode.Month}, 15) 
            BETWEEN DateSerial(Year(c.debut), Month(c.debut), 1) 
            AND DateSerial(Year(c.fin), Month(c.fin) + 1, 0);
        """;
        

        bool misy = false;
        udb.Connect ();
        try {
            OleDbDataReader reader = udb.read_data (requete);
            while (reader.Read ()) {
                misy = true;
                break;
            }
            reader.Close ();
        }
        catch (System.Exception) {
            throw;
        }

        return misy;
    }
        

}
