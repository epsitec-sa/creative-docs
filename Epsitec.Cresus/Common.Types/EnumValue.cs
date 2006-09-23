//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Types.EnumValue))]

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>EnumValue</c> class describes a single value in an enumeration.
	/// See also <see cref="T:EnumType"/>.
	/// </summary>
	public class EnumValue : NamedDependencyObject, IEnumValue
	{
		public EnumValue(System.Enum value, int rank, bool hidden, string name)
			: base (name)
		{
			this.DefineValue (value);
			this.DefineRank (rank);
			this.DefineHidden (hidden);
		}
		
		public EnumValue(System.Enum value, int rank, bool hidden, Support.Druid captionId)
			: base (captionId)
		{
			// TODO: use captionId instead of name
			this.DefineValue (value);
			this.DefineRank (rank);
			this.DefineHidden (hidden);
		}

		
		public void DefineValue(System.Enum value)
		{
			if (this.Value != value)
			{
				this.value = value;
			}
		}

		public void DefineRank(int rank)
		{
			if (this.Rank != rank)
			{
				this.rank = rank;
			}
		}

		public void DefineHidden(bool hide)
		{
			if (this.IsHidden != hide)
			{
				this.hidden = hide;
			}
		}


		#region IEnumValue Members

		public System.Enum Value
		{
			get
			{
				return this.value;
			}
		}
		
		public int Rank
		{
			get
			{
				return this.rank;
			}
		}

		public bool IsHidden
		{
			get
			{
				return this.hidden;
			}
		}
		
		#endregion

		public static DependencyProperty ValueProperty = DependencyProperty.RegisterAttached ("Value", typeof (System.Enum), typeof (EnumValue));

		private System.Enum value;
		private int rank;
		private bool hidden;
	}
}
