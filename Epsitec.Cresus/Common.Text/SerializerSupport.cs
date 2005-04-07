//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// La classe SerializerSupport implémente une série de méthodes utiles
	/// pour la sérialisation/désérialisation.
	/// </summary>
	public sealed class SerializerSupport
	{
		private SerializerSupport()
		{
		}
		
		
		public static string[] Split(string text)
		{
			return text.Split ('/');
		}
		
		public static string[] Split(string text, int pos, int length)
		{
			return text.Substring (pos, length).Split ('/');
		}
		
		public static string Join(params string[] args)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			SerializerSupport.Join (buffer, args);
			
			return buffer.ToString ();
		}
		
		public static void Join(System.Text.StringBuilder buffer, params string[] args)
		{
			Debug.Assert.IsTrue (args.Length > 0);
			Debug.Assert.IsTrue (args[0].IndexOf ("/") == -1);
			
			buffer.Append (args[0]);
			
			for (int i = 1; i < args.Length; i++)
			{
				Debug.Assert.IsTrue (args[i].IndexOf ("/") == -1);
				
				buffer.Append ("/");
				buffer.Append (args[i]);
			}
		}
		
		
		public static string SerializeString(string value)
		{
			Debug.Assert.IsTrue (value != "[null]");
			
			if (value == null)
			{
				return "[null]";
			}
			else
			{
				return SerializerSupport.Escape (value);
			}
		}
		
		private static string Escape(string value)
		{
			if (value.IndexOfAny (new char[] { '/', '\\', '[', ']' }) == -1)
			{
				return value;
			}
			
			value = value.Replace ("\\", "\\\\");
			value = value.Replace ("/", "\\:");
			value = value.Replace ("[", "\\[");
			value = value.Replace ("]", "\\]");
			
			return value;
		}
		
		private static string Unescape(string value)
		{
			if (value.IndexOf ('\\') == -1)
			{
				return value;
			}
			
			value = value.Replace ("\\]", "]");
			value = value.Replace ("\\[", "[");
			value = value.Replace ("\\:", "/");
			value = value.Replace ("\\\\", "\\");
			
			return value;
		}
		
		public static string SerializeDouble(double value)
		{
			if (double.IsNaN (value))
			{
				return "[NaN]";
			}
			else
			{
				return value.ToString (System.Globalization.CultureInfo.InvariantCulture);
			}
		}
		
		public static string SerializeBoolean(bool value)
		{
			return value ? "[true]" : "[false]";
		}
		
		
		public static string DeserializeString(string value)
		{
			if (value == "[null]")
			{
				return null;
			}
			else
			{
				return SerializerSupport.Unescape (value);
			}
		}
		
		public static double DeserializeDouble(string value)
		{
			if (value == "[NaN]")
			{
				return double.NaN;
			}
			else
			{
				return double.Parse (value, System.Globalization.CultureInfo.InvariantCulture);
			}
		}
		
		public static bool DeserializeBoolean(string value)
		{
			if (value == "[true]")
			{
				return true;
			}
			if (value == "[false]")
			{
				return false;
			}
			
			throw new System.FormatException (string.Format ("'{0}' is not a boolean", value));
		}
	}
}
