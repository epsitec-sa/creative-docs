using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Ribbons
{
	/// <summary>
	/// La classe View d�finit les modes d'affichage.
	/// </summary>
	[SuppressBundleSupport]
	public class View : Abstract
	{
		public View() : base()
		{
			this.title.Text = Res.Strings.Action.ViewMain;

			this.buttonPreview     = this.CreateIconButton("Preview");
			this.buttonGrid        = this.CreateIconButton("Grid");
			this.buttonTextGrid    = this.CreateIconButton("TextGrid");
			this.buttonMagnet      = this.CreateIconButton("Magnet");
			this.buttonMagnetLayer = this.CreateIconButton("MagnetLayer");
			this.buttonRulers      = this.CreateIconButton("Rulers");
			this.buttonLabels      = this.CreateIconButton("Labels");
			this.buttonAggregates  = this.CreateIconButton("Aggregates");
			
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


		// Met � jour la g�om�trie.
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
