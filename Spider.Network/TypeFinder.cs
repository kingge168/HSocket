using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Spider.Util
{
    public static class TypeFinder
    {
        public static IEnumerable<Type> FindFromDirectory<T>(string dirPath)
        {
            ArgumentValidator.Validate("dirPath", dirPath, arg => arg == null || !Directory.Exists(dirPath));

            DirectoryInfo dir = new DirectoryInfo(dirPath);
            FileInfo[] files = dir.GetFiles("*.dll", SearchOption.AllDirectories);
            List<Type> types = new List<Type>();
            foreach (var dll in files)
            {
                types.AddRange(FindFromAssembly<T>(Assembly.LoadFile(dll.FullName)));
            }
            return types;

        }

        public static IEnumerable<Type> FindFromAssembly<T>(Assembly assembly)
        {
            ArgumentValidator.Validate("assembly", assembly, arg => arg == null);

            List<Type> types = new List<Type>();
            //try
            //{
                Type[] allTypes = assembly.GetTypes();
                Type superType = typeof(T);
                foreach (var type in allTypes)
                {
                    if (superType.IsAssignableFrom(type) && type.IsClass && !type.IsAbstract)
                    {
                        types.Add(type);
                    }
                }
            //}
            //catch
            //{
            //}
            return types;
        }

        public static IEnumerable<Type> FindFromAssemblies<T>(IEnumerable<Assembly> assemblies)
        {
            ArgumentValidator.Validate("assemblies", assemblies, arg => arg == null);
            List<Type> types = new List<Type>();
            foreach (var assembly in assemblies)
            {
                types.AddRange(FindFromAssembly<T>(assembly));
            }
            return types;
        }
    }
}
