//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Support.Extensions
{
	/// <summary>
	/// The <c>NumericExtensions</c> class provides extension methods for numeric types
	/// (such as <c>decimal</c> or <c>int</c>).
	/// </summary>
	public static class NumericExtensions
	{
		public static bool InRange(this decimal value, decimal? beginDate, decimal? endDate)
		{
			if (beginDate.HasValue && beginDate.Value > value)
			{
				return false;
			}
			if (endDate.HasValue && endDate.Value < value)
			{
				return false;
			}

			return true;
		}
	}
}
