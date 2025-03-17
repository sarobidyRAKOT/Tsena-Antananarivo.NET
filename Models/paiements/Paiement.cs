using System.Data.OleDb;
using Tsena_Antananarivo.NET.Data;
using Tsena_Antananarivo.NET.Models.Contrats;

namespace Tsena_Antananarivo.NET.Models.paiements;

public class Paiement
{

    private Util_DB udb;
    private Penalite penalite;


    public Paiement (Util_DB util_DB) {
        this.udb = util_DB;
        penalite = new Penalite().get_ (udb: this.udb);
    }



    // FONCTION INV__________________________

    public double montantDu_parLoc (int id_locataire, int periode_mois, int periode_annees) {

        // PAIEMENT PERIODE FARANY TAMIN'CONTRAT 
        List <Contrat_detail> contrats_ppFarany = new Contrat_detail().getAll_paiementFarany(id_locataire, this.udb);
        double montant_du = 0;
        foreach (Contrat_detail contrat_ppFarany in contrats_ppFarany) { // Tous contrat du locataire ...
            
            DateTime contrat_farany = DateTime.Parse ($"{contrat_ppFarany.annees}-{contrat_ppFarany.mois}-01");
            DateTime periode = DateTime.Parse ($"{periode_annees}-{periode_mois}-01");

            while (contrat_farany <= periode) {
                
                if (Contrat.cheack_contrat(contrat_ppFarany, contrat_farany, this.udb)) {
                    montant_du += this.get_montantDU (contrat_farany.Month, contrat_farany.Year, contrat_ppFarany);
                } else break;
                contrat_farany = contrat_farany.AddMonths(1);
            }

        }
        // Console.WriteLine ($"TROSA TTL locataire {id_locataire} : {montant_du}");

        return montant_du;
    }


    private double get_montantDU (int mois, int annees, Contrat_detail contrat) {

        // CREATION OBJECT ...
        double montant_du = 0;
        if (this.cheack(contrat.id_contrat, mois, annees)) { // VERIFIE S'IL Y A DEJA UN PAIEMENT 
            // Console.WriteLine($"EFA MIS PAIEMENT IO PERIODE IO mois: {mois} - annee: {annees}");
            return montant_du;
        }
        else {
            Box box = Box.get_byID(contrat.id_box, udb);
            double loyer = Marchee.get_loyerBY(box.id_marchee, mois, annees, this.udb);
            Tarif tarif = Tarif.get_tarif(box.id_marchee, mois, annees, this.udb);
            montant_du += tarif.get_montant (loyer);
        }
        return montant_du;
    }

    // FONCTION INV__________________________

    private void create_paiement (string requete, int id_locataire, int periode_mois, int periode_annees) {

        List <Contrat_detail> contrats = new Contrat_detail().getAll_paiementFarany(id_locataire, this.udb);
        
        foreach (Contrat_detail contrat in contrats) {
            bool insert = true;
            DateTime periode = DateTime.Parse ($"{periode_annees}-{periode_mois}-01");
            DateTime p_contrat = DateTime.Parse ($"{contrat.annees}-{contrat.mois}-01");

            if (this.non_payee (contrat.id_contrat, p_contrat.Month, p_contrat.Year)) insert = false;
            if (this.est_payee (contrat.id_contrat, p_contrat.Month, p_contrat.Year)) p_contrat = p_contrat.AddMonths(1);


            if (contrat.cheack_paiementFarany(p_contrat.Month, p_contrat.Year, this.udb)) Contrat.update_reglee(id_contrat:contrat.id_contrat, udb:this.udb);
            if (insert && p_contrat < periode && Contrat.cheack_contrat(contrat, p_contrat, this.udb)) {
                this.insert_paiement(requete, p_contrat.Month, p_contrat.Year, false, contrat.id_box, contrat.id_contrat);
            }
        }
    }


    public void paiement (int id_locataire, int id_box, int mois, int annees, DateTime date_paiement, double montant) {
        
        
        // VALIDATION DU PAIEMENT ___
        if (!Contrat.cheack_premierContrat (id_locataire, mois, annees, this.udb)) throw new Exception ($"LE LOCATAIRE N'AVAIT MEME PAS DE CONTRAT AVANT  {mois}-{annees}, PAS DE PAIEMENT ___________");
        
        Contrat? contrat = Contrat.get_by(id_locataire, id_box, this.udb);
        if (contrat == null) {
            throw new Exception ($"Il n'y a pas de contrat a reglee avant et a cette periode ___ box [{id_box}] - locataire [{id_locataire}] periode [mois:{mois} annee:{annees}], reste montant : {montant} => CONTRAT REGLEE!!!");
        } else {
            if (this.est_payee(contrat.id_contrat, mois, annees)) throw new Exception ($"Ce loyer est deja payee ___ box:{id_box}, locataire:{id_locataire} periode [mois:{mois} annee:{annees}]");
        }
        // fin VALIDATION DU PAIEMENT ___

        string requete = $"INSERT INTO paiement (mois, annee, montant_du, id_contrat, payee, date_echeance) VALUES ";
        this.create_paiement(requete, id_locataire, mois, annees);
        List <Paiement_detail> non_payees = Paiement_detail.get_allNON_payee_par(id_locataire, this.udb);

        if (non_payees.Count() == 0) { // EFA EO AMIN'ILAY PERIODE TENENINY ...
            Console.WriteLine ("PAYEE LE PERIODE MAINTENANT ");
            this.insert_paiement (requete, mois, annees, false, contrat.id_box, contrat.id_contrat);
            Paiement_detail? non_payee = Paiement_detail.get_NON_payeePar (id_locataire, id_box, this.udb);
            if (non_payee != null) montant = Histo_paiement.insert (non_payee, this.penalite, montant, date_paiement, this.udb);
            
            // Paiement en arriere ...
            DateTime p = DateTime.Parse($"{annees}-{mois}-01").AddMonths(1);
            // Console.WriteLine ($"{p}");
            this.paiement_arriere(id_locataire, id_box, p.Month, p.Year, date_paiement, montant);
            
        }
        else this._paiement (non_payees, date_paiement, montant, id_locataire, id_box);
    }


