namespace PyCross
{
    public static class PyAPI
    {
        public static void Init(Form1 form) => API.Core.CoreAPI.Init(form);

        public static void log(string message) => API.Core.CoreAPI.log(message);

        public static dynamic get_info() => API.Core.CoreAPI.get_info();

        public static string get_character_data(string name) => API.Core.CoreAPI.get_character_data(name);

        public static void start() => API.Core.CoreAPI.start();

        public static void stop() => API.Core.CoreAPI.stop();
        public static Dictionary<string, object> move_item(int from, int to, int quantity) => API.Inventory.InventoryAPI.MoveItem(from, to, quantity = 1);
    }
}