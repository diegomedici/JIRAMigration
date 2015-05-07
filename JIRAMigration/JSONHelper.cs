using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace JIRAMigration
{
    public static class JSONHelper
    {
        public static T Deserialise<T>(string json)
        {
            T obj = Activator.CreateInstance<T>();
            MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(json));
            DataContractJsonSerializer serialiser = new DataContractJsonSerializer(obj.GetType());
            ms.Close();
            return obj;
        }
    }
}