//	Copyright © 2004-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Common.Pdf.Engine;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Pdf.Common
{
	public abstract class CommonPdf
	{
		public CommonPdf(ExportPdfInfo info, CommonSetup setup)
		{
			this.info  = info;
			this.setup = setup;

			this.layers = new List<Layer> ();
		}


		public void AddWatermark(FormattedText text)
		{
			//	Ajoute un texte en filigrane, c'est-à-dire très grand, oblique et gris très clair.
			var s = new TextStyle (this.setup.TextStyle)
			{
				Alignment = ContentAlignment.MiddleCenter,
				FontSize  = 1.0,
				FontColor = Color.FromBrightness (0.95),  // gris très clair
			};

			//	Calcul approximatif, à améliorer !
			double len = Port.GetTextSingleLineSize (text, s).Width;

			if (this.info.PageSize.Width < this.info.PageSize.Height)  // portrait ?
			{
				s.FontSize = this.info.PageSize.Height / len;
				this.AddLayer (text, Margins.Zero, s, 60.0);
			}
			else  // paysage ?
			{
				s.FontSize = this.info.PageSize.Width / len;
				this.AddLayer (text, Margins.Zero, s, 30.0);
			}
		}

		public void AddTopCenterLayer(FormattedText text, double topMargin, TextStyle style = null)
		{
			var margins = new Margins (0.0, 0.0, topMargin, 0.0);

			var s = new TextStyle (style ?? this.setup.TextStyle)
			{
				Alignment = ContentAlignment.TopCenter,
			};

			this.AddLayer (text, margins, s);
		}

		public void AddTopLeftLayer(FormattedText text, double topMargin, double? leftMargin = null, TextStyle style = null)
		{
			if (!leftMargin.HasValue)
			{
				leftMargin = this.setup.PageMargins.Left;
			}

			var margins = new Margins (leftMargin.Value, 0.0, topMargin, 0.0);

			var s = new TextStyle (style ?? this.setup.TextStyle)
			{
				Alignment = ContentAlignment.TopLeft,
			};

			this.AddLayer (text, margins, s);
		}

		public void AddTopRightLayer(FormattedText text, double topMargin, double? rightMargin = null, TextStyle style = null)
		{
			if (!rightMargin.HasValue)
			{
				rightMargin = this.setup.PageMargins.Right;
			}

			var margins = new Margins (0.0, rightMargin.Value, topMargin, 0.0);

			var s = new TextStyle (style ?? this.setup.TextStyle)
			{
				Alignment = ContentAlignment.TopRight,
			};

			this.AddLayer (text, margins, s);
		}

		public void AddBottomCenterLayer(FormattedText text, double bottomMargin, TextStyle style = null)
		{
			var margins = new Margins (0.0, 0.0, 0.0, bottomMargin);

			var s = new TextStyle (style ?? this.setup.TextStyle)
			{
				Alignment = ContentAlignment.BottomCenter,
			};

			this.AddLayer (text, margins, s);
		}

		public void AddBottomLeftLayer(FormattedText text, double bottomMargin, double? leftMargin = null, TextStyle style = null)
		{
			if (!leftMargin.HasValue)
			{
				leftMargin = this.setup.PageMargins.Left;
			}

			var margins = new Margins (leftMargin.Value, 0.0, 0.0, bottomMargin);

			var s = new TextStyle (style ?? this.setup.TextStyle)
			{
				Alignment = ContentAlignment.BottomLeft,
			};

			this.AddLayer (text, margins, s);
		}

		public void AddBottomRightLayer(FormattedText text, double bottomMargin, double? rightMargin = null, TextStyle style = null)
		{
			if (!rightMargin.HasValue)
			{
				rightMargin = this.setup.PageMargins.Right;
			}

			var margins = new Margins (0.0, rightMargin.Value, 0.0, bottomMargin);

			var s = new TextStyle (style ?? this.setup.TextStyle)
			{
				Alignment = ContentAlignment.BottomRight,
			};

			this.AddLayer (text, margins, s);
		}

		private void AddLayer(FormattedText text, Margins margins, TextStyle style, double rotation = 0)
		{
			var layer = new Layer (this.info, this.setup)
			{
				Text        = text,
				TextMargins = margins,
				TextStyle   = style,
				Rotation    = rotation,
			};

			this.layers.Add (layer);
		}


		protected void RenderLayers(Port port, int page)
		{
			foreach (var layer in this.layers)
			{
				layer.RenderPage (port, page);
			}
		}


		protected readonly CommonSetup setup;
		protected readonly ExportPdfInfo info;
		private readonly List<Layer> layers;
	}
}
