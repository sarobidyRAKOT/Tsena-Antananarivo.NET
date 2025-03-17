
using System.Drawing;
using System;



namespace Tsena_Antananarivo.NET.views;


partial class MainForm
{

    private System.ComponentModel.IContainer components = null;


    protected override void Dispose (bool disposing)
    {
        if (disposing && (components != null)) {
            components.Dispose();
        }
        base.Dispose(disposing);
    }



    private void InitializeComponent()
    {   

        this.affichage_formulaire ();

        this.ClientSize = new Size(800, 450);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.Load += new EventHandler(this.MainForm_Load);
        this.comboBox_locataire.SelectedIndexChanged += new System.EventHandler(this.comboBoxLocataire_SelectedIndexChanged);

        this.Text = "Tsena Antananarivo";
        this.Icon = SystemIcons.Application;

        // Empêcher le redimension
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false; 
        this.MinimizeBox = true;


    }

    private void affichage_formulaire () {

        this.comboBox_locataire = new ComboBox {
            DropDownStyle = ComboBoxStyle.DropDownList,
            DisplayMember = "Value", ValueMember = "Key",
            Location = new Point(370, 90), Size = new Size(200, 30)
        };

        this.comboBox_box = new ComboBox {
            DropDownStyle = ComboBoxStyle.DropDownList,
            DisplayMember = "Value", ValueMember = "Key",
            Location = new Point(370, 120+50), Size = new Size(200, 30)
        };
        this.comboBox_annees = new ComboBox  {
            DropDownStyle = ComboBoxStyle.DropDownList,
            DisplayMember = "Value", ValueMember = "Key",
            Location = new Point(500, 150+50), Size = new Size(70, 30),
        };
        foreach (var kv_mois in MainForm.annees) this.comboBox_annees.Items.Add (new KeyValuePair <int, int> (kv_mois.Key, kv_mois.Value));    
        this.comboBox_annees.SelectedIndex = 0;
        
        this.comboBox_mois = new ComboBox {
            DropDownStyle = ComboBoxStyle.DropDownList,
            DisplayMember = "Value", ValueMember = "Key",
            Location = new Point(300, 150+50), Size = new Size(90, 30)
        };
        foreach (var kv_mois in MainForm.mois) this.comboBox_mois.Items.Add (new KeyValuePair <int, string> (kv_mois.Key, kv_mois.Value));    
        this.comboBox_mois.SelectedIndex = 0;
        
        this.picker_date = new DateTimePicker {
            Location = new Point(370, 180+50),
            Size = new Size(200, 30),
            CustomFormat = "dd-MM-yyyy"
        };
        this.textBox_montant = new TextBox {
            Location = new Point(370, 210+50),
            Size = new Size(200, 30)
        };
        this.textBox_montant.TextChanged += textBox_montant_TextChanged;

        Button btn_paiment = new Button {   
            Text = "Enregister - Payer",
            Size = new Size(200, 30),
            Location = new Point(300, 260+50)
        };
        Button btn_carte = new Button {
            Text = "Voir la carte",
            Size = new Size(343, 30),
            Location = new Point(230, 300+50),
        };

        // LISTNER BOUTTON ___________
        btn_carte.Click += new EventHandler (this.btnCarte_Click);
        btn_paiment.Click += new EventHandler (this.btnPaiement_Click);


        // unitil LABEL ___________
        Label lb_locataire = new Label { Text = "Choisir locataire :", Location = new Point(230, 93), Size = new Size(100, 25) };
        Label lb_box = new Label { Text = "Choisir box :", Location = new Point(230, 123+50), Size = new Size(100, 25) };
        Label lb_mois = new Label { Text = "Mois :", Location = new Point(230, 153+50), Size = new Size(50, 25) };
        Label lb_annees = new Label { Text = "Annees :", Location = new Point(420, 153+50), Size = new Size(52, 25) };        
        Label lb_date = new Label { Text = "Date paiement :", Location = new Point(230, 183+50), Size = new Size(100, 25) };
        Label lb_montant = new Label { Text = "Montant :", Location = new Point(230, 213+50), Size = new Size(100, 25) };
        Label titreLabel = new Label { Text = "PAIEMENT LOYER - TSENA",Font = new Font("Arial avec Serif", 18, FontStyle.Bold),AutoSize = true,TextAlign = ContentAlignment.MiddleCenter};

        // AJOUTER LE DANS LE WINFORM ______
        this.Controls.Add(titreLabel);
        this.Controls.Add(lb_locataire);
        this.Controls.Add(lb_box);
        this.Controls.Add(lb_mois);
        this.Controls.Add(lb_annees);
        this.Controls.Add(lb_date);
        this.Controls.Add(lb_montant);

        this.Controls.Add(this.comboBox_locataire);
        this.Controls.Add(this.comboBox_box);
        this.Controls.Add(this.comboBox_mois);
        this.Controls.Add(this.comboBox_annees);
        this.Controls.Add(this.picker_date);
        this.Controls.Add(this.textBox_montant);
        this.Controls.Add(btn_paiment);
        this.Controls.Add(btn_carte);
        // Positionnement correct (centrage dynamique)
        this.Load += (s, e) => CenterLabel(titreLabel);
        this.Resize += (s, e) => CenterLabel(titreLabel);


    }


    private void CenterLabel(Label label) {
        label.Left = (this.ClientSize.Width - label.Width) / 2;
        label.Top = 30; // Position verticale en haut
    }


    private void Fill_Locataires () {
        // ATTRIBUT DES VALEURS POUR LE DROPDOWN LOCATAIRE
        this.comboBox_locataire.Items.Clear();  // Vider les éléments avant de les remplir
        foreach (string locataire in MainForm.locataireBox_Map.Keys) {
            string[] kv = locataire.Split(" - ");
            this.comboBox_locataire.Items.Add(new KeyValuePair <int, string> (int.Parse(kv[0]), kv[1]));
        }
        if (this.comboBox_locataire.Items.Count > 0) { this.comboBox_locataire.SelectedIndex = 0; }
    }

    private void Fill_Boxs_ForLocataire(string locataire) {
        
        // ATTRIBUT DES VALEURS POUR LE DROPDOWN BOX par rapport au LOCATAIRE CHOISI
        
        if (locataire == null) {
            return;
        }

        this.comboBox_box.Items.Clear();
        if (MainForm.locataireBox_Map.ContainsKey(locataire)) {
            
            List <string> boxs = MainForm.locataireBox_Map[locataire];
            foreach (string box in boxs) {
                string[] kv = box.Split(" - ");
                this.comboBox_box.Items.Add(new KeyValuePair <int, string> (int.Parse(kv[0]), kv[1]));
            }
            if (this.comboBox_box.Items.Count > 0) this.comboBox_box.SelectedItem = 0;
        }
    }
}
