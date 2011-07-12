//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

#if DOTNET35
namespace Epsitec.Common.Support.Extensions
{
	public static class EnumExtensions
	{
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
	}
}
#endif
