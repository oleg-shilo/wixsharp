using System;

namespace WixSharp
{
    /// <summary>
    /// Service or group of services that must start before the parent service.
    /// </summary>
    public class ServiceDependency : WixEntity, IGenericEntity
    {
        /// <summary>
        /// The value of this attribute should be one of the following:
        /// 1) The name(not the display name) of a previously installed service.
        /// 2) The name of a service group(in which case the Group attribute must be set to 'yes').
        /// </summary>
        [Xml]
        public new string Id;

        /// <summary>
        /// Set to 'yes' to indicate that the value in the Id attribute is the name of a group of services.
        /// </summary>
        [Xml]
        public bool? Group;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceDependency" /> class.
        /// </summary>
        /// <param name="id">The ID.</param>
        public ServiceDependency(string id)
        {
            Id = id;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceDependency" /> class.
        /// </summary>
        /// <param name="id">The ID.</param>
        /// <param name="group">The Group</param>
        public ServiceDependency(string id, bool group)
        {
            Id = id;
            Group = group;
        }

        /// <summary>
        /// Adds itself as an XML content into the WiX source being generated from the <see cref="WixSharp.Project"/>.
        /// See 'Wix#/samples/Extensions' sample for the details on how to implement this interface correctly.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Process(ProcessingContext context)
        {
            context.XParent.Add(this.ToXElement(GetType().Name));
        }
    }
}