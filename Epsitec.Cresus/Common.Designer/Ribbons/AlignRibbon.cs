using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Ribbons
{
	/// <summary>
	/// La classe Align permet de choisir les commandes d'alignement de la sélection.
	/// </summary>
	public class AlignRibbon : AbstractRibbon
	{
		public AlignRibbon(DesignerApplication designerApplication) : base(designerApplication)
		{
			this.Title = Res.Strings.Ribbon.Section.Align;
			this.PreferredWidth = 8 + 22*4 + this.separatorWidth + 22;

			this.buttonAlignLeft     = this.CreateIconButton("AlignLeft");
			this.buttonAlignCenterX  = this.CreateIconButton("AlignCenterX");
			this.buttonAlignRight    = this.CreateIconButton("AlignRight");
			this.buttonAlignTop      = this.CreateIconButton("AlignTop");
			this.buttonAlignCenterY  = this.CreateIconButton("AlignCenterY");
			this.buttonAlignBottom   = this.CreateIconButton("AlignBottom");
			this.buttonAlignBaseLine = this.CreateIconButton("AlignBaseLine");

			this.separator1 = new IconSeparator(this);

			this.buttonAdjustWidth  = this.CreateIconButton("AdjustWidth");
			this.buttonAdjustHeight = this.CreateIconButton("AdjustHeight");

			this.UpdateClientGeometry();
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
			}
			
			base.Dispose(disposing);
		}


		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();

			if ( this.buttonAlignLeft == null )  return;

			double dx = this.buttonAlignLeft.PreferredWidth;
			double dy = this.buttonAlignLeft.PreferredHeight;

			Rectangle rect = this.UsefulZone;
			rect.Left += dx*4;
			rect.Width = this.separatorWidth;
			this.separator1.SetManualBounds(rect);

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(0, dy+5);
			this.buttonAlignLeft.SetManualBounds(rect);
			rect.Offset(dx, 0);
			this.buttonAlignCenterX.SetManualBounds(rect);
			rect.Offset(dx, 0);
			this.buttonAlignRight.SetManualBounds(rect);
			rect.Offset(dx+dx+this.separatorWidth, 0);
			this.buttonAdjustWidth.SetManualBounds(rect);

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			this.buttonAlignTop.SetManualBounds(rect);
			rect.Offset(dx, 0);
			this.buttonAlignCenterY.SetManualBounds(rect);
			rect.Offset(dx, 0);
			this.buttonAlignBottom.SetManualBounds(rect);
			rect.Offset(dx, 0);
			this.buttonAlignBaseLine.SetManualBounds(rect);
			rect.Offset(dx+this.separatorWidth, 0);
			this.buttonAdjustHeight.SetManualBounds(rect);
		}

		
		protected IconButton				buttonAlignLeft;
		protected IconButton				buttonAlignCenterX;
		protected IconButton				buttonAlignRight;
		protected IconButton				buttonAlignTop;
		protected IconButton				buttonAlignCenterY;
		protected IconButton				buttonAlignBottom;
		protected IconButton				buttonAlignBaseLine;
		protected IconSeparator				separator1;
		protected IconButton				buttonAdjustWidth;
		protected IconButton				buttonAdjustHeight;
	}
}
