//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Debug
{
	public static class Sos
	{
		public static string GetAddress(object o)
		{
			if (o == null)
			{
				return "00000000";
			}
			else
			{
				unsafe
				{
					System.TypedReference tr = __makeref(o);
					System.IntPtr ptr = **(System.IntPtr**) (&tr);
					return ptr.ToString ("X");
				}
			}
		}
	}
}

