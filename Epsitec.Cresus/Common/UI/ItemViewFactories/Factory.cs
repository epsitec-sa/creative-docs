//	Copyright © 2006-2012, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.UI.ItemViewFactories
{
	using Assembly=System.Reflection.Assembly;

	/// <summary>
	/// The <c>ItemViewFactoryGetter</c> delegate is used to find an <see cref="IItemViewFactory"/>
	/// interface for the specified <see cref="ItemView"/>.
	/// </summary>
	/// <param name="view">The item view.</param>
	/// <returns>The item view factory or <c>null</c>.</returns>
	public delegate IItemViewFactory ItemViewFactoryGetter(ItemView view);

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
			Factory.cache = new Dictionary<System.Type, IItemViewFactory> ();

			Assembly[] assemblies = TypeEnumerator.Instance.GetLoadedAssemblies ().ToArray ();

			AssemblyLoader.AssemblyLoaded += Factory.HandleDomainAssemblyLoaded;

			foreach (Assembly assembly in assemblies)
			{
				Factory.Analyze (assembly);
			}
		}

		private static void Analyze(Assembly assembly)
		{
			foreach (KeyValuePair<System.Type, System.Type> pair in ItemViewFactoryAttribute.GetRegisteredTypes (assembly))
			{
				IItemViewFactory factory = System.Activator.CreateInstance (pair.Key) as IItemViewFactory;
				
				lock (Factory.exclusion)
				{
					Factory.cache[pair.Value] = factory;
				}
			}
		}

		private static void HandleDomainAssemblyLoaded(object sender, System.AssemblyLoadEventArgs args)
		{
			if (!args.LoadedAssembly.ReflectionOnly)
			{
				Factory.Analyze (args.LoadedAssembly);
			}
		}

		#endregion

		private static readonly Dictionary<System.Type, IItemViewFactory> cache;
		private static readonly object exclusion = new object ();
	}
}
