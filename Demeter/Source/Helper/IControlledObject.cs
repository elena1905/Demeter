using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Demeter
{
    interface IControlledObject
    {
        void Control(IController controller);
    }
}
