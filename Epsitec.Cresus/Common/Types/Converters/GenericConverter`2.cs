//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Types.Converters
{
	/// <summary>
	/// The <c>GenericConverter</c> class is the base class for specific type converters.
	/// Converters must derive from this class, not from <see cref="GenericConverter{T}"/>
	/// and must implement a default constructor, without any parameter.
	/// </summary>
	/// <typeparam name="T">The type on which the converter operates.</typeparam>
	/// <typeparam name="TSelf">The type of the converter itself.</typeparam>
	public abstract class GenericConverter<T, TSelf> : GenericConverter<T>
		where TSelf : GenericConverter<T, TSelf>, new ()
	{
		protected GenericConverter(System.Globalization.CultureInfo culture)
			: base (culture)
		{
		}
	}
}
