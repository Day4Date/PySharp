using System;
using Python.Runtime;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PyCross.API;

namespace PyCross.API.Inventory
{
    /// <summary>
    /// Dieses Plugin stellt Inventar-Funktionen für Python bereit,
    /// z.B. Items verschieben, Inventar aktualisieren, usw.
    /// </summary>
    public class InventoryAPI : IPythonPlugin
    {
        private Form1? _form;

        /// <summary>
        /// Eindeutiger Name des Plugins.
        /// </summary>
        public string ModuleName => "inventory";

        /// <summary>
        /// Init wird einmal aufgerufen, um die Form zu übergeben.
        /// </summary>
        public void Init(Form1 form)
        {
            _form = form;
        }
        public void Refresh()
        {
            _form?.AppendLog("Inventardaten aktualisiert.");
        }
        public PyDict MoveItem(int fromSlot, int toSlot, int quantity = 1)
        {
            using (Py.GIL())
            {
                var result = new PyDict();
                result.SetItem(new PyString("from"), new PyInt(fromSlot));
                result.SetItem(new PyString("to"), new PyInt(toSlot));
                result.SetItem(new PyString("quantity"), new PyInt(quantity));

                return result;
            }
        }
    }
}