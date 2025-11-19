using System;
using System.IO;
using System.Windows.Forms;
using Python.Runtime;
using PyCross.API.ModuleLoader;

namespace PyCross.API.HotReload
{
    public static class PythonPluginHotReload
    {
        private static FileSystemWatcher? _watcher;
        private static Form1? _form;

        public static void Start(Form1 form, string pluginFolder)
        {
            _form = form;

            _watcher = new FileSystemWatcher(pluginFolder, "*.py");
            _watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.FileName;
            _watcher.Changed += OnChanged;
            _watcher.Created += OnChanged;
            _watcher.Renamed += OnChanged;
            _watcher.EnableRaisingEvents = true;

            form.AppendLog("[HotReload] aktiviert.");
        }

        private static void OnChanged(object sender, FileSystemEventArgs e)
        {
            // kleine Delays helfen bei Datei-Locks
            System.Threading.Thread.Sleep(100);

            _form?.Invoke(new Action(() =>
            {
                _form.AppendLog($"[HotReload] Änderung erkannt: {e.Name}");

                try
                {
                    ReloadPython(e.FullPath);
                }
                catch (PythonException ex)
                {
                    var msg = PythonErrorHandler.FormatException(ex);
                    _form.AppendLog(msg);
                }
            }));
        }

        private static void ReloadPython(string filePath)
        {
            using (Py.GIL())
            {
                dynamic sys = Py.Import("sys");

                string moduleName = Path.GetFileNameWithoutExtension(filePath);
                var guiPlugin = ModuleLoader.ModuleLoader.Get("gui") as PyCross.API.GUI.WFAPI;
                if (guiPlugin != null)
                    //guiPlugin.ResetPage(moduleName);
                try
                {
                    sys.modules.pop(moduleName);
                    // Jetzt neu importieren
                    PythonEngine.Exec($@"
import importlib
import {moduleName}
importlib.reload({moduleName})
");

                    _form?.AppendLog($"[HotReload] Plugin neu geladen: {moduleName}");
                }
                catch
                {
                    // Modul war nicht geladen → egal
                }

                
            }
        }
    }
}
