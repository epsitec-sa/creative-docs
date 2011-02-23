//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.DataAccessors
{
	/// <summary>
	/// The <c>CollectionAccessor</c> class implements the <see cref="ICollectionAccessor"/> interface.
	/// It is used to wrap a collection of entities and provide optional insert/delete access to it.
	/// </summary>
	public abstract class CollectionAccessor : ICollectionAccessor
	{
		public static CollectionAccessor Create<T1, T2, T3>(System.Func<T1> source, System.Func<T1, IList<T2>> collectionResolver, CollectionTemplate<T3> template)
			where T1 : AbstractEntity, new ()
			where T2 : AbstractEntity, new ()
			where T3 : T2, new ()
		{
			return new CollectionAccessor<T1, T2, T3> (source, collectionResolver, template);
		}

		public static CollectionAccessor Create<T1, T2>(System.Func<T1> source, CollectionTemplate<T2> template)
			where T1 : AbstractEntity, new ()
			where T2 : AbstractEntity, new ()
		{
			var data    = template.BusinessContext.Data;
			var context = template.BusinessContext.DataContext;

			return new CollectionAccessor<T1, T2, T2> (source, () => data.GetAllEntities<T2> (DataExtractionMode.Sorted, dataContext: context), template);
		}

		public abstract CollectionTemplate Template
		{
			get;
		}

		public abstract IEnumerable<TileDataItem> Resolve(System.Func<string, int, TileDataItem> tileDataGetter);


		#region IReadOnly Members

		public abstract bool IsReadOnly
		{
			get;
		}

		#endregion

		#region ICollectionAccessor Members

		public abstract void InsertItem(int index, AbstractEntity item);

		public abstract int AddItem(AbstractEntity item);

		public abstract bool RemoveItem(AbstractEntity item);

		public abstract IEnumerable<AbstractEntity> GetItemCollection();

		#endregion
	}
}
