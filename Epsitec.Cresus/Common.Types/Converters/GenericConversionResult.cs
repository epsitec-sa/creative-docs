//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Types.Converters
{
	/// <summary>
	/// The <c>GenericConversionResult</c> class stores a conversion result. See
	/// also <see cref="GenericConverter&lt;T&gt;.GetConversionResult"/>.
	/// </summary>
	public abstract class GenericConversionResult
	{
		public bool IsNull
		{
			get;
			set;
		}

		public bool HasValue
		{
			get
			{
				return !this.IsNull && !this.IsInvalid;
			}
		}

		public bool IsInvalid
		{
			get;
			set;
		}

		public bool IsValid
		{
			get
			{
				return !this.IsInvalid;
			}
		}
	}
}
