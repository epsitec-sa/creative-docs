using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Ribbons
{
	/// <summary>
	/// La classe Geom permet de choisir l'ordre.
	/// </summary>
	[SuppressBundleSupport]
	public class Geom : Abstract
	{
		public Geom(Document document) : base(document)
		{
			this.title.Text = "Geometry";

			this.buttonBooleanOr = this.CreateIconButton("BooleanOr", Misc.Icon("BooleanOr"), Res.Strings.Action.BooleanOr);
			this.buttonBooleanAnd = this.CreateIconButton("BooleanAnd", Misc.Icon("BooleanAnd"), Res.Strings.Action.BooleanAnd);
			this.buttonBooleanXor = this.CreateIconButton("BooleanXor", Misc.Icon("BooleanXor"), Res.Strings.Action.BooleanXor);
			this.buttonBooleanFrontMinus = this.CreateIconButton("BooleanFrontMinus", Misc.Icon("BooleanFrontMinus"), Res.Strings.Action.BooleanFrontMinus);
			this.buttonBooleanBackMinus = this.CreateIconButton("BooleanBackMinus", Misc.Icon("BooleanBackMinus"), Res.Strings.Action.BooleanBackMinus);

			this.CreateSeparator(ref this.separator);

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

		// Retourne la largeur compacte.
		public override double CompactWidth
		{
			get
			{
				return 8 + 22*3;
			}
		}

		// Retourne la largeur étendue.
		public override double ExtendWidth
		{
			get
			{
				return 8 + 22*3 + this.separatorWidth + 22*3;
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
			rect.Left += dx*3;
			rect.Width = this.separatorWidth;
			this.separator.Bounds = rect;

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(0, dy);
			this.buttonBooleanOr.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonBooleanAnd.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonBooleanXor.Bounds = rect;
			rect.Offset(dx+this.separatorWidth, 0);
			this.buttonCombine.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonUncombine.Bounds = rect;

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			this.buttonBooleanFrontMinus.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonBooleanBackMinus.Bounds = rect;
			rect.Offset(dx*2+this.separatorWidth, 0);
			this.buttonToBezier.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonToPoly.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonFragment.Bounds = rect;
		}

		// Met à jour les boutons.
		protected override void UpdateButtons()
		{
			base.UpdateButtons();

			if ( this.buttonCombine == null )  return;

			this.separator.SetVisible(this.isExtendedSize);
			this.buttonCombine.SetVisible(this.isExtendedSize);
			this.buttonUncombine.SetVisible(this.isExtendedSize);
			this.buttonToBezier.SetVisible(this.isExtendedSize);
			this.buttonToPoly.SetVisible(this.isExtendedSize);
			this.buttonFragment.SetVisible(this.isExtendedSize);
		}


		protected IconButton				buttonBooleanOr;
		protected IconButton				buttonBooleanAnd;
		protected IconButton				buttonBooleanXor;
		protected IconButton				buttonBooleanFrontMinus;
		protected IconButton				buttonBooleanBackMinus;
		protected IconSeparator				separator;
		protected IconButton				buttonCombine;
		protected IconButton				buttonUncombine;
		protected IconButton				buttonToBezier;
		protected IconButton				buttonToPoly;
		protected IconButton				buttonFragment;
	}
}
