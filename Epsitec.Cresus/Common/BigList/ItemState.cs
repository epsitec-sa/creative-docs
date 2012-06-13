//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.BigList
{
	/// <summary>
	/// The <c>ItemState</c> class maintains the state of an item manipulated in a
	/// big list.
	/// </summary>
	public class ItemState : System.IEquatable<ItemState>
	{
		public ItemState()
		{
		}


		public bool								Loaded
		{
			get;
			set;
		}

		public bool								Selected
		{
			get;
			set;
		}

		public bool								Hidden
		{
			get;
			set;
		}

		public bool								Hilite1
		{
			get;
			set;
		}

		public bool								Hilite2
		{
			get;
			set;
		}

		public bool								Partial
		{
			get;
			private set;
		}

		public bool								RequiresExtraState
		{
			get
			{
				if ((this.height + 1 > ItemState.MaxCompactHeight) ||
					(this.MarginBefore  != 0) ||
					(this.MarginAfter   != 0) ||
					(this.PaddingBefore != 0) ||
					(this.PaddingAfter  != 0) ||
					(this.HasExtraArguments ()))
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		public bool								IsEmpty
		{
			get;
			private set;
		}

		public int								Height
		{
			get
			{
				return this.height;
			}
			set
			{
				if (value < 0)
				{
					throw new System.ArgumentOutOfRangeException ("value", string.Format ("Height may not be negative: {0}", value));
				}

				this.height = value;
			}
		}

		public int								MarginBefore
		{
			get;
			set;
		}
		
		public int								MarginAfter
		{
			get;
			set;
		}
		
		public int								PaddingBefore
		{
			get;
			set;
		}
		
		public int								PaddingAfter
		{
			get;
			set;
		}


		public static ItemState					Empty
		{
			get
			{
				return new ItemState ()
				{
					IsEmpty = true
				};
			}
		}

		public static readonly ushort			EmptyCompactState = 0;

		
		public ushort ToCompactState()
		{
			bool needsPartial = this.Partial || this.RequiresExtraState;

			int compact =
				(this.Loaded   ? Mask.Loaded   : 0) |
				(this.Selected ? Mask.Selected : 0) |
				(this.Hidden   ? Mask.Hidden   : 0) |
				(this.Hilite1  ? Mask.Hilite1  : 0) |
				(this.Hilite2  ? Mask.Hilite2  : 0) |
				(needsPartial  ? Mask.Partial  : 0) |
				System.Math.Min (this.height + 1, ItemState.MaxCompactHeight);

			return (ushort) compact;
		}

		public static TState FromCompactState<TState>(ushort state)
			where TState : ItemState, new ()
		{
			return new TState ()
			{
				Height   = (state & Mask.Height) - 1,
				Loaded   = (state & Mask.Loaded)   != 0,
				Selected = (state & Mask.Selected) != 0,
				Hidden   = (state & Mask.Hidden)   != 0,
				Hilite1  = (state & Mask.Hilite1)  != 0,
				Hilite2  = (state & Mask.Hilite2)  != 0,
				Partial  = (state & Mask.Partial)  != 0,
			};
		}

		public void CopyFrom(ItemState other)
		{
			this.Height   = other.Height;
			this.Loaded   = other.Loaded;
			this.Selected = other.Selected;
			this.Hidden   = other.Hidden;
			this.Hilite1  = other.Hilite1;
			this.Hilite2  = other.Hilite2;
			this.Partial  = other.Partial;
			
			this.ApplyExtraArguments (other);
		}

		public void ApplyExtraState(ItemState other)
		{
			if (this.height+1 == ItemState.MaxCompactHeight)
			{
				this.height = other.height;
			}

			this.ApplyExtraArguments (other);
			this.Partial = false;
		}

		public void Select(ItemSelection selectionMode)
		{
			switch (selectionMode)
			{
				case ItemSelection.Select:
					this.Selected = true;
					break;

				case ItemSelection.Deselect:
					this.Selected = false;
					break;

				case ItemSelection.Toggle:
					this.Selected = !this.Selected;
					break;

				default:
					throw selectionMode.NotSupportedException ();
			}
		}


		public TState Clone<TState>()
			where TState : ItemState, new ()
		{
			var copy = new TState ();

			copy.CopyFrom (this);

			return copy;
		}

		public virtual ItemState Clone()
		{
			var copy = this.CreateInstance ();

			copy.CopyFrom (this);
			
			return copy;
		}

		protected virtual ItemState CreateInstance()
		{
			return new ItemState ();
		}

		protected virtual bool HasExtraArguments()
		{
			//	TODO: evaluate the extra arguments to see if they still require that the full
			//	state, including extra information, needs to be stored.
			
			return false;
		}

		protected virtual bool EqualExtraArgument(ItemState other)
		{
			return true;
		}

		protected virtual bool ApplyExtraArguments(ItemState other)
		{
			this.MarginBefore  = other.MarginBefore;
			this.MarginAfter   = other.MarginAfter;
			this.PaddingBefore = other.PaddingBefore;
			this.PaddingAfter  = other.PaddingAfter;

			//	TODO: recopy other properties which do not fit into a compact state...

			if ((this.MarginBefore != 0) ||
				(this.MarginAfter != 0) ||
				(this.PaddingBefore != 0) ||
				(this.PaddingAfter != 0))
			{
				return true;
			}
			
			return false;
		}

		#region Mask Constants

		private static class Mask
		{
			public const int Height	  = 0x03ff;
			public const int Loaded	  = 0x0400;
			public const int Selected = 0x0800;
			public const int Hidden   = 0x1000;
			public const int Hilite1  = 0x2000;
			public const int Hilite2  = 0x4000;
			public const int Partial  = 0x8000;
		}

		#endregion

		#region IEquatable<ItemState> Members

		public bool Equals(ItemState other)
		{
			return this.Loaded == other.Loaded
				&& this.Selected == other.Selected
				&& this.Hidden == other.Hidden
				&& this.Hilite1 == other.Hilite1
				&& this.Hilite2 == other.Hilite2
				&& this.Partial == other.Partial
				&& this.Height == other.Height
				&& this.EqualExtraArgument (other);
		}

		#endregion

		public const int MaxCompactHeight		= 1000;

		private int								height;
	}
}
