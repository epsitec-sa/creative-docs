//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types.Converters;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Core.Controllers.DataAccessors
{
	public class CollectionAccessor<T1, T2, T3> : CollectionAccessor
		where T1 : AbstractEntity, new ()
		where T2 : AbstractEntity, new ()
		where T3 : T2, new ()
	{
		public CollectionAccessor(System.Func<T1> source, System.Func<T1, IList<T2>> collectionResolver, CollectionTemplate<T3> template)
		{
			this.source = source;
			this.collectionResolver = collectionResolver;
			this.template = template;
		}

		public CollectionAccessor(System.Func<T1> source, System.Func<IEnumerable<T2>> collectionResolver, CollectionTemplate<T3> template)
		{
			this.source = source;
			this.readOnlyCollectionResolver = collectionResolver;
			this.template = template;
		}

		public override CollectionTemplate Template
		{
			get
			{
				return this.template;
			}
		}

		public override IEnumerable<SummaryData> Resolve(System.Func<string, int, SummaryData> summaryDataGetter)
		{
			var source = this.source ();
			var collection = this.GetItemCollection ();

			int index = 0;

			foreach (T2 item in collection)
			{
				if (this.template.IsCompatible (item))
				{
					var name = SummaryData.BuildName (this.template.NamePrefix, index);
					var data = summaryDataGetter (name, index);

					Marshaler<T2> marshaler;

					if (this.collectionResolver != null)
					{
						marshaler = Marshaler.Create (source, this.collectionResolver, collection.IndexOf (item));
					}
					else
					{
						marshaler = Marshaler.Create (this.readOnlyCollectionResolver, collection.IndexOf (item));
					}

					this.template.BindSummaryData (data, item, marshaler, this);

					yield return data;

					index++;
				}
			}
		}

		public override void InsertItem(int index, AbstractEntity item)
		{
			var source = this.source ();
			var collection = this.collectionResolver (source);
			collection.Insert (index, item as T3);
		}

		public override void AddItem(AbstractEntity item)
		{
			var source = this.source ();
			var collection = this.collectionResolver (source);
			collection.Add (item as T3);
		}

		public override bool RemoveItem(AbstractEntity item)
		{
			var source = this.source ();
			var collection = this.collectionResolver (source);
			return collection.Remove (item as T3);
		}

		public override System.Collections.IList GetItemCollection()
		{
			if (this.collectionResolver != null)
			{
				var source = this.source ();
				return this.collectionResolver (source) as System.Collections.IList;
			}
			else
			{
				return this.readOnlyCollectionResolver ().ToList ();
			}
		}

		private readonly System.Func<T1> source;
		private readonly System.Func<T1, IList<T2>> collectionResolver;
		private readonly System.Func<IEnumerable<T2>> readOnlyCollectionResolver;
		private readonly CollectionTemplate<T3> template;
	}
}
