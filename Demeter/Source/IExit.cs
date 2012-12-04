using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Demeter
{
    public interface IExit
    {
        string LevelFileName
        { get; }
        bool Leave
        { get; }
    }
}
