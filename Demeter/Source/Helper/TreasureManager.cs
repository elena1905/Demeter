using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace Demeter
{
    public class TreasureManager
    {
        string currentLevel;

        int stillToGet;
        public int StillToGet
        {
            get { return stillToGet; }
        }

        public TreasureManager(string levelFileName, int stillToGet)
        {
            this.currentLevel = levelFileName;
            this.stillToGet = stillToGet;
        }

        public void ResetLevel()
        {
            if (File.Exists(@"Profile\profile.xml"))
            {
                File.Delete(@"Profile\profile.xml");
            }
        }

        public void GetTreasure(string levelStr, string childLevelStr, string id)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(@"Profile\profile.xml");

            XmlNode node = xmlDoc.SelectSingleNode("profile");
            XmlElement level = xmlDoc.CreateElement(levelStr);
            node.AppendChild(level);

            XmlElement childLevel = xmlDoc.CreateElement(childLevelStr);
            level.AppendChild(childLevel);

            XmlElement xe = xmlDoc.CreateElement("treasure");
            xe.SetAttribute("id", id);
            childLevel.AppendChild(xe);

            xmlDoc.Save(@"Profile\profile.xml");

            stillToGet--;
        }

        public void GetTreasureNum(string childLevelStr, out int gotten, out int total)
        {
            gotten = 0;
            total = 0;

            string path = "Content/level/" + childLevelStr;

            if (File.Exists(path))
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(path);
                XmlNodeList list = xmlDoc.GetElementsByTagName("treasureCount");
                foreach(XmlNode node in list)
                {
                    if (node.NodeType == XmlNodeType.Element)
                    {
                        string totalStr = ((XmlElement)node).GetAttribute("total");
                        total = int.Parse(totalStr);
                    }
                }

                xmlDoc.Load("Profile/profile.xml");
                list = xmlDoc.GetElementsByTagName("treasure");
                gotten = list.Count;
            }
        }

        public List<String> AreGotten()
        {
            return Xml.GetValues("treasure", "id");
        }
    }
}
