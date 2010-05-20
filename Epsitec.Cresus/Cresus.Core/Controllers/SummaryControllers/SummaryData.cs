//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Support.EntityEngine;

namespace Epsitec.Cresus.Core.Controllers.SummaryControllers
{
	public class SummaryData : System.IComparable<SummaryData>, ITileController
	{
		public SummaryData()
		{
			this.bindings = new List<AccessorBinding> ();
		}

		public string Name
		{
			get;
			set;
		}

		public int Rank
		{
			get;
			set;
		}
		
		public string IconUri
		{
			get;
			set;
		}

		public SummaryDataType DataType
		{
			get;
			set;
		}

		public FormattedText Title
		{
			get;
			set;
		}

		public FormattedText Text
		{
			get;
			set;
		}

		public FormattedText CompactTitle
		{
			get;
			set;
		}

		public FormattedText CompactText
		{
			get;
			set;
		}


		public TitleTile TitleTile
		{
			get;
			set;
		}

		public SummaryTile SummaryTile
		{
			get;
			set;
		}


		public System.Func<AbstractEntity> EntityAccessor
		{
			get;
			set;
		}

		public Accessor<FormattedText> TitleAccessor
		{
			set
			{
				if (value != null)
				{
					this.bindings.Add (AccessorBinding.Create (value, x => this.Title = x));
				}
			}
		}

		public Accessor<FormattedText> TextAccessor
		{
			set
			{
				if (value != null)
				{
					this.bindings.Add (AccessorBinding.Create (value, x => this.Text = x));
				}
			}
		}

		public Accessor<FormattedText> CompactTitleAccessor
		{
			set
			{
				if (value != null)
				{
					this.bindings.Add (AccessorBinding.Create (value, x => this.CompactTitle = x));
				}
			}
		}

		public Accessor<FormattedText> CompactTextAccessor
		{
			set
			{
				if (value != null)
				{
					this.bindings.Add (AccessorBinding.Create (value, x => this.CompactText = x));
				}
			}
		}

		public void ExecuteAccessors()
		{
			this.bindings.ForEach (x => x.Execute ());
		}

		
		protected virtual void OnChanged()
		{
			var handler = this.Changed;

			if (handler != null)
			{
				handler (this);
			}
		}

		#region ITileController Members

		EntityViewController ITileController.CreateSubViewController(Orchestrators.DataViewOrchestrator orchestrator)
		{
			if (this.EntityAccessor != null)
			{
				var entity = this.EntityAccessor ();

				if (entity != null)
				{
					return EntityViewController.CreateEntityViewController ("ViewController", entity, ViewControllerMode.Edition, orchestrator);
				}
			}

			return null;
		}

		#endregion
		
		#region IComparable<SummaryData> Members

		public int CompareTo(SummaryData other)
		{
			if (this.Rank < other.Rank)
			{
				return -1;
			}
			else if (this.Rank > other.Rank)
			{
				return 1;
			}

			var options = System.Globalization.CompareOptions.StringSort | System.Globalization.CompareOptions.IgnoreCase;
			var culture = System.Globalization.CultureInfo.CurrentCulture;

			return string.Compare (this.Title.ToSimpleText (), other.Title.ToSimpleText (), culture, options);
		}

		#endregion

		public event EventHandler Changed;

		private readonly List<AccessorBinding> bindings;
	}

