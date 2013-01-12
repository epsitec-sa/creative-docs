//	Copyright © 2004-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Pdf.Engine;

namespace Epsitec.Common.Pdf.Common
{
	public class Layer
	{
		public Layer(ExportPdfInfo info, CommonSetup setup)
		{
			this.info  = info;
			this.setup = setup;

			this.TextMargins = new Margins (0.0, 0.0, 0.0, 50.0);
			this.TextStyle = new TextStyle (this.setup.TextStyle)
			{
				Alignment = ContentAlignment.BottomCenter,
			};
		}

		public FormattedText Text
		{
			set;
			get;
		}

		public Margins TextMargins
		{
			set;
			get;
		}

		public TextStyle TextStyle
		{
			set;
			get;
		}

		public double Rotation
		{
			//	Angle de rotation CCW en degrés.
			set;
			get;
		}

		public void RenderPage(Port port, int page)
		{
			var box = new Rectangle (Point.Zero, this.info.PageSize);
			box.Deflate (this.TextMargins);

			var text = this.Text;

			//	{0} est remplacé par le numéro de la page.
			if (text.ToString ().Contains ("{0}"))
			{
				text = text.ToString ().Replace ("{0}", page.ToString ());
			}

			var style = this.TextStyle;
			if (style == null)
			{
				style = setup.TextStyle;
			}

			if (this.Rotation == 0)
			{
				port.PaintText (box, text, style);
			}
			else
			{
				//	On est dans le mode spécial pour le Watermark !
				//	Comme le texte est centré (ContentAlignment.MiddleCenter), on augmente
				//	fortement les dimensions, pour être certain d'avoir assez de place.
				box.Inflate (10000);

				var t = port.Transform;
				port.RotateTransformDeg (this.Rotation, this.info.PageSize.Width/2, this.info.PageSize.Height/2);
				port.PaintText (box, text, style);
				port.Transform = t;
			}
		}

		private readonly ExportPdfInfo info;
		private readonly CommonSetup setup;
	}
}
