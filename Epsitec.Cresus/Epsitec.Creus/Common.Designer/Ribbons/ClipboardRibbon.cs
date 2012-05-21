using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Ribbons
{
	/// <summary>
	/// La classe Clipboard permet de gérer le presse-papiers.
	/// </summary>
	public class ClipboardRibbon : AbstractRibbon
	{
		public ClipboardRibbon(DesignerApplication designerApplication) : base(designerApplication)
		{
			this.Title = Res.Strings.Ribbon.Section.Clipboard;
			this.PreferredWidth = 8 + 22 + 4 + 22*1.5;

			this.buttonCut   = this.CreateIconButton("Cut");
			this.buttonCopy  = this.CreateIconButton("Copy");
			this.buttonPaste = this.CreateIconButton("Paste", "Large");
			
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

			if ( this.buttonCut == null )  return;

			double dx = this.buttonCut.PreferredWidth;
			double dy = this.buttonCut.PreferredHeight;

			Rectangle rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(0, dy+5);
			this.buttonCut.SetManualBounds(rect);
			rect.Offset(0, -dy-5);
			this.buttonCopy.SetManualBounds(rect);

			rect = this.UsefulZone;
			rect.Width  = dx*1.5;
			rect.Height = dy*1.5;
			rect.Offset(dx+4, dy*0.5);
			this.buttonPaste.SetManualBounds(rect);
		}


		protected IconButton				buttonCut;
		protected IconButton				buttonCopy;
		protected IconButton				buttonPaste;
	}
}
