//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>EnumValue</c> class describes a single value in an enumeration.
	/// See also <see cref="T:EnumType"/>.
	/// </summary>
	public class EnumValue : NamedDependencyObject, IEnumValue
	{
		public EnumValue(System.Enum value, int rank, bool hidden, string name, long captionId)
			: base (name, captionId)
		{
			this.DefineValue (value);
			this.DefineRank (rank);
			this.DefineHidden (hidden);
		}

		
		public void DefineValue(System.Enum value)
		{
			this.value = value;
		}

		public void DefineRank(int rank)
		{
			this.rank = rank;
		}

		public void DefineHidden(bool hide)
		{
			this.hidden = hide;
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


		private System.Enum value;
		private int rank;
		private bool hidden;
	}
}
