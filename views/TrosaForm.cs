using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Tsena_Antananarivo.NET.Models;


namespace Tsena_Antananarivo.NET.views;



public partial class TrosaForm : Form
{
    private DataGridView dataGridView;
    private string? mois;
    int annees;
    public TrosaForm (string? mois, int annees)
    {
        this.mois = mois;
        this.annees = annees;
        this.dataGridView = new DataGridView();
        InitializeComponent();
        // LoadData();
    }

    private void InitializeComponent()
    {
        this.dataGridView = new DataGridView();
        this.SuspendLayout();
        
        // DataGridView configuration
        this.dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        this.dataGridView.Location = new System.Drawing.Point(12, 12);
        this.dataGridView.Size = new System.Drawing.Size(400, 200);
        this.dataGridView.TabIndex = 0;
        this.dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Ajustement automatique des colonnes
        
        // MainForm configuration
        this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
        this.AutoScaleMode = AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(450, 250);
        this.Controls.Add(this.dataGridView);
        this.Name = "TROSA";
        this.Text = $"Trosa {this.mois} {this.annees}";
        this.ResumeLayout(false);
    }


    public void LoadData (List <Locataire> locataires) {
        List<Trosa> data = new List<Trosa> ();
        foreach (Locataire l in locataires) {
            if (l.trosa > 0) data.Add(new Trosa ($"{l.id_locataire} {l.nom} -> trosa : {Program.format_NOTATIONCOMPTABLE(l.trosa)} AR"));
        }

        dataGridView.DataSource = data;
    }
}

public class Trosa {
    public string Detail { get; set; }
    public Trosa (string donnee) {
        Detail = donnee;
    }
}


