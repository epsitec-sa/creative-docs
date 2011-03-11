//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Factories;

namespace Epsitec.Cresus.Core.Data
{
	/// <summary>
	/// The <c>DataSetGetter</c> class resolves a data set name to the data
	///	set itself.
	/// </summary>
	public class DataSetGetter : CoreDataComponent
	{
		public DataSetGetter(CoreData data)
			: base (data)
		{
		}

		public DataSetCollectionGetter ResolveDataSet(string dataSetName)
		{
			var entityId   = DataSetGetter.FindEntityId (dataSetName);
			var entityType = EntityClassFactory.FindEntityType (entityId);

			return Resolver.ResolveGetter (entityType, this.Host);
		}
		
		public Druid GetRootEntityId(string dataSetName)
		{
			return DataSetGetter.FindEntityId (dataSetName);
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


			System.Diagnostics.Debug.WriteLine (string.Join ("\n", types.OrderBy (x => x.Name).Select (x => x.Name).ToArray ()));

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
				var getter = Resolver.GetResolver (entityType);
				return getter.Resolve (data);
			}

			private static Resolver GetResolver(System.Type entityType)
			{
				System.Diagnostics.Debug.Assert (entityType != null);

				return System.Activator.CreateInstance (typeof (Implementation<>).MakeGenericType (entityType)) as Resolver;
			}
			
			protected abstract DataSetCollectionGetter Resolve(CoreData data);
			
			private sealed class Implementation<T> : Resolver
				where T : AbstractEntity, new ()
			{
				protected override DataSetCollectionGetter Resolve(CoreData data)
				{
					return context => data.GetAllEntities<T> (dataContext: context);
				}
			}
		}

		#endregion

		public sealed class Factory : ICoreDataComponentFactory
		{
			#region ICoreDataComponentFactory Members

			public bool CanCreate(CoreData data)
			{
				return data.DataInfrastructure != null;
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
	}
}
