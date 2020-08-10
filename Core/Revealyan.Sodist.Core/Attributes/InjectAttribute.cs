using System;
using System.Collections.Generic;
using System.Text;

namespace Revealyan.Sodist.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class InjectAttribute : Attribute
    {
        public string Name { get; } = string.Empty;

        public InjectAttribute()
        {

        }
    }
}
