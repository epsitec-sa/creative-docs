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

using System;
using Epsitec.Common.Drawing;
using NUnit.Framework;

namespace Epsitec.Common.Tests.Drawing
{
    [TestFixture]
    public class DrawingTest
    {
        [Test]
        public void CheckPixmap()
        {
            DrawingBitmap pixmap = new DrawingBitmap(200, 100);
            pixmap.Dispose();
        }

        [Test]
        public void CheckPointToStringParse()
        {
            Point pt1 = new Point(10, 20);
            Point pt2 = Point.Parse("10;20");
            Point pt3 = Point.Parse("*;30", pt1);
            Point pt4 = Point.Parse("40;*", pt1);

            Assert.AreEqual(pt1, pt2);
            Assert.AreEqual(new Point(10, 30), pt3);
            Assert.AreEqual(new Point(40, 20), pt4);
            Assert.AreEqual("10;20", pt1.ToString());
        }

        [Test]
        public void CheckSizeToStringParse()
        {
            Size sz1 = new Size(10, 20);
            Size sz2 = Size.Parse("10;20");
            Size sz3 = Size.Parse("*;30", sz1);
            Size sz4 = Size.Parse("40;*", sz1);

            Assert.AreEqual(sz1, sz2);
            Assert.AreEqual(new Size(10, 30), sz3);
            Assert.AreEqual(new Size(40, 20), sz4);
            Assert.AreEqual("10;20", sz1.ToString());
        }
        // TODO bl-net8-cross update RendererGradient tests

        //[Test]
        //public void CheckRendererGradientEx1()
        //{
        //    Common.Drawing.Graphics graphics = new Graphics();
        //    Common.Drawing.Renderers.Gradient gradient = new Common.Drawing.Renderers.Gradient(
        //        graphics
        //    );
        //    Assert.Throws<NullReferenceException>(() => gradient.SetColors(0, 0, 0, 0, 1, 1, 1, 1));
        //}

        //[Test]
        //public void CheckRendererGradientEx2()
        //{
        //    Common.Drawing.Graphics graphics = new Graphics();
        //    Common.Drawing.Renderers.Gradient gradient = new Common.Drawing.Renderers.Gradient(
        //        graphics
        //    );
        //    Assert.Throws<NullReferenceException>(() => gradient.SetParameters(0, 100));
        //}

        //[Test]
        //public void CheckRendererGradientEx3()
        //{
        //    Common.Drawing.Graphics graphics = new Graphics();
        //    Common.Drawing.Renderers.Gradient gradient = new Common.Drawing.Renderers.Gradient(
        //        graphics
        //    );
        //    Assert.Throws<NullReferenceException>(
        //        () => gradient.Fill = Common.Drawing.GradientFill.Conic
        //    );
        //}

        //[Test]
        //public void CheckRendererGradientEx4()
        //{
        //    Common.Drawing.Graphics graphics = new Graphics();
        //    Common.Drawing.Renderers.Gradient gradient = new Common.Drawing.Renderers.Gradient(
        //        graphics
        //    );
        //    Assert.Throws<ArgumentOutOfRangeException>(
        //        () =>
        //            gradient.SetColors(
        //                new double[100],
        //                new double[256],
        //                new double[256],
        //                new double[256]
        //            )
        //    );
        //}

        //[Test]
        //public void CheckRendererGradient()
        //{
        //    DrawingBitmap pixmap = new DrawingBitmap();
        //    Common.Drawing.Graphics graphics = new Graphics();
        //    Common.Drawing.Renderers.Gradient gradient = new Common.Drawing.Renderers.Gradient(
        //        graphics
        //    );

        //    pixmap.Size = new System.Drawing.Size(200, 200);
        //    gradient.DrawingBitmap = pixmap;
        //    gradient.Fill = Common.Drawing.GradientFill.Circle;
        //    gradient.SetColors(Color.FromBrightness(0.0), Color.FromBrightness(1.0));
        //    gradient.SetParameters(0, 100);

        //    gradient.Dispose();
        //    pixmap.Dispose();
        //}
    }
}
