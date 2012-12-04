using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Xml;

namespace Demeter
{
    public class Ogre
    {
        OgreBase ogreBase;
        public OgreBase OgreBase
        {
            get { return ogreBase; }
            set { ogreBase = value; }
        }

        OgreCore ogreCore;
        public OgreCore OgreCore
        {
            get { return ogreCore; }
            set { ogreCore = value; }
        }

        public Ogre(Game1 game, XmlTextReader reader)
        {
            string pxStr = reader.GetAttribute("px");
            string pyStr = reader.GetAttribute("py");
            string offsetStr = reader.GetAttribute("offset");

            float px = float.Parse(pxStr);
            float py = float.Parse(pyStr);
            float offset = float.Parse(offsetStr);

            Vector2 position = new Vector2(px, py);

            ogreBase = new OgreBase(game, position);
            ogreCore = new OgreCore(game, position, offset);
        }
    }
}
