using System;
using System.Collections.Generic;
using System.Linq;

namespace XAF_Bootstrap.ModelExtensions
{
    [FlagsAttribute]
    public enum TagSourceKind
    {
        TypeSource = 0,
        Values = 1,
        ObjectValue = 2
    }
}
