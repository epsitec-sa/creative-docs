using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Ribbons
{
	/// <summary>
	/// La classe Display correspond aux commandes du mode d'affichage.
	/// </summary>
	public class DisplayRibbon : AbstractRibbon
	{
		public DisplayRibbon(DesignerApplication designerApplication) : base(designerApplication)
		{
			this.Title = Res.Strings.Ribbon.Section.Display;

			if (DisplayRibbon.large)
			{
				this.PreferredWidth = 8 + 22*1.5*4;

				this.buttonHorizontal = this.CreateIconButton("DisplayHorizontal", "Large");
				this.buttonVertical   = this.CreateIconButton("DisplayVertical",   "Large");
				this.buttonFullScreen = this.CreateIconButton("DisplayFullScreen", "Large");
				this.buttonWindow     = this.CreateIconButton("DisplayWindow",     "Large");
			}
			else
			{
				this.PreferredWidth = 8 + 22*2;

				this.buttonHorizontal = this.CreateIconButton("DisplayHorizontal");
				this.buttonVertical   = this.CreateIconButton("DisplayVertical");
				this.buttonFullScreen = this.CreateIconButton("DisplayFullScreen");
				this.buttonWindow     = this.CreateIconButton("DisplayWindow");
			}
			
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

			if (DisplayRibbon.large)
			{
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
			else
			{
				Rectangle rect = this.UsefulZone;
				rect.Width  = dx;
				rect.Height = dy;
				rect.Offset(0, dy+5);
				this.buttonHorizontal.SetManualBounds(rect);
				rect.Offset(dx, 0);
				this.buttonVertical.SetManualBounds(rect);

				rect = this.UsefulZone;
				rect.Width  = dx;
				rect.Height = dy;
				this.buttonFullScreen.SetManualBounds(rect);
				rect.Offset(dx, 0);
				this.buttonWindow.SetManualBounds(rect);
			}
		}


		static protected readonly bool		large = false;

		protected IconButton				buttonHorizontal;
		protected IconButton				buttonVertical;
		protected IconButton				buttonFullScreen;
		protected IconButton				buttonWindow;
	}
}
