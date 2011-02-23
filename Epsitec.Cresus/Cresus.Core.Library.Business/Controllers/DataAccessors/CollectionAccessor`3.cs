//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Core.Controllers.DataAccessors
{
	/// <summary>
	/// The <c>CollectionAccessor{T1,T2,T3}</c> class is a specific implementation of
	/// <see cref="CollectionAccessor"/>.
	/// </summary>
	/// <typeparam name="T1">The type of the data source.</typeparam>
	/// <typeparam name="T2">The type of the data items.</typeparam>
	/// <typeparam name="T3">The type of the data items represented by the collection template (same type as <c>T2</c> or derived from <c>T2</c>).</typeparam>
	public class CollectionAccessor<T1, T2, T3> : CollectionAccessor
		where T1 : AbstractEntity, new ()
		where T2 : AbstractEntity, new ()
		where T3 : T2, new ()
	{
		public CollectionAccessor(System.Func<T1> source, System.Func<T1, IList<T2>> writableCollectionResolver, CollectionTemplate<T3> template)
		{
			this.source = source;
			this.writableCollectionResolver = writableCollectionResolver;
			this.template = template;
		}

		public CollectionAccessor(System.Func<T1> source, System.Func<IEnumerable<T2>> readOnlyCollectionResolver, CollectionTemplate<T3> template)
		{
			this.source = source;
			this.readOnlyCollectionResolver = readOnlyCollectionResolver;
			this.template = template;
		}

		
		public override CollectionTemplate		Template
		{
			get
			{
				return this.template;
			}
		}

		public override bool					IsReadOnly
		{
			get
			{
				return this.writableCollectionResolver == null;
			}
		}

		
		public override IEnumerable<TileDataItem> Resolve(System.Func<string, int, TileDataItem> tileDataGetter)
		{
			var source     = this.GetSource ();
			var collection = this.GetItemCollection ();

			int index = 0;

			foreach (T2 item in collection)
			{
				if (this.template.IsCompatible (item))
				{
					var current   = item;
					var name      = TileDataItem.BuildName (this.template.NamePrefix, index);
					var data      = tileDataGetter (name, index);
					var marshaler = Marshaler.Create (() => current, null);
					
					this.template.BindTileData (data, current, marshaler, this);

					yield return data;

					index++;
				}
			}
		}

		public override void InsertItem(int index, AbstractEntity item)
		{
			if (this.IsReadOnly)
			{
				return;
			}

			var collection = this.GetWritableCollection ();
			collection.Insert (index, item as T3);
		}

		public override int AddItem(AbstractEntity item)
		{
			if (this.IsReadOnly)
			{
				return this.GetItemCollection ().Count ();
			}

			var collection = this.GetWritableCollection ();
			int index = collection.Count;
			collection.Add (item as T3);
			return index;
		}

		public override bool RemoveItem(AbstractEntity item)
		{
			var collection = this.GetWritableCollection ();
			return collection.Remove (item as T3);
		}

		public override IEnumerable<AbstractEntity> GetItemCollection()
		{
			return this.IsReadOnly ? this.GetReadOnlyCollection () : this.GetWritableCollection ();
		}

		
		private T1 GetSource()
		{
			return this.source ();
		}

		private IEnumerable<T2> GetReadOnlyCollection()
		{
			return this.readOnlyCollectionResolver ();
		}

		private IList<T2> GetWritableCollection()
		{
			var source     = this.GetSource ();
			var collection = this.writableCollectionResolver (source);

			if (collection == null)
			{
				throw new System.InvalidOperationException ("Read-only collection cannot be modified");
			}
			
			return collection;
		}

		
		private readonly System.Func<T1>				source;
		private readonly System.Func<T1, IList<T2>>		writableCollectionResolver;
		private readonly System.Func<IEnumerable<T2>>	readOnlyCollectionResolver;
		private readonly CollectionTemplate<T3>			template;
	}
}
