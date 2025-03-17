using System.Data.OleDb;
using Tsena_Antananarivo.NET.Data;

namespace Tsena_Antananarivo.NET.Models.paiements;

public class Penalite
{   

    public int id_penalite {get; private set;}
    public int decalage {get; private set;}
    public double penalite {get; private set;}
    
    public Penalite () {
        // DONNEES par defaut ___
        this.penalite = 0;
        this.decalage = 1;
    }

    public Penalite get_ (Util_DB udb) {
        
        string requete = ""+
        "SELECT TOP 1\r\n"+ 
        "    *\r\n"+
        "FROM penalite ";
        Penalite penalite = new Penalite ();

        udb.Connect ();
        try {
            OleDbDataReader reader = udb.read_data (requete);
            
            while (reader.Read ()) {
                penalite.id_penalite = reader.GetInt32(reader.GetOrdinal("id_penalite"));
                penalite.decalage = reader.GetInt32(reader.GetOrdinal("decalage"));
                penalite.penalite = reader.GetDouble(reader.GetOrdinal("penalite"));
            }
            reader.Close ();
        } 
        catch (Exception) {
            throw;
        }
        return penalite;
    }

}
