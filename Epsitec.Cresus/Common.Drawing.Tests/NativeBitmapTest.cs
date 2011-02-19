//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing.Platform;
using NUnit.Framework;

namespace Epsitec.Common.Drawing
{
	[TestFixture]
	public class NativeBitmapTest
	{
		[Test]
		public void TestRgbTiff()
		{
			NativeBitmap image = NativeBitmap.Load (@"..\..\images\rgb.tif");

			NativeBitmap imageR    = image.GetChannel (BitmapColorChannel.Red);
			NativeBitmap imageG    = image.GetChannel (BitmapColorChannel.Green);
			NativeBitmap imageB    = image.GetChannel (BitmapColorChannel.Blue);
			NativeBitmap imageGray = image.GetChannel (BitmapColorChannel.Grayscale);
			NativeBitmap imageCmyk = image.ConvertToCmyk32 ();

			Assert.AreEqual (BitmapColorType.Rgb, image.ColorType);
			Assert.AreEqual (BitmapColorType.MinIsBlack, imageR.ColorType);
			Assert.AreEqual (BitmapColorType.MinIsBlack, imageG.ColorType);
			Assert.AreEqual (BitmapColorType.MinIsBlack, imageB.ColorType);
			Assert.AreEqual (BitmapColorType.MinIsBlack, imageGray.ColorType);
			Assert.AreEqual (BitmapColorType.Cmyk, imageCmyk.ColorType);

			Assert.AreEqual (32, image.BitsPerPixel);
			Assert.AreEqual (8, imageR.BitsPerPixel);
			Assert.AreEqual (8, imageG.BitsPerPixel);
			Assert.AreEqual (8, imageB.BitsPerPixel);
			Assert.AreEqual (8, imageGray.BitsPerPixel);
			Assert.AreEqual (32, imageCmyk.BitsPerPixel);

			System.IO.File.WriteAllBytes ("rgb-R.png", imageR.SaveToMemory (new BitmapFileFormat () { Type = BitmapFileType.Png }));
			System.IO.File.WriteAllBytes ("rgb-G.png",    imageG.SaveToMemory (new BitmapFileFormat () { Type = BitmapFileType.Png }));
			System.IO.File.WriteAllBytes ("rgb-B.png",    imageB.SaveToMemory (new BitmapFileFormat () { Type = BitmapFileType.Png }));
			System.IO.File.WriteAllBytes ("rgb-gray.png", imageGray.SaveToMemory (new BitmapFileFormat () { Type = BitmapFileType.Png }));
			System.IO.File.WriteAllBytes ("rgb-cmyk.tif", imageCmyk.SaveToMemory (new BitmapFileFormat () { Type = BitmapFileType.Tiff, TiffCompression = TiffCompressionOption.Lzw, TiffCmyk = true }));

			image.Dispose ();
			imageR.Dispose ();
			imageG.Dispose ();
			imageB.Dispose ();
			imageGray.Dispose ();
			imageCmyk.Dispose ();
		}

