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

			this.buttonBooleanOr = this.CreateIconButton("BooleanOr", Misc.Icon("BooleanOr"), Res.Strings.Action.BooleanOr);
			this.buttonBooleanAnd = this.CreateIconButton("BooleanAnd", Misc.Icon("BooleanAnd"), Res.Strings.Action.BooleanAnd);
			this.buttonBooleanXor = this.CreateIconButton("BooleanXor", Misc.Icon("BooleanXor"), Res.Strings.Action.BooleanXor);
			this.buttonBooleanFrontMinus = this.CreateIconButton("BooleanFrontMinus", Misc.Icon("BooleanFrontMinus"), Res.Strings.Action.BooleanFrontMinus);
			this.buttonBooleanBackMinus = this.CreateIconButton("BooleanBackMinus", Misc.Icon("BooleanBackMinus"), Res.Strings.Action.BooleanBackMinus);
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

			if ( this.buttonBooleanOr == null )  return;

			double dx = this.buttonBooleanOr.DefaultWidth;
			double dy = this.buttonBooleanOr.DefaultHeight;

			Rectangle rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(0, dy);
			this.buttonBooleanOr.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonBooleanAnd.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonBooleanXor.Bounds = rect;

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			this.buttonBooleanFrontMinus.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonBooleanBackMinus.Bounds = rect;
		}


		protected IconButton				buttonBooleanOr;
		protected IconButton				buttonBooleanAnd;
		protected IconButton				buttonBooleanXor;
		protected IconButton				buttonBooleanFrontMinus;
		protected IconButton				buttonBooleanBackMinus;
	}
}
