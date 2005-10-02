using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Ribbons
{
	/// <summary>
	/// La classe Align permet de choisir l'ordre.
	/// </summary>
	[SuppressBundleSupport]
	public class Align : Abstract
	{
		public Align() : base()
		{
			this.title.Text = "Align";

			this.buttonAlignLeft    = this.CreateIconButton("AlignLeft",    Misc.Icon("AlignLeft"),    Res.Strings.Action.AlignLeft);
			this.buttonAlignCenterX = this.CreateIconButton("AlignCenterX", Misc.Icon("AlignCenterX"), Res.Strings.Action.AlignCenterX);
			this.buttonAlignRight   = this.CreateIconButton("AlignRight",   Misc.Icon("AlignRight"),   Res.Strings.Action.AlignRight);
			this.buttonAlignTop     = this.CreateIconButton("AlignTop",     Misc.Icon("AlignTop"),     Res.Strings.Action.AlignTop);
			this.buttonAlignCenterY = this.CreateIconButton("AlignCenterY", Misc.Icon("AlignCenterY"), Res.Strings.Action.AlignCenterY);
			this.buttonAlignBottom  = this.CreateIconButton("AlignBottom",  Misc.Icon("AlignBottom"),  Res.Strings.Action.AlignBottom);

			this.separator1 = new IconSeparator(this);

			this.buttonShareSpaceX  = this.CreateIconButton("ShareSpaceX",  Misc.Icon("ShareSpaceX"),  Res.Strings.Action.ShareSpaceX);
			this.buttonShareLeft    = this.CreateIconButton("ShareLeft",    Misc.Icon("ShareLeft"),    Res.Strings.Action.ShareLeft);
			this.buttonShareCenterX = this.CreateIconButton("ShareCenterX", Misc.Icon("ShareCenterX"), Res.Strings.Action.ShareCenterX);
			this.buttonShareRight   = this.CreateIconButton("ShareRight",   Misc.Icon("ShareRight"),   Res.Strings.Action.ShareRight);
			this.buttonShareSpaceY  = this.CreateIconButton("ShareSpaceY",  Misc.Icon("ShareSpaceY"),  Res.Strings.Action.ShareSpaceY);
			this.buttonShareTop     = this.CreateIconButton("ShareTop",     Misc.Icon("ShareTop"),     Res.Strings.Action.ShareTop);
			this.buttonShareCenterY = this.CreateIconButton("ShareCenterY", Misc.Icon("ShareCenterY"), Res.Strings.Action.ShareCenterY);
			this.buttonShareBottom  = this.CreateIconButton("ShareBottom",  Misc.Icon("ShareBottom"),  Res.Strings.Action.ShareBottom);

			this.separator2 = new IconSeparator(this);

			this.buttonAdjustWidth  = this.CreateIconButton("AdjustWidth",  Misc.Icon("AdjustWidth"),  Res.Strings.Action.AdjustWidth);
			this.buttonAdjustHeight = this.CreateIconButton("AdjustHeight", Misc.Icon("AdjustHeight"), Res.Strings.Action.AdjustHeight);

			this.separator3 = new IconSeparator(this);

			this.buttonAlignGrid    = this.CreateIconButton("AlignGrid",    Misc.Icon("AlignGrid"),    Res.Strings.Action.AlignGrid);
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
				return 8 + 22*3 + this.separatorWidth + 22*4 + this.separatorWidth + 22 + this.separatorWidth + 22;
			}
		}


		// Met à jour la géométrie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.buttonAlignLeft == null )  return;

			double dx = this.buttonAlignLeft.DefaultWidth;
			double dy = this.buttonAlignLeft.DefaultHeight;

			Rectangle rect = this.UsefulZone;
			rect.Left += dx*3;
			rect.Width = this.separatorWidth;
			this.separator1.Bounds = rect;

			rect = this.UsefulZone;
			rect.Left += dx*3 + this.separatorWidth + dx*4;
			rect.Width = this.separatorWidth;
			this.separator2.Bounds = rect;

			rect = this.UsefulZone;
			rect.Left += dx*3 + this.separatorWidth + dx*4 + this.separatorWidth + dx;
			rect.Width = this.separatorWidth;
			this.separator3.Bounds = rect;

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(0, dy);
			this.buttonAlignLeft.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonAlignCenterX.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonAlignRight.Bounds = rect;
			rect.Offset(dx+this.separatorWidth, 0);
			this.buttonShareLeft.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonShareCenterX.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonShareSpaceX.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonShareRight.Bounds = rect;
			rect.Offset(dx+this.separatorWidth, 0);
			this.buttonAdjustWidth.Bounds = rect;
			rect.Offset(dx+this.separatorWidth, 0);
			this.buttonAlignGrid.Bounds = rect;

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			this.buttonAlignTop.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonAlignCenterY.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonAlignBottom.Bounds = rect;
			rect.Offset(dx+this.separatorWidth, 0);
			this.buttonShareTop.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonShareCenterY.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonShareSpaceY.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonShareBottom.Bounds = rect;
			rect.Offset(dx+this.separatorWidth, 0);
			this.buttonAdjustHeight.Bounds = rect;
		}

		
		protected IconButton				buttonAlignLeft;
		protected IconButton				buttonAlignCenterX;
		protected IconButton				buttonAlignRight;
		protected IconButton				buttonAlignTop;
		protected IconButton				buttonAlignCenterY;
		protected IconButton				buttonAlignBottom;
		protected IconSeparator				separator1;
		protected IconButton				buttonShareLeft;
		protected IconButton				buttonShareCenterX;
		protected IconButton				buttonShareSpaceX;
		protected IconButton				buttonShareRight;
		protected IconButton				buttonShareTop;
		protected IconButton				buttonShareCenterY;
		protected IconButton				buttonShareSpaceY;
		protected IconButton				buttonShareBottom;
		protected IconSeparator				separator2;
		protected IconButton				buttonAdjustWidth;
		protected IconButton				buttonAdjustHeight;
		protected IconSeparator				separator3;
		protected IconButton				buttonAlignGrid;
	}
}
