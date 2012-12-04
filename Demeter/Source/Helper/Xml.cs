using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace Demeter
{
    static class Xml
    {

        public static void CreateXmlFile()
        {
            DirectoryInfo dir = new DirectoryInfo("Profile");
            if (!dir.Exists)
            {
                dir.Create();
            }

            XmlDocument doc = new XmlDocument();

            XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            doc.AppendChild(dec);

            XmlElement xmlelement;
            xmlelement = doc.CreateElement("profile");
            doc.AppendChild(xmlelement);

            doc.Save(@"Profile\profile.xml");
            doc = null;
        }

        public static List<string> GetValues(string tagName, string attributeName)
        {
            List<string> values = new List<string>();

            if (!File.Exists(@"Profile\profile.xml"))
            {
                CreateXmlFile();
            }
            else
            {
                XmlTextReader reader = new XmlTextReader(@"Profile\profile.xml");
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.Name == tagName)
                        {
                            string value = reader.GetAttribute(attributeName);
                            if (value != null)
                                values.Add(value);
                        }
                    }
                }
                reader.Close();
            }

            return values;
        }

        public static void AddValue(string tagName, string attributeName, string value)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(@"Profile\profile.xml");

            XmlNode node = xmlDoc.SelectSingleNode("profile");

            XmlElement xe = xmlDoc.CreateElement("treasure");
            xe.SetAttribute("id", value);

            node.AppendChild(xe);

            xmlDoc.Save(@"Profile\profile.xml");
        }

        public static void SetValue(string tagName, string attributeName, string value)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(@"Profile\profile.xml");

            XmlNodeList nodeList = xmlDoc.GetElementsByTagName(tagName);
            if (nodeList.Count == 0)
            {
                XmlNode node = xmlDoc.SelectSingleNode("profile");

                XmlElement xe = xmlDoc.CreateElement(tagName);
                xe.SetAttribute(attributeName, value);

                node.AppendChild(xe);
            }
            else
            {
                foreach (XmlNode node in nodeList)
                {
                    if (node.NodeType == XmlNodeType.Element)
                    {
                        ((XmlElement)node).SetAttribute(attributeName, value);
                    }
                }
            }

            xmlDoc.Save(@"Profile\profile.xml");
        }

        public static string StartLevel
        {
            get
            {
                List<string> list = GetValues("startLevel", "name");
                if (list != null && list.Count == 1)
                    return list.First();
                else
                    return null;
            }
            set
            {
                SetValue("startLevel", "name", value);
            }
        }
    }
}
