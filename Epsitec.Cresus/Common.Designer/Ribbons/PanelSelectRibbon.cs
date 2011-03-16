using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Ribbons
{
	/// <summary>
	/// La classe PanelSelect permet de gérer la sélection.
	/// </summary>
	public class PanelSelectRibbon : AbstractRibbon
	{
		public PanelSelectRibbon(DesignerApplication designerApplication) : base(designerApplication)
		{
			this.Title = Res.Strings.Ribbon.Section.PanelSelect;
			this.PreferredWidth = 8 + 22*3;

			this.buttonDeselectAll  = this.CreateIconButton("PanelDeselectAll");
			this.buttonSelectAll    = this.CreateIconButton("PanelSelectAll");
			this.buttonSelectInvert = this.CreateIconButton("PanelSelectInvert");
			this.buttonSelectRoot   = this.CreateIconButton("PanelSelectRoot");
			this.buttonSelectParent = this.CreateIconButton("PanelSelectParent");

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

			if ( this.buttonDeselectAll == null )  return;

			double dx = this.buttonDeselectAll.PreferredWidth;
			double dy = this.buttonDeselectAll.PreferredHeight;

			Rectangle rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(0, dy+5);
			this.buttonDeselectAll.SetManualBounds(rect);
			rect.Offset(dx, 0);
			this.buttonSelectAll.SetManualBounds(rect);
			rect.Offset(dx, 0);
			this.buttonSelectInvert.SetManualBounds(rect);

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			this.buttonSelectRoot.SetManualBounds(rect);
			rect.Offset(dx, 0);
			this.buttonSelectParent.SetManualBounds(rect);
		}


		protected IconButton				buttonDeselectAll;
		protected IconButton				buttonSelectAll;
		protected IconButton				buttonSelectInvert;
		protected IconButton				buttonSelectRoot;
		protected IconButton				buttonSelectParent;
	}
}
