//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Jonas SCHMID, Maintainer: Jonas SCHMID

#if false
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Epsitec.BingMapsWrapper;
using Epsitec.Common.Drawing;
using Epsitec.Common.Graph.Data;

namespace Epsitec.Common.Graph.Renderers
{
    public class GeoChartRenderer : AbstractRenderer
    {

        static GeoChartRenderer ()
        {
            GeoChartRenderer.imageryWrapper = new BingMapsWrapper.ImageryWrapper (GeoChartRenderer.BingKey);
        }

        public GeoChartRenderer ()
        {
            this.fetchImageAsync ();
        }

        public override void BeginRender (IPaintPort port, Epsitec.Common.Drawing.Rectangle portSize, Epsitec.Common.Drawing.Rectangle portBounds)
        {
            if (this.PortSize.Width != portSize.Width || this.PortSize.Height != portSize.Height)
            {
                this.fetchImageAsync ((int)portSize.Width, (int)portSize.Height);
            }

            base.BeginRender (port, portSize, portBounds);
        }

        public override void BeginPass (IPaintPort port, int pass)
        {
            base.BeginPass (port, pass);
        }

        public override void EndRender (IPaintPort port)
        {
            base.EndRender (port);
        }

        public override Point GetPoint (int index, double value)
        {
            throw new System.NotImplementedException ();
        }

        public override void UpdateCaptions (IEnumerable<Data.ChartSeries> series)
        {
            this.Captions.Clear ();

            int index = 0;

            foreach (var item in series)
            {
                this.CreateCaption (item, index++);
            }
        }

        public override Path GetDetectionPath (Data.ChartSeries series, int seriesIndex, double detectionRadius)
        {
            return new Path(new Epsitec.Common.Drawing.Rectangle(0, 0, 200, 300));
        }

        /// <summary>
        /// Does not show these captions
        /// </summary>
        public override SeriesCaptionPosition GetSeriesCaptionPosition (Data.ChartSeries series, int seriesIndex)
        {
            return new SeriesCaptionPosition ()
            {
                ShowCaption = false
            };
        }
    
        protected override void Render (IPaintPort port, Data.ChartSeries series, int pass, int seriesIndex)
        {
            if (this.image != null)
            {
                port.PaintImage (this.image, this.PortSize);
            }
        }

        protected override System.Action<IPaintPort, Epsitec.Common.Drawing.Rectangle> CreateCaptionSamplePainter (Data.ChartSeries series, int seriesIndex)
        {
            return (p, r) =>
            {
                using (Path sample = new Path ())
                {
                    this.FindStyle ("line-color").ApplyStyle (seriesIndex, p);

                    double x1 = r.Left;
                    double y1 = r.Center.Y - 4;
                    double x2 = r.Right;
                    double y2 = r.Center.Y + 4;

                    sample.MoveTo (x1, y1);
                    sample.LineTo (x2, y1);
                    sample.LineTo (x2, y2);
                    sample.LineTo (x1, y2);
                    sample.Close ();

                    p.PaintSurface (sample);

                    p.Color = Color.FromBrightness (1);
                    p.LineWidth = 1;
                    p.LineCap = CapStyle.Butt;
                    p.LineJoin = JoinStyle.Miter;
                    p.PaintOutline (sample);
                }
            };
        }

        private void fetchImageAsync (int width = 200, int height = 200)
        {
            if (this.myThread != null)
            {
                this.myThread.Abort ();
                this.myThread.Join ();
            }

            this.currentWidth = width;
            this.currentHeight = height;
            this.myThread = new Thread (this.fetchImage);
            this.myThread.Start ();
            //new Thread (this.fetchImageAsync).Start ();
        }

        private void fetchImage ()
        {
            string url = "none";

            try
            {
                url = GeoChartRenderer.GetUrl (this.currentWidth, this.currentHeight);

                System.Diagnostics.Debug.WriteLine (url);

                HttpWebRequest httpWebRequest =
                (HttpWebRequest)WebRequest.Create (url);

                HttpWebResponse httpWebResponse =
                        (HttpWebResponse)httpWebRequest.GetResponse ();

                System.IO.Stream stream = httpWebResponse.GetResponseStream ();

                byte[] b = GeoChartRenderer.ReadFully (stream, 100);

                this.image = Drawing.Bitmap.FromData (b);

            }
            catch (ThreadAbortException)
            {
                this.image = null;
                System.Diagnostics.Debug.WriteLine (string.Format("Aborting fetching image {0}", url));
            }
        }

        private static string GetUrl (int width, int height)
        {
            var pushpins = new List<Pushpin> ();
            pushpins.Add (
                new Pushpin ()
                {
                    Location = new Location ()
                    {
                        Latitude = 46.774316,
                        Longitude = 6.659994
                    },
                    Label = "Jonas",
                    Style = 36
                }
            );

            pushpins.Add (
                new Pushpin ()
                {
                    Location = new Location ()
                    {
                        Latitude = 46.781879,
                        Longitude = 6.641235
                    },
                    Label = "G",
                }
            );

            var reqImg = new MapUriRequest ()
            {
                Options = new MapUriOptions ()
                {
                    Style = MapStyle.Style.Road,
                    Zoom = 10,
                    ImageSize = new MapSize ()
                    {
                        Height = height,
                        Width = width
                    },
                    PreventIconCollision = false,
                    ImageType = ImageType.Type.Jpeg
                },
                Pushpins = pushpins
            };

            var res = GeoChartRenderer.imageryWrapper.GetMapUri (reqImg);

            return res.ImageUri;
        }

        /// <summary>
        /// Reads data from a stream until the end is reached. The
        /// data is returned as a byte array. An IOException is
        /// thrown if any of the underlying IO calls fail.
        /// </summary>
        /// <param name="stream">The stream to read data from</param>
        /// <param name="initialLength">The initial buffer length</param>
        private static byte[] ReadFully (System.IO.Stream stream, int initialLength)
        {
            // If we've been passed an unhelpful initial length, just
            // use 32K.
            if (initialLength < 1)
            {
                initialLength = 32768;
            }

            byte[] buffer = new byte[initialLength];
            int read = 0;

            int chunk;
            while ((chunk = stream.Read (buffer, read, buffer.Length - read)) > 0)
            {
                read += chunk;

                // If we've reached the end of our buffer, check to see if there's
                // any more information
                if (read == buffer.Length)
                {
                    int nextByte = stream.ReadByte ();

                    // End of stream? If so, we're done
                    if (nextByte == -1)
                    {
                        return buffer;
                    }

                    // Nope. Resize the buffer, put in the byte we've just
                    // read, and continue
                    byte[] newBuffer = new byte[buffer.Length * 2];
                    System.Array.Copy (buffer, newBuffer, buffer.Length);
                    newBuffer[read] = (byte)nextByte;
                    buffer = newBuffer;
                    read++;
                }
            }
            // Buffer is now too big. Shrink it.
            byte[] ret = new byte[read];
            System.Array.Copy (buffer, ret, read);
            return ret;
        }

        private static string BingKey = "ArjEUH2p_AaSYGF1wH-9JzcKAnNmn3B3ZiGAlSHTbvWfV_jVaRO3TbYlz6503XkN";
        private Image image;
        private static ImageryWrapper imageryWrapper;
        private int currentHeight;
        private int currentWidth;
        private Thread myThread;

    }
}
#endif