    private void paiement_arriere (int id_locataire, int id_box, int mois, int annees, DateTime date_paiement, double montant) {
        
        Console.WriteLine ("PAIEMENT ARIERE __________");

        Contrat? contrat = Contrat.get_by(id_locataire, id_box, this.udb);
        if (contrat == null) {
            throw new Exception ($"Il n'y a pas de contrat a reglee a cette periode ___ box [{id_box}] - locataire [{id_locataire}] periode [mois:{mois} annee:{annees}], reste montant : {montant} => CONTRAT REGLEE!!!");
        } else {
            if (this.est_payee(contrat.id_contrat, mois, annees)) throw new Exception ($"Ce loyer est deja payee ___ box:{id_box}, locataire:{id_locataire} periode [mois:{mois} annee:{annees}]");
        }


        string requete = $"INSERT INTO paiement (mois, annee, montant_du, id_contrat, payee, date_echeance) VALUES ";
        this.create_paiement (requete, id_locataire, mois, annees);
        List <Paiement_detail> non_payees = Paiement_detail.get_allNON_payee_OrderByNumpar (id_locataire, this.udb);

        double reste = montant;
        foreach (Paiement_detail pd in non_payees) {
            reste = Histo_paiement.insert (pd, this.penalite, reste, date_paiement, this.udb);
            if (reste <= 0) break;
        }

        if (reste > 0) {
            DateTime periode = DateTime.Parse($"{annees}-{mois}-01").AddMonths(1);
            // Console.WriteLine ($"{mois} {annees}");
            this.paiement_arriere(id_locataire, id_box, periode.Month, periode.Year, date_paiement, reste);
        }

    }



    private void _paiement (List <Paiement_detail> non_payees, DateTime date_paiement, double montant, int id_locataire, int id_box) {

        double reste = montant;
        foreach (Paiement_detail non_payee in non_payees) {

            reste = Histo_paiement.insert (non_payee, this.penalite, reste, date_paiement, this.udb);
            if (reste <= 0) break;
        }

        if(reste > 0) {
            Paiement_detail pd = non_payees[non_payees.Count() - 1];
            DateTime next_p = DateTime.Parse ($"{pd.annees}-{pd.mois}-1").AddMonths(1);
            this.paiement (id_locataire, id_box, next_p.Month, next_p.Year, date_paiement, reste);
        }

    }



    private bool non_payee (int id_contrat, int mois, int annees) {
        string requete = $"""
            SELECT * 
            FROM paiement WHERE mois = {mois} AND annee = {annees} AND id_contrat = {id_contrat}
            AND payee = False
        """;

        this.udb.Connect ();

        bool est_payee = false;
        try {
            OleDbDataReader reader = this.udb.read_data (requete);
            while (reader.Read ()) {
                est_payee = true;
                break;
            }
            reader.Close ();
        }
        catch (Exception ex) {
            throw new Exception(ex.Message);
        }
        return est_payee;
    }

    private bool est_payee (int id_contrat, int mois, int annees) {
        string requete = $"""
            SELECT * 
            FROM paiement WHERE mois = {mois} AND annee = {annees} AND id_contrat = {id_contrat}
            AND payee = True
        """;

        this.udb.Connect ();

        bool est_payee = false;
        try {
            OleDbDataReader reader = this.udb.read_data (requete);
            while (reader.Read ()) {
                est_payee = true;
                break;
            }
            reader.Close ();
        }
        catch (Exception ex) {
            throw new Exception(ex.Message);
        }
        return est_payee;
    }


    private bool cheack (int id_contrat, int mois, int annees) {

        string requete = $"SELECT * FROM paiement WHERE mois = {mois} AND annee = {annees} AND id_contrat = {id_contrat}";

        bool misy = false;
        this.udb.Connect ();
        try {
            OleDbDataReader reader = this.udb.read_data (requete);
            while (reader.Read ()) {
                misy = true;
                break;
            }
            reader.Close ();
        }
        catch (Exception) {
            throw;
        }
        return misy;
    } 


    public static DateTime Date_echeance(int annee, int mois) {
        int dernierJour = DateTime.DaysInMonth(annee, mois);
        return new DateTime(annee, mois, dernierJour);
    }


    private void insert_paiement (string requete, int mois, int annees, bool payee, int id_box, int id_contrat) {

        if (this.cheack(id_contrat, mois, annees)) Console.WriteLine($"EFA MIS PAIEMENT IO PERIODE IO mois: {mois} - annee: {annees} id_contrat : {id_contrat}");
        else {
            Box? box = Box.get_byID(id_box, udb);
            if (box != null) {
                Tarif tarif = Tarif.get_tarif(box.id_marchee, mois, annees, this.udb);
                double montant_du = tarif.get_montant (Marchee.get_loyerBY(box.id_marchee, mois, annees, this.udb));
                DateTime date_echeance = Date_echeance (annees, mois);
                string values = $"({mois}, {annees}, {box.get_ttlLoyer(montant_du)}, {id_contrat}, {payee}, '{date_echeance}')";
                udb.Connect ();
                udb.CUD_data($"{requete}{values}");
            }
        }
    }

    public static void update_payee (int id_paiement, Util_DB udb) {
        string requete = $"UPDATE paiement SET payee = True WHERE id_paiement = {id_paiement}";
        udb.Connect ();
        udb.CUD_data(requete);
    }


}