	public class CollectionTemplate<T>
		where T : AbstractEntity, new ()
	{
		public CollectionTemplate(string name)
		{
			this.name = name;
		}

		public IndirectAccessor<T, FormattedText> TitleAccessor
		{
			get;
			set;
		}

		public IndirectAccessor<T, FormattedText> TextAccessor
		{
			get;
			set;
		}
		
		public IndirectAccessor<T, FormattedText> CompactTitleAccessor
		{
			get;
			set;
		}

		public IndirectAccessor<T, FormattedText> CompactTextAccessor
		{
			get;
			set;
		}

		public System.Predicate<T> Filter
		{
			get;
			set;
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public bool IsCompatible(AbstractEntity entity)
		{
			T source = entity as T;

			if (source == null)
			{
				return false;
			}
			else
			{
				return this.Filter (source);
			}
		}

		public void BindSummaryData(SummaryData data, AbstractEntity entity)
		{
			T source = entity as T;

			data.EntityAccessor		  = () => source;
			data.TitleAccessor        = IndirectAccessor<T, FormattedText>.GetAccessor (this.TitleAccessor,        source);
			data.TextAccessor         = IndirectAccessor<T, FormattedText>.GetAccessor (this.TextAccessor,         source);
			data.CompactTitleAccessor = IndirectAccessor<T, FormattedText>.GetAccessor (this.CompactTitleAccessor, source);
			data.CompactTextAccessor  = IndirectAccessor<T, FormattedText>.GetAccessor (this.CompactTextAccessor,  source);
		}

		private readonly string name;
	}

	public abstract class CollectionAccessor
	{
		public static CollectionAccessor Create<T1, T2, T3>(T1 source, System.Func<T1, System.Collections.Generic.IList<T2>> collectionResolver, CollectionTemplate<T3> template)
			where T1 : AbstractEntity, new ()
			where T2 : AbstractEntity, new ()
			where T3 : T2, new ()
		{
			return new CollectionAccessor<T1, T2, T3> (source, collectionResolver, template);
		}

		public abstract IEnumerable<SummaryData> Resolve(System.Func<string, int, SummaryData> summaryDataGetter);
		
		public static SummaryData FindTemplate(List<SummaryData> collection, string name, int index)
		{
			System.Diagnostics.Debug.Assert (name.Contains ('.'));

			string prefix = name.Substring (0, name.LastIndexOf ('.') + 1);

			SummaryData template = null;

			foreach (var item in collection)
			{
				if (item.Name == name)
				{
					return item;
				}

				if ((template == null) &&
					(item.Name.StartsWith (prefix, System.StringComparison.Ordinal)))
				{
					template = item;
				}
			}

			if (template == null)
			{
				return null;
			}

			return new SummaryData
			{
				Name         = string.Format (System.Globalization.CultureInfo.InvariantCulture, "{0}.{1}", prefix, index),
				IconUri      = template.IconUri,
				Title        = template.Title,
				CompactTitle = template.CompactTitle,
				Rank         = (template.Rank / 1000) * 1000 + index,
			};
		}

	}

	public class CollectionAccessor<T1, T2, T3> : CollectionAccessor
		where T1 : AbstractEntity, new ()
		where T2 : AbstractEntity, new ()
		where T3 : T2, new ()
	{
		public CollectionAccessor(T1 source, System.Func<T1, System.Collections.Generic.IList<T2>> collectionResolver, CollectionTemplate<T3> template)
		{
			this.source = source;
			this.collectionResolver = collectionResolver;
			this.template = template;
		}

		public override IEnumerable<SummaryData> Resolve(System.Func<string, int, SummaryData> summaryDataGetter)
		{
			var collection = this.collectionResolver (this.source);

			int index = 0;

			foreach (var item in collection)
			{
				if (this.template.IsCompatible (item))
				{
					var name = string.Format (System.Globalization.CultureInfo.InvariantCulture, "{0}.{1}", this.template.Name, index);
					var data = summaryDataGetter (name, index);

					this.template.BindSummaryData (data, item);

					yield return data;

					index++;
				}
			}
		}

		private readonly T1 source;
		private System.Func<T1, System.Collections.Generic.IList<T2>> collectionResolver;
		private readonly CollectionTemplate<T3> template;
	}

	public class Accessor
	{
		public static Accessor<TResult> Create<T, TResult>(T value, System.Func<T, TResult> action)
			where T : new ()
		{
			return new Accessor<TResult> (() => action (value));
		}
	}

	public class IndirectAccessor
	{
	}

	public class IndirectAccessor<T> : IndirectAccessor
		where T : new ()
	{
		public static IndirectAccessor<T, TResult> Create<TResult>(System.Func<T, TResult> action)
		{
			return new IndirectAccessor<T, TResult> (action);
		}
	}

	public class IndirectAccessor<T, TResult> : IndirectAccessor<T>
		where T : new ()
	{
		public IndirectAccessor(System.Func<T, TResult> getter)
		{
			this.getter = getter;
		}

		public static Accessor<TResult> GetAccessor(IndirectAccessor<T, TResult> accessor, T source)
		{
			if (accessor == null)
			{
				return null;
			}
			else
			{
				return accessor.GetAccessor (source);
			}
		}

		public Accessor<TResult> GetAccessor(T source)
		{
			if (source == null)
			{
				return null;
			}
			else
			{
				return new Accessor<TResult> (() => this.getter (source));
			}
		}

		private readonly System.Func<T, TResult> getter;
	}

	public class Accessor<TResult> : Accessor
	{
		public Accessor(System.Func<TResult> getter)
		{
			this.getter = getter;
		}

		public TResult ExecuteGetter()
		{
			return this.getter ();
		}

		private readonly System.Func<TResult> getter;
	}

	public class AccessorBinding
	{
		public AccessorBinding(System.Action action)
		{
			this.action = action;
		}

		public void Execute()
		{
			this.action ();
		}

		public static AccessorBinding Create<T>(Accessor<T> accessor, System.Action<T> setter)
		{
			return new AccessorBinding (() => setter (accessor.ExecuteGetter ()));
		}

		private readonly System.Action action;
	}
}
