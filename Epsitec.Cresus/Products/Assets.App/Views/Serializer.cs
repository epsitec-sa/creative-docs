//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class Serializer
	{
		public Serializer()
		{
			this.data = new Dictionary<string, string> ();
		}

		public Serializer(string data)
		{
			this.data = new Dictionary<string, string> ();
			this.SetData (data);
		}


		public string Data
		{
			get
			{
				return this.GetData ();
			}
		}


		public void Clear()
		{
			this.data.Clear ();
		}


		public void SetString(string name, string value)
		{
			this.SetData (name, value);
		}

		public void SetBool(string name, bool value)
		{
			string s = value.ToString (System.Globalization.CultureInfo.InvariantCulture);
			this.SetData (name, s);
		}

		public void SetInt(string name, int value)
		{
			string s = value.ToString (System.Globalization.CultureInfo.InvariantCulture);
			this.SetData (name, s);
		}

		public void SetDecimal(string name, decimal value)
		{
			string s = value.ToString (System.Globalization.CultureInfo.InvariantCulture);
			this.SetData (name, s);
		}

		public void SetTimestamp(string name, Timestamp? value)
		{
			string s = value.HasValue ? value.Value.ToString () : "null";
			this.SetData (name, s);
		}

		public void SetGuid(string name, Guid value)
		{
			string s = value.ToString ();
			this.SetData (name, s);
		}


		public string GetString(string name)
		{
			return this.GetData (name);
		}

		public bool GetBool(string name)
		{
			string s = this.GetData (name);

			if (!string.IsNullOrEmpty (s))
			{
				bool value;
				if (bool.TryParse (s, out value))
				{
					return value;
				}
			}

			return false;
		}

		public int GetInt(string name)
		{
			string s = this.GetData (name);

			if (!string.IsNullOrEmpty (s))
			{
				int value;
				if (int.TryParse (s, out value))
				{
					return value;
				}
			}

			return 0;
		}

		public decimal GetDecimal(string name)
		{
			string s = this.GetData (name);

			if (!string.IsNullOrEmpty (s))
			{
				decimal value;
				if (decimal.TryParse (s, out value))
				{
					return value;
				}
			}

			return 0.0m;
		}

		public Timestamp? GetTimestamp(string name)
		{
			string s = this.GetData (name);

			if (!string.IsNullOrEmpty (s) && s != "null")
			{
				try
				{
					return Timestamp.Parse (s);
				}
				catch
				{
				}
			}

			return null;
		}

		public Guid GetGuid(string name)
		{
			string s = this.GetData (name);

			if (!string.IsNullOrEmpty (s))
			{
				try
				{
					return Guid.Parse (s);
				}
				catch
				{
				}
			}

			return Guid.Empty;
		}


		private void SetData(string data)
		{
			this.data.Clear ();

			if (!string.IsNullOrEmpty (data))
			{
				var lines = data.Split (new string[] { "<br/>" }, System.StringSplitOptions.RemoveEmptyEntries);

				foreach (var line in lines)
				{
					var words = line.Split ('=');
					System.Diagnostics.Debug.Assert (words.Length == 2);

					string name  = Serializer.IntToExt (words[0]);
					string value = Serializer.IntToExt (words[1]);

					this.SetData (name, value);
				}
			}
		}

		private void SetData(string name, string value)
		{
			System.Diagnostics.Debug.Assert (!this.data.ContainsKey (name));
			if (value != null)
			{
				this.data.Add (name, value);
			}
		}


		private string GetData(string name)
		{
			string value;
			if (this.data.TryGetValue (name, out value))
			{
				return value;
			}
			else
			{
				return null;
			}
		}

		private string GetData()
		{
			var builder = new System.Text.StringBuilder ();

			foreach (var pair in this.data)
			{
				string name  = Serializer.ExtToInt (pair.Key);
				string value = Serializer.ExtToInt (pair.Value);

				builder.Append (name);
				builder.Append ("=");
				builder.Append (value);
				builder.Append ("<br/>");
			}

			return builder.ToString ();
		}


		private static string ExtToInt(string text)
		{
			if (!string.IsNullOrEmpty (text))
			{
				text = text.Replace ("&", "&amp;")
						   .Replace ("=", "&eq;")
						   .Replace ("<", "&lt;")
						   .Replace (">", "&gt;");
			}

			return text;
		}

		private static string IntToExt(string text)
		{
			if (!string.IsNullOrEmpty (text))
			{
				text = text.Replace ("&eq;", "=")
						   .Replace ("&lt;", "<")
						   .Replace ("&gt;", ">")
						   .Replace ("&amp;", "&");
			}

			return text;
		}


#if true
		//	Le constructeur statique implémente un "auto-test" de la classe.
		static Serializer()
		{
			{
				var date = new System.DateTime (2014, 3, 31);
				var time = new Timestamp (date, 5);
				var guid = new Guid ();

				var serial1 = new Serializer ();
				serial1.SetString ("A", "coucou");
				serial1.SetBool ("B", true);
				serial1.SetInt ("C", 123);
				serial1.SetDecimal ("D", 123.456m);
				serial1.SetTimestamp ("E", time);
				serial1.SetGuid ("F", guid);
				var result = serial1.Data;

				//------------

				var serial2 = new Serializer (result);
				System.Diagnostics.Debug.Assert (serial2.GetString ("A") == "coucou");
				System.Diagnostics.Debug.Assert (serial2.GetBool ("B") == true);
				System.Diagnostics.Debug.Assert (serial2.GetInt ("C") == 123);
				System.Diagnostics.Debug.Assert (serial2.GetDecimal ("D") == 123.456m);
				System.Diagnostics.Debug.Assert (serial2.GetTimestamp ("E") == time);
				System.Diagnostics.Debug.Assert (serial2.GetGuid ("F") == guid);
			}

			{
				var serial1 = new Serializer ();
				serial1.SetString ("A", "coucou");
				serial1.SetString ("B", "");
				serial1.SetString ("C", null);
				serial1.SetString ("x=y", "<b>bold</b>");
				serial1.SetString ("a&b", "ligne 1<br/>ligne 2");
				serial1.SetString ("a", "&lt;b&gt;coucou&lt;/b&gt;");
				var result = serial1.Data;

				//------------

				var serial2 = new Serializer (result);
				System.Diagnostics.Debug.Assert (serial2.GetString ("A") == "coucou");
				System.Diagnostics.Debug.Assert (serial2.GetString ("B") == "");
				System.Diagnostics.Debug.Assert (serial2.GetString ("C") == null);
				System.Diagnostics.Debug.Assert (serial2.GetString ("x=y") == "<b>bold</b>");
				System.Diagnostics.Debug.Assert (serial2.GetString ("a&b") == "ligne 1<br/>ligne 2");
				System.Diagnostics.Debug.Assert (serial2.GetString ("a") == "&lt;b&gt;coucou&lt;/b&gt;");
				System.Diagnostics.Debug.Assert (serial2.GetString ("X") == null);
			}
		}
#endif


		private readonly Dictionary<string, string> data;
	}
}
