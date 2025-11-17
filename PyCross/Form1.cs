using System.Diagnostics;
using System.Windows.Forms;
using Python.Runtime;
using PyCross.API.ModuleLoader;

namespace PyCross
{
    public partial class Form1 : Form
    {
        private Thread pythonThread;
        private PeriodicTimer _pluginTimer;
        private CancellationTokenSource _cts;
        private List<PyObject> _loadedPlugins = new List<PyObject>();

        public Form1()
        {
            InitializeComponent();
            ModuleLoader.InitAll(this);
            listView1.View = View.Details;
            listView1.Columns.Add("Python Plugins", 250);
            InitPythonRuntime();
            StartPluginEventLoop();
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (PythonEngine.IsInitialized)
                PythonEngine.Shutdown();
        }


        public void AppendLog(string text)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => AppendLog(text)));
                return;
            }

            richTextBox1.AppendText(text + Environment.NewLine);
        }

        private void LoadPythonPlugins()
        {
            try
            {
                // Projektverzeichnis (z. B. bin/Debug/... → gehe 2 Ebenen hoch)
                string projectDir = Directory.GetParent(Application.StartupPath).Parent.Parent.FullName;

                // Plugin-Ordner
                string pluginDir = Path.Combine(projectDir, "Plugins");

                if (!Directory.Exists(pluginDir))
                {
                    Directory.CreateDirectory(pluginDir);
                }

                // Alle .py-Dateien im Plugin-Ordner laden
                string[] pythonFiles = Directory.GetFiles(pluginDir, "*.py", SearchOption.TopDirectoryOnly);

                // ListView leeren
                listView1.Items.Clear();

                foreach (string file in pythonFiles)
                {
                    string fileName = Path.GetFileName(file);
                    listView1.Items.Add(new ListViewItem(fileName));
                }

                richTextBox1.Text = $"Es wurden {pythonFiles.Length} Python-Plugins gefunden.";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Laden der Plugins:\n" + ex.Message);
            }
        }
//        private void InitPythonRuntime()
//        {
//            string projectDir = Directory.GetParent(Application.StartupPath).Parent.Parent.FullName;
//            string pythonHome = Path.Combine(projectDir, "PyRuntime");
//            string pythonDll = Directory.GetFiles(pythonHome, "python31*.dll").FirstOrDefault();

//            if (pythonDll == null)
//            {
//                AppendLog("Keine Python-DLL gefunden!");
//                return;
//            }

//            Runtime.PythonDLL = pythonDll;
//            PythonEngine.PythonHome = pythonHome;
//            PythonEngine.PythonPath = pythonHome + ";" + Path.Combine(pythonHome, "Lib");

//            PythonEngine.Initialize();
//            PythonEngine.BeginAllowThreads(); // wichtig für spätere GIL-Nutzung

