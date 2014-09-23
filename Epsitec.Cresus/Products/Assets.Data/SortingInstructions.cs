//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Data
{
	public struct SortingInstructions
	{
		public SortingInstructions(ObjectField primaryField, SortedType primaryType, ObjectField secondaryField, SortedType secondaryType)
		{
			this.PrimaryField   = primaryField;
			this.PrimaryType    = primaryType;
			this.SecondaryField = secondaryField;
			this.SecondaryType  = secondaryType;
		}

		public bool IsEmpty
		{
			get
			{
				return this.PrimaryField   == ObjectField.Unknown
					&& this.PrimaryType    == SortedType.None
					&& this.SecondaryField == ObjectField.Unknown
					&& this.SecondaryType  == SortedType.None;
			}
		}


		public static bool operator ==(SortingInstructions a, SortingInstructions b)
		{
			return object.Equals (a, b);
		}

		public static bool operator !=(SortingInstructions a, SortingInstructions b)
		{
			return !(a == b);
		}

		public override bool Equals(object obj)
		{
			var other = (SortingInstructions) obj;

			return !object.ReferenceEquals (other, null)
				&& this.PrimaryField   == other.PrimaryField
				&& this.PrimaryType    == other.PrimaryType
				&& this.SecondaryField == other.SecondaryField
				&& this.SecondaryType  == other.SecondaryType;
		}

		public override int GetHashCode()
		{
			return this.PrimaryField.GetHashCode ()
				 ^ this.PrimaryType.GetHashCode ()
				 ^ this.SecondaryField.GetHashCode ()
				 ^ this.SecondaryType.GetHashCode ();
		}

	
		public static SortingInstructions Default = new SortingInstructions (ObjectField.Name,    SortedType.Ascending, ObjectField.Unknown, SortedType.None);
		public static SortingInstructions Empty   = new SortingInstructions (ObjectField.Unknown, SortedType.None,      ObjectField.Unknown, SortedType.None);

		public readonly ObjectField			PrimaryField;
		public readonly SortedType			PrimaryType;
		public readonly ObjectField			SecondaryField;
		public readonly SortedType			SecondaryType;
	}
}
