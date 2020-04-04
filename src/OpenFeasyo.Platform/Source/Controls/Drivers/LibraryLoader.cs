using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace OpenFeasyo.Platform.Controls.Drivers
{
    public class LibraryLoader<T> where T : class
    {
        private Dictionary<string, T> loadedModules = new Dictionary<string,T>();

        public ObservableCollection<T> LoadedModules
        {
            get { return new ObservableCollection<T>(loadedModules.Values); }
        }

        public bool RegisterModule(string moduleName, T module) {
            if (loadedModules.ContainsKey(moduleName)) {
                Trace.TraceError("Module " + moduleName + " is already registered!");
                return false;
            }

            loadedModules.Add(moduleName, module);
            return true;
        }

        public void UpdateModules(string modulesPath)
        {

            DirectoryInfo di = null;
            FileInfo[] dllFiles = null;

            try
            {
                di = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "\\" + modulesPath);
                dllFiles = di.GetFiles("*.dll");
            }
            catch (DirectoryNotFoundException e) {
                Trace.TraceError("Directory was not found (" + AppDomain.CurrentDomain.BaseDirectory + "\\" + modulesPath + ")\n" + e.Message + "\n" + e.StackTrace);
            }

            if( dllFiles != null)
            { 
                foreach (FileInfo file in dllFiles)
                {
                    if (loadedModules.ContainsKey(file.Name))
                    {
                        continue;
                    }

                    try
                    {
                        var m = LoadFrom(file.Name, modulesPath);
                        if(m != null){
                            loadedModules.Add(file.Name, m);
                        }
                    }
                    catch (DriverNotLoadedException e) {
                        Trace.TraceError(e.Message + ": " + file.Name + " Could not be loaded!");
                    }
                }
            }    
        }

        public bool ModuleExists(string moduleName){
            return loadedModules.ContainsKey(moduleName);
        }

        public string GetModuleName(T module)
        {
            foreach (string moduleName in loadedModules.Keys)
            {
                if (loadedModules[moduleName].GetType().Equals(module.GetType())) {
                    return moduleName;
                }
            }
            throw new ApplicationException("Module is not in the loaded modules. Something went seriously wrong, please, contact the support department!");
        }

        public T GetModule(string moduleName) {
            return loadedModules[moduleName];
        }

        private T LoadFrom(string fileName, string modulesPath)
        {
            try
            {
                Assembly commandAssembly = Assembly.LoadFile(
                    Environment.CurrentDirectory + "\\" + modulesPath + "\\" + fileName);

                foreach (Type type in commandAssembly.GetTypes())
                {
                    if (type.GetInterface(typeof(T).FullName) != null)
                            return (T) Activator.CreateInstance(
                                    type);
                }

            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Warning: Unable to process file "
                                + fileName + " as an GamingControl driver");
                throw new DriverNotLoadedException();
            }
            return null;
        }
    }

    public class DriverNotLoadedException : System.ApplicationException
    { 
        // No need to add anything,
    }

}
