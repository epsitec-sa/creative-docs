//	Copyright © 2012-2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor;

using Epsitec.Cresus.WebCore.Server.Core.PropertyAutoCreator;

using System;


namespace Epsitec.Cresus.WebCore.Server.Core
{
	/// <summary>
	/// The Caches class contains all the different caches that are used to access to global data
	/// throughout the server.
	/// The main idea of those caches, is that they allow the transfer of data to and from the
	/// javascript client with minimal overhead. Take the type cache for instance. At some places
	/// we want to give to the client a reference to a .Net type, that it will send later on. If
	/// we send the whole type name, that is very long. So we have this cache that contains a
	/// mapping from short ids (typically id1, id2, id3, ...) to the type instances.
	/// Note that all these caches are fully thread safe, and can therefore be accessed
	/// concurrently by several threads.
	/// </summary>
	public sealed class Caches : IDisposable
	{


		public Caches()
		{
			this.propertyAccessorCache = new PropertyAccessorCache ();
			this.autoCreatorCache = new AutoCreatorCache ();
			this.columnIdCache = new IdCache<string> ();
			this.typeCache = new IdCache<Type> ();
		}


		/// <summary>
		/// Stores a mapping from auto generated ids or lambda expressions to the PropertyAccessor
		/// instances.
		/// </summary>
		internal PropertyAccessorCache PropertyAccessorCache
		{
			get
			{
				return this.propertyAccessorCache;
			}
		}


		/// <summary>
		/// Stores a mapping from auto generated ids or lambda expressions to the AutoCreator
		/// instances.
		/// </summary>
		internal AutoCreatorCache AutoCreatorCache
		{
			get
			{
				return this.autoCreatorCache;
			}
		}


		/// <summary>
		/// Stores a mapping from auto generated ids to the column names that are used in the
		/// data sets.
		/// </summary>
		internal IdCache<string> ColumnIdCache
		{
			get
			{
				return this.columnIdCache;
			}
		}


		/// <summary>
		/// Stores a mapping from auto generated ids to .Net type instances.
		/// </summary>
		internal IdCache<Type> TypeCache
		{
			get
			{
				return this.typeCache;
			}
		}


		public void Dispose()
		{
			this.propertyAccessorCache.Dispose ();
			this.autoCreatorCache.Dispose ();
			this.columnIdCache.Dispose ();
			this.typeCache.Dispose ();
		}


		private readonly PropertyAccessorCache	propertyAccessorCache;
		private readonly AutoCreatorCache		autoCreatorCache;
		private readonly IdCache<string>		columnIdCache;
		private readonly IdCache<Type>			typeCache;
	}
}
