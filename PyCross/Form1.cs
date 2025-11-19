using PyCross.API;
using PyCross.API.HotReload;
using PyCross.API.ModuleLoader;
using PyCross.API.GUI;
using Python.Runtime;
using System.Diagnostics;
using System.Windows.Forms;

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
            string stubPath = Path.Combine(Directory.GetParent(Application.StartupPath).Parent.Parent.FullName, "Plugins", "pycross.pyi");
            PythonStubGenerator.Generate(stubPath);
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

# C# Assembly laden
clr.AddReference('PyCross')

# Richtige Accessor-Klasse importieren!
from PyCross.API.ModuleLoader import PythonPluginAccessor

# Neues Modul erstellen
pycross = types.ModuleType('pycross')

# Plugin-Instanzen abrufen
plugins = PythonPluginAccessor.all()


# Export aller öffentlichen Methoden aller Plugins
for name, plugin in plugins.items():

    # Durch alle Attribute der Plugin-Klasse gehen
    for attr_name in dir(plugin):

        # interne/private Methoden überspringen
        if attr_name.startswith('_'):
            continue

        # Attribut abrufen
        attr = getattr(plugin, attr_name)

        # nur Funktionen exportieren
        if callable(attr):
            setattr(pycross, attr_name, attr)

# Modul offiziell registrieren
sys.modules['pycross'] = pycross

from PyCross.API.GUI import WFAPI
pycross.GUI = WFAPI.GUI
");
                    AppendLog("Python initialisiert und pycross-Modul erstellt.");
                }
                catch (PythonException ex)
                {
                    var formatted = PythonErrorHandler.FormatException(ex);
                    AppendLog(formatted);          // ins Log
                }

            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            var gui = ModuleLoader.Get("gui") as WFAPI;
            if (gui != null)
            {
                gui.ResetAll();
            }
            LoadPythonPlugins();
            string pluginFolder = Path.Combine(Directory.GetParent(Application.StartupPath).Parent.Parent.FullName, "Plugins");
            //PythonPluginHotReload.Start(this, pluginFolder);
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
