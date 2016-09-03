using System;
using System.IO;
using System.Text;
using Spider.Data;

namespace Spider.Common
{
    public class ConfigurationReader
    {
        public static T Read<T>(Encoding encoding,string name)
        {
            string directory = AppDomain.CurrentDomain.BaseDirectory;
            return ReadFromFile<T>(Path.Combine(directory,name), encoding);
        }

        public static T ReadFromFile<T>(string path, Encoding encoding)
        {
            if (!File.Exists(path))
            {
                throw new ArgumentException();
            }
            using (StreamReader sr = new StreamReader(path, encoding))
            {
                string json = sr.ReadToEnd();
                return SmartJosnSerializer.Deserialize<T>(json);
            }
        }
    }
}
