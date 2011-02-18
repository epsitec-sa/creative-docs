//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Types.Converters
{
	/// <summary>
	/// The <c>ConversionResult</c> class stores a conversion result for a
	/// specific type converter.
	/// </summary>
	/// <typeparam name="T">The type on which the converter operates.</typeparam>
	public class ConversionResult<T> : GenericConversionResult
	{
		/// <summary>
		/// Gets or sets the result value.
		/// </summary>
		/// <value>The result value.</value>
		public T Value
		{
			get;
			set;
		}
	}
}
