namespace PyCross.API.ModuleLoader
{
    public static class PythonPluginAccessor
    {
        public static dynamic? get(string moduleName)
        {
            return ModuleLoader.Get(moduleName);
        }

        public static dynamic all()
        {
            return ModuleLoader.GetAll();
        }
    }
}
