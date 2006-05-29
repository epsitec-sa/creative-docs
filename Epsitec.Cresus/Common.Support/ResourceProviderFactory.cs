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

		public IEnumerable<Allocator> Allocators
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
						this.allocators.Add (this.BuildDynamicAllocator (type));
					}
				}
			}
		}

		private Allocator BuildDynamicAllocator(System.Type type)
		{
			System.Type[] constructorArgumentTypes = new System.Type[] { typeof (ResourceManager) };

			System.Reflection.Module module = typeof (ResourceProviderFactory).Module;
			System.Reflection.Emit.DynamicMethod dynamicMethod = new System.Reflection.Emit.DynamicMethod ("DynamicAllocator", type, constructorArgumentTypes, module, true);
			System.Reflection.Emit.ILGenerator ilGen = dynamicMethod.GetILGenerator ();

			ilGen.Emit (System.Reflection.Emit.OpCodes.Nop);
			ilGen.Emit (System.Reflection.Emit.OpCodes.Ldarg_0);
			ilGen.Emit (System.Reflection.Emit.OpCodes.Newobj, type.GetConstructor (constructorArgumentTypes));
			ilGen.Emit (System.Reflection.Emit.OpCodes.Ret);

			return (Allocator) dynamicMethod.CreateDelegate (typeof (Allocator));
		}

		public delegate IResourceProvider Allocator(ResourceManager manager);

		private List<Allocator> allocators = new List<Allocator> ();
	}
}
