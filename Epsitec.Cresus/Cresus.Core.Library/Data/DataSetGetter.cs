//	Copyright © 2010-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Factories;
using Epsitec.Cresus.Core.Metadata;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Data
{
	/// <summary>
	/// The <c>DataSetGetter</c> class resolves a data set name to the data
	///	set itself.
	/// </summary>
	public sealed class DataSetGetter : CoreDataComponent
	{
		public DataSetGetter(CoreData data)
			: base (data)
		{
		}

		public DataSetAccessor ResolveAccessor(DataSetMetadata metadata)
		{
			return Resolver.ResolveAccessor (this.Host, metadata);
		}

		#region Resolver Class

		private abstract class Resolver
		{
			public static DataSetAccessor ResolveAccessor(CoreData data, DataSetMetadata metadata)
			{
				var resolver = Resolver.GetResolver (metadata);

				return resolver.ResolveGenericAccessor (data, metadata);
			}


			protected abstract DataSetAccessor ResolveGenericAccessor(CoreData data, DataSetMetadata metadata);

			
			private static Resolver GetResolver(DataSetMetadata metadata)
			{
				var type = metadata.EntityTableMetadata.EntityType;
				
				return SingletonFactory.GetSingleton<Resolver> (typeof (Implementation<>), type);
			}
			

			#region Implementation<T> Class

			private sealed class Implementation<T> : Resolver
				where T : AbstractEntity, new ()
			{
				protected override DataSetAccessor ResolveGenericAccessor(CoreData data, DataSetMetadata metadata)
				{
					return new DataSetAccessor<T> (data, metadata);
				}
			}

			#endregion
		}

		#endregion

		#region Factory Class

		private sealed class Factory : ICoreDataComponentFactory
		{
			#region ICoreDataComponentFactory Members

			public bool CanCreate(CoreData data)
			{
				return data.IsReady;
			}

			public bool ShouldCreate(CoreData host)
			{
				return true;
			}

			public CoreDataComponent Create(CoreData data)
			{
				return new DataSetGetter (data);
			}

			public System.Type GetComponentType()
			{
				return typeof (DataSetGetter);
			}

			#endregion
		}

		#endregion
	}
}
