using System.Data.OleDb;
using Tsena_Antananarivo.NET.Data;

namespace Tsena_Antananarivo.NET.Models;

public class Box
{

    public int id_box, id_marchee;
    int px, py, longueur, largeur;
    public string nom;  

    public double payee {get; private set;} 
    public double reste { private set; get;}
    public int id_locataire {private set; get;}
    public string? locataire {get; private set;}


    public Box (int id_box, int px, int py, int longueur, int largeur, string nom, int id_marchee) {
        
        this.id_box = id_box;
        this.px = px;
        this.py = py;
        this.longueur = longueur;
        this.largeur = largeur;
        this.nom = nom;
        this.id_marchee = id_marchee;
    }


    public static List <Box> constructs (OleDbDataReader reader) {
        
        List <Box> liste = new List<Box> ();

        while (reader.Read ()) {
            int id_box =  reader.GetInt32(reader.GetOrdinal("id_box"));
            int px =  reader.GetInt32(reader.GetOrdinal("px"));
            int py =  reader.GetInt32(reader.GetOrdinal("py"));
            int longueur = reader.GetInt32(reader.GetOrdinal("longueur"));
            int largeur = reader.GetInt32(reader.GetOrdinal("largeur"));
            string nom = reader.GetString(reader.GetOrdinal("nom"));
            int id_marchee =  reader.GetInt32(reader.GetOrdinal("id_marchee"));

            Box b = new Box (id_box:id_box, px:px, py:py, longueur:longueur, largeur:largeur, nom:nom, id_marchee:id_marchee);
            liste.Add (b);
        }

        reader.Close ();
        return liste;
    }

    public static Box? construct (OleDbDataReader reader) {
        
        Box? box = null;

        while (reader.Read ()) {
            int id_box =  reader.GetInt32(reader.GetOrdinal("id_box"));
            int px =  reader.GetInt32(reader.GetOrdinal("px"));
            int py =  reader.GetInt32(reader.GetOrdinal("py"));
            int longueur = reader.GetInt32(reader.GetOrdinal("longueur"));
            int largeur = reader.GetInt32(reader.GetOrdinal("largeur"));
            string nom = reader.GetString(reader.GetOrdinal("nom"));
            int id_marchee =  reader.GetInt32(reader.GetOrdinal("id_marchee"));

            box = new Box (id_box:id_box, px:px, py:py, longueur:longueur, largeur:largeur, nom:nom, id_marchee:id_marchee);
            break;
        }

        reader.Close ();
        return box;
    }

    public static Box get_byID (int id_box, Util_DB udb) {

        string requete = $"SELECT * FROM box WHERE id_box = {id_box}";

        udb.Connect ();
        try {
            OleDbDataReader reader = udb.read_data (requete);
            Box? box = construct(reader);     
            if (box == null) throw new Exception ("Box id {id_box} n'existe pas _____");
            else return box;
        }
        catch (System.Exception) {
            throw;
        }
    }


