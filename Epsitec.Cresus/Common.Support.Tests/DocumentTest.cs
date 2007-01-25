using NUnit.Framework;

namespace Epsitec.Common.Support
{
	[TestFixture] public class DocumentTest
	{
		[Test]
		public void CheckDocumentManager()
		{
			string path = @"\\bigdell\c$\x.zip";

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
