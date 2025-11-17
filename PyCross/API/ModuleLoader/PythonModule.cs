using PyCross.API;

namespace PyCross.API.ModuleLoader
{
    public static class PythonModule
    {
        /// <summary>
        /// Gibt ein Plugin anhand seines Namens zurück,
        /// aber als "dynamic", damit Python es nutzen kann.
        /// </summary>
        public static dynamic? get(string moduleName)
        {            
            return ModuleLoader.Get(moduleName);
        }

        /// <summary>
        /// Gibt alle Plugins zurück (als Dictionary).
        /// </summary>
        public static dynamic all()
        {
            return ModuleLoader.GetAll();
        }
    }
}
