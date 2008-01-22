using NUnit.Framework;

namespace Epsitec.Common.Drawing
{
	using ColorChannel=Opac.FreeImage.ColorChannel;
	using FileFormat=Opac.FreeImage.FileFormat;
	using SaveMode=Opac.FreeImage.LoadSaveMode;
	[TestFixture]
	public class FreeImageTest
	{
		[Test]
		public void TestRgbTiff()
		{
			Opac.FreeImage.Image image = Opac.FreeImage.Image.Load (@"..\..\images\rgb.tif");

			Opac.FreeImage.Image imageR    = image.GetChannel (ColorChannel.Red);
			Opac.FreeImage.Image imageG    = image.GetChannel (ColorChannel.Green);
			Opac.FreeImage.Image imageB    = image.GetChannel (ColorChannel.Blue);
			Opac.FreeImage.Image imageGray = image.GetChannel (ColorChannel.Grayscale);
			Opac.FreeImage.Image imageCmyk = image.ConvertToCmyk ();

			Assert.AreEqual (Opac.FreeImage.ColorType.Rgb, image.GetColorType ());
			Assert.AreEqual (Opac.FreeImage.ColorType.MinIsBlack, imageR.GetColorType ());
			Assert.AreEqual (Opac.FreeImage.ColorType.MinIsBlack, imageG.GetColorType ());
			Assert.AreEqual (Opac.FreeImage.ColorType.MinIsBlack, imageB.GetColorType ());
			Assert.AreEqual (Opac.FreeImage.ColorType.MinIsBlack, imageGray.GetColorType ());
			Assert.AreEqual (Opac.FreeImage.ColorType.Cmyk, imageCmyk.GetColorType ());

			Assert.AreEqual (24, image.GetBitsPerPixel ());
			Assert.AreEqual ( 8, imageR.GetBitsPerPixel ());
			Assert.AreEqual ( 8, imageG.GetBitsPerPixel ());
			Assert.AreEqual ( 8, imageB.GetBitsPerPixel ());
			Assert.AreEqual ( 8, imageGray.GetBitsPerPixel ());
			Assert.AreEqual (32, imageCmyk.GetBitsPerPixel ());

			System.IO.File.WriteAllBytes ("rgb-R.png",    imageR.SaveToMemory (FileFormat.Png));
			System.IO.File.WriteAllBytes ("rgb-G.png",    imageG.SaveToMemory (FileFormat.Png));
			System.IO.File.WriteAllBytes ("rgb-B.png",    imageB.SaveToMemory (FileFormat.Png));
			System.IO.File.WriteAllBytes ("rgb-gray.png", imageGray.SaveToMemory (FileFormat.Png));
			System.IO.File.WriteAllBytes ("rgb-cmyk.tif", imageCmyk.SaveToMemory (FileFormat.Tiff, SaveMode.TiffCmyk));

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
			Opac.FreeImage.Image image = Opac.FreeImage.Image.Load (@"..\..\images\rgba.tif");

			Opac.FreeImage.Image imageR = image.GetChannel (ColorChannel.Red);
			Opac.FreeImage.Image imageG = image.GetChannel (ColorChannel.Green);
			Opac.FreeImage.Image imageB = image.GetChannel (ColorChannel.Blue);
			Opac.FreeImage.Image imageA = image.GetChannel (ColorChannel.Alpha);
			Opac.FreeImage.Image imageT = image.GetChannel (ColorChannel.Transparency);
			
			Assert.AreEqual (Opac.FreeImage.ColorType.RgbAlpha, image.GetColorType ());
			Assert.AreEqual (Opac.FreeImage.ColorType.MinIsBlack, imageR.GetColorType ());
			Assert.AreEqual (Opac.FreeImage.ColorType.MinIsBlack, imageG.GetColorType ());
			Assert.AreEqual (Opac.FreeImage.ColorType.MinIsBlack, imageB.GetColorType ());
			Assert.AreEqual (Opac.FreeImage.ColorType.MinIsBlack, imageA.GetColorType ());
			Assert.AreEqual (Opac.FreeImage.ColorType.MinIsWhite, imageT.GetColorType ());

			Assert.AreEqual (32, image.GetBitsPerPixel ());
			Assert.AreEqual ( 8, imageR.GetBitsPerPixel ());
			Assert.AreEqual ( 8, imageG.GetBitsPerPixel ());
			Assert.AreEqual ( 8, imageB.GetBitsPerPixel ());
			Assert.AreEqual ( 8, imageA.GetBitsPerPixel ());
			Assert.AreEqual ( 8, imageT.GetBitsPerPixel ());

			System.IO.File.WriteAllBytes ("rgba-R.png", imageR.SaveToMemory (FileFormat.Png));
			System.IO.File.WriteAllBytes ("rgba-G.png", imageG.SaveToMemory (FileFormat.Png));
			System.IO.File.WriteAllBytes ("rgba-B.png", imageB.SaveToMemory (FileFormat.Png));
			System.IO.File.WriteAllBytes ("rgba-A.png", imageA.SaveToMemory (FileFormat.Png));
			System.IO.File.WriteAllBytes ("rgba-T.png", imageT.SaveToMemory (FileFormat.Png));
			
			byte[] memory = image.GetChannel (ColorChannel.Alpha).GetRawImageSource8Bits (true);

			Assert.AreEqual (128*128, memory.Length);

			image.Dispose ();
			imageR.Dispose ();
			imageG.Dispose ();
			imageB.Dispose ();
			imageA.Dispose ();
			imageT.Dispose ();
		}

