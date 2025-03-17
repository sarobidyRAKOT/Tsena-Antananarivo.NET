using System.Globalization;
using Tsena_Antananarivo.NET.Models;
using Tsena_Antananarivo.NET.views;

namespace Tsena_Antananarivo.NET.Views;

public class CarteForm : Form
{
    private ComboBox comboBoxMois, comboBoxAnnees;
    private Button btnVoir, btnRetour, btnVoirTrosa;
    private Panel cartePanel;
    private MainForm _mainForm;
    private static Carte carte = new Carte();

    public CarteForm (MainForm mainForm)
    {
        this.DoubleBuffered = true;
        this._mainForm = mainForm;
        this.Text = "Carte des Locataires";
        this.WindowState = FormWindowState.Maximized;

        // Initialisation des composants
        this.comboBoxMois = new ComboBox {
            Location = new Point(50, 23), Size = new Size(120, 30),
            DisplayMember = "Value", ValueMember = "Key",
            DropDownStyle = ComboBoxStyle.DropDownList
        };

        this.comboBoxAnnees = new ComboBox {
            Location = new Point(180, 23), Size = new Size(80, 30),
            DisplayMember = "Value", ValueMember = "Key",
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        InitComboBoxes();
        btnVoir = new Button { Text = "Voir", Location = new Point(280, 20), Size = new Size(80, 30) };
        btnVoirTrosa = new Button { Text = "Voir trosa", Location = new Point(360, 20), Size = new Size(100, 30) };
        btnRetour = new Button { Text = "Paiement Loyer", Size = new Size(200, 40) };
        cartePanel = new Panel
        {
            Location = new Point(50, 70),
            BorderStyle = BorderStyle.FixedSingle,
            BackColor = Color.LightGray,
            AutoScroll = true // Ajout d'un défilement si la carte dépasse
        };

        // Ajout des composants à la fenêtre
        this.Controls.Add(comboBoxMois);
        this.Controls.Add(comboBoxAnnees);
        this.Controls.Add(btnVoir);
        this.Controls.Add(btnVoirTrosa);
        this.Controls.Add(cartePanel);
        this.Controls.Add(btnRetour);

        // Gestion des événements
        btnRetour.Click += new EventHandler (BtnRetour_Click);
        btnVoir.Click += new EventHandler (BtnVoirCarte);
        btnVoirTrosa.Click += new EventHandler (btn_VoirTrosa);
        this.Resize += CarteForm_Resize;
    }

    private void InitComboBoxes()
    {
        foreach (var kv in MainForm.mois) comboBoxMois.Items.Add(new KeyValuePair<int, string>(kv.Key, kv.Value));
        foreach (var kv in MainForm.annees) comboBoxAnnees.Items.Add(new KeyValuePair<int, int>(kv.Key, kv.Value));
        
        if (comboBoxMois.Items.Count > 0) comboBoxMois.SelectedIndex = 0;
        if (comboBoxAnnees.Items.Count > 0) comboBoxAnnees.SelectedIndex = 0;
    }


    private void BtnRetour_Click(object? sender, EventArgs e) {
        this._mainForm.Show();
        this.Hide();
    }

    private void CarteForm_Resize(object? sender, EventArgs e) {
        cartePanel.Size = new Size(this.ClientSize.Width - 100, this.ClientSize.Height - 150);
        btnRetour.Location = new Point((this.ClientSize.Width - 200) / 2, this.ClientSize.Height - 60);
        btnVoir.Location = new Point(280, 20);
    }

    private void btn_VoirTrosa (object? sender, EventArgs e) {
        TrosaForm trosaForm = new TrosaForm (CarteForm.carte.mois, CarteForm.carte.annees);
        trosaForm.LoadData(CarteForm.carte.locataires);
        trosaForm.Show();
    }

    private void BtnVoirCarte(object? sender, EventArgs e) {

        if (comboBoxMois.SelectedItem is KeyValuePair<int, string> moisSelected &&
            comboBoxAnnees.SelectedItem is KeyValuePair<int, int> anneesSelected)
        {
            this.cartePanel.Paint -= DrawBoxes;
            this.cartePanel.Invalidate();
            
            int mois = moisSelected.Key;
            int annee = anneesSelected.Key;

            CarteForm.carte.annees = annee;
            CarteForm.carte.mois = moisSelected.Value;
            CarteForm.carte.get_carte(mois, annee, this._mainForm.util_DB);

            // Redessiner la carte après récupération des données
            Console.WriteLine ($"\nCARTE dans la PERIODE {moisSelected.Value} - {annee}");
            this.cartePanel.Paint += DrawBoxes;
        }
    }

    private void DrawBoxes (object? sender, PaintEventArgs e) {

        e.Graphics.Clear(Color.LightGray);
        foreach (Box box in CarteForm.carte.boxs) box.draw(e.Graphics, MainForm.echelle);
        foreach (Marchee marchee in CarteForm.carte.marchees) marchee.draw(e.Graphics, MainForm.echelle);
    }


}
