//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Support.Extensions
{
	public static class StringExtensions
	{
		public static bool IsNullOrWhiteSpace(this string text)
		{
#if DOTNET35
			return string.IsNullOrEmpty (text)
						|| text.Trim ().Length == 0;
#else
					return string.IsNullOrWhiteSpace (text);
#endif
		}
	}
}
