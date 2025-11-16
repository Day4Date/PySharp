using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PyCross.API.Inventory
{
    public static class InventoryAPI
    {
        private static Form1? _form;

        public static void Init(Form1 form)
        {
            _form = form;
        }
        public static void refresh()
        {
            _form?.AppendLog("Inventardaten aktualisiert.");
        }
        public static Dictionary<string,object> MoveItem(int fromSlot, int toSlot, int quantity = 1)
        {
            return new Dictionary<string, object>
            {
                { "from", fromSlot },
                { "to", toSlot },
                { "quantity", quantity }
            };
        }
    }
}