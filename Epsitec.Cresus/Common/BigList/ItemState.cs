//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.BigList
{
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
				if (value+1 > ItemState.MaxCompactHeight)
				{
					this.Partial = true;
				}

				this.height = value;
			}
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
			bool partialFlag = this.ComputePartialFlag ();
			
			int compact =
				(this.Loaded   ? Mask.Loaded   : 0) |
				(this.Selected ? Mask.Selected : 0) |
				(this.Hidden   ? Mask.Hidden   : 0) |
				(this.Hilite1  ? Mask.Hilite1  : 0) |
				(this.Hilite2  ? Mask.Hilite2  : 0) |
				(partialFlag   ? Mask.Partial  : 0) |
				System.Math.Min (this.height + 1, ItemState.MaxCompactHeight);

			return (ushort) compact;
		}

		public static TState FromCompactState<TState>(ushort state)
			where TState : ItemState, new ()
		{
			return new TState ()
			{
				Loaded   = (state & Mask.Loaded)   != 0,
				Selected = (state & Mask.Selected) != 0,
				Hidden   = (state & Mask.Hidden)   != 0,
				Hilite1  = (state & Mask.Hilite1)  != 0,
				Hilite2  = (state & Mask.Hilite2)  != 0,
				Partial  = (state & Mask.Partial)  != 0,
				Height   = (state & Mask.Height) - 1
			};
		}

		public void CopyFrom(ItemState other)
		{
			this.Loaded   = other.Loaded;
			this.Selected = other.Selected;
			this.Hidden   = other.Hidden;
			this.Hilite1  = other.Hilite1;
			this.Hilite2  = other.Hilite2;
			this.Partial  = other.Partial;
			this.Height   = other.Height;

			this.ApplyExtraArguments (other);
		}

		public void Apply(ItemState other)
		{
			if (this.height+1 == ItemState.MaxCompactHeight)
			{
				this.height = other.Height;
			}

			bool applied = this.ApplyExtraArguments (other);

			if (applied)
			{
				this.Partial = true;
			}
		}

		public bool ComputePartialFlag()
		{
			if (this.Partial == false)
			{
				return false;
			}
			
			return this.height+1 > ItemState.MaxCompactHeight
				|| this.HasExtraArguments ();
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
			//	TODO: recopy other properties which do not fit into a compact state...

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
