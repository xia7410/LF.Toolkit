using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LF.Toolkit.IOC
{
    /// <summary>
    /// 全局注入标签类
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = true)]
    public class InjectableAttribute : Attribute
    {
        /// <summary>
        /// Specifies that a type provides its own concrete type as a service.
        /// </summary>
        public bool AsSelf { get; set; } = true;

        /// <summary>
        /// Specifies that a type is registered as providing all of its implemented interfaces.
        /// </summary>
        public bool AsImplementedInterfaces { get; set; } = true;

        /// <summary>
        /// Configure the component so that every dependent component or call to Resolve() gets the same, shared instance.
        /// </summary>
        public bool SingleInstance { get; set; } = true;
    }
}
