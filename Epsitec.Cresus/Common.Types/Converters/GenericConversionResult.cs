//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Types.Converters
{
	/// <summary>
	/// The <c>GenericConversionResult</c> class stores a conversion result. See
	/// also method <see cref="GenericConverter{T}.ConvertFromString"/> and class
	/// <see cref="ConversionResult{T}"/>.
	/// </summary>
	public abstract class GenericConversionResult
	{
		/// <summary>
		/// Gets or sets a value indicating whether the result is null.
		/// </summary>
		/// <value><c>true</c> if the result is null; otherwise, <c>false</c>.</value>
		public bool IsNull
		{
			get;
			set;
		}

		/// <summary>
		/// Gets a value indicating whether the result has a valid, non-null value.
		/// </summary>
		/// <value><c>true</c> if this instance has a valid, non-null value; otherwise, <c>false</c>.</value>
		public bool HasValue
		{
			get
			{
				return !this.IsNull && !this.IsInvalid;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the result is invalid.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if the result is invalid; otherwise, <c>false</c>.
		/// </value>
		public bool IsInvalid
		{
			get;
			set;
		}

		/// <summary>
		/// Gets a value indicating whether the result is valid.
		/// </summary>
		/// <value><c>true</c> if the result is valid; otherwise, <c>false</c>.</value>
		public bool IsValid
		{
			get
			{
				return !this.IsInvalid;
			}
		}
	}
}
