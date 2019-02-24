#region Licence...

/*
The MIT License (MIT)

Copyright (c) 2014 Oleg Shilo

Permission is hereby granted,
free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

#endregion Licence...

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using IO = System.IO;

namespace WixSharp
{
    /// <summary>
    /// Base class for all Wix# related types
    /// </summary>
    public class WixObject
    {
        /// <summary>
        /// Collection of Attribute/Value pairs for WiX element attributes not supported directly by Wix# objects.
        /// <para>You should use <c>Attributes</c> if you want to inject specific XML attributes
        /// for a given WiX element.</para>
        /// <para>For example <c>Hotkey</c> attribute is not supported by Wix# <see cref="T:WixSharp.Shortcut"/>
        /// but if you want them to be set in the WiX source file you may achieve this be setting
        /// <c>WixEntity.Attributes</c> member variable:
        /// <para> <code>new Shortcut { Attributes= new { {"Hotkey", "0"} }</code> </para>
        /// <remarks>
        /// You can also inject attributes into WiX components "related" to the <see cref="WixEntity"/> but not directly
        /// represented in the Wix# entities family. For example if you need to set a custom attribute for the WiX <c>Component</c>
        /// XML element you can use corresponding <see cref="T:WixSharp.File"/> attributes. The only difference from
        /// the <c>Hotkey</c> example is the composite (column separated) key name:
        /// <para> <code>new File { Attributes= new { {"Component:SharedDllRefCount", "yes"} }</code> </para>
        /// The code above will force the Wix# compiler to insert "SharedDllRefCount" attribute into <c>Component</c>
        /// XML element, which is automatically generated for the <see cref="T:WixSharp.File"/>.
        /// <para>Currently the only supported "related" attribute is  <c>Component</c>.</para>
        /// <para>Note the attribute key can include xml namespace prefix:
        /// <code>
        /// { "{dep}ProviderKey", "01234567-8901-2345-6789-012345678901" }
        /// </code>
        /// Though in this case the required namespace must be already added to the element/document.</para>
        /// </remarks>
        /// </para>
        /// </summary>
        public Dictionary<string, string> Attributes
        {
            get
            {
                ProcessAttributesDefinition();
                return attributes;
            }
            set
            {
                attributes = value;
            }
        }

        private Dictionary<string, string> attributes = new Dictionary<string, string>();

        /// <summary>
        /// Optional attributes of the <c>WiX Element</c> (e.g. Secure:YesNoPath) expressed as a string KeyValue pairs (e.g. "StartOnInstall=Yes; Sequence=1").
        /// <para>OptionalAttributes just redirects all access calls to the <see cref="T:WixEntity.Attributes"/> member.</para>
        /// <para>You can also use <see cref="T:WixEntity.AttributesDefinition"/> to keep the code cleaner.</para>
        /// <para>Note <c>name</c> can include xml namespace prefix:
        /// <code>
        /// AttributesDefinition = "{dep}ProviderKey=01234567-8901-2345-6789-012345678901"
        /// </code>
        /// Though in this case the required namespace must be already added to the element/document.</para>
        /// </summary>
        /// <example>
        /// <code>
        /// var webSite = new WebSite
        /// {
        ///     Description = "MyWebSite",
        ///     Attributes = new Dictionary&lt;string, string&gt; { { "StartOnInstall", "Yes" },  { "Sequence", "1" } }
        ///     //or
        ///     AttributesDefinition = "StartOnInstall=Yes; Sequence=1"
        ///     ...
        /// </code>
        /// </example>
        public string AttributesDefinition { get; set; }

        internal Dictionary<string, string> attributesBag = new Dictionary<string, string>();

        private void ProcessAttributesDefinition()
        {
            if (!AttributesDefinition.IsEmpty())
            {
                var attrToAdd = new Dictionary<string, string>();
                var attrs = AttributesDefinition.ToDictionary();

                try
                {
                    this.Attributes = AttributesDefinition.ToDictionary();
                }
                catch (Exception e)
                {
                    throw new Exception("Invalid AttributesDefinition", e);
                }
            }

            foreach (var item in attributesBag)
                this.attributes[item.Key] = item.Value;
        }

        internal string GetAttributeDefinition(string name)
        {
            var preffix = name + "=";

            return (AttributesDefinition ?? "").Trim()
                                               .Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                                               .Where(x => x.StartsWith(preffix))
                                               .Select(x => x.Substring(preffix.Length))
                                               .FirstOrDefault();
        }

        internal void SetAttributeDefinition(string name, string value, bool append = false)
        {
            var preffix = name + "=";

            var allItems = (AttributesDefinition ?? "").Trim()
                                                       .Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                                                       .ToList();

            var items = allItems;

            if (value.IsNotEmpty())
            {
                if (append)
                {
                    //add index to the items with the same key
                    var similarNamedItems = allItems.Where(x => x.StartsWith(name)).ToArray();
                    items.Add(name + similarNamedItems.Count() + "=" + value);
                }
                else
                {
                    //reset items with the same key
                    items.RemoveAll(x => x.StartsWith(preffix));
                    items.Add(name + "=" + value);
                }
            }

            AttributesDefinition = string.Join(";", items.ToArray());
        }

        /// <summary>
        /// <see cref="Feature"></see> the Wix object belongs to. This member is processed only for the
        /// WiX objects/elements that can be associated with the features (e.g. WebSite, FirewallException, ODBCDataSource, User,
        /// EnvironmentVariable, Merge, Dir, RegFile, RegValue, Shortcut, SqlDatabase, SqlScript).
        /// <remarks>
        /// <para>
        /// Wix# makes an emphasis on the main stream scenarios when an entity (e.g. File) belongs to a single feature.
        /// And this member is to serve these scenarios via constructors or initializers.
        /// </para>
        /// However MSI/WiX allows the same component to be included into multiple features. If it is what your deployment logic dictates
        /// then you need to use <see cref="WixSharp.WixObject.Features"/>.
        /// </remarks>
        /// </summary>
        public Feature Feature
        {
            set { feature = value; }
            internal get { return feature; }
        }

        private Feature feature;

        /// <summary>
        /// The collection of <see cref="Feature"></see>s the Wix object belongs to. This member is processed only for the
        /// WiX objects/elements that can be associated with the features (e.g. WebSite, FirewallException, ODBCDataSource, User,
        /// EnvironmentVariable, Merge, Dir, RegFile, RegValue, Shortcut, SqlDatabase).
        /// <remarks>
        /// Note, this member is convenient for scenarios where the same component is to be included into multiple features.
        /// However, if the component is to be associated only with a single feature then <see cref="WixSharp.WixObject.Feature"/>
        /// is a more convenient choice as it can be initialized either via constructors or object initializers.eature.
        /// </remarks>
        /// </summary>
        public Feature[] Features = new Feature[0];

        /// <summary>
        /// Gets the actual list of features associated with the Wix object/element. It is a combined
        /// collection of <see cref="WixSharp.WixObject.Features"/> and <see cref="WixSharp.WixObject.Feature"/>.  union of the
        /// </summary>
        /// <value>
        /// The actual features.
        /// </value>
        public Feature[] ActualFeatures
        {
            get
            {
                return Features.Concat(this.feature.ToItems())
                               .Where(x => x != null)
                               .Distinct()
                               .ToArray();
            }
        }

        /// <summary>
        /// Maps the component to features. If no features specified then the component is added to the default ("Complete") feature.
        /// </summary>
        /// <param name="componentId">The component identifier.</param>
        /// <param name="features">The features.</param>
        /// <param name="context">The context.</param>
        public static void MapComponentToFeatures(string componentId, Feature[] features, ProcessingContext context)
        {
            var project = (Project)context.Project;

            if (features.Any())
                context.FeatureComponents.Map(features, componentId);
            else if (context.FeatureComponents.ContainsKey(project.DefaultFeature))
                context.FeatureComponents[project.DefaultFeature].Add(componentId);
            else
                context.FeatureComponents[project.DefaultFeature] = new List<string> { componentId };
        }
    }

    /// <summary>
    /// Alias for the <c>Dictionary&lt;string, string&gt; </c> type.
    /// </summary>
    public class Attributes : Dictionary<string, string>
    {
    }

    /// <summary>
    /// Generic <see cref="T:WixSharp.WixEntity"/> container for defining WiX <c>Package</c> element attributes.
    /// <para>These attributes are the properties of the package to be placed in the Summary Information Stream. These are visible from COM through the IStream interface, and can be seen in Explorer.</para>
    ///<example>The following is an example of defining the <c>Package</c> attributes.
    ///<code>
    /// var project =
    ///     new Project("My Product",
    ///         new Dir(@"%ProgramFiles%\My Company\My Product",
    ///
    ///     ...
    ///
    /// project.Package.AttributesDefinition = @"AdminImage=Yes;
    ///                                          Comments=Release Candidate;
    ///                                          Description=Fantastic product...";
    ///
    /// Compiler.BuildMsi(project);
    /// </code>
    /// </example>
    /// </summary>
    public class Package : WixEntity
    {
    }

    /// <summary>
    /// Base class for all Wix# types representing WiX XML elements (entities)
    /// </summary>
    public class WixEntity : WixObject
    {
        internal void MoveAttributesTo(WixEntity dest)
        {
            var attrs = this.Attributes;
            var attrsDefinition = this.AttributesDefinition;
            this.Attributes.Clear();
            this.AttributesDefinition = null;
            dest.Attributes = attrs;
            dest.AttributesDefinition = attrsDefinition;
        }

        internal string GenerateComponentId(Project project, string suffix = "")
        {
            return this.GetExplicitComponentId() ??
                   project.ComponentId($"Component.{this.Id}{suffix}");
        }

        internal string GetExplicitComponentId()
        {
            var attrs = this.AttributesDefinition.ToDictionary();
            if (attrs.ContainsKey("Component:Id"))
                return attrs["Component:Id"];
            else
                return null;
        }

        /// <summary>
        /// Gets or sets the id of the Component element that is to contain XML equivalent of the <see cref="WixEntity"/>.
        /// </summary>
        /// <value>
        /// The component identifier.
        /// </value>
        public string ComponentId
        {
            get => GetAttributeDefinition("Component:Id");
            set => SetAttributeDefinition("Component:Id", value);
        }

        /// <summary>
        /// Gets or sets the Condition attribute of the Component element that is to contain XML equivalent of the
        /// <see cref="WixEntity"/>.
        /// </summary>
        /// <value>
        /// The component condition.
        /// </value>
        public string ComponentCondition
        {
            get => GetAttributeDefinition("Component:Condition");
            set => SetAttributeDefinition("Component:Condition", value);
        }

        internal void AddInclude(string xmlFile, string parentElement)
        {
            SetAttributeDefinition("WixSharpCustomAttributes:xml_include", parentElement + "|" + xmlFile, append: true);
        }

        /// <summary>
        /// Name of the <see cref="WixEntity"/>.
        /// <para>This value is used as a <c>Name</c> for the corresponding WiX XML element.</para>
        /// </summary>
        public string Name = "";

        /// <summary>
        /// Gets or sets the <c>Id</c> value of the <see cref="WixEntity"/>.
        /// <para>This value is used as a <c>Id</c> for the corresponding WiX XML element.</para>
        /// <para>If the <see cref="Id"/> value is not specified explicitly by the user the Wix# compiler
        /// generates it automatically insuring its uniqueness.</para>
        /// <remarks>
        ///  Note: The ID auto-generation is triggered on the first access (evaluation) and in order to make the id
        ///  allocation deterministic the compiler resets ID generator just before the build starts. However if you
        ///  accessing any auto-id before the Build*() is called you can it interferes with the ID auto generation and eventually
        ///  lead to the WiX ID duplications. To prevent this from happening either:"
        ///  <para> - Avoid evaluating the auto-generated IDs values before the call to Build*()</para>
        ///  <para> - Set the IDs (to be evaluated) explicitly</para>
        ///  <para> - Prevent resetting auto-ID generator by setting WixEntity.DoNotResetIdGenerator to true";</para>
        /// </remarks>
        /// </summary>
        /// <value>The id.</value>
        public string Id
        {
            get
            {
                if (id.IsEmpty())
                {
                    if (this is Feature)
                    {
                        // break point parking spot
                    }
                    id = Compiler.AutoGeneration.CustomIdAlgorithm?.Invoke(this) ?? IncrementalIdFor(this);
                }

                return id;
            }
            set
            {
                id = value;
                isAutoId = false;
            }
        }

        /// <summary>
        /// Index based Id generation algorithm.
        /// <para>It is the default algorithm, which generates the most human readable Id. Thus if the
        /// project has two `index.html` files one will be assigned Id `index.html` and another one
        /// `index.html.1`.</para>
        /// <para> Limitations: If two files have the same name it is non-deterministic which one gets
        /// clear Id and which one the indexed one.</para>
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public static string IncrementalIdFor(WixEntity entity)
        {
            lock (idMaps)
            {
                if (!idMaps.ContainsKey(entity.GetType()))
                    idMaps[entity.GetType()] = new Dictionary<string, int>();

                var rawName = entity.Name.Expand();
                if (rawName.IsEmpty())
                    rawName = entity.GetType().Name;

                if (IO.Path.IsPathRooted(entity.Name))
                    rawName = IO.Path.GetFileName(entity.Name).Expand();

                if (entity.GetType() != typeof(Dir) && entity.GetType().BaseType != typeof(Dir) && entity.Name.IsNotEmpty())
                    rawName = IO.Path.GetFileName(entity.Name).Expand();

                //Maximum allowed length for a stream name is 62 characters long; In some cases more but to play it safe keep 62 limit
                //
                //Note: the total limit 62 needs to include in some cases MSI auto prefix (e.g. table name) ~15 chars
                // our hash code (max 10 chars) and our decoration (separators). So 30 seems to be a safe call
                //
                int maxLength = 30;
                if (rawName.Length > maxLength)
                {
                    //some chars are illegal as star if the ID so work around this with '_' prefix
                    rawName = "_..." + rawName.Substring(rawName.Length - maxLength);
                }

                string rawNameKey = rawName.ToLower();

                /*
                 "bin\Release\similarFiles.txt" and "bin\similarfiles.txt" will produce the following IDs
                 "Component.similarFiles.txt" and "Component.similariles.txt", which will be treated by Wix compiler as duplication
                 */

                if (!idMaps[entity.GetType()].ContainsSimilarKey(rawName)) //this Type has not been generated yet
                {
                    idMaps[entity.GetType()][rawNameKey] = 0;
                    entity.id = rawName;
                    if (char.IsDigit(entity.id.Last()))
                        entity.id += "_"; // work around for https://wixsharp.codeplex.com/workitem/142
                                          // to avoid potential collision between ids that end with digit
                                          // and auto indexed (e.g. [rawName + "." + index])
                }
                else
                {
                    //The Id has been already generated for this Type with this rawName
                    //so just increase the index
                    var index = idMaps[entity.GetType()][rawNameKey] + 1;

                    entity.id = rawName + "." + index;
                    idMaps[entity.GetType()][rawNameKey] = index;
                }

                //Trace.WriteLine(">>> " + GetType() + " >>> " + id);

                if (rawName.IsNotEmpty() && char.IsDigit(rawName[0]))
                    entity.id = "_" + entity.id;

                while (alreadyTakenIds.Contains(entity.id)) //last line of defense against duplication
                    entity.id += "_";

                alreadyTakenIds.Add(entity.id);

                return entity.id;
            }
        }

        private static List<string> alreadyTakenIds = new List<string>();

        internal bool isAutoId = true;

        /// <summary>
        /// Backing value of <see cref="Id"/>.
        /// </summary>
        protected string id;

        internal string RawId { get { return id; } }

        /// <summary>
        /// The do not reset auto-ID generator before starting the build.
        /// </summary>
        static public bool DoNotResetIdGenerator = false;

        private static Dictionary<Type, Dictionary<string, int>> idMaps = new Dictionary<Type, Dictionary<string, int>>();

        /// <summary>
        /// Resets the <see cref="Id"/> generator. This method is exercised by the Wix# compiler before any
        /// <c>Build</c> operations to ensure reproducibility of the <see cref="Id"/> set between <c>Build()</c>
        /// calls.
        /// </summary>
        static public void ResetIdGenerator()
        {
            idMaps.Clear();
            alreadyTakenIds.Clear();
        }

        static internal void ResetIdGenerator(bool supressWarning)
        {
            if (!DoNotResetIdGenerator)
            {
                // ServiceInstaller and SvcEvent are the only types that are expected to be allocated before
                // the BuildMsi calls. This is because they are triggered on setting `ServiceInstaller.StartOn`
                // and other similar service actions.
                bool anyAlreadyAllocatedIds = idMaps.Keys.Any(x => x != typeof(ServiceInstaller) && x != typeof(SvcEvent));

                if (anyAlreadyAllocatedIds && !supressWarning)
                {
                    var message = "----------------------------\n" +
                    "Warning: Wix# compiler detected that some IDs has been auto-generated before the build started. " +
                    "This can lead to the WiX ID duplications on consecutive 'Build*' calls.\n" +
                    "To prevent this from happening either:\n" +
                    "   - Avoid evaluating the auto-generated IDs values before the call to Build*\n" +
                    "   - Set the IDs (to be evaluated) explicitly\n" +
                    "   - Prevent resetting auto-ID generator by setting WixEntity.DoNotResetIdGenerator to true\n" +
                    "----------------------------";
                    Compiler.OutputWriteLine(message);
                }
                ResetIdGenerator();
            }
        }

        internal bool IsIdSet()
        {
            return !id.IsEmpty();
        }
    }
}