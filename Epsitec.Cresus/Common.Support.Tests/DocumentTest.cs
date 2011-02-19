//	Copyright © 2007-2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using NUnit.Framework;

namespace Epsitec.Common.Support
{
	[TestFixture] public class DocumentTest
	{
		[Test]
		public void CheckDocumentManager()
		{
			string path = @"\\bigdell\c$\do-not-delete used-by-common-support-tests.zip";

			bool ok;
			
			try
			{
				ok = System.IO.File.Exists (path);
			}
			catch
			{
				ok = false;
			}

			if (ok == false)
			{
				Assert.Ignore ("Cannot run this test; file " + path + " not present");
			}
			else
			{
				IO.ZipFile zipFile = new IO.ZipFile ();
				DocumentManager manager = new DocumentManager ();

				manager.Open (path);
				System.IO.Stream stream = manager.GetLocalFileStream (System.IO.FileAccess.Read);

				System.Text.StringBuilder buffer = new System.Text.StringBuilder ();

				zipFile = new IO.ZipFile ();
				zipFile.LoadFileName = manager.GetLocalFilePath ();
				zipFile.LoadFile (stream,
					delegate (string entryName)
					{
						buffer.AppendFormat ("Entry: {0}, copied so far: {1}\r\n", entryName, manager.LocalCopyLength);
						return true;
					});

				manager.WaitForLocalCopyReady (-1);

				stream.Close ();
				System.Console.Out.WriteLine ("Local copy length: {0}/{1}", manager.LocalCopyLength, manager.SourceLength);

				manager.Close ();
				manager.Dispose ();

				manager = new DocumentManager ();
				manager.Open (path);
				manager.Close ();
				manager.Dispose ();

				System.Console.Out.WriteLine ("Done.");
			}
		}
	}
}
