using NUnit.Framework;

namespace Epsitec.Common.Drawing
{
	[TestFixture] public class CompressLineBreakTool
	{
		[Test] public void CompressLineBreak()
		{
			System.IO.FileStream stream = new System.IO.FileStream (@"..\..\LineBreak-4.0.0.txt", System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read);
			byte[] buffer = new byte[stream.Length];
			stream.Read (buffer, 0, buffer.Length);
			stream.Close ();
			
			System.IO.FileStream target = new System.IO.FileStream (@"..\..\LineBreak.compressed", System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.None);
			System.IO.Stream compressor = IO.Compression.CreateBZip2Stream (target);
			
			compressor.Write (buffer, 0, buffer.Length);
			compressor.Close ();
			target.Close ();
		}
	}
}
