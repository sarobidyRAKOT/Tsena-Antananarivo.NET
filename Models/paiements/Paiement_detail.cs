

using System.Data.OleDb;
using Tsena_Antananarivo.NET.Data;

namespace Tsena_Antananarivo.NET.Models.paiements;

public class Paiement_detail
{

    public int id_paiement {get;}
    int id_contrat, id_locataire, id_box;
    public int mois {get;} 
    public int annees {get;}
    public double reste {get; set;}
    public double payee {get;}
    public DateTime date_echeance { get;}

    public Paiement_detail (int id_paiement, int mois, int annees, double payee, double reste, int id_contrat, int id_locataire, int id_box, DateTime date_echeance) {
        this.id_paiement = id_paiement;
        this.mois = mois;
        this.annees = annees;
        this.payee = payee;
        this.reste = reste;
        this.id_contrat = id_contrat;
        this.id_locataire = id_locataire;
        this.id_box = id_box;
        this.date_echeance = date_echeance;
    }




    private static List <Paiement_detail> constructs (OleDbDataReader reader) {

        List <Paiement_detail> liste = new List<Paiement_detail> ();
        while (reader.Read ()) {

            int id_paiement = reader.GetInt32(reader.GetOrdinal("id_paiement"));
            int mois = reader.GetInt32(reader.GetOrdinal("mois"));
            int annees = reader.GetInt32(reader.GetOrdinal("annee"));
            double payee = reader.GetDouble(reader.GetOrdinal("payee"));
            double reste = reader.GetDouble(reader.GetOrdinal("reste"));
            int id_contrat = reader.GetInt32 (reader.GetOrdinal("id_contrat"));
            int id_locataire = reader.GetInt32 (reader.GetOrdinal("id_locataire"));
            int id_box = reader.GetInt32 (reader.GetOrdinal("id_box"));
            DateTime date_echeance = reader.GetDateTime (reader.GetOrdinal("date_echeance"));

            Paiement_detail cd = new Paiement_detail (id_paiement, mois, annees, payee, reste, id_contrat, id_locataire, id_box, date_echeance);
            liste.Add (cd);
        }   

        reader.Close ();
        return liste;
    }
    
    private static Paiement_detail? construct (OleDbDataReader reader) {

        while (reader.Read ()) {

            int id_paiement = reader.GetInt32(reader.GetOrdinal("id_paiement"));
            int mois = reader.GetInt32(reader.GetOrdinal("mois"));
            int annees = reader.GetInt32(reader.GetOrdinal("annee"));
            double payee = reader.GetDouble(reader.GetOrdinal("payee"));
            double reste = reader.GetDouble(reader.GetOrdinal("reste"));
            int id_contrat = reader.GetInt32 (reader.GetOrdinal("id_contrat"));
            int id_locataire = reader.GetInt32 (reader.GetOrdinal("id_locataire"));
            int id_box = reader.GetInt32 (reader.GetOrdinal("id_box"));
            DateTime date_echeance = reader.GetDateTime (reader.GetOrdinal("date_echeance"));

            return new Paiement_detail (id_paiement, mois, annees, payee, reste, id_contrat, id_locataire, id_box, date_echeance);
        }   

        reader.Close ();
        return null;
    }

    public static List <Paiement_detail> get_allNON_payee_par (int id_locataire, Util_DB udb) {

        string  requete = $"""
            SELECT 
                *
            FROM paiement_detail 
            WHERE id_locataire = {id_locataire}
            AND reste <> 0
            ORDER BY annee ASC, mois ASC,
            debut ASC
        """;

        List <Paiement_detail> liste = new List<Paiement_detail> ();
        udb.Connect ();
        try {
            OleDbDataReader reader = udb.read_data (requete);
            liste = constructs(reader);
        }
        catch (System.Exception) { 
            throw;
        }
        return liste;
    }
    

    public static Paiement_detail? get_NON_payeePar (int id_locataire, int id_box, Util_DB udb) {

        string  requete = $"""
            SELECT TOP 1
                *
            FROM paiement_detail 
            WHERE id_box = {id_box} AND id_locataire = {id_locataire}
            AND reste <> 0
            ORDER BY annee ASC, mois ASC,
            debut ASC
        """;

        udb.Connect ();
        try {
            OleDbDataReader reader = udb.read_data (requete);
            return construct (reader);
        }
        catch (System.Exception) { 
            throw;
        }
    }


    public static List <Paiement_detail> get_allNON_payee_OrderByNumpar (int id_locataire, Util_DB udb) {

        string  requete = $"""
            SELECT 
                pd.*
            FROM paiement_detail AS pd
            LEFT JOIN box AS b ON pd.id_box = b.id_box
            WHERE pd.id_locataire = {id_locataire}
            AND pd.reste <> 0
            ORDER BY b.numero ASC
        """;

        requete += "";

        List <Paiement_detail> liste = new List<Paiement_detail> ();
        udb.Connect ();
        try {
            OleDbDataReader reader = udb.read_data (requete);
            liste = constructs(reader);
        }
        catch (System.Exception) { 
            throw;
        }
        return liste;
    }

    // public static List <Paiement_detail> get_allNON_payeeOrderByNum (int id_locataire, int id_box, Util_DB udb) {

    //     string  requete = $"""
    //         SELECT 
    //             *
    //         FROM paiement_detail 
    //         WHERE id_box = {id_box} AND id_locataire = {id_locataire}
    //         AND reste <> 0
    //         ORDER BY id_box ASC
    //     """;

    //     List <Paiement_detail> liste = new List<Paiement_detail> ();
    //     udb.Connect ();
    //     try {
    //         OleDbDataReader reader = udb.read_data (requete);
    //         liste = constructs(reader);
    //     }
    //     catch (System.Exception) { 
    //         throw;
    //     }
    //     return liste;
    // }

}
