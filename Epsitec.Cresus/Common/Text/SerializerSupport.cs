//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
		
		public static string SerializeStringArray(string[] value)
		{
			if (value == null)
			{
				return "[null]";
			}
			else if (value.Length == 0)
			{
				return "[empty]";
			}
			else
			{
				string[] array = new string[value.Length];
				
				for (int i = 0; i < value.Length; i++)
				{
					if (value[i] == null)
					{
						array[i] = "[null]";
					}
					else
					{
						array[i] = SerializerSupport.Escape (value[i]);
					}
				}
				
				return SerializerSupport.Escape (SerializerSupport.Join (array));
			}
		}
		
		public static string SerializeInt(int value)
		{
			return value.ToString (System.Globalization.CultureInfo.InvariantCulture);
		}
		
		public static string SerializeLong(long value)
		{
			return value.ToString (System.Globalization.CultureInfo.InvariantCulture);
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
		
		public static string SerializeEnum(System.Enum value)
		{
			return value.ToString ();
		}
		
		public static string SerializeBoolean(bool value)
		{
			return value ? "[true]" : "[false]";
		}
		
		public static string SerializeThreeState(Properties.ThreeState value)
		{
			switch (value)
			{
				case Properties.ThreeState.Undefined:	return "[?]";
				case Properties.ThreeState.True:		return "[true]";
				case Properties.ThreeState.False:		return "[false]";
			}
			
			throw new System.ArgumentException ();
		}
		
		public static string SerializeSizeUnits(Properties.SizeUnits value)
		{
			return Properties.UnitsTools.SerializeSizeUnits (value);
		}
		
		
		public static void SerializeStringStringHash(System.Collections.Hashtable hash, System.Text.StringBuilder buffer)
		{
			int count = hash.Count;
			
			buffer.Append (SerializerSupport.SerializeInt (count));
			
			if (count > 0)
			{
				foreach (System.Collections.DictionaryEntry entry in hash)
				{
					string key   = entry.Key as string;
					string value = entry.Value as string;
					
					buffer.Append ("/");
					buffer.Append (SerializerSupport.SerializeString (key));
					buffer.Append ("/");
					buffer.Append (SerializerSupport.SerializeString (value));
				}
			}
		}
		
		public static void SerializeStringIntHash(System.Collections.Hashtable hash, System.Text.StringBuilder buffer)
		{
			int count = hash.Count;
			
			buffer.Append (SerializerSupport.SerializeInt (count));
			
			if (count > 0)
			{
				foreach (System.Collections.DictionaryEntry entry in hash)
				{
					string key   = entry.Key as string;
					int    value = (int) entry.Value;
					
					buffer.Append ("/");
					buffer.Append (SerializerSupport.SerializeString (key));
					buffer.Append ("/");
					buffer.Append (SerializerSupport.SerializeInt (value));
				}
			}
		}
		
		public static void DeserializeStringStringHash(string[] args, ref int offset, System.Collections.Hashtable hash)
		{
			int count = SerializerSupport.DeserializeInt (args[offset++]);
			
			for (int i = 0; i < count; i++)
			{
				string key   = SerializerSupport.DeserializeString (args[offset++]);
				string value = SerializerSupport.DeserializeString (args[offset++]);
				
				hash[key] = value;
			}
		}
		
		public static void DeserializeStringIntHash(string[] args, ref int offset, System.Collections.Hashtable hash)
		{
			int count = SerializerSupport.DeserializeInt (args[offset++]);
			
			for (int i = 0; i < count; i++)
			{
				string key   = SerializerSupport.DeserializeString (args[offset++]);
				int    value = SerializerSupport.DeserializeInt (args[offset++]);
				
				hash[key] = value;
			}
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
		
		public static string[] DeserializeStringArray(string value)
		{
			if (value == "[null]")
			{
				return null;
			}
			else if (value == "[empty]")
			{
				return new string[0];
			}
			else
			{
				string[] array = SerializerSupport.Split (SerializerSupport.Unescape (value));
				
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i] == "[null]")
					{
						array[i] = null;
					}
					else
					{
						array[i] = SerializerSupport.Unescape (array[i]);
					}
				}
				
				return array;
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
		
		public static int DeserializeInt(string value)
		{
			return int.Parse (value, System.Globalization.CultureInfo.InvariantCulture);
		}
		
		public static long DeserializeLong(string value)
		{
			return long.Parse (value, System.Globalization.CultureInfo.InvariantCulture);
		}
		
		public static object DeserializeEnum(System.Type type, string value)
		{
			return System.Enum.Parse (type, value);
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
		
		public static Properties.ThreeState DeserializeThreeState(string value)
		{
			switch (value)
			{
				case "[true]":		return Properties.ThreeState.True;
				case "[false]":		return Properties.ThreeState.False;
				case "[?]":			return Properties.ThreeState.Undefined;
			}
			
			throw new System.FormatException (string.Format ("'{0}' is not ThreeState", value));
		}
		
		public static Properties.SizeUnits DeserializeSizeUnits(string value)
		{
			return Properties.UnitsTools.DeserializeSizeUnits (value);
		}
		
		
		
		private static string Escape(string value)
		{
			if (value.IndexOfAny (new char[] { '\\', '[', ']', '{', '}', '/', '|', ':', '<', '>', ';' }) == -1)
			{
				return value;
			}
			
			value = value.Replace ("\\", "\\0");
			value = value.Replace ("[",  "\\1");
			value = value.Replace ("]",  "\\2");
			value = value.Replace ("{",  "\\3");
			value = value.Replace ("}",  "\\4");
			value = value.Replace ("/",  "\\5");
			value = value.Replace ("|",  "\\6");
			value = value.Replace (":",  "\\7");
			value = value.Replace ("<",  "\\8");
			value = value.Replace (">",  "\\9");
			value = value.Replace (";",  "\\A");
			
			return value;
		}
		
		private static string Unescape(string value)
		{
			if (value.IndexOf ('\\') == -1)
			{
				return value;
			}
			
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			for (int i = 0; i < value.Length; i++)
			{
				char c = value[i];
				
				if (c == '\\')
				{
					if (i == value.Length-1)
					{
						throw new System.ArgumentException ("Invalid truncated escape sequence.");
					}
					
					switch (value[++i])
					{
						case '0':		c = '\\';	break;
						case '1':		c = '[';	break;
						case '2':		c = ']';	break;
						case '3':		c = '{';	break;
						case '4':		c = '}';	break;
						case '5':		c = '/';	break;
						case '6':		c = '|';	break;
						case '7':		c = ':';	break;
						case '8':		c = '<';	break;
						case '9':		c = '>';	break;
						case 'A':		c = ';';	break;
						
						default:
							throw new System.ArgumentException ("Invalid escape sequence.");
					}
				}
				
				buffer.Append (c);
			}
			
			return buffer.ToString ();
		}
		
	}
}
