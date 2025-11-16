using PyCross;

namespace PyCross.API.Botting
{
    public static class BottingAPI
    {
        private static Form1? _form;

        public static void Init(Form1 form)
        {
            _form = form;
        }

        public static void start_profile(string profileName)
        {
            _form?.AppendLog($"Bot-Profil '{profileName}' gestartet.");
        }

        public static void stop_profile()
        {
            _form?.AppendLog("Bot-Profil gestoppt.");
        }
    }
}
