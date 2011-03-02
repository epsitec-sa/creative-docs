using NUnit.Framework;
using Epsitec.Common.Support;

namespace Epsitec.Common.Tests.Support
{
	[TestFixture] public class ClipboardTest
	{
		[SetUp] public void Initialize()
		{
			Epsitec.Common.Widgets.Widget.Initialize ();
			Assert.AreEqual (System.Threading.ApartmentState.STA, System.Threading.Thread.CurrentThread.GetApartmentState ());
		}

		
		[Test]
		public void CheckGetData()
		{
			ClipboardReadData data = Clipboard.GetData ();
			
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
				
				Assert.IsTrue (ok, string.Format ("{0} not found in derived formats.", fmt_1[i]));
				
				System.Console.Out.WriteLine ("  {0}: {1}", i, fmt_1[i]);
			}
			
			System.Console.Out.WriteLine ("All possible formats:");
			
			for (int i = 0; i < fmt_2.Length; i++)
			{
				object as_object = data.Read (fmt_2[i]);
				string as_string = data.ReadAsString (fmt_2[i]);
				string type      = (as_object == null) ? "<null>" : as_object.GetType ().Name;
				
				System.Console.Out.WriteLine ("  {0}: {1}, {2}, [ {3} ]", i, fmt_2[i], type, as_string);
			}
		}
		
		[Test]
		public void CheckConvertBrokenUtf8ToString()
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			string source = "ABC‡ÈË";
			byte[] bytes  = System.Text.Encoding.UTF8.GetBytes (source);
			
			Assert.AreEqual (9, bytes.Length);
			
			for (int i = 0; i < bytes.Length; i++)
			{
				buffer.Append ((char)bytes[i]);
			}
			
			string utf8fix = buffer.ToString ();
			string unicode = Clipboard.ConvertBrokenUtf8ToString (utf8fix);
			
			Assert.AreEqual (source, unicode);
		}
		
		[Test]
		public void CheckReadHtmlFragment()
		{
			ClipboardReadData data = Clipboard.GetData ();
			string             html = data.ReadHtmlFragment ();
			
			if (html != null)
			{
				System.Console.Out.WriteLine ("[ {0} ]", html);
				System.Console.Out.WriteLine ("[ {0} ]", Clipboard.ConvertHtmlToSimpleXml (html));
			}
			else
			{
				System.Console.Out.WriteLine ("*** No HTML format found ***");
			}
		}
		
		[Test]
		public void CheckReadHtmlDocument()
		{
			ClipboardReadData data = Clipboard.GetData ();
			string             html = data.ReadHtmlDocument ();
			
			if (html != null)
			{
				System.Console.Out.WriteLine ("[ {0} ]", html);
				System.Console.Out.WriteLine ("[ {0} ]", Clipboard.ConvertHtmlToSimpleXml (html));
			}
			else
			{
				System.Console.Out.WriteLine ("*** No HTML format found ***");
			}
		}
		
		[Test]
		public void CheckIsCompatible()
		{
			ClipboardReadData data = Clipboard.GetData ();

			foreach (int i in System.Enum.GetValues (typeof (ClipboardDataFormat)))
			{
				ClipboardDataFormat format = (ClipboardDataFormat) i;
				System.Console.Out.WriteLine ("Compatible with {0}: {1}", format, data.IsCompatible (format));
			}
		}
		
		[Test]
		public void CheckWriteHtmlFragment()
		{
			ClipboardWriteData data = new ClipboardWriteData ();
			
			data.WriteText ("Hello world\u00A0! Three [   ] spaces.\r\n\r\nLast line.");
			data.WriteHtmlFragment ("Hello <i>world</i>&#160;! Three [   ] spaces.<br/><br/>Last line.");
			
			Clipboard.SetData (data);
		}
	}
}
