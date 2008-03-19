using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Ribbons
{
	/// <summary>
	/// La classe Display correspond aux commandes du mode d'affichage.
	/// </summary>
	public class Display : Abstract
	{
		public Display(DesignerApplication designerApplication) : base(designerApplication)
		{
			this.Title = Res.Strings.Ribbon.Section.Display;
			this.PreferredWidth = 8 + 22*1.5*4;

			this.buttonHorizontal = this.CreateIconButton("DisplayHorizontal", "Large");
			this.buttonVertical   = this.CreateIconButton("DisplayVertical",   "Large");
			this.buttonFullScreen = this.CreateIconButton("DisplayFullScreen", "Large");
			this.buttonWindow     = this.CreateIconButton("DisplayWindow",     "Large");
			
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

			if ( this.buttonHorizontal == null )  return;

			double dx = this.buttonHorizontal.PreferredWidth;
			double dy = this.buttonHorizontal.PreferredHeight;

			Rectangle rect = this.UsefulZone;
			rect.Width  = dx*1.5;
			rect.Height = dy*1.5;
			rect.Offset(0, dy*0.5);
			this.buttonHorizontal.SetManualBounds(rect);
			rect.Offset(dx*1.5, 0);
			this.buttonVertical.SetManualBounds(rect);
			rect.Offset(dx*1.5, 0);
			this.buttonFullScreen.SetManualBounds(rect);
			rect.Offset(dx*1.5, 0);
			this.buttonWindow.SetManualBounds(rect);
		}


		protected IconButton				buttonHorizontal;
		protected IconButton				buttonVertical;
		protected IconButton				buttonFullScreen;
		protected IconButton				buttonWindow;
	}
}
