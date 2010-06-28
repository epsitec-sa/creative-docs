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
	public abstract class CollectionAccessor : ICollectionAccessor
	{
		public static CollectionAccessor Create<T1, T2, T3>(System.Func<T1> source, Expression<System.Func<T1, System.Collections.Generic.IList<T2>>> collectionResolver, CollectionTemplate<T3> template)
			where T1 : AbstractEntity, new ()
			where T2 : AbstractEntity, new ()
			where T3 : T2, new ()
		{
			return new CollectionAccessor<T1, T2, T3> (source, collectionResolver, template);
		}

		public abstract CollectionTemplate Template
		{
			get;
		}

		public abstract IEnumerable<SummaryData> Resolve(System.Func<string, int, SummaryData> summaryDataGetter);


		#region ICollectionAccessor Members

		public abstract void AddItem(AbstractEntity item);

		public abstract bool RemoveItem(AbstractEntity item);

		public abstract System.Collections.IList GetItemCollection();

		#endregion

		public static SummaryData GetTemplate(IEnumerable<SummaryData> collection, string name, int index)
		{
			SummaryData template;

			System.Diagnostics.Debug.Assert (name.Contains ('.'));

			if (CollectionAccessor.FindTemplate (collection, name, out template))
			{
				return CollectionAccessor.CreateSummayData (template, name, index);
			}
			else
			{
				return template;
			}
		}

		/// <summary>
		/// Finds the template and returns <c>true</c> if the template must be used to create a
		/// new instance of <see cref="SummaryData"/>.
		/// </summary>
		/// <param name="collection">The collection.</param>
		/// <param name="name">The item name.</param>
		/// <param name="result">The matching template (if any).</param>
		/// <returns><c>true</c> if the caller should create a new <see cref="SummaryData"/>; otherwise, <c>false</c>.</returns>
		private static bool FindTemplate(IEnumerable<SummaryData> collection, string name, out SummaryData result)
		{
			string prefix = SummaryData.GetNamePrefix (name);
			string search = prefix + ".";

			SummaryData template = null;

			foreach (var item in collection)
			{
				if (item.Name == name)
				{
					//	Exact match: return the item and tell the caller there is no need to
					//	create a new SummaryData -- the template can be reused as is.

					result = item;
					return false;
				}

				if (item.Name == prefix)
				{
					template = item;
				}

				if ((template == null) &&
							(item.Name.StartsWith (search, System.StringComparison.Ordinal)))
				{
					template = item;
				}
			}

			result = template;
			return result != null;
		}

		private static SummaryData CreateSummayData(SummaryData template, string name, int index)
		{
			string prefix = SummaryData.GetNamePrefix (name);

			return new SummaryData
			{
				Name         = SummaryData.BuildName (prefix, index),
				AutoGroup    = template.AutoGroup,
				IconUri      = template.IconUri,
				Title        = template.Title,
				CompactTitle = template.CompactTitle,
				Rank         = SummaryData.CreateRank (template.GroupingRank, index),
			};
		}
	}
	
	public class CollectionAccessor<T1, T2, T3> : CollectionAccessor
		where T1 : AbstractEntity, new ()
		where T2 : AbstractEntity, new ()
		where T3 : T2, new ()
	{
		public CollectionAccessor(System.Func<T1> source, Expression<System.Func<T1, System.Collections.Generic.IList<T2>>> collectionResolver, CollectionTemplate<T3> template)
		{
			this.source = source;
			this.collectionResolver = collectionResolver.Compile ();
			this.collectionResolverExpression = collectionResolver;
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
			var collection = this.collectionResolver (source);

			int index = 0;

			foreach (var item in collection)
			{
				if (this.template.IsCompatible (item))
				{
					var name = SummaryData.BuildName (this.template.NamePrefix, index);
					var data = summaryDataGetter (name, index);

					var marshaler = Marshaler.Create (source, this.collectionResolverExpression, collection.IndexOf (item));

					this.template.BindSummaryData (data, item, marshaler, this);

					yield return data;

					index++;
				}
			}
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
			var source = this.source ();
			var collection = this.collectionResolver (source);
			return collection as System.Collections.IList;
		}

		private readonly System.Func<T1> source;
		private readonly System.Func<T1, System.Collections.Generic.IList<T2>> collectionResolver;
		private readonly Expression<System.Func<T1, System.Collections.Generic.IList<T2>>> collectionResolverExpression;
		private readonly CollectionTemplate<T3> template;
	}
}
