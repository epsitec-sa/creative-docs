//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using Epsitec.Common.Support.EntityEngine;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Widgets
{
	/// <summary>
	/// Ce widget montre le contenu d'une page imprimée dans une zone rectangulaire.
	/// </summary>
	public class PrintedPagePreviewer : Widget
	{
		public PrintedPagePreviewer()
		{
			this.currentPage = -1;
		}


		public Printers.AbstractDocumentPrinter DocumentPrinter
		{
			get
			{
				return this.documentPrinter;
			}
			set
			{
				this.documentPrinter = value;
			}
		}

		public int CurrentPage
		{
			get
			{
				return this.currentPage;
			}
			set
			{
				if (this.currentPage != value)
				{
					this.currentPage = value;
					this.UpdateTooltip ();
				}
			}
		}

		public bool IsContinuousPreview
		{
			get;
			set;
		}

		public double VerticalOffset
		{
			get
			{
				return this.verticalOffset;
			}
			set
			{
				if (this.verticalOffset != value)
				{
					this.verticalOffset = value;
					this.Invalidate ();
				}
			}
		}

		/// <summary>
		/// La description vient dans la deuxième ligne du tooltip.
		/// </summary>
		public FormattedText Description
		{
			get
			{
				return this.description;
			}
			set
			{
				if (this.description != value)
				{
					this.description = value;
					this.UpdateTooltip ();
				}
			}
		}

		/// <summary>
		/// Indique s'il faut mettre en évidence les pages non imprimées (fond rosé avec une croix).
		/// </summary>
		public bool NotPrinting
		{
			get;
			set;
		}

		/// <summary>
		/// Indique s'il exite plusieurs options pour représenter cette page.
		/// </summary>
		public bool HasManyOptions
		{
			get;
			set;
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			base.PaintBackgroundImplementation (graphics, clipRect);

			if (this.documentPrinter != null)
			{
				var clientBounds = this.Client.Bounds;

				double sx = clientBounds.Width  / this.documentPrinter.RequiredPageSize.Width;
				double sy = clientBounds.Height / this.documentPrinter.RequiredPageSize.Height;
				double scale = System.Math.Min (sx, sy);

				double offsetX = clientBounds.Left   + System.Math.Ceiling ((clientBounds.Width  - this.documentPrinter.RequiredPageSize.Width *scale) / 2);
				double offsetY = clientBounds.Bottom + System.Math.Ceiling ((clientBounds.Height - this.documentPrinter.RequiredPageSize.Height*scale) / 2);

				if (this.IsContinuousPreview)
				{
					scale = sx;
					offsetX = 0;
					offsetY = (this.verticalOffset-Printers.AbstractDocumentPrinter.continuousHeight)*scale + clientBounds.Height;
				}

				//	Dessine le fond d'une page blanche.
				Rectangle bounds = new Rectangle (offsetX, offsetY, System.Math.Floor (this.documentPrinter.RequiredPageSize.Width*scale), System.Math.Floor (this.documentPrinter.RequiredPageSize.Height*scale));

				graphics.AddFilledRectangle (bounds);
				graphics.RenderSolid (this.NotPrinting ? Color.FromHexa ("fff0f0") : Color.FromBrightness (1));  // fond rosé ou blanc

				if (this.NotPrinting)
				{
					double thickness = System.Math.Min (bounds.Width, bounds.Height) * 0.1;

					var b = bounds;
					b.Deflate (thickness);

					graphics.LineWidth = thickness;
					graphics.LineCap = CapStyle.Round;
					graphics.AddLine (b.BottomLeft, b.TopRight);
					graphics.AddLine (b.BottomRight, b.TopLeft);
					graphics.RenderSolid (Color.FromBrightness (1));  // croix blanche
				}

				//	Dessine l'entité dans la page.
				Transform initial = graphics.Transform;
				graphics.TranslateTransform (offsetX, offsetY);
				graphics.ScaleTransform (scale, scale, 0.0, 0.0);

				this.documentPrinter.CurrentPage = this.currentPage;
				this.documentPrinter.PrintBackgroundCurrentPage (graphics);
				this.documentPrinter.PrintForegroundCurrentPage (graphics);

				graphics.Transform = initial;

				//	Dessine le cadre de la page en dernier, pour recouvrir la page.
				bounds.Deflate (0.5);

				if (this.HasManyOptions)
				{
					graphics.LineWidth = 1;
					graphics.Color = Color.FromBrightness (0);
					PrintedPagePreviewer.PaintDashedRectangle (graphics, bounds);
				}
				else
				{
					graphics.LineWidth = 1;
					graphics.AddRectangle (bounds);
					graphics.RenderSolid (Color.FromBrightness (0));
				}
			}
		}

		private static void PaintDashedRectangle(Graphics graphics, Rectangle rect)
		{
			var path = new DashedPath ();
			path.AppendRectangle (rect);
			path.AddDash (5, 3);

			using (Path dashed = path.GenerateDashedPath ())
			{
				graphics.PaintOutline (dashed);
			}
		}

		private void UpdateTooltip()
		{
			if (this.IsContinuousPreview)
			{
				ToolTip.Default.HideToolTipForWidget (this);
			}
			else
			{
				var builder = new TextBuilder ();

				builder.Append ("<font size=\"13\"><b>");
				builder.Append ("Page ");
				builder.Append ((this.currentPage+1).ToString ());
				builder.Append ("</b></font>");

				if (this.documentPrinter != null)
				{
					builder.Append (" (");
					builder.Append (this.documentPrinter.RequiredPageSize.Width.ToString ());
					builder.Append (" × ");
					builder.Append (this.documentPrinter.RequiredPageSize.Height.ToString ());
					builder.Append (" mm)");
				}

				if (!this.description.IsNullOrEmpty)
				{
					builder.Append ("<br/>");
					builder.Append (this.description);
				}

				ToolTip.Default.SetToolTip (this, builder.ToFormattedText ());
			}
		}


		private Printers.AbstractDocumentPrinter	documentPrinter;
		private int									currentPage;
		private FormattedText						description;
		private double								verticalOffset;
	}
}
