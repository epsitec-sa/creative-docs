using NUnit.Framework;

namespace Epsitec.Common.Support
{
	[TestFixture] public class ClipboardTest
	{
		[Test] public void CheckGetData()
		{
			Clipboard.Data data = Clipboard.GetData ();
			
			string[] fmt_1 = data.NativeFormats;
			string[] fmt_2 = data.AllPossibleFormats;
			
			System.Console.Out.WriteLine ("Native formats:");
			
			for (int i = 0; i < fmt_1.Length; i++)
			{
				bool ok = false;
				
				for (int j = 0; j < fmt_2.Length; j++)
				{
					if (fmt_1[i] == fmt_2[j])
					{
						ok = true;
						break;
					}
				}
				
				Assertion.Assert (string.Format ("{0} not found in derived formats.", fmt_1[i]), ok);
				
				System.Console.Out.WriteLine ("  {0}: {1}", i, fmt_1[i]);
			}
			
			System.Console.Out.WriteLine ("All possible formats:");
			
			for (int i = 0; i < fmt_2.Length; i++)
			{
				object                 as_object = data.Read (fmt_2[i]);
				string                 as_string = data.ReadAsString (fmt_2[i]);
				System.IO.MemoryStream as_stream = data.ReadAsStream (fmt_2[i]);
				
				string type = (as_object == null) ? "<null>" : as_object.GetType ().Name;
				
				System.Console.Out.WriteLine ("  {0}: {1}, {2}, [ {3} ]", i, fmt_2[i], type, as_string);
			}
		}
		
		[Test] public void CheckConvertBrokenUtf8ToString()
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			string source = "ABC‡ÈË";
			byte[] bytes  = System.Text.Encoding.UTF8.GetBytes (source);
			
			Assertion.AssertEquals (9, bytes.Length);
			
			for (int i = 0; i < bytes.Length; i++)
			{
				buffer.Append ((char)bytes[i]);
			}
			
			string utf8fix = buffer.ToString ();
			string unicode = Clipboard.ConvertBrokenUtf8ToString (utf8fix);
			
			Assertion.AssertEquals (source, unicode);
		}
		
		[Test] public void CheckGetDataHtml()
		{
			Clipboard.Data data = Clipboard.GetData ();
			string         html = data.ReadAsString ("HTML Format");
			
			if (html != null)
			{
				int idx_start = html.IndexOf ("<!--StartFragment-->");
				int idx_end   = html.IndexOf ("<!--EndFragment-->");
				int idx_begin = idx_start + 20;
				
				Assertion.Assert (idx_start > 0);
				Assertion.Assert (idx_end > idx_start);
				
				string clean = Clipboard.ConvertBrokenUtf8ToString (html.Substring (idx_begin, idx_end - idx_begin));
				System.Console.Out.WriteLine (clean);
				System.Console.Out.WriteLine (Clipboard.ConvertToSimpleXml (clean));
			}
			else
			{
				System.Console.Out.WriteLine ("*** No HTML format found ***");
			}
		}
		
		[Test] public void CheckIsCompatible()
		{
			Clipboard.Data data = Clipboard.GetData ();
			
			foreach (int i in System.Enum.GetValues (typeof (Clipboard.Format)))
			{
				Clipboard.Format format = (Clipboard.Format) i;
				System.Console.Out.WriteLine ("Compatible with {0}: {1}", format, data.IsCompatible (format));
			}
		}
	}
}
