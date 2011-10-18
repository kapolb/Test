using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace ExcelBL
{
    public static class Serializator
    {
        /// <summary>
        /// Serializes the settings to the xml file on specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="settings">The settings object.</param>
        public static void Serialize(string path, Template settings)
        {
            try
            {
                using (TextWriter textWriter = new StreamWriter(path))
                {
                    var xmlSerializer = new XmlSerializer(typeof (Template));
                    xmlSerializer.Serialize(textWriter, settings);
                }
            }
            catch (Exception ex)
            {
                throw new SerializationException("Serialization failed!", ex);
            }
        }

        /// <summary>
        /// Deserializes the settings object from the xml file onthe specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static Template Deserialize(string path)
        {
            try
            {
                using (TextReader textReader = new StreamReader(path))
                {
                    var xmlSerializer = new XmlSerializer(typeof (Template));
                    return (Template) xmlSerializer.Deserialize(textReader);
                }
            }
            catch (Exception ex)
            {
                throw new SerializationException("Deserialization failed!", ex);
            }
        }
    }
}
