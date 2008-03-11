//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Widgets
{
	public class CommandParameters : Dictionary<string, string>
	{
		public CommandParameters()
		{
		}

		public CommandParameters(string source)
		{
			if (string.IsNullOrEmpty (source))
			{
				return;
			}

			foreach (string pair in source.Split (';'))
			{
				string[] args = pair.Split ('=');

				System.Diagnostics.Debug.Assert (args.Length == 2);

				string key   = CommandParameters.Unescape (args[0]);
				string value = CommandParameters.Unescape (args[1]);

				this[key] = value;
			}

		}

		public new string this[string key]
		{
			get
			{
				if (this.ContainsKey (key))
				{
					return base[key];
				}
				else
				{
					return null;
				}
			}
			set
			{
				if (value ==  null)
				{
					this.Remove (key);
				}
				else
				{
					base[key] = value;
				}
			}
		}
		
		public static CommandParameters Parse(string source)
		{
			return new CommandParameters (source);
		}

		public static string SetParameter(string source, string key, string value)
		{
			CommandParameters param = new CommandParameters (source);
			param[key] = value;
			return param.ToString ();
		}

		public override string ToString()
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();

			foreach (var item in this)
			{
				string pair = string.Concat (CommandParameters.Escape (item.Key), "=", CommandParameters.Escape (item.Value));

				if (buffer.Length > 0)
				{
					buffer.Append (";");
				}

				buffer.Append (pair);
			}

			return buffer.ToString ();
		}

		private static string Escape(string text)
		{
			if (string.IsNullOrEmpty (text))
			{
				return "";
			}

			text = text.Replace (@"\", @"\\");
			text = text.Replace (@"=", @"\-");
			text = text.Replace (@";", @"\,");

			return text;
		}

		private static string Unescape(string text)
		{
			if (string.IsNullOrEmpty (text))
			{
				return "";
			}

			if (text.Contains (@"\"))
			{
				text = text.Replace (@"\,", @";");
				text = text.Replace (@"\-", @"=");
				text = text.Replace (@"\\", @"\");
			}

			return text;
		}

	}
}
