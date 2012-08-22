//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Xml.Linq;

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <c>Xml</c> class provides support for XML related work.
	/// </summary>
	public static class Xml
	{
		/// <summary>
		/// Converts the specified value to zero, one or more XML &lt;![CDATA[..]]&gt;
		/// nodes. Several nodes get created if the text contains the ']]&gt;' sequence.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>The <see cref="XCData"/> nodes.</returns>
		public static IEnumerable<XCData> ToCData(string value)
		{
			int start = 0;

			while (true)
			{
				if (start >= value.Length)
				{
					yield break;
				}

				int index = value.IndexOf ("]]>", start);

				if (index < 0)
				{
					yield return new XCData (value.Substring (start));
					yield break;
				}

				yield return new XCData (value.Substring (start, index-start+2));

				start = index+2;
			}
		}

		/// <summary>
		/// Converts the specified XML &lt;![CDATA[..]]&gt; sequence(s) back into a
		/// text, merging consecutive CDATA nodes.
		/// </summary>
		/// <param name="xmlCDataNode">The first <see cref="XCData"/> node.</param>
		/// <returns>The text.</returns>
		public static string FromCData(XNode xmlNode)
		{
			System.Text.StringBuilder buffer = null;

			var xmlCDataNode = xmlNode as XCData;

			if (xmlCDataNode == null)
			{
				return "";
			}

			while (true)
			{
				string item = xmlCDataNode.Value;
				XCData next = xmlCDataNode.NextNode as XCData;

				if (next == null)
				{
					if (buffer == null)
					{
						return item;
					}

					buffer.Append (item);
					return buffer.ToString ();
				}

				if (buffer == null)
				{
					buffer = new System.Text.StringBuilder ();
				}

				buffer.Append (item);
				xmlCDataNode = next;
			}
		}
	}
}