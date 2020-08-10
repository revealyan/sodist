using System;
using System.Runtime.Serialization;

namespace Revealyan.Sodist.Core.Exceptions
{

    [Serializable]
    public class ComponentException : Exception
    {
        #region data
        public IComponent Component { get; }
        #endregion

        #region ctors
        public ComponentException(IComponent component) : this(component, string.Empty, null) { }
        public ComponentException(IComponent component, string message) : this(component, message, null) { }
        public ComponentException(IComponent component, string message, Exception? inner) : base($"Произошла ошибка в компоненте \"{component.Name}\".{message}", inner)
        {
            Component = component;
        }
        protected ComponentException(IComponent component, SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Component = component;
        }
        #endregion
    }
}
