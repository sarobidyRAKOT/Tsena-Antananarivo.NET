namespace Tsena_Antananarivo.NET;

// IMPORTATION ...
using Tsena_Antananarivo.NET.views;

static class Program
{
    [STAThread]
    static void Main ()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm ());
    }    
}