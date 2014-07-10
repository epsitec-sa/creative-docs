//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Data
{
	public struct BaseType : System.IEquatable<BaseType>
	{
		public BaseType(BaseTypeKind kind)
		{
			this.Kind              = kind;
			this.AccountsDateRange = DateRange.Empty;
		}

		public BaseType(BaseTypeKind kind, DateRange accountsDateRange)
		{
			this.Kind              = kind;
			this.AccountsDateRange = accountsDateRange;
		}


		#region IEquatable<Guid> Members
		public bool Equals(BaseType other)
		{
			return this.Kind              == other.Kind
				&& this.AccountsDateRange == other.AccountsDateRange;
		}
		#endregion


		public static bool operator ==(BaseType t1, BaseType t2)
		{
			return t1.Kind              == t2.Kind
				&& t1.AccountsDateRange == t2.AccountsDateRange;
		}

		public static bool operator !=(BaseType t1, BaseType t2)
		{
			return t1.Kind              != t2.Kind
				|| t1.AccountsDateRange != t2.AccountsDateRange;
		}

		public override bool Equals(object obj)
		{
			if (obj is BaseType)
			{
				return this.Equals ((BaseType) obj);
			}
			else
			{
				return false;
			}
		}

		public override int GetHashCode()
		{
			return this.Kind.GetHashCode ()
				 ^ this.AccountsDateRange.GetHashCode ();
		}


		public override string ToString()
		{
			//	Pour le debug.
			return string.Join (", ", Kind.ToString (), AccountsDateRange.ToString ());
		}


		public static BaseType Unknown    = new BaseType (BaseTypeKind.Unknown);
		public static BaseType Assets     = new BaseType (BaseTypeKind.Assets);
		public static BaseType Categories = new BaseType (BaseTypeKind.Categories);
		public static BaseType Groups     = new BaseType (BaseTypeKind.Groups);
		public static BaseType Persons    = new BaseType (BaseTypeKind.Persons);
		public static BaseType UserFields = new BaseType (BaseTypeKind.UserFields);
		public static BaseType Entries    = new BaseType (BaseTypeKind.Entries);
		public static BaseType Accounts   = new BaseType (BaseTypeKind.Accounts);

		public readonly BaseTypeKind			Kind;
		public readonly DateRange				AccountsDateRange;
	}
}