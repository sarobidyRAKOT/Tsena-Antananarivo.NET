namespace Tsena_Antananarivo.NET;

using System.Globalization;
using Tsena_Antananarivo.NET.Data;
using Tsena_Antananarivo.NET.views;




static class Program
{
    [STAThread]
    static void Main () {

        AllocConsole(); // ALLOUER UN AUTRE CONSOLE POUR FAIRE DES PRINTS _____

        Util_DB util_DB = new Util_DB();
        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm (util_DB));

        util_DB.Disconnect();
    }   

    [System.Runtime.InteropServices.DllImport("kernel32.dll")]
    static extern bool AllocConsole(); 

    public static string format_NOTATIONCOMPTABLE (double nombre) {

        CultureInfo culture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
        culture.NumberFormat.NumberGroupSeparator = " ";
        return nombre.ToString("N0", culture);
    }
}