using System.Data.OleDb;
using Tsena_Antananarivo.NET.Data;

namespace Tsena_Antananarivo.NET.Models;

public class Marchee
{

    public int id_marchee {get;}
    public int x {get;}
    public int y {get;}
    public int height {get;}
    public int width {get;}
    public string nom {get;}


    public Marchee (int id_marchee, int x, int y, int height, int width, string nom) {
        this.id_marchee = id_marchee;
        this.x = x;
        this.y = y;
        this.height = height;
        this.width = width;
        this.nom = nom;
    }

    public static List <Marchee> constructs (OleDbDataReader reader) {
        
        List <Marchee> liste = new List <Marchee> ();

        while (reader.Read ()) {
            int id_marchee =  reader.GetInt32(reader.GetOrdinal("id_marchee"));
            int px =  reader.GetInt32(reader.GetOrdinal("px"));
            int py =  reader.GetInt32(reader.GetOrdinal("py"));
            int width = reader.GetInt32(reader.GetOrdinal("width"));
            int height = reader.GetInt32(reader.GetOrdinal("height"));
            string nom = reader.GetString(reader.GetOrdinal("nom"));

            Marchee m = new Marchee (id_marchee, px, py, height, width, nom);
            liste.Add(m);
        }
        reader.Close ();
        return liste;
    }


    public static List <Marchee> get_all (Util_DB udb) {
        
        List <Marchee> liste = new List<Marchee> ();
        string requete = $"SELECT * FROM marchee";

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
    public void draw(Graphics g, int echelle) 
    {
        int x1 = this.x * echelle;
        int y1 = this.y * echelle;
        int width = (this.width) * echelle;
        int height = (this.height) * echelle;

        using (Pen pen = new Pen(Color.Black, 1)) {
            g.DrawRectangle(pen, x1, y1, width, height);
        }

        // Centrer le texte correctement
        Box.add_text(this.nom, g, "Consolas", FontStyle.Bold, Color.Green, x1, y1+height+10, width, 15);
        
    }



    public static double get_loyerBY (int id_marchee, int mois, int annees, Util_DB udb) {
        
        string requete = $"""
            SELECT TOP 1
                l.id_marchee, l.id_loyer, l.daty, l.loyer
            FROM loyer AS l
            WHERE (Year(l.daty) < {annees} OR (Year(l.daty) <= {annees} AND Month(l.daty) <= {mois}))
            AND l.id_marchee = {id_marchee}
            ORDER BY Year(l.daty) DESC, Month(l.daty) DESC;
        """;
            // AND DateSerial({annees}, {mois}, 15) 
            // BETWEEN DateSerial(Year(debut_periode), Month(debut_periode), 1) 
            // AND DateSerial(Year(fin_periode), Month(fin_periode) + 1, 0);

        // Console.WriteLine(requete);
        double loyer = 0;
        udb.Connect ();
        try {
            OleDbDataReader reader = udb.read_data (requete);
            while (reader.Read ()) {
                loyer = reader.GetDouble(reader.GetOrdinal("loyer"));
                break;
            }
            reader.Close ();
        }
        catch (System.Exception) { 
            throw;
        }

        return loyer;
    }

    
}

