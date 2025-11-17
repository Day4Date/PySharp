using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Python.Runtime;
using PyCross;                  // Für Form1
using PyCross.API;       // Für IPythonPlugin

namespace PyCross.API.GUI
{
    /// <summary>
    /// Dieses Plugin stellt GUI-Funktionen zur Verfügung, die von Python
    /// aus verwendet werden können (Buttons, Labels, Seiten/Tabs etc.).
    /// </summary>
    public class WFAPI : IPythonPlugin
    {
        private Form1? _form;

        // Für jede Plugin-Seite merken wir uns ein Panel/TabPage.
        private readonly Dictionary<string, TabPage> _pluginPages = new();

        public string ModuleName => "gui";

        public void Init(Form1 form)
        {
            _form = form;
        }

        /// <summary>
        /// Erstellt eine neue Seite (Tab) für ein Plugin.
        /// </summary>
        public void CreatePage(string pluginName)
        {
            if (_form == null) return;

            _form.Invoke(new Action(() =>
            {
                if (!_pluginPages.ContainsKey(pluginName))
                {
                    var page = new TabPage
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

        /// <summary>
        /// Zeigt die Seite (Tab) eines bestimmten Plugins an.
        /// </summary>
        public void ShowPage(string pluginName)
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

        /// <summary>
        /// Erstellt einen Button auf der Seite eines Plugins und verknüpft ihn mit einem Python-Callback.
        /// </summary>
        public Button? Button(string pluginName, int x, int y, string text, PyObject clickHandler)
        {
            if (_form == null) return null;

            Button? btn = null;

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
                        // Klick wird in einem separaten Task ausgeführt
                        Task.Run(() =>
                        {
                            using (Py.GIL())
                            {
                                dynamic func = clickHandler;
                                func(sender, e); // Python-Funktion wird aufgerufen
                            }
                        });
                    };

                    _pluginPages[pluginName].Controls.Add(btn);
                }
            }));

            return btn;
        }

        /// <summary>
        /// Erstellt ein Label auf der Seite eines Plugins.
        /// </summary>
        public Label? Label(string pluginName, int x, int y, string text)
        {
            if (_form == null) return null;

            Label? lbl = null;

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

            return lbl;
        }

        /// <summary>
        /// Setzt den Text eines Labels.
        /// </summary>
        public void SetText(Label element, string text)
        {
            if (_form == null) return;

            _form.Invoke(new Action(() =>
            {
                element.Text = text;
            }));
        }

        /// <summary>
        /// Setzt den Text eines Buttons.
        /// </summary>
        public void SetText(Button element, string text)
        {
            if (_form == null) return;

            _form.Invoke(new Action(() =>
            {
                element.Text = text;
            }));
        }
    }
}
