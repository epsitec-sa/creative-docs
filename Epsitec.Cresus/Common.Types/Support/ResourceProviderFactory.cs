//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Support
{
	public sealed class ResourceProviderFactory
	{
		internal ResourceProviderFactory()
		{
			this.LoadImplementation ();
		}

		public IEnumerable<Allocator<IResourceProvider, ResourceManager>> Allocators
		{
			get
			{
				return this.allocators;
			}
		}

		private void LoadImplementation()
		{
			System.Type[] constructorArgumentTypes = new System.Type[] { typeof (ResourceManager) };
			System.Reflection.Assembly assembly = AssemblyLoader.Load ("Common.Support.Implementation");

			foreach (System.Type type in assembly.GetTypes ())
			{
				if ((type.IsClass) &&
					(!type.IsAbstract))
				{
					if ((type.GetInterface ("IResourceProvider") != null) &&
						(type.GetConstructor (constructorArgumentTypes) != null))
					{
						this.allocators.Add (DynamicCodeFactory.CreateAllocator<IResourceProvider, ResourceManager> (type));
					}
				}
			}
		}

		private List<Allocator<IResourceProvider, ResourceManager>> allocators = new List<Allocator<IResourceProvider, ResourceManager>> ();
	}
}
