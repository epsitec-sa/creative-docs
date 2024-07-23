/*
This file is part of CreativeDocs.

Copyright © 2003-2024, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland

CreativeDocs is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
any later version.

CreativeDocs is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using NUnit.Framework;

namespace Epsitec.Common.Tests.Drawing
{
    [TestFixture]
    public class BitmapTest
    {
        [SetUp]
        public void Initialize()
        {
            Epsitec.Common.Widgets.Widget.Initialize();
        }

        [Test]
        [Ignore("Disabled (does not build)")]
        public void CheckImageFormat()
        {
            /*
            Assert.IsNotNull(Bitmap.MapToMicrosoftImageFormat(ImageFormat.Bmp));
            Assert.IsNotNull(Bitmap.MapToMicrosoftImageFormat(ImageFormat.Gif));
            Assert.IsNotNull(Bitmap.MapToMicrosoftImageFormat(ImageFormat.Png));
            Assert.IsNotNull(Bitmap.MapToMicrosoftImageFormat(ImageFormat.Tiff));
            Assert.IsNotNull(Bitmap.MapToMicrosoftImageFormat(ImageFormat.Jpeg));
            Assert.IsNotNull(Bitmap.MapToMicrosoftImageFormat(ImageFormat.Exif));
            Assert.IsNotNull(Bitmap.MapToMicrosoftImageFormat(ImageFormat.WindowsIcon));
            Assert.IsNotNull(Bitmap.MapToMicrosoftImageFormat(ImageFormat.WindowsEmf));
            Assert.IsNotNull(Bitmap.MapToMicrosoftImageFormat(ImageFormat.WindowsWmf));
            */
        }

        [Test]
        public void CheckEncoders()
        {
            Assert.IsNotNull(Bitmap.GetFilenameExtensions(ImageFormat.Bmp));
            Assert.IsNotNull(Bitmap.GetFilenameExtensions(ImageFormat.Gif));
            Assert.IsNotNull(Bitmap.GetFilenameExtensions(ImageFormat.Png));
            Assert.IsNotNull(Bitmap.GetFilenameExtensions(ImageFormat.Tiff));
            Assert.IsNotNull(Bitmap.GetFilenameExtensions(ImageFormat.Jpeg));
        }

        [Test]
        [Ignore("Reported broken by Marc Bettex")]
        public void CheckBitmapSave()
        {
            Bitmap bitmap = Common
                .Drawing.Platform.NativeBitmap.FromFile(@"..\..\..\..\Images\picture.png")
                .BitmapImage;

            byte[] data;
            System.IO.FileStream stream;

            data = bitmap.Save(ImageFormat.Bmp, 24, 0, ImageCompression.None);
            stream = new System.IO.FileStream("picture-24.bmp", System.IO.FileMode.CreateNew);
            stream.Write(data, 0, data.Length);
            stream.Close();

            data = bitmap.Save(ImageFormat.Bmp, 32, 0, ImageCompression.None);
            stream = new System.IO.FileStream("picture-32.bmp", System.IO.FileMode.CreateNew);
            stream.Write(data, 0, data.Length);
            stream.Close();

            data = bitmap.Save(ImageFormat.Gif, 0, 0, ImageCompression.None);
            stream = new System.IO.FileStream("picture.gif", System.IO.FileMode.CreateNew);
            stream.Write(data, 0, data.Length);
            stream.Close();

            data = bitmap.Save(ImageFormat.Png, 16, 0, ImageCompression.None);
            stream = new System.IO.FileStream("picture-16.png", System.IO.FileMode.CreateNew);
            stream.Write(data, 0, data.Length);
            stream.Close();

            data = bitmap.Save(ImageFormat.Png, 24, 0, ImageCompression.None);
            stream = new System.IO.FileStream("picture-24.png", System.IO.FileMode.CreateNew);
            stream.Write(data, 0, data.Length);
            stream.Close();

            data = bitmap.Save(ImageFormat.Png, 32, 0, ImageCompression.None);
            stream = new System.IO.FileStream("picture-32.png", System.IO.FileMode.CreateNew);
            stream.Write(data, 0, data.Length);
            stream.Close();

            data = bitmap.Save(ImageFormat.Tiff, 24, 0, ImageCompression.None);
            stream = new System.IO.FileStream("picture-24.tif", System.IO.FileMode.CreateNew);
            stream.Write(data, 0, data.Length);
            stream.Close();

            data = bitmap.Save(ImageFormat.Tiff, 32, 0, ImageCompression.Lzw);
            stream = new System.IO.FileStream("picture-32-lzw.tif", System.IO.FileMode.CreateNew);
            stream.Write(data, 0, data.Length);
            stream.Close();

            data = bitmap.Save(ImageFormat.Jpeg, 24, 25, ImageCompression.None);
            stream = new System.IO.FileStream("picture-Q25.jpg", System.IO.FileMode.CreateNew);
            stream.Write(data, 0, data.Length);
            stream.Close();

            data = bitmap.Save(ImageFormat.Jpeg, 24, 50, ImageCompression.None);
            stream = new System.IO.FileStream("picture-Q50.jpg", System.IO.FileMode.CreateNew);
            stream.Write(data, 0, data.Length);
            stream.Close();

            data = bitmap.Save(ImageFormat.Jpeg, 24, 75, ImageCompression.None);
            stream = new System.IO.FileStream("picture-Q75.jpg", System.IO.FileMode.CreateNew);
            stream.Write(data, 0, data.Length);
            stream.Close();

            data = bitmap.Save(ImageFormat.Jpeg, 24, 100, ImageCompression.None);
            stream = new System.IO.FileStream("picture-Q100.jpg", System.IO.FileMode.CreateNew);
            stream.Write(data, 0, data.Length);
            stream.Close();

            data = bitmap.Save(ImageFormat.WindowsIcon, 0, 0, ImageCompression.None);
            stream = new System.IO.FileStream("picture.ico", System.IO.FileMode.CreateNew);
            stream.Write(data, 0, data.Length);
            stream.Close();
        }

        [TearDown]
        public void CheckBitmapSaveCleanup()
        {
            System.IO.File.Delete("picture-24.bmp");
            System.IO.File.Delete("picture-32.bmp");
            System.IO.File.Delete("picture.gif");
            System.IO.File.Delete("picture-16.png");
            System.IO.File.Delete("picture-24.png");
            System.IO.File.Delete("picture-32.png");
            System.IO.File.Delete("picture-24.tif");
            System.IO.File.Delete("picture-32-lzw.tif");
            System.IO.File.Delete("picture-Q25.jpg");
            System.IO.File.Delete("picture-Q50.jpg");
            System.IO.File.Delete("picture-Q75.jpg");
            System.IO.File.Delete("picture-Q100.jpg");
            System.IO.File.Delete("picture.ico");
        }

        [Test]
        [Ignore("Disabled (does not build)")]
        public void CheckBitmapToMouseCursor()
        {
            /*
            System.Drawing.Bitmap nativeBitmap = new System.Drawing.Bitmap(
                32,
                32,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb
            );
            Image bitmap = Bitmap.FromNativeBitmap(nativeBitmap);

            for (int i = 0; i < 1000; i++)
            {
                MouseCursor cursor = MouseCursor.FromImage(bitmap, 16, 16);
                cursor.Dispose();
            }
            */
        }
    }
}
