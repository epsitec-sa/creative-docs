using NUnit.Framework;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Drawing
{
	[TestFixture] public class BitmapTest
	{
		[Test] public void CheckImageFormat()
		{
			Assertion.AssertNotNull (Bitmap.MapToMicrosoftImageFormat (ImageFormat.Bmp));
			Assertion.AssertNotNull (Bitmap.MapToMicrosoftImageFormat (ImageFormat.Gif));
			Assertion.AssertNotNull (Bitmap.MapToMicrosoftImageFormat (ImageFormat.Png));
			Assertion.AssertNotNull (Bitmap.MapToMicrosoftImageFormat (ImageFormat.Tiff));
			Assertion.AssertNotNull (Bitmap.MapToMicrosoftImageFormat (ImageFormat.Jpeg));
			Assertion.AssertNotNull (Bitmap.MapToMicrosoftImageFormat (ImageFormat.Exif));
			Assertion.AssertNotNull (Bitmap.MapToMicrosoftImageFormat (ImageFormat.WindowsIcon));
			Assertion.AssertNotNull (Bitmap.MapToMicrosoftImageFormat (ImageFormat.WindowsEmf));
			Assertion.AssertNotNull (Bitmap.MapToMicrosoftImageFormat (ImageFormat.WindowsWmf));
		}
		
		[Test] public void CheckEncoders()
		{
			Assertion.AssertNotNull (Bitmap.GetFilenameExtensions (ImageFormat.Bmp));
			Assertion.AssertNotNull (Bitmap.GetFilenameExtensions (ImageFormat.Gif));
			Assertion.AssertNotNull (Bitmap.GetFilenameExtensions (ImageFormat.Png));
			Assertion.AssertNotNull (Bitmap.GetFilenameExtensions (ImageFormat.Tiff));
			Assertion.AssertNotNull (Bitmap.GetFilenameExtensions (ImageFormat.Jpeg));
		}
		
		[Test] public void CheckBitmapSave()
		{
			Bitmap bitmap = Bitmap.FromFile (@"..\..\images\picture.png").BitmapImage;
			
			byte[] data;
			System.IO.FileStream stream;
			
			data   = bitmap.Save (ImageFormat.Bmp, 24, 0, ImageCompression.None);
			stream = new System.IO.FileStream ("picture-24.bmp", System.IO.FileMode.CreateNew);
			stream.Write (data, 0, data.Length);
			stream.Close ();
			
			data   = bitmap.Save (ImageFormat.Bmp, 32, 0, ImageCompression.None);
			stream = new System.IO.FileStream ("picture-32.bmp", System.IO.FileMode.CreateNew);
			stream.Write (data, 0, data.Length);
			stream.Close ();
			
			data   = bitmap.Save (ImageFormat.Gif, 0, 0, ImageCompression.None);
			stream = new System.IO.FileStream ("picture.gif", System.IO.FileMode.CreateNew);
			stream.Write (data, 0, data.Length);
			stream.Close ();
			
			data   = bitmap.Save (ImageFormat.Png, 16, 0, ImageCompression.None);
			stream = new System.IO.FileStream ("picture-16.png", System.IO.FileMode.CreateNew);
			stream.Write (data, 0, data.Length);
			stream.Close ();
			
			data   = bitmap.Save (ImageFormat.Png, 24, 0, ImageCompression.None);
			stream = new System.IO.FileStream ("picture-24.png", System.IO.FileMode.CreateNew);
			stream.Write (data, 0, data.Length);
			stream.Close ();
			
			data   = bitmap.Save (ImageFormat.Png, 32, 0, ImageCompression.None);
			stream = new System.IO.FileStream ("picture-32.png", System.IO.FileMode.CreateNew);
			stream.Write (data, 0, data.Length);
			stream.Close ();
			
			data   = bitmap.Save (ImageFormat.Tiff, 24, 0, ImageCompression.None);
			stream = new System.IO.FileStream ("picture-24.tif", System.IO.FileMode.CreateNew);
			stream.Write (data, 0, data.Length);
			stream.Close ();
			
			data   = bitmap.Save (ImageFormat.Tiff, 32, 0, ImageCompression.Lzw);
			stream = new System.IO.FileStream ("picture-32-lzw.tif", System.IO.FileMode.CreateNew);
			stream.Write (data, 0, data.Length);
			stream.Close ();
			
			data   = bitmap.Save (ImageFormat.Jpeg, 24, 25, ImageCompression.None);
			stream = new System.IO.FileStream ("picture-Q25.jpg", System.IO.FileMode.CreateNew);
			stream.Write (data, 0, data.Length);
			stream.Close ();
			
			data   = bitmap.Save (ImageFormat.Jpeg, 24, 50, ImageCompression.None);
			stream = new System.IO.FileStream ("picture-Q50.jpg", System.IO.FileMode.CreateNew);
			stream.Write (data, 0, data.Length);
			stream.Close ();
			
			data   = bitmap.Save (ImageFormat.Jpeg, 24, 75, ImageCompression.None);
			stream = new System.IO.FileStream ("picture-Q75.jpg", System.IO.FileMode.CreateNew);
			stream.Write (data, 0, data.Length);
			stream.Close ();
			
			data   = bitmap.Save (ImageFormat.Jpeg, 24, 100, ImageCompression.None);
			stream = new System.IO.FileStream ("picture-Q100.jpg", System.IO.FileMode.CreateNew);
			stream.Write (data, 0, data.Length);
			stream.Close ();
			
			data   = bitmap.Save (ImageFormat.WindowsIcon, 0, 0, ImageCompression.None);
			stream = new System.IO.FileStream ("picture.ico", System.IO.FileMode.CreateNew);
			stream.Write (data, 0, data.Length);
			stream.Close ();
		}
		
		[Test] public void CheckBitmapSaveCleanup()
		{
			System.IO.File.Delete ("picture-24.bmp");
			System.IO.File.Delete ("picture-32.bmp");
			System.IO.File.Delete ("picture.gif");
			System.IO.File.Delete ("picture-16.png");
			System.IO.File.Delete ("picture-24.png");
			System.IO.File.Delete ("picture-32.png");
			System.IO.File.Delete ("picture-24.tif");
			System.IO.File.Delete ("picture-32-lzw.tif");
			System.IO.File.Delete ("picture-Q25.jpg");
			System.IO.File.Delete ("picture-Q50.jpg");
			System.IO.File.Delete ("picture-Q75.jpg");
			System.IO.File.Delete ("picture-Q100.jpg");
			System.IO.File.Delete ("picture.ico");
		}
	}
}
