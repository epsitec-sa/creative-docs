//	Copyright � 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
		
		public DependencyObjectType FindObjectType(string name)
		{
			if (this.types.ContainsKey (name))
			{
				return this.types[name];
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
	}
}
