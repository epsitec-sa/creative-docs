//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;

namespace Epsitec.Common.UI.Controllers
{
	public class ControllerParameters
	{
		public ControllerParameters(string source)
		{
			this.source = string.IsNullOrEmpty (source) ? null : source;
		}


		public string GetParameterValue(string key)
		{
			if (source == null)
			{
				return null;
			}

			if (this.dictionary == null)
			{
				this.AllocateDictionary ();
			}

			string value;

			if (this.dictionary.TryGetValue (key, out value))
			{
				return value;
			}
			else
			{
				return null;
			}
		}


		public static string MergeParameters(params string[] sources)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();

			foreach (string source in sources)
			{
				if (string.IsNullOrEmpty (source))
				{
					continue;
				}
				
				if (buffer.Length > 0)
				{
					buffer.Append (" ");
				}

				buffer.Append (source);
			}

			return buffer.ToString ();
		}
		
		
		private void AllocateDictionary()
		{
			System.Diagnostics.Debug.Assert (this.source != null);
			System.Diagnostics.Debug.Assert (this.source.Length > 0);

			Dictionary<string, string> dictionary = new Dictionary<string, string> ();

			string[] tokens = this.source.Split (new char[] { ' ', '\t', '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);

			foreach (string token in tokens)
			{
				string[] args = token.Split ('=');

				string key;
				string value;

				if (args.Length > 2)
				{
					key   = args[0];
					value = string.Join ("=", args, 1, args.Length-1);
				}
				else if (args.Length == 1)
				{
					key   = args[0];
					value = "";
				}
				else
				{
					key   = args[0];
					value = args[1];
				}

				string simpleKey = key.TrimStart ('+', '-');

				if (dictionary.ContainsKey (simpleKey))
				{
					//	TODO: what to do in case of multiple definitions ?
				}

				dictionary[simpleKey] = value;
			}

			this.dictionary = dictionary;
		}


		private readonly string source;
		private Dictionary<string, string> dictionary;
	}
}
