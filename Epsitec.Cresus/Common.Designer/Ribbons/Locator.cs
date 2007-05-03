using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Ribbons
{
	/// <summary>
	/// La classe Locator correspond au menu fichiers.
	/// </summary>
	public class Locator : Abstract
	{
		public Locator(MainWindow mainWindow) : base(mainWindow)
		{
			this.Title = Res.Strings.Ribbon.Section.Locator;
			this.PreferredWidth = 8 + 22*1.5*2;

			this.buttonPrev = this.CreateIconButton("LocatorPrev", "Large");
			this.buttonNext = this.CreateIconButton("LocatorNext", "Large");
			
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

			if ( this.buttonPrev == null )  return;

			double dx = this.buttonPrev.PreferredWidth;
			double dy = this.buttonPrev.PreferredHeight;

			Rectangle rect = this.UsefulZone;
			rect.Width  = dx*1.5;
			rect.Height = dy*1.5;
			rect.Offset(0, dy*0.5);
			this.buttonPrev.SetManualBounds(rect);
			rect.Offset(dx*1.5, 0);
			this.buttonNext.SetManualBounds(rect);
		}


		protected IconButton				buttonPrev;
		protected IconButton				buttonNext;
	}
}
