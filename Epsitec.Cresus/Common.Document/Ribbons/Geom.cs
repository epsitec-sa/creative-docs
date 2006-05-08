using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Ribbons
{
	/// <summary>
	/// La classe Geom permet de modifier la géométrie de la sélection.
	/// </summary>
	public class Geom : Abstract
	{
		public Geom() : base()
		{
			this.title.Text = Res.Strings.Action.GeometryMain;

			this.buttonCombine    = this.CreateIconButton("Combine");
			this.buttonUncombine  = this.CreateIconButton("Uncombine");
			this.buttonToBezier   = this.CreateIconButton("ToBezier");
			this.buttonToPoly     = this.CreateIconButton("ToPoly");
			this.buttonToTextBox2 = this.CreateIconButton("ToTextBox2");
			this.buttonFragment   = this.CreateIconButton("Fragment");
			
//			this.UpdateClientGeometry();
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
			}
			
			base.Dispose(disposing);
		}

		public override double DefaultWidth
		{
			//	Retourne la largeur standard.
			get
			{
				return 8 + 22*4;
			}
		}


		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();

			if ( this.buttonCombine == null )  return;

			double dx = this.buttonCombine.PreferredWidth;
			double dy = this.buttonCombine.PreferredHeight;

			Rectangle rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(0, dy+5);
			this.buttonCombine.SetManualBounds(rect);
			rect.Offset(dx, 0);
			this.buttonUncombine.SetManualBounds(rect);

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			this.buttonToBezier.SetManualBounds(rect);
			rect.Offset(dx, 0);
			this.buttonToPoly.SetManualBounds(rect);
			rect.Offset(dx, 0);
			this.buttonToTextBox2.SetManualBounds(rect);
			rect.Offset(dx, 0);
			this.buttonFragment.SetManualBounds(rect);
		}


		protected IconButton				buttonCombine;
		protected IconButton				buttonUncombine;
		protected IconButton				buttonToBezier;
		protected IconButton				buttonToPoly;
		protected IconButton				buttonToTextBox2;
		protected IconButton				buttonFragment;
	}
}
