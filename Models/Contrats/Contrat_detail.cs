
using System.Data.OleDb;
using Tsena_Antananarivo.NET.Data;

namespace Tsena_Antananarivo.NET.Models.Contrats;

public class Contrat_detail : Contrat
{

    public int mois {get; private set;}
    public int annees {get; private set;}

    public Contrat_detail () {}
    public Contrat_detail (int id_contrat, DateTime debut, DateTime fin, int id_locataire, int id_box, bool reglee, int mois, int annees)
        : base (id_contrat, debut, fin, id_locataire, id_box, reglee) 
    {
        this.mois = mois;
        this.annees = annees;
    }


    private List<Contrat_detail> construct(OleDbDataReader reader)
    {
        List<Contrat_detail> liste = new List<Contrat_detail>();
        
        while (reader.Read())
        {
            // Extraire les valeurs de chaque colonne et vérifier les valeurs nulles
            int id_contrat = reader.IsDBNull(reader.GetOrdinal("id_contrat")) ? 0 : reader.GetInt32(reader.GetOrdinal("id_contrat"));
            DateTime debut = reader.IsDBNull(reader.GetOrdinal("debut")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("debut"));
            DateTime fin = reader.IsDBNull(reader.GetOrdinal("fin")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("fin"));
            int id_locataire = reader.IsDBNull(reader.GetOrdinal("id_locataire")) ? 0 : reader.GetInt32(reader.GetOrdinal("id_locataire"));
            int id_box = reader.IsDBNull(reader.GetOrdinal("id_box")) ? 0 : reader.GetInt32(reader.GetOrdinal("id_box"));
            bool reglee = reader.IsDBNull(reader.GetOrdinal("reglee")) ? false : reader.GetBoolean(reader.GetOrdinal("reglee"));
            
            int annees = reader.GetInt16 (reader.GetOrdinal("annee"));
            int mois = reader.GetInt16 (reader.GetOrdinal("mois"));
            // Console.WriteLine($"periode {mois} {annees}");
            // Créer un objet Contrat_detail
            Contrat_detail cd = new Contrat_detail(id_contrat, debut, fin, id_locataire, id_box, reglee, mois, annees);
            
            // Ajouter l'objet à la liste
            liste.Add(cd);
        }

        reader.Close ();
        return liste;
    }



    // private List <Contrat_detail> construct (OleDbDataReader reader) {

    //     List <Contrat_detail> liste = new List<Contrat_detail> ();
    //     while (reader.Read ()) {

    //         // int id_contrat = reader.GetInt32 (reader.GetOrdinal("id_contrat"));
    //         // DateTime debut = reader.GetDateTime (reader.GetOrdinal("debut"));
    //         // DateTime fin = reader.GetDateTime (reader.GetOrdinal("fin"));
    //         // int id_locataire = reader.GetInt32 (reader.GetOrdinal("id_locataire"));
    //         // int id_box = reader.GetInt32 (reader.GetOrdinal("id_box"));
    //         // bool reglee = reader.GetBoolean (reader.GetOrdinal("reglee"));
    //         // int annees = reader.GetInt32 (reader.GetOrdinal("annee"));
    //         // int mois = reader.GetInt32 (reader.GetOrdinal("mois"));
    //         for (int i = 0; i < reader.FieldCount; i++)
    //         {
    //             string columnName = reader.GetName(i); // Nom de la colonne
    //             object value = reader.GetValue(i); // Valeur de la colonne
    //             // string valueStr = value == DBNull.Value ? "NULL" : value.ToString(); // Gérer les valeurs nulles
    //             Console.WriteLine($"{columnName}: {value}"); // Afficher dans la console ou log
    //         }
    //         Console.WriteLine("\n");

    //         // Contrat_detail cd = new Contrat_detail (id_contrat, debut, fin, id_locataire, id_box, reglee, mois, annees);
    //         // liste.Add (cd);
    //     }   

    //     return liste;
    // }


    public List <Contrat_detail> getAll_paiementFarany (int id_locataire, Util_DB udb) {

        List <Contrat_detail> liste;

        string requete = $"""
            SELECT 
                cf.id_contrat AS id_contrat,
                cf.debut AS debut,
                cf.fin AS fin,
                cf.id_locataire AS id_locataire,
                cf.id_box AS id_box,
                cf.reglee AS reglee,
                IIf(IsNull(cf.annee), YEAR(cf.debut), cf.annee) AS annee,
                IIf(IsNull(cf.mois), MONTH(cf.debut), cf.mois) AS mois
            FROM paiement_contrat_farany AS cf
            WHERE id_locataire = {id_locataire}
        """;
        
        
        udb.Connect ();
        try {        
            OleDbDataReader reader = udb.read_data (requete);
            liste = this.construct (reader);
            reader.Close();
        }
        catch (Exception) {
            liste = new List<Contrat_detail> ();
            throw;
        }

        return liste;
    }

    public bool cheack_paiementFarany (int mois, int annees, Util_DB udb) {
        
        
        string requete = $"""
            SELECT *
            FROM paiement_contrat_farany
            WHERE id_locataire = {this.id_locataire} AND id_box = {this.id_box} AND id_contrat = {this.id_contrat}
            AND annee = {annees} AND mois = {mois}
        """;

        return false;
    }
}
