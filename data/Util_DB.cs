
using System;
using System.Data.OleDb;


namespace Tsena_Antananarivo.NET.Data;

public class Util_DB : IDisposable
{
    private OleDbConnection connection;
    // private string base_path = @"./tsena.accdb";
    private string base_path = @"./base/tsena.accdb";

    public Util_DB () {
        string string_conn = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={base_path};Persist Security Info=False;";
        this.connection = new OleDbConnection(string_conn);
    }

    public void Connect () {
        try
        {
            if (this.connection.State == System.Data.ConnectionState.Closed) {
                this.connection.Open();
            }
            // Console.WriteLine("Connexion reussie");
        } catch (Exception ex) {
            throw new Exception("Erreur lors de la connexion a la base de donnees.", ex);
        }
    }



    public void Disconnect () {
        try {
            if (this.connection.State == System.Data.ConnectionState.Open) {
                this.connection.Close();
            }
        } catch (Exception ex) {
            throw new Exception("Erreur lors de la deconnexion de la base de donnees.", ex);
        }
    }

    public void Dispose () {
        this.Disconnect();
        this.connection.Dispose();
    }

    public void CUD_data (string query)
    {

        OleDbCommand? command = null;
        // FONCTION POUR EXECUTER LES REQUETES CREATE, UPADATE, DELETE
        try {
            // string query = $"INSERT INTO {nomTable} ({colonnes}) VALUES ({valeurs})";
            using (command = new OleDbCommand(query, this.connection)) {
                command.ExecuteNonQuery();
            }
        }
        catch (Exception ex) {
            throw new Exception("Erreur lors de l'insertion des donnees.", ex);
        } finally {
            if (command != null) command.Dispose ();
        }
    }

    public OleDbDataReader read_data (string query)
    {
        OleDbCommand? command = null;
        try {
            command = new OleDbCommand(query, this.connection);
            OleDbDataReader reader = command.ExecuteReader();
            return reader;
        }
        catch (Exception ex) {
            throw new Exception("Erreur lors de la recuperation des donnees.", ex);
        } finally {
            if (command != null) command.Dispose ();
        }
    }


}

