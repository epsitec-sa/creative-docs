//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;

namespace Epsitec.Common.UI.ItemViewFactories
{
	using Assembly=System.Reflection.Assembly;

	/// <summary>
	/// The <c>Factory</c> class provides access to the <see cref="IItemViewFactory"/>
	/// instances, based on the item types which must be represented.
	/// </summary>
	public static class Factory
	{
		/// <summary>
		/// Gets the item view factory for the specified item view.
		/// </summary>
		/// <param name="view">The item view.</param>
		/// <returns>The <see cref="IItemViewFactory"/> or <c>null</c>.</returns>
		public static IItemViewFactory GetItemViewFactory(ItemView view)
		{
			if ((view == null) ||
				(view.Item == null))
			{
				return null;
			}

			System.Type type = view.Item.GetType ();

			IItemViewFactory factory;

			lock (Factory.exclusion)
			{
				if (Factory.cache.TryGetValue (type, out factory))
				{
					return factory;
				}

				//	There is no explicit factory for the type we are looking for.
				//	But maybe one of our base types has a factory. Walk the tree.
				
				System.Type iter = type.BaseType;

				while (iter != null)
				{
					if (Factory.cache.TryGetValue (iter, out factory))
					{
						//	We found a base type for which there is a factory; keep
						//	track of the factory for our type in order to speed up
						//	subsequent searches.
						
						Factory.cache[type] = factory;
						return factory;
					}
					
					iter = iter.BaseType;
				}
			}
			
			return null;
		}

		public static void Setup()
		{
		}

		#region Setup and Run-Time Analysis Methods

		static Factory()
		{
			Factory.domain     = System.AppDomain.CurrentDomain;
			Factory.assemblies = new List<Assembly> ();
			Factory.cache      = new Dictionary<System.Type, IItemViewFactory> ();

			Assembly[] assemblies = Factory.domain.GetAssemblies ();

			Factory.domain.AssemblyLoad += new System.AssemblyLoadEventHandler (Factory.HandleDomainAssemblyLoad);

			foreach (Assembly assembly in assemblies)
			{
				Factory.Analyse (assembly);
			}
		}

		private static void Analyse(Assembly assembly)
		{
			foreach (System.Type type in ItemViewFactoryAttribute.GetRegisteredTypes (assembly))
			{
				IItemViewFactory factory = System.Activator.CreateInstance (type) as IItemViewFactory;
				
				lock (Factory.exclusion)
				{
					Factory.cache[type] = factory;
				}
			}
		}

		private static void HandleDomainAssemblyLoad(object sender, System.AssemblyLoadEventArgs args)
		{
			Factory.assemblies.Add (args.LoadedAssembly);
			Factory.Analyse (args.LoadedAssembly);
		}

		#endregion

		private static System.AppDomain domain;
		private static List<Assembly> assemblies;
		private static Dictionary<System.Type, IItemViewFactory> cache;
		private static object exclusion = new object ();
	}
}
