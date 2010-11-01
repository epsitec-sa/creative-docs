using NUnit.Framework;

namespace Epsitec.Common.Drawing
{
#if false
	using ColorChannel=Opac.FreeImage.ColorChannel;
	using FileFormat=Opac.FreeImage.FileFormat;
	using SaveMode=Opac.FreeImage.LoadSaveMode;
#endif
	[TestFixture]
	public class FreeImageTest
	{
#if false
		[Test]
		public void TestRgbTiff()
		{
			Opac.FreeImage.ImageClient image = Opac.FreeImage.ImageClient.Load (@"..\..\images\rgb.tif");

			Opac.FreeImage.ImageClient imageR    = image.GetChannel (ColorChannel.Red);
			Opac.FreeImage.ImageClient imageG    = image.GetChannel (ColorChannel.Green);
			Opac.FreeImage.ImageClient imageB    = image.GetChannel (ColorChannel.Blue);
			Opac.FreeImage.ImageClient imageGray = image.GetChannel (ColorChannel.Grayscale);
			Opac.FreeImage.ImageClient imageCmyk = image.ConvertToCmyk ();

			Assert.AreEqual (Opac.FreeImage.ColorType.Rgb, image.ColorType);
			Assert.AreEqual (Opac.FreeImage.ColorType.MinIsBlack, imageR.ColorType);
			Assert.AreEqual (Opac.FreeImage.ColorType.MinIsBlack, imageG.ColorType);
			Assert.AreEqual (Opac.FreeImage.ColorType.MinIsBlack, imageB.ColorType);
			Assert.AreEqual (Opac.FreeImage.ColorType.MinIsBlack, imageGray.ColorType);
			Assert.AreEqual (Opac.FreeImage.ColorType.Cmyk, imageCmyk.ColorType);

			Assert.AreEqual (24, image.BitsPerPixel);
			Assert.AreEqual (8, imageR.BitsPerPixel);
			Assert.AreEqual (8, imageG.BitsPerPixel);
			Assert.AreEqual (8, imageB.BitsPerPixel);
			Assert.AreEqual (8, imageGray.BitsPerPixel);
			Assert.AreEqual (32, imageCmyk.BitsPerPixel);

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
			Opac.FreeImage.ImageClient image = Opac.FreeImage.ImageClient.Load (@"..\..\images\rgba.tif");

			Opac.FreeImage.ImageClient imageR = image.GetChannel (ColorChannel.Red);
			Opac.FreeImage.ImageClient imageG = image.GetChannel (ColorChannel.Green);
			Opac.FreeImage.ImageClient imageB = image.GetChannel (ColorChannel.Blue);
			Opac.FreeImage.ImageClient imageA = image.GetChannel (ColorChannel.Alpha);
			Opac.FreeImage.ImageClient imageT = image.GetChannel (ColorChannel.Transparency);
			
			Assert.AreEqual (Opac.FreeImage.ColorType.RgbAlpha, image.ColorType);
			Assert.AreEqual (Opac.FreeImage.ColorType.MinIsBlack, imageR.ColorType);
			Assert.AreEqual (Opac.FreeImage.ColorType.MinIsBlack, imageG.ColorType);
			Assert.AreEqual (Opac.FreeImage.ColorType.MinIsBlack, imageB.ColorType);
			Assert.AreEqual (Opac.FreeImage.ColorType.MinIsBlack, imageA.ColorType);
			Assert.AreEqual (Opac.FreeImage.ColorType.MinIsWhite, imageT.ColorType);

			Assert.AreEqual (32, image.BitsPerPixel);
			Assert.AreEqual (8, imageR.BitsPerPixel);
			Assert.AreEqual (8, imageG.BitsPerPixel);
			Assert.AreEqual (8, imageB.BitsPerPixel);
			Assert.AreEqual (8, imageA.BitsPerPixel);
			Assert.AreEqual (8, imageT.BitsPerPixel);

			System.IO.File.WriteAllBytes ("rgba-R.png", imageR.SaveToMemory (FileFormat.Png));
			System.IO.File.WriteAllBytes ("rgba-G.png", imageG.SaveToMemory (FileFormat.Png));
			System.IO.File.WriteAllBytes ("rgba-B.png", imageB.SaveToMemory (FileFormat.Png));
			System.IO.File.WriteAllBytes ("rgba-A.png", imageA.SaveToMemory (FileFormat.Png));
			System.IO.File.WriteAllBytes ("rgba-T.png", imageT.SaveToMemory (FileFormat.Png));
			
			byte[] memory = image.GetChannel (ColorChannel.Alpha).GetRawImageDataInCompactFormFor8BitImage (true);

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
			Opac.FreeImage.ImageClient image = Opac.FreeImage.ImageClient.Load (@"..\..\images\cmyk.tif");

			Opac.FreeImage.ImageClient imageC = image.GetChannel (ColorChannel.Cyan);
			Opac.FreeImage.ImageClient imageM = image.GetChannel (ColorChannel.Magenta);
			Opac.FreeImage.ImageClient imageY = image.GetChannel (ColorChannel.Yellow);
			Opac.FreeImage.ImageClient imageK = image.GetChannel (ColorChannel.Black);
			Opac.FreeImage.ImageClient imageGray = image.GetChannel (ColorChannel.Grayscale);
			Opac.FreeImage.ImageClient imageRgb = image.ConvertToRgb ();

			Assert.AreEqual (Opac.FreeImage.ColorType.Cmyk, image.ColorType);
			Assert.AreEqual (Opac.FreeImage.ColorType.MinIsBlack, imageC.ColorType);
			Assert.AreEqual (Opac.FreeImage.ColorType.MinIsBlack, imageM.ColorType);
			Assert.AreEqual (Opac.FreeImage.ColorType.MinIsBlack, imageY.ColorType);
			Assert.AreEqual (Opac.FreeImage.ColorType.MinIsBlack, imageK.ColorType);
			Assert.AreEqual (Opac.FreeImage.ColorType.MinIsBlack, imageGray.ColorType);
			Assert.AreEqual (Opac.FreeImage.ColorType.Rgb, imageRgb.ColorType);

			Assert.AreEqual (32, image.BitsPerPixel);
			Assert.AreEqual (8, imageC.BitsPerPixel);
			Assert.AreEqual (8, imageM.BitsPerPixel);
			Assert.AreEqual (8, imageY.BitsPerPixel);
			Assert.AreEqual (8, imageK.BitsPerPixel);
			Assert.AreEqual (8, imageGray.BitsPerPixel);
			Assert.AreEqual (24, imageRgb.BitsPerPixel);

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
			Opac.FreeImage.ImageClient image = Opac.FreeImage.ImageClient.Load (@"..\..\images\cmyka.tif");

			Opac.FreeImage.ImageClient imageC = image.GetChannel (ColorChannel.Cyan);
			Opac.FreeImage.ImageClient imageM = image.GetChannel (ColorChannel.Magenta);
			Opac.FreeImage.ImageClient imageY = image.GetChannel (ColorChannel.Yellow);
			Opac.FreeImage.ImageClient imageK = image.GetChannel (ColorChannel.Black);
			Opac.FreeImage.ImageClient imageA = image.GetChannel (ColorChannel.Alpha);
			Opac.FreeImage.ImageClient imageT = image.GetChannel (ColorChannel.Transparency);
			Opac.FreeImage.ImageClient imageGray = image.GetChannel (ColorChannel.Grayscale);
			Opac.FreeImage.ImageClient imageRgb = image.ConvertToRgb ();

			Assert.AreEqual (Opac.FreeImage.ColorType.Cmyk, image.ColorType);
			Assert.AreEqual (Opac.FreeImage.ColorType.MinIsBlack, imageC.ColorType);
			Assert.AreEqual (Opac.FreeImage.ColorType.MinIsBlack, imageM.ColorType);
			Assert.AreEqual (Opac.FreeImage.ColorType.MinIsBlack, imageY.ColorType);
			Assert.AreEqual (Opac.FreeImage.ColorType.MinIsBlack, imageK.ColorType);
			Assert.AreEqual (Opac.FreeImage.ColorType.MinIsBlack, imageGray.ColorType);
			Assert.AreEqual (Opac.FreeImage.ColorType.MinIsBlack, imageA.ColorType);
			Assert.AreEqual (Opac.FreeImage.ColorType.MinIsWhite, imageT.ColorType);
			Assert.AreEqual (Opac.FreeImage.ColorType.Rgb, imageRgb.ColorType);

			Assert.AreEqual (32, image.BitsPerPixel);
			Assert.AreEqual (8, imageC.BitsPerPixel);
			Assert.AreEqual (8, imageM.BitsPerPixel);
			Assert.AreEqual (8, imageY.BitsPerPixel);
			Assert.AreEqual (8, imageK.BitsPerPixel);
			Assert.AreEqual (8, imageGray.BitsPerPixel);
			Assert.AreEqual (8, imageA.BitsPerPixel);
			Assert.AreEqual (8, imageT.BitsPerPixel);
			Assert.AreEqual (24, imageRgb.BitsPerPixel);

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
#endif
	}
}
