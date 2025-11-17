using System;
using System.Windows.Forms;
using PyCross;                  // Für Form1
using PyCross.API;       // Für IPythonPlugin

namespace PyCross.API.Core
{
    /// <summary>
    /// Dieses Modul stellt Kernfunktionen für Python zur Verfügung,
    /// z.B. Logging, Infos, Charakterdaten und Start/Stop.
    /// </summary>
    public class CoreAPI : IPythonPlugin
    {
        // Referenz auf die Haupt-Form (Form1),
        // damit wir z.B. ins Log schreiben können.
        private Form1? _form;

        /// <summary>
        /// Der eindeutige Name dieses Plugins.
        /// Diesen Namen können wir später z.B. im ModuleLoader oder in Python nutzen.
        /// </summary>
        public string ModuleName => "core";

        /// <summary>
        /// Diese Methode wird beim Start einmal aufgerufen,
        /// damit das Plugin die Form (Form1) bekommt.
        /// </summary>
        /// <param name="form">Die Hauptform der Anwendung.</param>
        public void Init(Form1 form)
        {
            _form = form;
        }

        /// <summary>
        /// Schreibt eine Nachricht ins Log-Fenster.
        /// </summary>
        public void log(string message)
        {
            _form?.AppendLog(message);
        }

        /// <summary>
        /// Gibt einfache Info-Daten zurück (z.B. Datum, Zeichen).
        /// </summary>
        public dynamic get_info()
        {
            return new
            {
                Char = "DayDate",
                Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
        }

        /// <summary>
        /// Gibt Beispiel-Charakterdaten zurück.
        /// </summary>
        public string get_character_data(string name)
        {
            return $"Character {name} hat Level 42.";
        }

        /// <summary>
        /// Beispiel-Start-Funktion.
        /// </summary>
        public void start()
        {
            _form?.AppendLog("Plugin gestartet.");
        }

        /// <summary>
        /// Beispiel-Stop-Funktion.
        /// </summary>
        public void stop()
        {
            _form?.AppendLog("Plugin gestoppt.");
        }
    }
}
