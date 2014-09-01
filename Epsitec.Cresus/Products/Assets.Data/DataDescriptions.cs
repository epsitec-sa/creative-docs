//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Types;

namespace Epsitec.Cresus.Assets.Data
{
	public static class DataDescriptions
	{
		public static string GetObjectFieldDescription(ObjectField field)
		{
			if (field >= ObjectField.GroupGuidRatioFirst &&
				field <= ObjectField.GroupGuidRatioLast)
			{
				return Res.Strings.ObjectField.InGroupGuidRation.ToString ();
			}

			return EnumKeyValues.GetEnumKeyValue (field).Values.Last ().ToString ();
		}

		public static string GetEventDescription(EventType type)
		{
			return EnumKeyValues.GetEnumKeyValue (type).Values.Last ().ToString ();
		}
	}
}
