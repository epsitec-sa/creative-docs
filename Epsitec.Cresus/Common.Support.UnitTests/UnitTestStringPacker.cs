using Epsitec.Common.UnitTesting;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Common.Support.UnitTests
{
	
	
	[TestClass]
	public class UnitTestStringPacker
	{


		[TestMethod]
		public void PackArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => StringPacker.Pack (null, 'a', 'b')
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => StringPacker.Pack (new string[0], 'a', 'a')
			);
		}


		[TestMethod]
		public void UnPackArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => StringPacker.UnPack (null, 'a', 'b')
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => StringPacker.UnPack ("", 'a', 'b')
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => StringPacker.UnPack ("c", 'a', 'b')
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => StringPacker.UnPack ("a", 'a', 'a')
			);
		}


		[TestMethod]
		public void PackAndUnPack()
		{
			char separator = ';';
			char escaper = ':';

			List<List<string>> data = new List<List<string>> ()
			{
				new List<string> () { },
				new List<string> () { "" },
				new List<string> () { "a" },
				new List<string> () { "a", "b" },
				new List<string> () { "a", "b", "c" },
				new List<string> () { "", "", "" },
				new List<string> () { "", ":", "" },
				new List<string> () { " ", ":", "" },
				new List<string> () { ";", ";", ";" },
				new List<string> () { ":", ":", ":" },
				new List<string> () { ";", ":", ";" },
				new List<string> () { ":", ";", ":" },
				new List<string> () { "::", ":;:;:", ";" },
				new List<string> () { "", ":", ";;" },
				new List<string> () { "", ";", ";:;:;:" },
				new List<string> () { ";;", ":", "" },
				new List<string> () { ":;:;:;;:;:", ":", "" },
				new List<string> () { "::::", ":", "::" },
				new List<string> () { "", ":", ";;;;;" },
				new List<string> () { ";;;;;;;;;;", ";;;;", ";;;" },
				new List<string> () { ";a;", "b:b", ";c:c;c" },
			};

			foreach (var item in data)
			{
				var s = StringPacker.Pack (item, separator, escaper);
				var result = StringPacker.UnPack (s, separator, escaper).ToList ();

				CollectionAssert.AreEqual (item, result);
			}
		}


	}


}
