using System;

namespace Revealyan.Sodist.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class ParameterAttribute : Attribute
    {
        public string Name { get; set; } = string.Empty;
        public bool Required { get; set; } = false;
        public ParameterAttribute()
        {

        }
    }
}
