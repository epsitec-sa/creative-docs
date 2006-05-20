using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Ribbons
{
	/// <summary>
	/// La classe PanelShow permet de gérer la sélection.
	/// </summary>
	public class PanelShow : Abstract
	{
		public PanelShow() : base()
		{
			this.Title = Res.Strings.Ribbon.Section.PanelShow;
			this.PreferredWidth = 8 + (22+5)*3 - 4;

			this.buttonShowGrid      = this.CreateIconButton("PanelShowGrid");
			this.buttonShowConstrain = this.CreateIconButton("PanelShowConstrain");
			this.buttonShowAnchor    = this.CreateIconButton("PanelShowAnchor");

			this.buttonShowExpand    = this.CreateIconButton("PanelShowExpand");
			this.buttonShowZOrder    = this.CreateIconButton("PanelShowZOrder");
			this.buttonShowTabIndex  = this.CreateIconButton("PanelShowTabIndex");

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

			if ( this.buttonShowGrid == null )  return;

			double dx = this.buttonShowGrid.PreferredWidth;
			double dy = this.buttonShowGrid.PreferredHeight;

			Rectangle rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(0, dy+5);
			this.buttonShowGrid.SetManualBounds(rect);
			rect.Offset(dx+5, 0);
			this.buttonShowConstrain.SetManualBounds(rect);
			rect.Offset(dx+5, 0);
			this.buttonShowAnchor.SetManualBounds(rect);

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			this.buttonShowExpand.SetManualBounds(rect);
			rect.Offset(dx+5, 0);
			this.buttonShowZOrder.SetManualBounds(rect);
			rect.Offset(dx+5, 0);
			this.buttonShowTabIndex.SetManualBounds(rect);
		}


		protected IconButton				buttonShowGrid;
		protected IconButton				buttonShowConstrain;
		protected IconButton				buttonShowAnchor;
		protected IconButton				buttonShowExpand;
		protected IconButton				buttonShowZOrder;
		protected IconButton				buttonShowTabIndex;
	}
}