    public static List <Box> get_all (int mois, int annees, Util_DB udb) {
        
        string requete = $"""
            SELECT 
                b.*,
                d.id_locataire AS id_locataire,
                d.reste,
                d.nom AS locataire,
                d.payee
            FROM box AS b LEFT JOIN (
                SELECT 
                    c.id_box,
                    c.id_locataire,
                    c.nom,
                    IIf(IsNull(pd.mois), {mois}, pd.mois) AS mois,
                    IIf(IsNull(pd.annee), {annees}, pd.annee) AS annee,
                    IIf(IsNull(pd.reste), 0, pd.reste) AS reste,
                    IIf(IsNull(pd.payee), 0, pd.payee) AS payee
                FROM (
                    SELECT
                        c.id_contrat,
                        c.id_locataire,
                        l.nom,
                        c.id_box
                    FROM contrats AS c LEFT JOIN locataire AS l ON l.id_locataire = c.id_locataire
                    WHERE DateSerial({annees}, {mois}, 1) BETWEEN 
                    DateSerial(Year(c.debut), Month(c.debut), 1)  
                    AND DateSerial(Year(c.fin), Month(c.fin) + 1, 0)
                ) AS c LEFT JOIN (
                    SELECT 
                        *
                    FROM paiement_detail
                    WHERE mois = {mois} AND annee = {annees}
                ) AS pd 
                ON c.id_contrat = pd.id_contrat    
            ) AS d ON b.id_box = d.id_box
        """;

        List <Box> liste = new List <Box> ();

        udb.Connect ();
        try {
            OleDbDataReader reader = udb.read_data (requete);
            while (reader.Read ()) {
                int id_box =  reader.GetInt32(reader.GetOrdinal("id_box"));
                int px =  reader.GetInt32(reader.GetOrdinal("px"));
                int py =  reader.GetInt32(reader.GetOrdinal("py"));
                int longueur = reader.GetInt32(reader.GetOrdinal("longueur"));
                int largeur = reader.GetInt32(reader.GetOrdinal("largeur"));
                string nom = reader.GetString(reader.GetOrdinal("nom"));
                int id_marchee =  reader.GetInt32(reader.GetOrdinal("id_marchee"));

                int id_locataire = reader.IsDBNull(reader.GetOrdinal("id_locataire")) ? 0 : reader.GetInt32(reader.GetOrdinal("id_locataire"));
                string? locataire = reader.IsDBNull(reader.GetOrdinal("locataire")) ? null : reader.GetString(reader.GetOrdinal("locataire"));
                double reste =  reader.GetDouble(reader.GetOrdinal("reste"));
                double payee =  reader.GetDouble(reader.GetOrdinal("payee"));

                Box b = new Box (id_box:id_box, px:px, py:py, longueur:longueur, largeur:largeur, nom:nom, id_marchee:id_marchee);
                b.id_locataire = id_locataire;
                b.locataire = locataire;
                b.reste = reste;
                b.payee = payee;

                liste.Add (b);
            }
            reader.Close ();
        }
        catch (System.Exception) { 
            throw;
        } 
        return liste;
    }
    


    public void draw(Graphics g, int echelle) {
        
        int x1 = this.px * echelle;
        int y1 = this.py * echelle;
        int width = (this.longueur + this.px) * echelle;
        int height = (this.largeur + this.py) * echelle;
        double montantDu = this.reste + this.payee;


        Box.add_text (this.nom, g, "Consolas", FontStyle.Bold, Color.Black, x1, y1 - 15, width - x1, 15);
        // BOX en fonction de l'état du loyer
        if (this.id_locataire == 0) Box.paint_rect (g, Color.Gray, x1, y1, width - x1, height - y1);
        else {
            if (montantDu == 0) Box.paint_rect (g, Color.Red, x1, y1, width - x1, height - y1);
            else {
                double pp = ((montantDu - this.reste) * 100) / montantDu;
                int xp = (int)((pp * (width - x1)) / 100);
                int xn = x1 + xp;

                Box.paint_rect (g, Color.Green, x1, y1, xp, height - y1);
                if (xn != width) Box.paint_rect(g, Color.Red, xn, y1, width - xn, height - y1);
            }
            Box.add_text (this.locataire, g, "Consolas", FontStyle.Bold, Color.Black, x1, y1, width - x1, height - y1);
        }
        // Bordure NOIR
        using (Pen pen = new Pen(Color.Black, 1)) { 
            g.DrawRectangle(pen, x1, y1, width - x1, height - y1);
        }
    }


    private static void paint_rect (Graphics g, Color color, int x, int y, int width, int height) {
        using (Brush brush = new SolidBrush(color)) {
            g.FillRectangle(brush, x, y, width, height);
        }
    }

    public static void add_text (string? text, Graphics g, string _font, FontStyle style, Color color, int x, int y, int width, int height) {
        using (Font font = new Font(_font, 8, style))
        using (Brush textBrush = new SolidBrush(color))
        using (StringFormat format = new StringFormat { 
            Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center 
        }) { 
            g.DrawString(text, font, textBrush, new RectangleF(x, y, width, height), format); 
        }
    }



    
    public double get_ttlLoyer (double loyer) {
        return loyer * this.largeur * this.longueur;
    }


}
