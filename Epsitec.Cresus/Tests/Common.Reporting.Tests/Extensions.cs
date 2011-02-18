//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using System.Collections.Generic;

namespace Epsitec.Common.Reporting
{
	public static class Extensions
	{
		public static string FlattenSamples(this IEnumerable<AbstractEntity> samples, string separator)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();

			foreach (var item in samples)
			{
				buffer.Append (item.GetField<string> ("first"));
				buffer.Append (" ");
				buffer.Append (item.GetField<string> ("last"));
				buffer.Append (" ");
				buffer.Append (item.GetField<int> ("year").ToString (System.Globalization.CultureInfo.InvariantCulture));
				buffer.Append (separator);
			}

			return buffer.ToString ();
		}

		public static string Join(this IEnumerable<string> collection, string separator)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();

			foreach (string item in collection)
			{
				if (buffer.Length > 0)
				{
					buffer.Append (separator);
				}
				
				buffer.Append (item);
			}

			return buffer.ToString ();
		}
	}
}
