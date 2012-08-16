//	Copyright © 2010-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Data
{
	/// <summary>
	/// The <c>ItemCode</c> class represents a GUID or a named unique ID, which is
	/// usually stored in an <see cref="IItemCode.Code"/> field.
	/// </summary>
	public sealed class ItemCode : System.IEquatable<ItemCode>, System.IComparable<ItemCode>
	{
		public ItemCode(string code)
		{
			if (string.IsNullOrEmpty (code))
			{
				code = "";
			}
			else
			{
				if (code.IndexOf (' ') < 0)
				{
					//	OK, no space found in code or name.
				}
				else
				{
					throw new System.FormatException ("ItemCode may not contain spaces");
				}
			}

			this.code = code;
		}

		public ItemCode(System.Guid guid)
			: this (ItemCodeGenerator.FromGuid (guid))
		{
		}

		public ItemCode(System.Guid? guid)
			: this (guid.HasValue ? ItemCodeGenerator.FromGuid (guid.Value) : "")
		{
		}

		
		public static explicit operator string(ItemCode item)
		{
			return (item == null) ? null : item.code;
		}


		public string							Code
		{
			get
			{
				return this.code;
			}
		}

		public bool								IsGuid
		{
			get
			{
				if ((this.code == null) ||
					(this.code.Length != 16) ||
					(this.code[0] == Strings.NamePrefix[0]))
				{
					return false;
				}
				else
				{
					return true;
				}
			}
		}

		
		public System.Guid? ToGuid()
		{
			if (this.code.StartsWith (Strings.NamePrefix))
			{
				return null;
			}
			else
			{
				return ItemCodeGenerator.ToGuidOrNull (this.code);
			}
		}

		public string ToName()
		{
			if (this.code.StartsWith (Strings.NamePrefix))
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
			return new ItemCode (Strings.NamePrefix + name);
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

		#region Constant Strings Class

		private static class Strings
		{
			public const string NamePrefix = "x";				//	'x' will never show up in encoded ASCII-85 string
		}

		#endregion

		private readonly string					code;
	}
}