		[Test]
		public void TestRgbAlphaTiff()
		{
			NativeBitmap image = NativeBitmap.Load (@"..\..\images\rgba.tif");

			NativeBitmap imageR = image.GetChannel (BitmapColorChannel.Red);
			NativeBitmap imageG = image.GetChannel (BitmapColorChannel.Green);
			NativeBitmap imageB = image.GetChannel (BitmapColorChannel.Blue);
			NativeBitmap imageA = image.GetChannel (BitmapColorChannel.Alpha);
			
			Assert.AreEqual (BitmapColorType.RgbAlpha, image.ColorType);
			Assert.AreEqual (BitmapColorType.MinIsBlack, imageR.ColorType);
			Assert.AreEqual (BitmapColorType.MinIsBlack, imageG.ColorType);
			Assert.AreEqual (BitmapColorType.MinIsBlack, imageB.ColorType);
			Assert.AreEqual (BitmapColorType.MinIsBlack, imageA.ColorType);

			Assert.AreEqual (32, image.BitsPerPixel);
			Assert.AreEqual (8, imageR.BitsPerPixel);
			Assert.AreEqual (8, imageG.BitsPerPixel);
			Assert.AreEqual (8, imageB.BitsPerPixel);
			Assert.AreEqual (8, imageA.BitsPerPixel);

			System.IO.File.WriteAllBytes ("rgba-R.png", imageR.SaveToMemory (new BitmapFileFormat () { Type = BitmapFileType.Png }));
			System.IO.File.WriteAllBytes ("rgba-G.png", imageG.SaveToMemory (new BitmapFileFormat () { Type = BitmapFileType.Png }));
			System.IO.File.WriteAllBytes ("rgba-B.png", imageB.SaveToMemory (new BitmapFileFormat () { Type = BitmapFileType.Png }));
			System.IO.File.WriteAllBytes ("rgba-A.png", imageA.SaveToMemory (new BitmapFileFormat () { Type = BitmapFileType.Png }));
			
			byte[] memory = image.GetChannel (BitmapColorChannel.Alpha).GetRawImageDataInCompactFormFor8BitImage ();

			Assert.AreEqual (128*128, memory.Length);

			image.Dispose ();
			imageR.Dispose ();
			imageG.Dispose ();
			imageB.Dispose ();
			imageA.Dispose ();
		}

		[Test]
		public void TestCmykTiff()
		{
			NativeBitmap image = NativeBitmap.Load (@"..\..\images\cmyk.tif");

			NativeBitmap imageC = image.GetChannel (BitmapColorChannel.Cyan);
			NativeBitmap imageM = image.GetChannel (BitmapColorChannel.Magenta);
			NativeBitmap imageY = image.GetChannel (BitmapColorChannel.Yellow);
			NativeBitmap imageK = image.GetChannel (BitmapColorChannel.Black);
			NativeBitmap imageGray = image.GetChannel (BitmapColorChannel.Grayscale);
			NativeBitmap imageRgb32 = image.ConvertToArgb32 ();
			NativeBitmap imageRgb24 = image.ConvertToRgb24 ();

			Assert.AreEqual (BitmapColorType.Cmyk,       image.ColorType);
			Assert.AreEqual (BitmapColorType.MinIsBlack, imageC.ColorType);
			Assert.AreEqual (BitmapColorType.MinIsBlack, imageM.ColorType);
			Assert.AreEqual (BitmapColorType.MinIsBlack, imageY.ColorType);
			Assert.AreEqual (BitmapColorType.MinIsBlack, imageK.ColorType);
			Assert.AreEqual (BitmapColorType.MinIsBlack, imageGray.ColorType);
			Assert.AreEqual (BitmapColorType.RgbAlpha,   imageRgb32.ColorType);
			Assert.AreEqual (BitmapColorType.Rgb,        imageRgb24.ColorType);

			Assert.AreEqual (32, image.BitsPerPixel);
			Assert.AreEqual (8, imageC.BitsPerPixel);
			Assert.AreEqual (8, imageM.BitsPerPixel);
			Assert.AreEqual (8, imageY.BitsPerPixel);
			Assert.AreEqual (8, imageK.BitsPerPixel);
			Assert.AreEqual (8, imageGray.BitsPerPixel);
			Assert.AreEqual (32, imageRgb32.BitsPerPixel);
			Assert.AreEqual (24, imageRgb24.BitsPerPixel);

			System.IO.File.WriteAllBytes ("cmyk-C.png", imageC.SaveToMemory (new BitmapFileFormat () { Type = BitmapFileType.Png }));
			System.IO.File.WriteAllBytes ("cmyk-M.png", imageM.SaveToMemory (new BitmapFileFormat () { Type = BitmapFileType.Png }));
			System.IO.File.WriteAllBytes ("cmyk-Y.png", imageY.SaveToMemory (new BitmapFileFormat () { Type = BitmapFileType.Png }));
			System.IO.File.WriteAllBytes ("cmyk-K.png", imageK.SaveToMemory (new BitmapFileFormat () { Type = BitmapFileType.Png }));
			System.IO.File.WriteAllBytes ("cmyk-G.tif", imageGray.SaveToMemory (new BitmapFileFormat () { Type = BitmapFileType.Png }));
			System.IO.File.WriteAllBytes ("cmyk-RGB32.tif", imageRgb32.SaveToMemory (new BitmapFileFormat () { Type = BitmapFileType.Png }));
			System.IO.File.WriteAllBytes ("cmyk-RGB24.tif", imageRgb24.SaveToMemory (new BitmapFileFormat () { Type = BitmapFileType.Png }));

			image.Dispose ();
			imageC.Dispose ();
			imageM.Dispose ();
			imageY.Dispose ();
			imageK.Dispose ();
			imageGray.Dispose ();
			imageRgb32.Dispose ();
			imageRgb24.Dispose ();
		}

