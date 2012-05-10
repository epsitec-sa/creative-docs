//	Copyright © 2010-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Cresus.DataLayer.Context;

using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Factories;

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

		
		public DataSetCollectionGetter ResolveDataSet(string dataSetName)
		{
			return this.ResolveDataSet (DataSetGetter.GetRootEntityType (dataSetName));
		}

		public DataSetCollectionGetter ResolveDataSet(System.Type entityType)
		{
			return Resolver.ResolveGetter (entityType, this.Host);
		}

		public DataSetAccessor ResolveAccessor(System.Type entityType)
		{
			return Resolver.ResolveAccessor (entityType, this.Host);
		}
		
		public static Druid GetRootEntityId(string dataSetName)
		{
			return DataSetGetter.FindEntityId (dataSetName);
		}

		public static System.Type GetRootEntityType(string dataSetName)
		{
			return EntityInfo.GetType (DataSetGetter.GetRootEntityId (dataSetName));
		}
		
		private static Druid FindEntityId(string dataSetName)
		{
			var type = DataSetGetter.FindStructuredType (dataSetName);
			var entityId = type == null ? Druid.Empty : type.CaptionId;

			if (entityId.IsEmpty)
			{
				throw new System.ArgumentException (string.Format ("The data set {0} cannot be mapped to any entity", dataSetName));
			}

			return entityId;
		}
		
		private static StructuredType FindStructuredType(string dataSetName)
		{
			var types = from type in Infrastructure.GetManagedEntityStructuredTypes ()
						where type.Flags.HasFlag (StructuredTypeFlags.StandaloneDisplay)
						select new
						{
							Name = type.Caption.Name,
							Type = type
						};


			foreach (var type in types)
			{
				if ((type.Name == dataSetName) ||
					(StringPluralizer.GuessPluralForms (type.Name).Contains (dataSetName)))
				{
					return type.Type;
				}
			}

			return null;
		}

		#region Resolver Class

		private abstract class Resolver
		{
			public static DataSetCollectionGetter ResolveGetter(System.Type entityType, CoreData data)
			{
				var resolver = Resolver.GetResolver (entityType);
				return resolver.ResolveGetter (data);
			}

			public static DataSetAccessor ResolveAccessor(System.Type entityType, CoreData data)
			{
				var resolver = Resolver.GetResolver (entityType);
				return resolver.ResolveAccessor (data);
			}

			
			protected abstract DataSetCollectionGetter ResolveGetter(CoreData data);

			protected abstract DataSetAccessor ResolveAccessor(CoreData data);

			
			private static Resolver GetResolver(System.Type entityType)
			{
				System.Diagnostics.Debug.Assert (entityType != null);

				return SingletonFactory.GetSingleton<Resolver> (typeof (Implementation<>), entityType);
			}
			

			#region Implementation<T> Class

			private sealed class Implementation<T> : Resolver
				where T : AbstractEntity, new ()
			{
				protected override DataSetCollectionGetter ResolveGetter(CoreData data)
				{
					return context => data.GetAllEntities<T> (dataContext: context);
				}

				protected override DataSetAccessor ResolveAccessor(CoreData data)
				{
					return new DataSetAccessor<T> (data);
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
