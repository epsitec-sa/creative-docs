//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types.Serialization
{
	using Assembly = System.Reflection.Assembly;
	
	public sealed class DependencyClassManager
	{
		private DependencyClassManager()
		{
			this.domain     = System.AppDomain.CurrentDomain;
			this.assemblies = new List<Assembly> ();
			this.types      = new Dictionary<string, DependencyObjectType> ();
			this.converters = new Dictionary<System.Type, ISerializationConverter> ();

			Assembly[] assemblies = this.domain.GetAssemblies ();
			
			this.domain.AssemblyLoad += new System.AssemblyLoadEventHandler (this.HandleDomainAssemblyLoad);

			foreach (Assembly assembly in assemblies)
			{
				this.Analyse (assembly);
			}
		}

		public static DependencyClassManager	Current
		{
			get
			{
				return DependencyClassManager.current;
			}
		}

		public static void Setup()
		{
		}


		/// <summary>
		/// Finds the object type for the given name.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns>The object type or <c>null</c> if none can be found.</returns>
		public DependencyObjectType FindObjectType(string name)
		{
			DependencyObjectType objectType;
			
			if (this.types.TryGetValue (name, out objectType))
			{
				return objectType;
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Finds the serialization converter for the specified type.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns>The serialization converter or <c>null</c> if none can be found.</returns>
		public ISerializationConverter FindSerializationConverter(System.Type type)
		{
			ISerializationConverter converter;
			
			if (this.converters.TryGetValue (type, out converter))
			{
				return converter;
			}
			else
			{
				return null;
			}
		}
		
		private void Analyse(Assembly assembly)
		{
			foreach (System.Type type in DependencyClassAttribute.GetRegisteredTypes (assembly))
			{
				DependencyObjectType depType = DependencyObjectType.FromSystemType (type);
				string name = type.FullName;
				
				this.types[name] = depType;
			}

			foreach (DependencyClassAttribute attribute in DependencyClassAttribute.GetConverterAttributes (assembly))
			{
				this.converters[attribute.Type] = System.Activator.CreateInstance (attribute.Converter) as ISerializationConverter;
			}
		}

		private void HandleDomainAssemblyLoad(object sender, System.AssemblyLoadEventArgs args)
		{
			this.assemblies.Add (args.LoadedAssembly);
			this.Analyse (args.LoadedAssembly);
		}
		
		private static DependencyClassManager	current = new DependencyClassManager ();
		
		private System.AppDomain				domain;
		private List<Assembly>					assemblies;
		private Dictionary<string, DependencyObjectType> types;
		private Dictionary<System.Type, ISerializationConverter> converters;
	}
}
