# Repository-Überblick

## Zweck des Projekts
PySharp enthält eine Windows-Forms-Anwendung namens **PyCross**, die das Hosten von Python-Plugins innerhalb einer C#-Anwendung ermöglicht. Die App initialisiert einen eingebetteten Python-Runtime, lädt C#-Module als Python-API zur Laufzeit nach, generiert `.pyi`-Stubdateien und kann Plugins inklusive eigener GUI-Elemente und Event-Loops ausführen.

## Top-Level-Struktur
| Pfad | Inhalt |
| --- | --- |
| `PyCross.sln` | Visual-Studio-Solution mit einem Projekt (`PyCross`). |
| `PyCross/` | C#-Projektdateien, Formulare und API-Code. |
| `LICENSE.txt` | MIT-Lizenztext des Projekts. |

## Projekt `PyCross`
- `Program.cs`: Einstiegspunkt, startet `Form1` mittels `Application.Run` nach Standard-WinForms-Konfiguration. 
- `Form1.*`: Enthält Oberfläche (Buttons, ListView, Log, TabControl), Eventhandler zum Laden/Ausführen von Plugins und Verwaltung des Python-Runtimes inklusive periodischer Event-Loops.

### Python-Runtime und Pluginverwaltung
- **Initialisierung** (`Form1.InitPythonRuntime`): setzt `PythonHome` auf das gebündelte `PyRuntime`, sucht die DLL, initialisiert `PythonEngine` und exportiert registrierte C#-Plugin-Methoden als Python-Modul `pycross`. Zusätzlich wird die GUI-API (`WFAPI.GUI`) in das Modul injiziert.
- **Plugin-Lifecycle**:
  - `ModuleLoader.InitAll(this)` sucht zur Laufzeit alle `IPythonPlugin`-Implementierungen, ruft `Init(Form1)` auf und speichert sie im Dictionary für spätere Abfragen.
  - `LoadPythonPlugins` listet `.py`-Dateien im Ordner `Plugins` (neben dem Projekt) auf und füllt die ListView.
  - `RunPythonPlugin` lädt das ausgewählte Skript via `importlib` als Modul und merkt es sich in `_loadedPlugins` für Event-Loops und Events.
  - `StartPluginEventLoop` nutzt einen `PeriodicTimer`, um regelmäßig `plugin.event_loop()` aufzurufen, sofern implementiert.
  - `button3_Click` demonstriert das Triggern eines generischen `events`-Callbacks aller Plugins.

### UI-Komponenten
- Designer-Datei erstellt eine Steuerleiste mit Buttons (`Plugin laden`, `Plugin ausführen`, `Event`), eine Plugin-Liste, ein Log und ein `TabControl` für plugin-spezifische Seiten. Die `WFAPI`-GUI-API hängt diese Controls an die `TabPages` an.

## API-Layer (Ordner `PyCross/API`)
Die API ist modular aufgebaut; jedes Modul implementiert `IPythonPlugin` und wird vom `ModuleLoader` registriert.

### Interface
- `Interface/IPythonPlugin.cs`: definiert `ModuleName` und `Init(Form1)` als verpflichtende Elemente für alle Plugins.

### ModuleLoader
- `ModuleLoader/ModuleLoader.cs`: reflektiert alle Assemblies nach `IPythonPlugin`, instanziiert sie, ruft `Init`, loggt das Laden und stellt Zugriff über `Get(module)` und `GetAll()` bereit.
- `ModuleLoader/PythonPlugin.cs`: liefert Python-freundliche Zugriffsfunktionen `get()` und `all()` auf die registrierten Plugins.
- `ModuleLoader/StubGenerator.cs`: generiert `.pyi`-Stubdateien mit folgenden Anteilen:
  1. alle öffentlichen Methoden der Plugins als top-level Funktionen (mit Typ-Mapping C# → Python),
  2. eine Klasse `GUI` basierend auf der verschachtelten Klasse `WFAPI.GUI`,
  3. Wrapper-Klassen für alle `GuiControlWrapper`-Ableitungen (Label-, Button-, CheckBox-, TextBox-, ComboBoxWrapper).

### Core- und Inventory-Module
- `Core/CoreAPI`: stellt Logging, allgemeine Info-/Charakterdaten sowie `start/stop`-Methoden bereit, indem es die an `Init` übergebene `Form1` nutzt.
- `Inventory/InventoryAPI`: bietet ein `Refresh`-Log, sowie `MoveItem`, das beispielhaft ein `PyDict` zurückliefert (Slots, Menge).

### GUI-Modul
- `GUI/API.Gui.cs`: implementiert `WFAPI`, das pro Plugin TabPages erzeugt und Controls verwaltet.
  - Innere Klasse `GUI`: dient als API, die Python-Plugins instanziieren, um Label, Button, CheckBox, TextBox und ComboBox zu erzeugen. Jede Methode liefert einen Wrapper zurück und registriert das Control.
  - Methoden wie `ClearAllControls`, `DeleteAllPages` und `ResetAll` ermöglichen das Zurücksetzen der GUI.
- `GUI/Controls`: enthält konkrete Wrapper (Label/Button/CheckBox/TextBox/ComboBox), die `GuiControlWrapper` erweitern. Diese Wrapper kapseln Zugriff auf WinForms-Controls (Thread-sichere Setter, Event-Bindings zu Python-Callbacks).
- `GUI/Wrapper/API.Gui.Wrapper.cs`: Basisklasse `GuiControlWrapper` mit Hilfsmethoden für Sichtbarkeit, Text, Enabled-State und Position.

### Fehlerbehandlung und Hot-Reload
- `Handler/PythonErrorHandler.cs`: formatiert `PythonException`-Infos mit Kopfzeile und Stacktrace.
- `HotReload/HotReload.cs`: FileSystemWatcher für den Plugins-Ordner, loggt Änderungen und lädt Module via `importlib.reload`. Geplante GUI-Reset-Integration ist kommentiert.

## Zusätzliche Hinweise & Anmerkungen
- **Plugins-Verzeichnis**: `Form1` erwartet einen Ordner `Plugins` im Projektstamm. Beim ersten Laden wird er erstellt; `.py`-Dateien dienen als Plugins.
- **Python-Laufzeit**: Die Anwendung sucht eine `python31*.dll` im Ordner `PyRuntime`. Ohne diese wird ein Log-Eintrag erzeugt, aber keine Initialisierung.
- **Threading**: Zugriff auf WinForms-Controls erfolgt stets über `Invoke`, sowohl in `Form1` (Log, Control-Updates) als auch in den GUI-Wrappern, um Cross-Thread-Aufrufe zu vermeiden.
- **Event-Loops**: Plugins dürfen optionale Methoden `event_loop` und `events(args)` bereitstellen; beide werden zyklisch bzw. auf Button-Klick getriggert.
- **Stub-Generierung**: `PythonStubGenerator.Generate` wird im Form-Konstruktor mit dem Ziel `Plugins/pycross.pyi` aufgerufen. So verfügen Plugin-Autoren über IntelliSense/Typinformationen.
- **HotReload**: `PythonPluginHotReload.Start` ist vorbereitet, aber aktuell im UI auskommentiert. Aktiviert man den Aufruf, reagiert die App auf Dateiänderungen mit Reload und Log-Ausgaben.

