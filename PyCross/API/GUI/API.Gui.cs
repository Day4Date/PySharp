using System;
using System.Windows.Forms;
using Python.Runtime;
using PyCross.API.GUI.Controls;

namespace PyCross.API.GUI
{
    public class WFAPI : IPythonPlugin
    {
        private Form1 _form;
        public Form1 Form => _form;

        public string ModuleName => "gui";

        public void Init(Form1 form)
        {
            _form = form;
        }

        private readonly List<GuiControlWrapper> _activeControls = new();

        // ----------------------------
        // GUI WRAPPER SYSTEM
        // ----------------------------

        public class GUI
        {
            private readonly WFAPI _api;
            public string PluginName { get; }

            public GUI(string pluginName)
            {
                PluginName = pluginName;
                _api = ModuleLoader.ModuleLoader.Get("gui") as WFAPI;

                _api.CreatePage(pluginName);
            }

            public LabelWrapper Label(string text, int x, int y)
            {
                var lbl = _api.CreateLabel(PluginName, x, y, text);
                var wrapper = new LabelWrapper(lbl, _api.Form);
                _api._activeControls.Add(wrapper);
                return wrapper;
            }

            public ButtonWrapper Button(string text, int x, int y, PyObject handler)
            {
                var btn = _api.CreateButton(PluginName, x, y, text);
                var wrapper = new ButtonWrapper(btn, _api.Form, handler);
                _api._activeControls.Add(wrapper);
                return wrapper;
            }
            public CheckBoxWrapper CheckBox(string text, int x, int y, PyObject handler)
            {
                var cb = _api.CreateCheckBox(PluginName, x, y, text);
                var wrapper = new CheckBoxWrapper(cb, _api.Form, handler);
                _api._activeControls.Add(wrapper);
                return wrapper;
            }

            public TextBoxWrapper TextBox(string defaultText, int x, int y, PyObject handler)
            {
                var tb = _api.CreateTextBox(PluginName, x, y, defaultText);
                var wrapper = new TextBoxWrapper(tb, _api.Form, handler);
                _api._activeControls.Add(wrapper);
                return wrapper;
            }

            public ComboBoxWrapper ComboBox(int x, int y, PyObject handler)
            {
                var cb = _api.CreateComboBox(PluginName, x, y);
                var wrapper = new ComboBoxWrapper(cb, _api.Form, handler);
                _api._activeControls.Add(wrapper);
                return wrapper;
            }

        }

        // ===================================================
        // WinForms-Methoden (von vorher)
        // ===================================================

        private readonly Dictionary<string, TabPage> _pluginPages = new();

        private void CreatePage(string pluginName)
        {
            if (_form == null) return;

            _form.Invoke(new Action(() =>
            {
                if (!_pluginPages.ContainsKey(pluginName))
                {
                    var page = new TabPage
                    {
                        Name = pluginName,
                        Text = pluginName,
                        AutoScroll = true
                    };

                    _pluginPages[pluginName] = page;
                    _form.tabControl1.TabPages.Add(page);
                }
            }));
        }

        private Label CreateLabel(string pluginName, int x, int y, string text)
        {
            Label lbl = new Label { Text = text, Left = x, Top = y, AutoSize = true };

            AddControl(pluginName, lbl);
            return lbl;
        }

        private Button CreateButton(string pluginName, int x, int y, string text)
        {
            Button btn = new Button { Text = text, Left = x, Top = y, AutoSize = true };

            AddControl(pluginName, btn);
            return btn;
        }
        private CheckBox CreateCheckBox(string pageName, int x, int y, string text)
        {
            CheckBox cb = new CheckBox
            {
                Text = text,
                Left = x,
                Top = y,
                AutoSize = true
            };

            AddControl(pageName, cb);
            return cb;
        }

        private TextBox CreateTextBox(string pageName, int x, int y, string defaultText)
        {
            TextBox tb = new TextBox
            {
                Text = defaultText,
                Left = x,
                Top = y,
                Width = 150
            };

            AddControl(pageName, tb);
            return tb;
        }

        private ComboBox CreateComboBox(string pageName, int x, int y)
        {
            ComboBox cb = new ComboBox
            {
                Left = x,
                Top = y,
                Width = 150,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            AddControl(pageName, cb);
            return cb;
        }

        private void AddControl(string pluginName, Control c)
        {
            if (_pluginPages.TryGetValue(pluginName, out var page))
            {
                _form.Invoke(new Action(() => page.Controls.Add(c)));
            }
        }
        public void ClearAllControls()
        {
            if (_form == null) return;

            _form.Invoke(new Action(() =>
            {
                foreach (var page in _pluginPages.Values)
                {
                    page.Controls.Clear();
                }
            }));

            _activeControls.Clear();
        }
        public void DeleteAllPages()
        {
            if (_form == null) return;

            _form.Invoke(new Action(() =>
            {
                while (_form.tabControl1.TabPages.Count > 1)
                {
                    _form.tabControl1.TabPages.RemoveAt(1);
                }
            }));

            _pluginPages.Clear();
        }
        


        public void ResetAll()
        {
            ClearAllControls();
            DeleteAllPages();
        }

    }
}
