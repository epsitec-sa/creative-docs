//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public struct ViewType : System.IEquatable<ViewType>
	{
		public ViewType(ViewTypeKind kind)
		{
			this.Kind              = kind;
			this.AccountsDateRange = DateRange.Empty;
		}

		public ViewType(ViewTypeKind kind, DateRange accountsDateRange)
		{
			this.Kind              = kind;
			this.AccountsDateRange = accountsDateRange;
		}


		#region IEquatable<Guid> Members
		public bool Equals(ViewType other)
		{
			return this.Equals (other);
		}
		#endregion

		public static bool operator ==(ViewType a, ViewType b)
		{
			return object.Equals (a, b);
		}

		public static bool operator !=(ViewType a, ViewType b)
		{
			return !(a == b);
		}

		public override bool Equals(object obj)
		{
			var other = (ViewType) obj;

			return !object.ReferenceEquals (other, null)
				&& this.Kind              == other.Kind
				&& this.AccountsDateRange == other.AccountsDateRange;
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


		public static ViewType FromDefaultKind(DataAccessor accessor, ViewTypeKind kind)
		{
			if (kind == ViewTypeKind.Accounts)
			{
				//	On montre le dernier plan comptable importé.
				var range = accessor.Mandat.AccountsDateRanges.LastOrDefault ();
				return new ViewType (kind, range);
			}
			else
			{
				return new ViewType (kind);
			}
		}


		public static ViewType Unknown         = new ViewType (ViewTypeKind.Unknown);
		public static ViewType Assets          = new ViewType (ViewTypeKind.Assets);
		public static ViewType Amortizations   = new ViewType (ViewTypeKind.Amortizations);
		public static ViewType Entries         = new ViewType (ViewTypeKind.Entries);
		public static ViewType Categories      = new ViewType (ViewTypeKind.Categories);
		public static ViewType Groups          = new ViewType (ViewTypeKind.Groups);
		public static ViewType Persons         = new ViewType (ViewTypeKind.Persons);
		public static ViewType Reports         = new ViewType (ViewTypeKind.Reports);
		public static ViewType Warnings        = new ViewType (ViewTypeKind.Warnings);
		public static ViewType AssetsSettings  = new ViewType (ViewTypeKind.AssetsSettings);
		public static ViewType PersonsSettings = new ViewType (ViewTypeKind.PersonsSettings);
		public static ViewType Accounts        = new ViewType (ViewTypeKind.Accounts);

		public readonly ViewTypeKind			Kind;
		public readonly DateRange				AccountsDateRange;
	}
}
