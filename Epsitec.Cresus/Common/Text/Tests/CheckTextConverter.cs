//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Tests
{
	/// <summary>
	/// Vérifie le bon fonctionnement de la classe TextConverter.
	/// </summary>
	public sealed class CheckTextConverter
	{
		public static void RunTests()
		{
			string str1 = "Abc\u1234\ud840\udc01.";
			string str2;
			
			uint[] text;
			
			TextConverter.ConvertFromString (str1, out text);
			
			Debug.Assert.IsTrue (text.Length == 6);
			Debug.Assert.IsTrue (text[3] == 0x1234);
			Debug.Assert.IsTrue (text[4] == 0x20001);
			Debug.Assert.IsTrue (text[5] == '.');
			
			TextConverter.ConvertToString (text, out str2);
			
			Debug.Assert.IsTrue (str1 == str2);
			
			Debug.Expect.Exception (new Debug.Method (Ex1), typeof (Unicode.IllegalCodeException));
			Debug.Expect.Exception (new Debug.Method (Ex2), typeof (Unicode.IllegalCodeException));
		}
		
		
		public static void Ex1()
		{
			uint[] text = { 0xD800, 0xDC00, 0x1234 };
			string str;
			
			TextConverter.ConvertToString (text, out str);
		}
		
		public static void Ex2()
		{
			uint[] text = { 0x10FFFF, 0x110000 };
			string str;
			
			TextConverter.ConvertToString (text, out str);
		}
	}
}
