using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Ribbons
{
	/// <summary>
	/// La classe View définit les modes d'affichage.
	/// </summary>
	[SuppressBundleSupport]
	public class View : Abstract
	{
		public View() : base()
		{
			this.title.Text = Res.Strings.Action.ViewMain;

			this.buttonPreview = this.CreateIconButton("Preview", Misc.Icon("Preview"), Res.Strings.Action.Preview, true);
			this.buttonGrid = this.CreateIconButton("Grid", Misc.Icon("Grid"), Res.Strings.Action.Grid, true);
			this.buttonTextGrid = this.CreateIconButton("TextGrid", Misc.Icon("TextGrid"), Res.Strings.Action.TextGrid, true);
			this.buttonMagnet = this.CreateIconButton("Magnet", Misc.Icon("Magnet"), Res.Strings.Action.Magnet, true);
			this.buttonMagnetLayer = this.CreateIconButton("MagnetLayer", Misc.Icon("MagnetLayer"), Res.Strings.Action.MagnetLayer, true);
			this.buttonRulers = this.CreateIconButton("Rulers", Misc.Icon("Rulers"), Res.Strings.Action.Rulers, true);
			this.buttonLabels = this.CreateIconButton("Labels", Misc.Icon("Labels"), Res.Strings.Action.Labels, true);
			this.buttonAggregates = this.CreateIconButton("Aggregates", Misc.Icon("Aggregates"), Res.Strings.Action.Aggregates, true);
			
			this.UpdateClientGeometry();
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
			}
			
			base.Dispose(disposing);
		}

		// Retourne la largeur standard.
		public override double DefaultWidth
		{
			get
			{
				return 8 + 22*5 + 5;
			}
		}


		// Met à jour la géométrie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.buttonPreview == null )  return;

			double dx = this.buttonPreview.DefaultWidth;
			double dy = this.buttonPreview.DefaultHeight;

			Rectangle rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(0, dy+5);
			this.buttonPreview.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonGrid.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonTextGrid.Bounds = rect;
			rect.Offset(dx+5, 0);
			this.buttonMagnet.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonMagnetLayer.Bounds = rect;

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			this.buttonRulers.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonLabels.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonAggregates.Bounds = rect;
		}


		protected IconButton				buttonPreview;
		protected IconButton				buttonGrid;
		protected IconButton				buttonTextGrid;
		protected IconButton				buttonMagnet;
		protected IconButton				buttonMagnetLayer;
		protected IconButton				buttonRulers;
		protected IconButton				buttonLabels;
		protected IconButton				buttonAggregates;
	}
}
