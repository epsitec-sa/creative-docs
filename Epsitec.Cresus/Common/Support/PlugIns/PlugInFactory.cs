//	Copyright © 2008-2014, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

namespace Epsitec.Common.Support.PlugIns
{
    using Assembly = System.Reflection.Assembly;

    /// <summary>
    /// The <c>PlugInFactory</c> class provides a generic implementation of
    /// a class resolver which instantiates objects based on an id; the classes
    /// are identified through the use of a dedicated attribute.
    /// </summary>
    /// <typeparam name="TClass">The base type of the class to instantiate.</typeparam>
    /// <typeparam name="TAttribute">The type of the dedicated attribute.</typeparam>
    /// <typeparam name="TId">The type of the id.</typeparam>
    public class PlugInFactory<TClass, TAttribute, TId>
        where TAttribute : System.Attribute, IPlugInAttribute<TId>
    {
        /// <summary>
        /// Creates an instance of the specified class.
        /// </summary>
        /// <param name="id">The id of the class to instantiate.</param>
        /// <returns>The instance of the specified class.</returns>
        protected static TClass CreateInstance(TId id)
        {
            Record record;

            if (PlugInFactory<TClass, TAttribute, TId>.types.TryGetValue(id, out record))
            {
                var allocator = record.GetAllocator<Allocator<TClass>>(type =>
                    DynamicCodeFactory.CreateAllocator<TClass>(type)
                );
                return allocator();
            }
            else
            {
                return default(TClass);
            }
        }

        /// <summary>
        /// Gets the (singleton) template instance for the specified type.
        /// </summary>
        /// <param name="id">The id of the class to instantiate.</param>
        /// <returns>The instance of the specified class.</returns>
        protected static TClass GetTemplateInstance(TId id)
        {
            Record record;

            if (PlugInFactory<TClass, TAttribute, TId>.types.TryGetValue(id, out record))
            {
                if (record.Template == null)
                {
                    lock (record)
                    {
                        var allocator = record.GetAllocator<Support.Allocator<TClass>>(type =>
                            DynamicCodeFactory.CreateAllocator<TClass>(type)
                        );
                        record.Template = allocator();
                    }
                }

                return (TClass)record.Template;
            }
            else
            {
                throw new System.InvalidOperationException(
                    string.Format("Cannot find class for id '{0}'", id)
                );
            }
        }

        /// <summary>
        /// Creates an instance of the specified class, using the specified XML element
        /// to rehydrate the instance.
        /// </summary>
        /// <param name="id">The id of the class to instantiate.</param>
        /// <param name="xml">The XML element.</param>
        /// <returns>
        /// The instance of the specified class.
        /// </returns>
        protected static TClass CreateInstance(TId id, XElement xml)
        {
            Record record;

            if (PlugInFactory<TClass, TAttribute, TId>.types.TryGetValue(id, out record))
            {
                var restorer = record.GetRestorer();

                if (restorer != null)
                {
                    return restorer(xml);
                }
            }

            throw new System.InvalidOperationException(
                string.Format("Cannot find restorable class for id '{0}'", id)
            );
        }

        /// <summary>
        /// Creates an instance of the specified class.
        /// </summary>
        /// <typeparam name="TParameter">The type of the parameter.</typeparam>
        /// <param name="id">The id of the class to instanciate.</param>
        /// <param name="parameter">The parameter.</param>
        /// <returns>The instance of the specified class.</returns>
        protected static TClass CreateInstance<TParameter>(TId id, TParameter parameter)
        {
            Record record;

            if (PlugInFactory<TClass, TAttribute, TId>.types.TryGetValue(id, out record))
            {
                Allocator<TClass, TParameter> allocator = record.GetAllocator<Support.Allocator<
                    TClass,
                    TParameter
                >>(type => DynamicCodeFactory.CreateAllocator<TClass, TParameter>(type));
                return allocator(parameter);
            }
            else
            {
                return default(TClass);
            }
        }

        /// <summary>
        /// Finds the id for the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The id for the specified type.</returns>
        protected static TId FindId(System.Type type)
        {
            foreach (var item in PlugInFactory<TClass, TAttribute, TId>.types)
            {
                if (item.Value.Type == type)
                {
                    return item.Key;
                }
            }

            return default(TId);
        }

        /// <summary>
        /// Finds the type for the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The type.</returns>
        public static System.Type FindType(TId id)
        {
            Record record;

            if (PlugInFactory<TClass, TAttribute, TId>.types.TryGetValue(id, out record))
            {
                return record.Type;
            }
            else
            {
                return null;
            }
        }

        public static IEnumerable<System.Tuple<TId, System.Type>> FindAll()
        {
            foreach (var item in PlugInFactory<TClass, TAttribute, TId>.types)
            {
                yield return new System.Tuple<TId, System.Type>(item.Key, item.Value.Type);
            }
        }

        /// <summary>
        /// Sets up the plug-in factory; scans all available assemblies for
        /// classes marked with the dedicated attribute.
        /// </summary>
        public static void Setup() { }

        #region Setup and Run-Time Analysis Methods

        static PlugInFactory()
        {
            PlugInFactory<TClass, TAttribute, TId>.types = new Dictionary<TId, Record>();

            var assemblies = AssemblyLoader.FindMatching(AssemblyLoadMode.OnlySigned);

            AssemblyLoader.AssemblyLoaded += PlugInFactory<
                TClass,
                TAttribute,
                TId
            >.HandleDomainAssemblyLoaded;

            foreach (Assembly assembly in assemblies)
            {
                if (!assembly.ReflectionOnly)
                {
                    PlugInFactory<TClass, TAttribute, TId>.Analyze(assembly);
                }
            }
        }

        private static void Analyze(Assembly assembly)
        {
            System.Diagnostics.Debug.Assert(assembly.ReflectionOnly == false);

            foreach (
                TAttribute attribute in PlugInFactory<
                    TClass,
                    TAttribute,
                    TId
                >.GetRegisteredAttributes(assembly)
            )
            {
                PlugInFactory<TClass, TAttribute, TId>.types[attribute.Id] = new Record(
                    attribute.Type
                );
            }
        }

        private static void HandleDomainAssemblyLoaded(
            object sender,
            System.AssemblyLoadEventArgs args
        )
        {
            var assembly = args.LoadedAssembly;

            if ((assembly.ReflectionOnly) || (assembly.IsDynamic))
            {
                return;
            }

            if (AssemblyLoader.Match(assembly, AssemblyLoadMode.OnlySigned))
            {
                PlugInFactory<TClass, TAttribute, TId>.Analyze(assembly);
            }
        }

        #endregion

        private static IEnumerable<TAttribute> GetRegisteredAttributes(
            System.Reflection.Assembly assembly
        )
        {
            foreach (TAttribute attribute in assembly.GetCustomAttributes<TAttribute>())
            {
                if (typeof(TClass).IsClass)
                {
                    if (attribute.Type.IsSubclassOf(typeof(TClass)))
                    {
                        yield return attribute;
                    }
                }
                else if (typeof(TClass).IsInterface)
                {
                    if (attribute.Type.ContainsInterface<TClass>())
                    {
                        yield return attribute;
                    }
                }
            }
        }

        #region Private Record Structure

        private class Record
        {
            public Record(System.Type type)
            {
                this.exclusion = new object();
                this.allocator = null;
                this.type = type;
            }

            public System.Type Type
            {
                get { return this.type; }
            }

            public object Template
            {
                get { return this.template; }
                set { this.template = value; }
            }

            public TAlloc GetAllocator<TAlloc>(System.Func<System.Type, TAlloc> getAllocator)
            {
                if (this.allocator == null)
                {
                    lock (this.exclusion)
                    {
                        if (this.allocator == null)
                        {
                            this.allocator = getAllocator(this.type);
                        }
                    }
                }

                return (TAlloc)this.allocator;
            }

            public System.Func<XElement, TClass> GetRestorer()
            {
                if (this.restoreFunc == null)
                {
                    var restoreMethod = this.type.GetMethod(
                        "Restore",
                        System.Reflection.BindingFlags.Public
                            | System.Reflection.BindingFlags.Static
                            | System.Reflection.BindingFlags.InvokeMethod
                    );
                    this.restoreFunc = x => (TClass)restoreMethod.Invoke(null, new object[] { x });
                }

                return this.restoreFunc;
            }

            private readonly object exclusion;
            private readonly System.Type type;
            private object allocator;
            private object template;
            private System.Func<XElement, TClass> restoreFunc;
        }

        #endregion

        private static readonly Dictionary<TId, Record> types;
    }
}