		[Test]
		public void TestCmykTiff()
		{
			Opac.FreeImage.Image image = Opac.FreeImage.Image.Load (@"..\..\images\cmyk.tif");

			Opac.FreeImage.Image imageC = image.GetChannel (ColorChannel.Cyan);
			Opac.FreeImage.Image imageM = image.GetChannel (ColorChannel.Magenta);
			Opac.FreeImage.Image imageY = image.GetChannel (ColorChannel.Yellow);
			Opac.FreeImage.Image imageK = image.GetChannel (ColorChannel.Black);
			Opac.FreeImage.Image imageGray = image.GetChannel (ColorChannel.Grayscale);
			Opac.FreeImage.Image imageRgb = image.ConvertToRgb ();

			Assert.AreEqual (Opac.FreeImage.ColorType.Cmyk, image.GetColorType ());
			Assert.AreEqual (Opac.FreeImage.ColorType.MinIsBlack, imageC.GetColorType ());
			Assert.AreEqual (Opac.FreeImage.ColorType.MinIsBlack, imageM.GetColorType ());
			Assert.AreEqual (Opac.FreeImage.ColorType.MinIsBlack, imageY.GetColorType ());
			Assert.AreEqual (Opac.FreeImage.ColorType.MinIsBlack, imageK.GetColorType ());
			Assert.AreEqual (Opac.FreeImage.ColorType.MinIsBlack, imageGray.GetColorType ());
			Assert.AreEqual (Opac.FreeImage.ColorType.Rgb, imageRgb.GetColorType ());
			
			Assert.AreEqual (32, image.GetBitsPerPixel ());
			Assert.AreEqual ( 8, imageC.GetBitsPerPixel ());
			Assert.AreEqual ( 8, imageM.GetBitsPerPixel ());
			Assert.AreEqual ( 8, imageY.GetBitsPerPixel ());
			Assert.AreEqual ( 8, imageK.GetBitsPerPixel ());
			Assert.AreEqual ( 8, imageGray.GetBitsPerPixel ());
			Assert.AreEqual (24, imageRgb.GetBitsPerPixel ());

			System.IO.File.WriteAllBytes ("cmyk-C.png", imageC.SaveToMemory (FileFormat.Png));
			System.IO.File.WriteAllBytes ("cmyk-M.png", imageM.SaveToMemory (FileFormat.Png));
			System.IO.File.WriteAllBytes ("cmyk-Y.png", imageY.SaveToMemory (FileFormat.Png));
			System.IO.File.WriteAllBytes ("cmyk-K.png", imageK.SaveToMemory (FileFormat.Png));
			System.IO.File.WriteAllBytes ("cmyk-G.tif", imageGray.SaveToMemory (FileFormat.Png));
			System.IO.File.WriteAllBytes ("cmyk-RGB.tif", imageRgb.SaveToMemory (FileFormat.Png));

			image.Dispose ();
			imageC.Dispose ();
			imageM.Dispose ();
			imageY.Dispose ();
			imageK.Dispose ();
			imageGray.Dispose ();
			imageRgb.Dispose ();
		}

