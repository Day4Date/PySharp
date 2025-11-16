using Python.Runtime;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using PyCross;

namespace PyCross.API.Gui
{
    public static class GuiAPI
    {
        private static Form1? _form;
        private static readonly Dictionary<string, Panel> _pluginPages = new();

        public static void Init(Form1 form)
        {
            _form = form;
        }

        public static void create_page(string pluginName)
        {
            if (_form == null) return;

            _form.Invoke(new Action(() =>
            {
                if (!_pluginPages.ContainsKey(pluginName))
                {
                    TabPage page = new()
                    {
                        Name = pluginName,
                        Dock = DockStyle.Fill,
                        AutoScroll = true,
                        Text = pluginName,
                        Visible = false
                    };

                    _form.tabControl1.Controls.Add(page);
                    _pluginPages[pluginName] = page;
                }
            }));
        }

        public static void show_page(string pluginName)
        {
            if (_form == null) return;

            _form.Invoke(new Action(() =>
            {
                foreach (Control c in _form.tabControl1.Controls)
                    c.Visible = false;

                if (_pluginPages.ContainsKey(pluginName))
                    _pluginPages[pluginName].Visible = true;
            }));
        }

        public static Button button(string pluginName, int x, int y, string text, PyObject clickHandler)
        {
            Button? btn = null;
            if (_form == null) return null!;

            _form.Invoke(new Action(() =>
            {
                if (_pluginPages.ContainsKey(pluginName))
                {
                    btn = new Button
                    {
                        Text = text,
                        Location = new Point(x, y),
                        Size = new Size(100, 30)
                    };
                    btn.Click += (sender, e) =>
                    {
                        Task.Run(() =>
                        {
                            using (Py.GIL())
                            {
                                dynamic func = clickHandler;
                                func(sender, e);
                            }
                        });
                    };
                    _pluginPages[pluginName].Controls.Add(btn);
                }
            }));

            return btn!;
        }

        public static Label label(string pluginName, int x, int y, string text)
        {
            Label? lbl = null;
            if (_form == null) return null!;

            _form.Invoke(new Action(() =>
            {
                if (_pluginPages.ContainsKey(pluginName))
                {
                    lbl = new Label
                    {
                        Text = text,
                        Location = new Point(x, y),
                        AutoSize = true
                    };
                    _pluginPages[pluginName].Controls.Add(lbl);
                }
            }));

            return lbl!;
        }

        public static void SetText(Label element, string text)
        {
            if (_form == null) return;
            _form.Invoke(new Action(() =>
            {
                element.Text = text;
            }));
        }

        public static void SetText(Button element, string text)
        {
            if (_form == null) return;
            _form.Invoke(new Action(() =>
            {
                element.Text = text;
            }));
        }
    }
}
