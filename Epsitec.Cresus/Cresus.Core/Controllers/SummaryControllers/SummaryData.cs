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
	public class SummaryData : System.IComparable<SummaryData>
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
		public CollectionTemplate()
		{
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

			data.TitleAccessor        = IndirectAccessor<T, FormattedText>.GetAccessor (this.TitleAccessor,        source);
			data.TextAccessor         = IndirectAccessor<T, FormattedText>.GetAccessor (this.TextAccessor,         source);
			data.CompactTitleAccessor = IndirectAccessor<T, FormattedText>.GetAccessor (this.CompactTitleAccessor, source);
			data.CompactTextAccessor  = IndirectAccessor<T, FormattedText>.GetAccessor (this.CompactTextAccessor,  source);
		}
	}

	public class CollectionAccessor
	{
		public static void Create<T1, T2, T3>(T1 value, CollectionTemplate<T2> template, System.Func<T1, System.Collections.Generic.IList<T3>> action)
			where T1 : AbstractEntity, new ()
			where T2 : T3, new ()
			where T3 : AbstractEntity, new ()
		{
		}
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
