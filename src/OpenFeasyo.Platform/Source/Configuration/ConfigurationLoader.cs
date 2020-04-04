using OpenFeasyo.Platform.Configuration.Xml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using OpenFeasyo.Platform.Controls;
using OpenFeasyo.Platform.Controls.Analysis;
using OpenFeasyo.Platform.Platform;

namespace OpenFeasyo.Platform.Configuration
{
    public class ConfigurationLoader
    {
        private static string _currentConfigurationFile = "";
        public static string CurrentConfigurationFile
        {
            get
            {
                return _currentConfigurationFile;
            }
        }

        public static Configuration LoadConfiguration(string configurationFile, IGame game)
        {
            if (!File.Exists(configurationFile))
                return null;

            _currentConfigurationFile = configurationFile;
            XmlSerializer confSerializer = new XmlSerializer(typeof(XmlSerializableConfiguration));
            FileStream confFileStream = new FileStream(configurationFile, FileMode.Open);
            try
            {
                InputAnalyzerManager.CurrentGame = game;
                Configuration conf = confSerializer.Deserialize(confFileStream) as Configuration;
                confFileStream.Close();
                return conf;
            }
            catch (XmlException e)
            {
                UIThread.ShowMessage("", "Xml File cannot be parsed " + configurationFile  + "" + e.Message +"" + "\n" + e.StackTrace + 
                    (e.InnerException == null ? "InnerException is null." : "InnerException: " + e.InnerException.Message + "\n" + e.InnerException.StackTrace));
                return null;
            }
            catch (Exception e)
            {
                UIThread.ShowMessage("", e.Message + "" + "\n" + e.StackTrace +
                    (e.InnerException == null ? "InnerException is null." : "InnerException: " + e.InnerException.Message + "\n" + e.InnerException.StackTrace));
                return null;
            }
        }

        public static Configuration LoadConfigurationFromString(string configurationString, IGame game)
        {
            XmlSerializer confSerializer = new XmlSerializer(typeof(XmlSerializableConfiguration));
            try
            {
                using (TextReader reader = new StringReader(configurationString)) {
                    InputAnalyzerManager.CurrentGame = game;
                    Configuration conf = confSerializer.Deserialize(reader) as Configuration;
                    return conf;
                }
            }
            catch (XmlException e)
            {
                UIThread.ShowMessage("", "Xml File cannot be parsed: " + e.Message + "" + "\n" + e.StackTrace +
                    (e.InnerException == null ? "InnerException is null." : "InnerException: " + e.InnerException.Message + "\n" + e.InnerException.StackTrace));
                return null;
            }
            catch (Exception e)
            {
                UIThread.ShowMessage("", e.Message + "" + "\n" + e.StackTrace +
                    (e.InnerException == null ? "InnerException is null." : "InnerException: " + e.InnerException.Message + "\n" + e.InnerException.StackTrace));
                return null;
            }
        }

        public static Configuration LoadConfiguration()
        {
            if (File.Exists("default_config.xml"))
                return LoadConfiguration("default_config.xml",null);
            return null;
        }

        public static string GetConfigurationXml(Configuration configuration){
            XmlSerializer confSerializer = new XmlSerializer(typeof(XmlSerializableConfiguration));
            using (StringWriter textWriter = new StringWriter())
            {
                confSerializer.Serialize(textWriter, configuration);
                return textWriter.ToString();
            }
        }

        public static void SaveConfiguration(Configuration configuration, string configurationFile, string pathToFile)
        {
            _currentConfigurationFile = configurationFile;
            XmlSerializer confSerializer = new XmlSerializer(typeof(XmlSerializableConfiguration));

            if (configurationFile.Contains('\\'))
            {
                configurationFile = configurationFile.Substring(configurationFile.LastIndexOf('\\') + 1);
            }
            StreamWriter confWriter = new StreamWriter(pathToFile + configurationFile);
            confSerializer.Serialize(confWriter, configuration);
            confWriter.Close();
        }


        public static void SaveConfiguration(Configuration configuration)
        {
            //XmlSerializer confSerializer = new XmlSerializer(typeof(XmlSerializableConfiguration));
            //StreamWriter confWriter = new StreamWriter(_currentConfigurationFile);
            //confSerializer.Serialize(confWriter, configuration);
            //confWriter.Close();
            SaveConfiguration(configuration, _currentConfigurationFile, "");
        }

        public static void SaveConfiguration(Configuration configuration, string configurationFile)
        {
            SaveConfiguration(configuration, configurationFile, "");
        }

    }
}