		[Test]
		public void TestCmykAlphaTiff()
		{
			Opac.FreeImage.Image image = Opac.FreeImage.Image.Load (@"..\..\images\cmyka.tif");

			Opac.FreeImage.Image imageC = image.GetChannel (ColorChannel.Cyan);
			Opac.FreeImage.Image imageM = image.GetChannel (ColorChannel.Magenta);
			Opac.FreeImage.Image imageY = image.GetChannel (ColorChannel.Yellow);
			Opac.FreeImage.Image imageK = image.GetChannel (ColorChannel.Black);
			Opac.FreeImage.Image imageA = image.GetChannel (ColorChannel.Alpha);
			Opac.FreeImage.Image imageT = image.GetChannel (ColorChannel.Transparency);
			Opac.FreeImage.Image imageGray = image.GetChannel (ColorChannel.Grayscale);
			Opac.FreeImage.Image imageRgb = image.ConvertToRgb ();

			Assert.AreEqual (Opac.FreeImage.ColorType.Cmyk, image.GetColorType ());
			Assert.AreEqual (Opac.FreeImage.ColorType.MinIsBlack, imageC.GetColorType ());
			Assert.AreEqual (Opac.FreeImage.ColorType.MinIsBlack, imageM.GetColorType ());
			Assert.AreEqual (Opac.FreeImage.ColorType.MinIsBlack, imageY.GetColorType ());
			Assert.AreEqual (Opac.FreeImage.ColorType.MinIsBlack, imageK.GetColorType ());
			Assert.AreEqual (Opac.FreeImage.ColorType.MinIsBlack, imageGray.GetColorType ());
			Assert.AreEqual (Opac.FreeImage.ColorType.MinIsBlack, imageA.GetColorType ());
			Assert.AreEqual (Opac.FreeImage.ColorType.MinIsWhite, imageT.GetColorType ());
			Assert.AreEqual (Opac.FreeImage.ColorType.Rgb, imageRgb.GetColorType ());

			Assert.AreEqual (32, image.GetBitsPerPixel ());
			Assert.AreEqual ( 8, imageC.GetBitsPerPixel ());
			Assert.AreEqual ( 8, imageM.GetBitsPerPixel ());
			Assert.AreEqual ( 8, imageY.GetBitsPerPixel ());
			Assert.AreEqual ( 8, imageK.GetBitsPerPixel ());
			Assert.AreEqual ( 8, imageGray.GetBitsPerPixel ());
			Assert.AreEqual ( 8, imageA.GetBitsPerPixel ());
			Assert.AreEqual ( 8, imageT.GetBitsPerPixel ());
			Assert.AreEqual (24, imageRgb.GetBitsPerPixel ());

			System.IO.File.WriteAllBytes ("cmyka-C.png", imageC.SaveToMemory (FileFormat.Png));
			System.IO.File.WriteAllBytes ("cmyka-M.png", imageM.SaveToMemory (FileFormat.Png));
			System.IO.File.WriteAllBytes ("cmyka-Y.png", imageY.SaveToMemory (FileFormat.Png));
			System.IO.File.WriteAllBytes ("cmyka-K.png", imageK.SaveToMemory (FileFormat.Png));
			System.IO.File.WriteAllBytes ("cmyka-G.png", imageGray.SaveToMemory (FileFormat.Png));
			System.IO.File.WriteAllBytes ("cmyka-RGB.png", imageRgb.SaveToMemory (FileFormat.Png));
			System.IO.File.WriteAllBytes ("cmyka-A.jpg", imageA.SaveToMemory (FileFormat.Jpeg, SaveMode.JpegQualityGood));
			System.IO.File.WriteAllBytes ("cmyka-T.jpg", imageT.SaveToMemory (FileFormat.Jpeg, SaveMode.JpegQualityGood));

			image.Dispose ();
			imageC.Dispose ();
			imageM.Dispose ();
			imageY.Dispose ();
			imageK.Dispose ();
			imageA.Dispose ();
			imageT.Dispose ();
			imageGray.Dispose ();
			imageRgb.Dispose ();
		}
	}
}
