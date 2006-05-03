using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Ribbons
{
	/// <summary>
	/// La classe Bool permet de choisir les opérations booléennes.
	/// </summary>
	[SuppressBundleSupport]
	public class Bool : Abstract
	{
		public Bool() : base()
		{
			this.title.Text = Res.Strings.Action.BooleanMain;

			this.buttonBooleanOr         = this.CreateIconButton("BooleanOr");
			this.buttonBooleanAnd        = this.CreateIconButton("BooleanAnd");
			this.buttonBooleanXor        = this.CreateIconButton("BooleanXor");
			this.buttonBooleanFrontMinus = this.CreateIconButton("BooleanFrontMinus");
			this.buttonBooleanBackMinus  = this.CreateIconButton("BooleanBackMinus");
			
			this.UpdateClientGeometry();
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
				return 8 + 22*3;
			}
		}


		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();

			if ( this.buttonBooleanOr == null )  return;

			double dx = this.buttonBooleanOr.DefaultWidth;
			double dy = this.buttonBooleanOr.DefaultHeight;

			Rectangle rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(0, dy+5);
			this.buttonBooleanOr.SetManualBounds(rect);
			rect.Offset(dx, 0);
			this.buttonBooleanAnd.SetManualBounds(rect);
			rect.Offset(dx, 0);
			this.buttonBooleanXor.SetManualBounds(rect);

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			this.buttonBooleanFrontMinus.SetManualBounds(rect);
			rect.Offset(dx, 0);
			this.buttonBooleanBackMinus.SetManualBounds(rect);
		}


		protected IconButton				buttonBooleanOr;
		protected IconButton				buttonBooleanAnd;
		protected IconButton				buttonBooleanXor;
		protected IconButton				buttonBooleanFrontMinus;
		protected IconButton				buttonBooleanBackMinus;
	}
}
