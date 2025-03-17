
using Tsena_Antananarivo.NET.Data;
using Tsena_Antananarivo.NET.Models;
using Tsena_Antananarivo.NET.Models.paiements;
using Tsena_Antananarivo.NET.Views;


namespace Tsena_Antananarivo.NET.views;

public partial class MainForm : Form
{
    // attribut formulaire ____
    private ComboBox? comboBox_locataire, comboBox_box, comboBox_mois, comboBox_annees;
    private DateTimePicker? picker_date;
    private TextBox? textBox_montant;
    public static int echelle = 20;

    public Util_DB util_DB {get;}
    // static int echelle = 50;
    public static Dictionary <int, string> mois = new Dictionary<int, string> ();
    public static Dictionary <int, int> annees = new Dictionary<int, int> ();
    private static Dictionary<string, List<string>> locataireBox_Map = new Dictionary<string, List<string>> ();
    public CarteForm carteForm;


    public MainForm (Util_DB util_DB)
    {
        this.util_DB = util_DB; // initialiser la connection a la base de donnée
        this.config_data ();
        this.util_DB.Connect();

        InitializeComponent();
        this.carteForm = new CarteForm (this);
    }


    private void config_data () {

        // CONFIGURATION DU DONNéES ___
        MainForm.mois = new Dictionary<int, string>
        {
            { 1, "Janvier" },
            { 2, "Février" },
            { 3, "Mars" },
            { 4, "Avril" },
            { 5, "Mai" },
            { 6, "Juin" },
            { 7, "Juillet" },
            { 8, "Août" },
            { 9, "Septembre" },
            { 10, "Octobre" },
            { 11, "Novembre" },
            { 12, "Décembre" }
        };
        MainForm.annees = new Dictionary<int, int> {
            { 2022, 2022 },
            { 2023, 2023 },
            { 2024, 2024 },
            { 2025, 2025 }
        };


        List <Locataire> locataires = Locataire.get_All(this.util_DB);
        foreach (Locataire loc in locataires) {
            string l = $"{loc.id_locataire} - {loc.nom}";
            locataireBox_Map[l] = loc.boxs.Select(box => $"{box.id_box} - {box.nom}").ToList();
        }
    } 


    private void btnPaiement_Click (object sender, EventArgs e) {

        int id_locataire = -1;
        int id_box = -1;
        int annees = -1;
        int mois = -1;
        string? date = "";
        double montant = -1;
        // TRAITEMENT et VALIDATION DES DONNEES ______
        if (this.comboBox_locataire != null && this.comboBox_locataire.SelectedItem is KeyValuePair<int, string> locataire) id_locataire = locataire.Key;
        if (this.comboBox_box != null && this.comboBox_box.SelectedItem is KeyValuePair<int, string> box) id_box = box.Key;
        if (this.comboBox_annees != null && this.comboBox_annees.SelectedItem is KeyValuePair<int, int> a) annees = a.Key;
        if (this.comboBox_mois != null && this.comboBox_mois.SelectedItem is KeyValuePair<int, string> m) mois = m.Key;
        if (this.picker_date != null) date = picker_date.Value.ToString();
        if (this.textBox_montant != null) {
            string tm = textBox_montant.Text;
            double.TryParse(tm, out double montantDouble);
            montant = montantDouble;
        }

        DateTime date_paiement = DateTime.Parse(date);
        if (id_box == -1) throw new Exception ("CHOISIR UN BOX !!");
        if (montant <= 0) throw new Exception ("LE MONTANT NE DOIS PAS ETRE 0 ou NEGATIF !!");
        // fin TRAITEMENT et VALIDATION DES DONNEES ______


        // Console.WriteLine($"{id_locataire} {id_box} {annees} {mois} {date_paiement} {montant}");
        // paiement ____
        Paiement paiement = new Paiement (this.util_DB);
        Console.WriteLine ($"Paiement loyer {id_locataire} b:{id_box} mois: {mois} annees: {annees}");
        paiement.paiement(id_locataire:id_locataire, id_box:id_box, mois:mois, annees:annees, date_paiement:date_paiement, montant:montant);
    }

    private void btnCarte_Click(object sender, EventArgs e)
    {
        // CarteForm carteForm = new CarteForm(this);
        this.carteForm.Show();
        this.Hide(); // Cache la fenêtre actuelle
    }


    private void comboBoxLocataire_SelectedIndexChanged (object sender, EventArgs e) {
        
        // POUR LE CHANGEMENT LOCATAIRE DANS LE SELECT
        if (this.comboBox_locataire != null && this.comboBox_locataire.SelectedItem != null &&
         this.comboBox_locataire.SelectedItem is KeyValuePair <int, string> selected)
        {
            string? selectedLocataire = $"{selected.Key} - {selected.Value}";
            this.Fill_Boxs_ForLocataire(selectedLocataire);  // Passe la valeur sélectionnée
        }
    }

    private void textBox_montant_TextChanged (object sender, EventArgs e)
    {
        // LISTNER POUR LE FORMAT DU MONTANT 
        if (this.textBox_montant != null && decimal.TryParse(this.textBox_montant.Text.Replace(" ", ""), out decimal montant))
        {
            this.textBox_montant.Text = montant.ToString("#,##0").Replace(",", " ");
            this.textBox_montant.SelectionStart = this.textBox_montant.Text.Length; // Pour garder le curseur à la fin
        }
    }




    private void MainForm_Load(object sender, EventArgs e) {
        Fill_Locataires();
    }
}
