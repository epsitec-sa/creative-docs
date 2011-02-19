//	Copyright © 2006-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <c>ResourceProviderFactory</c> class loads the resource provider implementation
	/// from an external assembly and publishes the available <see cref="IResourceProvider"/>
	/// allocators.
	/// </summary>
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
			System.Reflection.Assembly assembly = System.Reflection.Assembly.GetAssembly (typeof (ResourceManager));

			System.Diagnostics.Debug.Assert (assembly != null);

			foreach (var type in assembly.GetTypes ())
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

		private readonly List<Allocator<IResourceProvider, ResourceManager>> allocators = new List<Allocator<IResourceProvider, ResourceManager>> ();
	}
}
