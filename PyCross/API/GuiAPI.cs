using Python.Runtime;
using System.Windows.Forms;

namespace PyCross
{
    public static class WFAPI
    {
        public static void Init(Form1 form) => API.Gui.GuiAPI.Init(form);

        public static void create_page(string pluginName) => API.Gui.GuiAPI.create_page(pluginName);

        public static void show_page(string pluginName) => API.Gui.GuiAPI.show_page(pluginName);

        public static Button button(string pluginName, int x, int y, string text, PyObject clickHandler) => API.Gui.GuiAPI.button(pluginName, x, y, text, clickHandler);

        public static Label label(string pluginName, int x, int y, string text) => API.Gui.GuiAPI.label(pluginName, x, y, text);

        public static void SetText(Label element, string text) => API.Gui.GuiAPI.SetText(element, text);

        public static void SetText(Button element, string text) => API.Gui.GuiAPI.SetText(element, text);
    }
}
