//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	public sealed class TypeEnumerator
	{
		public TypeEnumerator()
		{
			this.types = new List<System.Type> ();
			this.assemblies = new List<System.Reflection.Assembly> ();
			this.typeNames = new HashSet<string> ();

			System.AppDomain.CurrentDomain.AssemblyLoad += new System.AssemblyLoadEventHandler (this.HandleCurrentDomainAssemblyLoad);

			foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies ())
			{
				this.AnalyseAssembly (assembly);
			}
		}

		public static TypeEnumerator Instance
		{
			get
			{
				return TypeEnumerator.instance;
			}
		}

		public IEnumerable<System.Type> GetAllTypes()
		{
			for (int i = 0; i < this.types.Count; i++)
			{
				yield return this.types[i];
			}
		}

		private void HandleCurrentDomainAssemblyLoad(object sender, System.AssemblyLoadEventArgs args)
		{
			var assembly = args.LoadedAssembly;

			this.AnalyseAssembly (assembly);
		}

		private void AnalyseAssembly(System.Reflection.Assembly assembly)
		{
			if ((!assembly.IsDynamic) &&
				(!assembly.ReflectionOnly))
			{
				lock (this.assemblies)
				{
					this.assemblies.Add (assembly);
				}

				var types = assembly.GetTypes ();

				lock (this.types)
				{
					this.types.AddRange (types);
				}
			}
		}

		private static readonly TypeEnumerator instance = new TypeEnumerator ();

		private readonly List<System.Type> types;
		private readonly List<System.Reflection.Assembly> assemblies;
		private readonly HashSet<string> typeNames;
	}
}