		[Test]
		public void TestCmykAlphaTiff()
		{
			NativeBitmap image = NativeBitmap.Load (@"..\..\images\cmyka.tif");

			NativeBitmap imageC = image.GetChannel (BitmapColorChannel.Cyan);
			NativeBitmap imageM = image.GetChannel (BitmapColorChannel.Magenta);
			NativeBitmap imageY = image.GetChannel (BitmapColorChannel.Yellow);
			NativeBitmap imageK = image.GetChannel (BitmapColorChannel.Black);
			NativeBitmap imageA = image.GetChannel (BitmapColorChannel.Alpha);
			NativeBitmap imageGray = image.GetChannel (BitmapColorChannel.Grayscale);
			NativeBitmap imageRgb = image.ConvertToArgb32 ();

			Assert.AreEqual (BitmapColorType.Cmyk, image.ColorType);
			Assert.AreEqual (BitmapColorType.MinIsBlack, imageC.ColorType);
			Assert.AreEqual (BitmapColorType.MinIsBlack, imageM.ColorType);
			Assert.AreEqual (BitmapColorType.MinIsBlack, imageY.ColorType);
			Assert.AreEqual (BitmapColorType.MinIsBlack, imageK.ColorType);
			Assert.AreEqual (BitmapColorType.MinIsBlack, imageGray.ColorType);
			Assert.AreEqual (BitmapColorType.MinIsBlack, imageA.ColorType);
			Assert.AreEqual (BitmapColorType.Rgb, imageRgb.ColorType);

			Assert.AreEqual (32, image.BitsPerPixel);
			Assert.AreEqual (8, imageC.BitsPerPixel);
			Assert.AreEqual (8, imageM.BitsPerPixel);
			Assert.AreEqual (8, imageY.BitsPerPixel);
			Assert.AreEqual (8, imageK.BitsPerPixel);
			Assert.AreEqual (8, imageGray.BitsPerPixel);
			Assert.AreEqual (8, imageA.BitsPerPixel);
			Assert.AreEqual (24, imageRgb.BitsPerPixel);

			System.IO.File.WriteAllBytes ("cmyka-C.png", imageC.SaveToMemory (new BitmapFileFormat () { Type = BitmapFileType.Png }));
			System.IO.File.WriteAllBytes ("cmyka-M.png", imageM.SaveToMemory (new BitmapFileFormat () { Type = BitmapFileType.Png }));
			System.IO.File.WriteAllBytes ("cmyka-Y.png", imageY.SaveToMemory (new BitmapFileFormat () { Type = BitmapFileType.Png }));
			System.IO.File.WriteAllBytes ("cmyka-K.png", imageK.SaveToMemory (new BitmapFileFormat () { Type = BitmapFileType.Png }));
			System.IO.File.WriteAllBytes ("cmyka-G.png", imageGray.SaveToMemory (new BitmapFileFormat () { Type = BitmapFileType.Png }));
			System.IO.File.WriteAllBytes ("cmyka-RGB.png", imageRgb.SaveToMemory (new BitmapFileFormat () { Type = BitmapFileType.Png }));
			System.IO.File.WriteAllBytes ("cmyka-A.jpg", imageA.SaveToMemory (new BitmapFileFormat () { Type = BitmapFileType.Jpeg, Quality = 80 }));

			image.Dispose ();
			imageC.Dispose ();
			imageM.Dispose ();
			imageY.Dispose ();
			imageK.Dispose ();
			imageA.Dispose ();
			imageGray.Dispose ();
			imageRgb.Dispose ();
		}
	}
}
