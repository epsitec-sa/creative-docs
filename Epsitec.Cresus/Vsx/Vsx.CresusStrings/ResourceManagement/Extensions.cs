using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Epsitec.Cresus.ResourceManagement;

namespace Epsitec
{
	public static partial class Extensions
	{
		public static string SymbolName(this IReadOnlyDictionary<CultureInfo, ResourceItem> source)
		{
			return source.First ().Value.SymbolName;
		}
	}
}
