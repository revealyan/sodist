using System;
using System.Collections.Generic;
using System.Text;

namespace Revealyan.Sodist.Core.Configurations
{
    public class ComponentInfo
    {
        public string Name { get; set; }
        public TypeInfo Type { get; set; }
        public TypeInfo[] BaseTypes { get; set; }
        public ParameterInfo[] Parameters { get; set; }
    }
}