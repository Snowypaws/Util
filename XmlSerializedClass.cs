using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace Util
{
    public class XmlSerializedClass<T>
    {
        [XmlIgnore]
        public bool Saved { get; private set; }

        public static T Load(string fileName)
        {
            if (File.Exists(fileName))
            {
                XmlSerializer reader = new XmlSerializer(typeof(T));
                FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);

                T config = default(T);

                try
                {
                    config = (T)reader.Deserialize(stream);
                }
                catch (Exception e)
                {
                    return default(T);
                }

                if (config is XmlSerializedClass<T>)
                {
                    (config as XmlSerializedClass<T>).Saved = true;
                }

                stream.Close();

                var serialized = config as XmlSerializedClass<T>;

                if (serialized != null)
                    serialized.OnLoaded();

                return config;
            }

            return default(T);
        }

        public static T Load()
        {
            return Load(typeof(T).Name + ".cfg");
        }

        public virtual void Save()
        {
            Save(typeof(T).Name + ".cfg");
        }

        public virtual void OnLoaded()
        {
            
        }

        public virtual bool Read(string fileName)
        {
            if (File.Exists(fileName))
            {
                XmlSerializer reader = new XmlSerializer(GetType());
                FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);

                object temp = null;

                try
                {
                    temp = reader.Deserialize(stream);
                }
                catch (Exception ex)
                {
                    return false;
                }

                if (temp != null)
                {
                    var properties = temp.GetType().GetProperties();

                    foreach (var property in properties)
                    {
                        if (!property.GetCustomAttributes(typeof(XmlIgnoreAttribute), true).Any()
                            && property.CanWrite && property.CanRead)
                        {
                            property.SetValue(this, property.GetValue(temp));
                        }
                    }

                    return true;
                }
                else
                    return false;
            }

            return false;
        }

        public virtual void Save(string fileName)
        {
            XmlSerializer writer = new XmlSerializer(GetType());
            FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write);

            writer.Serialize(stream, this);

            stream.Close();
        }
    }
}
