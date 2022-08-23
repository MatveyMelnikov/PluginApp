using PluginBase;
using System.Reflection;

namespace PluginApp
{
    class Program
    {
        // Calls all plugins in the plugins folder
        static void Main(string[] args)
        {
            string? location = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);
            if (location == null)
                return;

            String pluginsPath = Path.Combine(
                location,
                "Plugins"
            );
            Console.WriteLine($"-Plugins path: {pluginsPath}\n");

            string[] plugins = Directory.GetFiles(pluginsPath, "*").Where(path =>
                Path.GetExtension(path) == ".dll" && File.Exists(path)
            ).ToArray();

            Console.WriteLine("All plugins output:");
            foreach (string pluginPath in plugins)
            {
                Console.WriteLine($"-Plugin file: {Path.GetFileName(pluginPath)}");

                try
                {
                    Assembly AboutAssembly = Assembly.LoadFrom(pluginPath);

                    // Loop through all public types of the assembly
                    foreach (Type t in AboutAssembly.GetExportedTypes())
                    {
                        // If this type is inherited from IPluginBase,
                        // then we create an instance of it and call its method
                        if (t.IsClass && typeof(IPluginBase).IsAssignableFrom(t))
                        {
                            IPluginBase? about = Activator.CreateInstance(t) as IPluginBase;
                            
                            if (about != null)
                                Console.WriteLine(about.GetAboutText());
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.GetType());
                }
                Console.Write("\n");
            }
        }
    }
}

