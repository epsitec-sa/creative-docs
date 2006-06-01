using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Ribbons
{
	/// <summary>
	/// La classe PanelSelect permet de gérer la sélection.
	/// </summary>
	public class PanelSelect : Abstract
	{
		public PanelSelect(MainWindow mainWindow) : base(mainWindow)
		{
			this.Title = Res.Strings.Ribbon.Section.PanelSelect;
			this.PreferredWidth = 8 + 22*1.5*2 + 4 + 22*3;

			this.buttonDelete       = this.CreateIconButton("PanelDelete", "Large");
			this.buttonDuplicate    = this.CreateIconButton("PanelDuplicate", "Large");
			this.buttonDeselectAll  = this.CreateIconButton("PanelDeselectAll");
			this.buttonSelectAll    = this.CreateIconButton("PanelSelectAll");
			this.buttonSelectInvert = this.CreateIconButton("PanelSelectInvert");

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

			if ( this.buttonDelete == null )  return;

			double dx = this.buttonDeselectAll.PreferredWidth;
			double dy = this.buttonDeselectAll.PreferredHeight;

			Rectangle rect = this.UsefulZone;
			rect.Width  = dx*1.5;
			rect.Height = dy*1.5;
			rect.Offset(0, dy*0.5);
			this.buttonDelete.SetManualBounds(rect);
			rect.Offset(dx*1.5, 0);
			this.buttonDuplicate.SetManualBounds(rect);

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(dx*1.5*2+4, dy+5);
			this.buttonDeselectAll.SetManualBounds(rect);
			rect.Offset(dx, 0);
			this.buttonSelectAll.SetManualBounds(rect);
			rect.Offset(dx, 0);
			this.buttonSelectInvert.SetManualBounds(rect);
		}


		protected IconButton				buttonDelete;
		protected IconButton				buttonDuplicate;
		protected IconButton				buttonDeselectAll;
		protected IconButton				buttonSelectAll;
		protected IconButton				buttonSelectInvert;
	}
}
