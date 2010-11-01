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

			NativeBitmap imageR    = image.GetChannel (ColorChannel.Red);
			NativeBitmap imageG    = image.GetChannel (ColorChannel.Green);
			NativeBitmap imageB    = image.GetChannel (ColorChannel.Blue);
			NativeBitmap imageGray = image.GetChannel (ColorChannel.Grayscale);
			NativeBitmap imageCmyk = image.ConvertToCmyk32 ();

			Assert.AreEqual (ColorType.Rgb, image.ColorType);
			Assert.AreEqual (ColorType.MinIsBlack, imageR.ColorType);
			Assert.AreEqual (ColorType.MinIsBlack, imageG.ColorType);
			Assert.AreEqual (ColorType.MinIsBlack, imageB.ColorType);
			Assert.AreEqual (ColorType.MinIsBlack, imageGray.ColorType);
			Assert.AreEqual (ColorType.Cmyk, imageCmyk.ColorType);

			Assert.AreEqual (32, image.BitsPerPixel);
			Assert.AreEqual (8, imageR.BitsPerPixel);
			Assert.AreEqual (8, imageG.BitsPerPixel);
			Assert.AreEqual (8, imageB.BitsPerPixel);
			Assert.AreEqual (8, imageGray.BitsPerPixel);
			Assert.AreEqual (32, imageCmyk.BitsPerPixel);

			System.IO.File.WriteAllBytes ("rgb-R.png", imageR.SaveToMemory (new FileFormat () { Type = FileFormatType.Png }));
			System.IO.File.WriteAllBytes ("rgb-G.png",    imageG.SaveToMemory (new FileFormat () { Type = FileFormatType.Png }));
			System.IO.File.WriteAllBytes ("rgb-B.png",    imageB.SaveToMemory (new FileFormat () { Type = FileFormatType.Png }));
			System.IO.File.WriteAllBytes ("rgb-gray.png", imageGray.SaveToMemory (new FileFormat () { Type = FileFormatType.Png }));
			System.IO.File.WriteAllBytes ("rgb-cmyk.tif", imageCmyk.SaveToMemory (new FileFormat () { Type = FileFormatType.Tiff, TiffCompression = TiffCompressionOption.Lzw, TiffCmyk = true }));

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

			NativeBitmap imageR = image.GetChannel (ColorChannel.Red);
			NativeBitmap imageG = image.GetChannel (ColorChannel.Green);
			NativeBitmap imageB = image.GetChannel (ColorChannel.Blue);
			NativeBitmap imageA = image.GetChannel (ColorChannel.Alpha);
			
			Assert.AreEqual (ColorType.RgbAlpha, image.ColorType);
			Assert.AreEqual (ColorType.MinIsBlack, imageR.ColorType);
			Assert.AreEqual (ColorType.MinIsBlack, imageG.ColorType);
			Assert.AreEqual (ColorType.MinIsBlack, imageB.ColorType);
			Assert.AreEqual (ColorType.MinIsBlack, imageA.ColorType);

			Assert.AreEqual (32, image.BitsPerPixel);
			Assert.AreEqual (8, imageR.BitsPerPixel);
			Assert.AreEqual (8, imageG.BitsPerPixel);
			Assert.AreEqual (8, imageB.BitsPerPixel);
			Assert.AreEqual (8, imageA.BitsPerPixel);

			System.IO.File.WriteAllBytes ("rgba-R.png", imageR.SaveToMemory (new FileFormat () { Type = FileFormatType.Png }));
			System.IO.File.WriteAllBytes ("rgba-G.png", imageG.SaveToMemory (new FileFormat () { Type = FileFormatType.Png }));
			System.IO.File.WriteAllBytes ("rgba-B.png", imageB.SaveToMemory (new FileFormat () { Type = FileFormatType.Png }));
			System.IO.File.WriteAllBytes ("rgba-A.png", imageA.SaveToMemory (new FileFormat () { Type = FileFormatType.Png }));
			
			byte[] memory = image.GetChannel (ColorChannel.Alpha).GetRawImageDataInCompactFormFor8BitImage ();

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

			NativeBitmap imageC = image.GetChannel (ColorChannel.Cyan);
			NativeBitmap imageM = image.GetChannel (ColorChannel.Magenta);
			NativeBitmap imageY = image.GetChannel (ColorChannel.Yellow);
			NativeBitmap imageK = image.GetChannel (ColorChannel.Black);
			NativeBitmap imageGray = image.GetChannel (ColorChannel.Grayscale);
			NativeBitmap imageRgb32 = image.ConvertToRgb32 ();
			NativeBitmap imageRgb24 = image.ConvertTo24Bits ();

			Assert.AreEqual (ColorType.Cmyk,       image.ColorType);
			Assert.AreEqual (ColorType.MinIsBlack, imageC.ColorType);
			Assert.AreEqual (ColorType.MinIsBlack, imageM.ColorType);
			Assert.AreEqual (ColorType.MinIsBlack, imageY.ColorType);
			Assert.AreEqual (ColorType.MinIsBlack, imageK.ColorType);
			Assert.AreEqual (ColorType.MinIsBlack, imageGray.ColorType);
			Assert.AreEqual (ColorType.RgbAlpha,   imageRgb32.ColorType);
			Assert.AreEqual (ColorType.Rgb,        imageRgb24.ColorType);

			Assert.AreEqual (32, image.BitsPerPixel);
			Assert.AreEqual (8, imageC.BitsPerPixel);
			Assert.AreEqual (8, imageM.BitsPerPixel);
			Assert.AreEqual (8, imageY.BitsPerPixel);
			Assert.AreEqual (8, imageK.BitsPerPixel);
			Assert.AreEqual (8, imageGray.BitsPerPixel);
			Assert.AreEqual (32, imageRgb32.BitsPerPixel);
			Assert.AreEqual (24, imageRgb24.BitsPerPixel);

			System.IO.File.WriteAllBytes ("cmyk-C.png", imageC.SaveToMemory (new FileFormat () { Type = FileFormatType.Png }));
			System.IO.File.WriteAllBytes ("cmyk-M.png", imageM.SaveToMemory (new FileFormat () { Type = FileFormatType.Png }));
			System.IO.File.WriteAllBytes ("cmyk-Y.png", imageY.SaveToMemory (new FileFormat () { Type = FileFormatType.Png }));
			System.IO.File.WriteAllBytes ("cmyk-K.png", imageK.SaveToMemory (new FileFormat () { Type = FileFormatType.Png }));
			System.IO.File.WriteAllBytes ("cmyk-G.tif", imageGray.SaveToMemory (new FileFormat () { Type = FileFormatType.Png }));
			System.IO.File.WriteAllBytes ("cmyk-RGB32.tif", imageRgb32.SaveToMemory (new FileFormat () { Type = FileFormatType.Png }));
			System.IO.File.WriteAllBytes ("cmyk-RGB24.tif", imageRgb24.SaveToMemory (new FileFormat () { Type = FileFormatType.Png }));

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

			NativeBitmap imageC = image.GetChannel (ColorChannel.Cyan);
			NativeBitmap imageM = image.GetChannel (ColorChannel.Magenta);
			NativeBitmap imageY = image.GetChannel (ColorChannel.Yellow);
			NativeBitmap imageK = image.GetChannel (ColorChannel.Black);
			NativeBitmap imageA = image.GetChannel (ColorChannel.Alpha);
			NativeBitmap imageGray = image.GetChannel (ColorChannel.Grayscale);
			NativeBitmap imageRgb = image.ConvertToRgb32 ();

			Assert.AreEqual (ColorType.Cmyk, image.ColorType);
			Assert.AreEqual (ColorType.MinIsBlack, imageC.ColorType);
			Assert.AreEqual (ColorType.MinIsBlack, imageM.ColorType);
			Assert.AreEqual (ColorType.MinIsBlack, imageY.ColorType);
			Assert.AreEqual (ColorType.MinIsBlack, imageK.ColorType);
			Assert.AreEqual (ColorType.MinIsBlack, imageGray.ColorType);
			Assert.AreEqual (ColorType.MinIsBlack, imageA.ColorType);
			Assert.AreEqual (ColorType.Rgb, imageRgb.ColorType);

			Assert.AreEqual (32, image.BitsPerPixel);
			Assert.AreEqual (8, imageC.BitsPerPixel);
			Assert.AreEqual (8, imageM.BitsPerPixel);
			Assert.AreEqual (8, imageY.BitsPerPixel);
			Assert.AreEqual (8, imageK.BitsPerPixel);
			Assert.AreEqual (8, imageGray.BitsPerPixel);
			Assert.AreEqual (8, imageA.BitsPerPixel);
			Assert.AreEqual (24, imageRgb.BitsPerPixel);

			System.IO.File.WriteAllBytes ("cmyka-C.png", imageC.SaveToMemory (new FileFormat () { Type = FileFormatType.Png }));
			System.IO.File.WriteAllBytes ("cmyka-M.png", imageM.SaveToMemory (new FileFormat () { Type = FileFormatType.Png }));
			System.IO.File.WriteAllBytes ("cmyka-Y.png", imageY.SaveToMemory (new FileFormat () { Type = FileFormatType.Png }));
			System.IO.File.WriteAllBytes ("cmyka-K.png", imageK.SaveToMemory (new FileFormat () { Type = FileFormatType.Png }));
			System.IO.File.WriteAllBytes ("cmyka-G.png", imageGray.SaveToMemory (new FileFormat () { Type = FileFormatType.Png }));
			System.IO.File.WriteAllBytes ("cmyka-RGB.png", imageRgb.SaveToMemory (new FileFormat () { Type = FileFormatType.Png }));
			System.IO.File.WriteAllBytes ("cmyka-A.jpg", imageA.SaveToMemory (new FileFormat () { Type = FileFormatType.Jpeg, Quality = 80 }));

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
