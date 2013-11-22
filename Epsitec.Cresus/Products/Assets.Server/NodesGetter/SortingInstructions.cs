//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.NodesGetter
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

		public static SortingInstructions Default = new SortingInstructions (ObjectField.Nom,     SortedType.Ascending, ObjectField.Unknown, SortedType.None);
		public static SortingInstructions Empty   = new SortingInstructions (ObjectField.Unknown, SortedType.None,      ObjectField.Unknown, SortedType.None);

		public readonly ObjectField			PrimaryField;
		public readonly SortedType			PrimaryType;
		public readonly ObjectField			SecondaryField;
		public readonly SortedType			SecondaryType;
	}
}