//            using (Py.GIL())
//            {
//                PythonEngine.Exec(@"
//import clr
//clr.AddReference('PyCross')
//from PyCross import PyAPI, WFAPI
//log = PyAPI.log
//get_info = PyAPI.get_info
//get_character_data = PyAPI.get_character_data
//start = PyAPI.start
//stop = PyAPI.stop
//move_item = PyAPI.move_item
//button = WFAPI.button
//label = WFAPI.label
//create_page = WFAPI.create_page
//show_page = WFAPI.show_page
//");
//            }
//        }
        private void InitPythonRuntime()
        {
            // 1. Pfade setzen (gleich wie bisher)
            string projectDir = Directory.GetParent(Application.StartupPath).Parent.Parent.FullName;
            string pythonHome = Path.Combine(projectDir, "PyRuntime");
            string pythonDll = Directory.GetFiles(pythonHome, "python31*.dll").FirstOrDefault();

            if (pythonDll == null)
            {
                AppendLog("Keine Python-DLL gefunden!");
                return;
            }

            Runtime.PythonDLL = pythonDll;
            PythonEngine.PythonHome = pythonHome;
            PythonEngine.PythonPath = pythonHome + ";" + Path.Combine(pythonHome, "Lib");

            PythonEngine.Initialize();
            PythonEngine.BeginAllowThreads();

            // 2. API-Namespace nach Python exportieren
            using (Py.GIL())
            {
                try
                {
                    PythonEngine.Exec(@"
import clr
import sys
import types

# C# Projekt referenzieren
clr.AddReference('PyCross')

# Plugin registry laden
from PyCross.API.ModuleLoader import PythonModule

# Neues Modul erzeugen
pycross = types.ModuleType('pycross')

# Plugins abrufen
plugins = PythonModule.all()

# Alle Plugin-Methoden exportieren
for name, plugin in plugins.items():
    for attr_name in dir(plugin):
        if attr_name.startswith('_'):
            continue

        attr = getattr(plugin, attr_name)

        if callable(attr):
            setattr(pycross, attr_name, attr)

# Modul registrieren
sys.modules['pycross'] = pycross
");
                }
                catch (PythonException ex)
                {
                    MessageBox.Show(ex.ToString(), "Python Fehler");
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            LoadPythonPlugins();
            bool is64 = Environment.Is64BitProcess;
            AppendLog($"Programm läuft als {(is64 ? "x64" : "x86")} - stelle sicher, dass PyRuntime dazu passt.");

        }

        private void button2_Click(object sender, EventArgs e)
        {
            //RunSelectedPlugin();
            string selectedPlugin = listView1.SelectedItems[0].Text;
            RunPythonPlugin(selectedPlugin);
        }
        private void RunPythonPlugin(string fileName)
        {
            Task.Run(() =>
            {
                try
                {
                    string projectDir = Directory.GetParent(Application.StartupPath).Parent.Parent.FullName;
                    string pluginsDir = Path.Combine(projectDir, "Plugins");
                    string pluginPath = Path.Combine(pluginsDir, fileName);

                    using (Py.GIL())
                    {
                        dynamic importlib = Py.Import("importlib.util");
                        string moduleName = "plugin_" + Path.GetFileNameWithoutExtension(fileName);

                        dynamic spec = importlib.spec_from_file_location(moduleName, pluginPath);
                        dynamic module = importlib.module_from_spec(spec);
                        spec.loader.exec_module(module);

                        _loadedPlugins.Add(module);
                    }
                }
                catch (Exception ex)
                {
                    AppendLog($"[Fehler] {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
                }
            });
        }

        private void StartPluginEventLoop()
        {
            _cts = new CancellationTokenSource();
            _pluginTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(500));

            Task.Run(async () =>
            {
                try
                {
                    while (await _pluginTimer.WaitForNextTickAsync(_cts.Token))
                    {
                        CallPluginEventLoops();
                    }
                }
                catch (OperationCanceledException)
                {
                    // Timer gestoppt
                }
            });
        }

        private void StopPluginEventLoop()
        {
            _cts?.Cancel();
            _pluginTimer?.Dispose();
        }
        private void CallPluginEventLoops()
        {
            using (Py.GIL())
            {
                foreach (dynamic plugin in _loadedPlugins)
                {
                    try
                    {
                        if (plugin.HasAttr("event_loop"))
                        {
                            plugin.event_loop();
                        }
                    }
                    catch (Exception ex)
                    {
                        AppendLog($"[Plugin-Loop Fehler] {ex.Message}");
                    }
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string f = "Args";
            using (Py.GIL())
            {
                foreach (dynamic plugin in _loadedPlugins)
                {
                    try
                    {
                        if (plugin.HasAttr("events"))
                        {
                            plugin.events(f);
                        }
                    }
                    catch (Exception ex)
                    {
                        AppendLog($"[Plugin-Loop Fehler] {ex.Message}");
                    }
                }
            }
        }
    }
}
