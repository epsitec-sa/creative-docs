//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Types.Serialization;
using Epsitec.Common.Support.Serialization.Converters;

using System.Collections.Generic;

[assembly:DependencyConverter (typeof (byte[]), Converter=typeof (BinaryArrayConverter))]

namespace Epsitec.Common.Support.Serialization.Converters
{
	public class BinaryArrayConverter : ISerializationConverter
	{
		#region ISerializationConverter Members

		public string ConvertToString(object value, IContextResolver context)
		{
			if (value == null)
			{
				return "<null>";
			}

			return Epsitec.Common.IO.Ascii85.Encode ((byte[]) value);
		}

		public object ConvertFromString(string value, IContextResolver context)
		{
			if (value == "<null>")
			{
				return null;
			}

			return Epsitec.Common.IO.Ascii85.Decode (value);
		}

		#endregion
	}
}
