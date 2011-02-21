//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Data
{
	/// <summary>
	/// The <c>ItemCodeGenerator</c> produces unique codes, which are used by the entities
	/// in the <see cref="IItemCode"/> interface.
	/// </summary>
	public static class ItemCodeGenerator
	{
		/// <summary>
		/// Produces a new globally unique code. The code corresponds to a 20-character
		/// encoded GUID (whereas a GUID usually gets encoded as a 32-digit string).
		/// </summary>
		/// <returns>The unique code.</returns>
		public static ItemCode NewCode()
		{
			return new ItemCode (System.Guid.NewGuid ());
		}


		public static System.Guid ToGuid(string code)
		{
			return Epsitec.Common.IO.Ascii85.DecodeGuid (Epsitec.Common.IO.Ascii85.MapXmlTransparentToEncodedString (code));
		}

		public static string FromGuid(System.Guid guid)
		{
			return Epsitec.Common.IO.Ascii85.MapEncodedStringToXmlTransparent (Epsitec.Common.IO.Ascii85.EncodeGuid (guid));
		}
		
		public static string FromGuid(string guid)
		{
			return ItemCodeGenerator.FromGuid (System.Guid.Parse (guid));
		}
	}
}