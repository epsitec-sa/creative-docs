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
		
		
		public static string[] Split(string text, int pos, int length)
		{
			return text.Substring (pos, length).Split ('/');
		}
		
		
		public static void Join(System.Text.StringBuilder buffer, params string[] args)
		{
			Debug.Assert.IsTrue (args.Length > 0);
			
			buffer.Append (args[0]);
			
			for (int i = 1; i < args.Length; i++)
			{
				buffer.Append ("/");
				buffer.Append (args[i]);
			}
		}
		
		
		public static string SerializeString(string value)
		{
			Debug.Assert.IsTrue (value != "<null>");
			
			if (value == null)
			{
				return "<null>";
			}
			else
			{
				return value;
			}
		}
		
		public static string SerializeDouble(double value)
		{
			if (double.IsNaN (value))
			{
				return "<NaN>";
			}
			else
			{
				return value.ToString (System.Globalization.CultureInfo.InvariantCulture);
			}
		}
		
		
		public static string DeserializeString(string value)
		{
			if (value == "<null>")
			{
				return null;
			}
			else
			{
				return value;
			}
		}
		
		public static double DeserializeDouble(string value)
		{
			if (value == "<NaN>")
			{
				return double.NaN;
			}
			else
			{
				return double.Parse (value, System.Globalization.CultureInfo.InvariantCulture);
			}
		}
	}
}
