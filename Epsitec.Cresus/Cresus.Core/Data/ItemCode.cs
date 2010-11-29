//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Data
{
	public sealed class ItemCode : System.IEquatable<ItemCode>, System.IComparable<ItemCode>
	{
		public ItemCode(string code)
		{
			this.code = code;
		}

		public ItemCode(System.Guid guid)
			: this (ItemCodeGenerator.FromGuid (guid))
		{
		}


		public static explicit operator string(ItemCode code)
		{
			return code.code;
		}


		public string							Code
		{
			get
			{
				return this.code;
			}
		}

		
		public System.Guid? ToGuid()
		{
			if (this.code.StartsWith ("x"))
			{
				return null;
			}
			else
			{
				return ItemCodeGenerator.ToGuid (this.code);
			}
		}

		public string ToName()
		{
			if (this.code.StartsWith ("x"))
			{
				return this.code.Substring (1);
			}
			else
			{
				return null;
			}
		}

		public static ItemCode Create(string name)
		{
			return new ItemCode ("x" + name);
		}

		#region IComparable<ItemCode> Members

		public int CompareTo(ItemCode other)
		{
			return string.CompareOrdinal (this.code, other.code);
		}

		#endregion

		#region IEquatable<ItemCode> Members

		public bool Equals(ItemCode other)
		{
			return (other != null) && (other.code == this.code);
		}

		#endregion

		public override bool Equals(object obj)
		{
			if (obj is ItemCode)
			{
				return this.Equals ((ItemCode) obj);
			}
			else
			{
				return false;
			}
		}

		public override string ToString()
		{
			return this.code;
		}

		public override int GetHashCode()
		{
			return this.code.GetHashCode ();
		}

		private readonly string code;
	}
}
