using Tsena_Antananarivo.NET.Data;

namespace Tsena_Antananarivo.NET.Models.paiements;

public class Histo_paiement
{

    public static double insert (Paiement_detail non_payee, Penalite penalite, double reste, DateTime date_paiement, Util_DB udb) {
        
        int decalage = decalage__________ (date_1:non_payee.date_echeance, date_2:date_paiement);
        double p_penalite = (decalage / penalite.decalage) * penalite.penalite;
        double m_penalite =  (non_payee.reste * p_penalite) / 100;
        non_payee.reste += m_penalite;

        reste -= non_payee.reste;
        bool update = false;
        double a_payee = 0;
        
        if (reste > 0) {
            a_payee = non_payee.reste; update = true;
        } else if (reste == 0) {
            a_payee = non_payee.reste; update = true;
        } else { // RESTE NEGATIF ...
            a_payee = non_payee.reste + reste; 
        }
        a_payee -= m_penalite;



        string  requete = $"""
            INSERT INTO histo_paiement (date_paiement, montant, penalite, id_paiement) 
            VALUES ('{date_paiement}', {a_payee}, {m_penalite}, {non_payee.id_paiement})
        """;
        
        udb.Connect ();
        udb.CUD_data(requete);
        if (update) Paiement.update_payee (non_payee.id_paiement, udb);

        return reste;
    }


    public static int decalage__________ (DateTime date_1, DateTime date_2) {

        if (date_1 > date_2) return 0;
        
        int dec_Jours = Math.Abs((date_2 - date_1).Days);
        int dec_Annees = Math.Abs(date_2.Year - date_1.Year);
        int dec_Mois = dec_Annees * 12 + Math.Abs(date_2.Month - date_1.Month);

        return dec_Mois;
    }
}
