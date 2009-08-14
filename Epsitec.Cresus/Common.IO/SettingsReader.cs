//	Copyright © 2008-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.IO
{
	/// <summary>
	/// The <c>SettingsReader</c> class provides utility functions to load settings
	/// as a collection of key/value pairs.
	/// </summary>
	public static class SettingsReader
	{
		public static IEnumerable<KeyValuePair<string, string>> ReadSettings(string path, System.Text.Encoding encoding)
		{
			using (System.IO.FileStream stream = new System.IO.FileStream (path, System.IO.FileMode.Open))
			{
				return SettingsReader.ReadSettings (stream, encoding);
			}
		}

		public static IEnumerable<KeyValuePair<string, string>> ReadSettings(System.IO.Stream stream, System.Text.Encoding encoding)
		{
			string text = ReaderHelper.ReadText (stream, encoding);

			foreach (string line in ReaderHelper.SplitLines (text))
			{
				if ((line.StartsWith ("#")) ||
					(line.StartsWith ("//")) ||
					(line.StartsWith ("-")))
				{
					continue;
				}

				int pos = line.IndexOf (':');

				if (pos < 0)
				{
					throw new System.FormatException ("Invalid settings line : " + line);
				}

				string key = line.Substring (0, pos).Trim ();
				string value = line.Substring (pos+1).Trim ();

				yield return new KeyValuePair<string, string> (key, value);
			}
		}
	}
}
