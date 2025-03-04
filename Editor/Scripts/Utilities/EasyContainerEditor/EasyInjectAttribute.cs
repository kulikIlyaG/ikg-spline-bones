using System;

namespace IKGTools.Editor.EasyContainer
{
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false)]
    public class EasyInjectAttribute : Attribute
    {
        public bool Optional { get; set; } = false;
    }
}