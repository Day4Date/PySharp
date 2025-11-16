using System.Collections.Generic;
using PyCross;

namespace PyCross.API.Inventory
{
    public static class InventoryAPI
    {
        private static Form1? _form;

        public static void Init(Form1 form)
        {
            _form = form;
        }

        public static IEnumerable<string> get_inventory_items()
        {
            return new List<string>();
        }

        public static void refresh()
        {
            _form?.AppendLog("Inventardaten aktualisiert.");
        }
    }
}
