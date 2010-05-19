//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;

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
				this.bindings.Add (AccessorBinding.Create (value, x => this.Title = x));
			}
		}

		public Accessor<FormattedText> TextAccessor
		{
			set
			{
				this.bindings.Add (AccessorBinding.Create (value, x => this.Text = x));
			}
		}

		public Accessor<FormattedText> CompactTitleAccessor
		{
			set
			{
				this.bindings.Add (AccessorBinding.Create (value, x => this.CompactTitle = x));
			}
		}

		public Accessor<FormattedText> CompactTextAccessor
		{
			set
			{
				this.bindings.Add (AccessorBinding.Create (value, x => this.CompactText = x));
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

	public class Accessor
	{
		public static Accessor<TResult> Create<T, TResult>(T value, System.Func<T, TResult> action)
		{
			return new Accessor<TResult> (() => action (value));
		}
	}

	public class Accessor<T> : Accessor
	{
		public Accessor(System.Func<T> getter)
		{
			this.getter = getter;
		}

		public T ExecuteGetter()
		{
			return this.getter ();
		}

		private readonly System.Func<T> getter;
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
