using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Ribbons
{
	/// <summary>
	/// La classe Geom permet de modifier la géométrie de la sélection.
	/// </summary>
	[SuppressBundleSupport]
	public class Geom : Abstract
	{
		public Geom() : base()
		{
			this.title.Text = Res.Strings.Action.GeometryMain;

			this.buttonCombine   = this.CreateIconButton("Combine",   Misc.Icon("Combine"),   Res.Strings.Action.Combine);
			this.buttonUncombine = this.CreateIconButton("Uncombine", Misc.Icon("Uncombine"), Res.Strings.Action.Uncombine);
			this.buttonToBezier  = this.CreateIconButton("ToBezier",  Misc.Icon("ToBezier"),  Res.Strings.Action.ToBezier);
			this.buttonToPoly    = this.CreateIconButton("ToPoly",    Misc.Icon("ToPoly"),    Res.Strings.Action.ToPoly);
			this.buttonFragment  = this.CreateIconButton("Fragment",  Misc.Icon("Fragment"),  Res.Strings.Action.Fragment);
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
				return 8 + 22*3;
			}
		}


		// Met à jour la géométrie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.buttonCombine == null )  return;

			double dx = this.buttonCombine.DefaultWidth;
			double dy = this.buttonCombine.DefaultHeight;

			Rectangle rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(0, dy);
			this.buttonCombine.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonUncombine.Bounds = rect;

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			this.buttonToBezier.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonToPoly.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonFragment.Bounds = rect;
		}


		protected IconButton				buttonCombine;
		protected IconButton				buttonUncombine;
		protected IconButton				buttonToBezier;
		protected IconButton				buttonToPoly;
		protected IconButton				buttonFragment;
	}
}
