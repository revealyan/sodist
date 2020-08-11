namespace Revealyan.Sodist.Core.Configurations
{
    public struct ComponentInfo
    {
        public string Name { get; set; }
        public TypeInfo Type { get; set; }
        public TypeInfo[] BaseTypes { get; set; }
        public DependencyInfo[] Dependencies { get; set; }
        public ParameterInfo[] Parameters { get; set; }
    }
}