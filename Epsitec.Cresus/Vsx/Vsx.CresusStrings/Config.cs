using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epsitec.Cresus.Strings
{
	public static class Config
	{
		public const int MaxAsyncDelay	= 50; // [ms]

		public const string EditResourceSmartTagMenu		= "Edit string with Cresus Designer";
		public const string EditResourceSmartTagMenuFormat	= "Edit '{0}' with Cresus Designer";

		public static string GetEditResourceSmartTagMenu(string symbolName)
		{
			return string.Format (Config.EditResourceSmartTagMenuFormat, symbolName);
		}
	}
}
