using System;
using System.Windows.Forms;

namespace PyCross
{
    public static class PyCrossAPI
    {
        private static Form1 _form;

        public static void Init(Form1 form)
        {
            _form = form;
        }

        public static void log(string message)
        {
            _form?.AppendLog(message);
        }

        public static dynamic get_info()
        {
            return new
            {
                Char = "DayDate",
                Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
        }

        public static string get_character_data(string name)
        {
            // Beispiel: später aus einer DB oder Datei laden
            return $"Character {name} hat Level 42.";
        }

        public static void start()
        {
            _form?.AppendLog("Plugin gestartet.");
        }

        public static void stop()
        {
            _form?.AppendLog("Plugin gestoppt.");
        }
    }
}
