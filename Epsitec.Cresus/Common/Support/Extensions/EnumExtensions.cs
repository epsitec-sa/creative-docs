//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Support.Extensions
{
	public static class EnumExtensions
	{
#if DOTNET35
		public static bool HasFlag(this System.Enum value, System.Enum mask)
		{
			int enumValue = (int) (object) value;
			int enumMask  = (int) (object) mask;
			if ((enumValue & enumMask) == 0)
			{
				return false;
			}
			else
			{
				return true;
			}
		}
#endif

		public static T SetFlag<T>(this System.Enum value, T mask)
			where T : struct
		{
			int enumValue = (int) (object) value;
			int enumMask  = (int) (object) mask;

			return (T) (object) (enumValue | enumMask);
		}

		public static T ClearFlag<T>(this System.Enum value, T mask)
			where T : struct
		{
			int enumValue = (int) (object) value;
			int enumMask  = (int) (object) mask;

			return (T) (object) (enumValue & ~enumMask);
		}


		public static string GetQualifiedName(this System.Enum value)
		{
			var type = value.GetType ();
			var name = type.Name;
			var symbols = System.Enum.Format (type, value, "f").Split (',').Select (x => string.Concat (name, ".", x.TrimStart ())).ToArray ();

			return string.Join (" | ", symbols);
		}
	}
}
