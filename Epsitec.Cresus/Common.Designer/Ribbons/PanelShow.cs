using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Ribbons
{
	/// <summary>
	/// La classe PanelShow permet de g�rer la s�lection.
	/// </summary>
	public class PanelShow : Abstract
	{
		public PanelShow() : base()
		{
			this.Title = Res.Strings.Ribbon.Section.PanelShow;
			this.PreferredWidth = 8 + (22+5)*2 - 5;

			this.buttonShowGrid     = this.CreateIconButton("PanelShowGrid");
			this.buttonShowExpand   = this.CreateIconButton("PanelShowExpand");
			this.buttonShowZOrder   = this.CreateIconButton("PanelShowZOrder");
			this.buttonShowTabIndex = this.CreateIconButton("PanelShowTabIndex");

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
			//	Met � jour la g�om�trie.
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
			this.buttonShowExpand.SetManualBounds(rect);

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			this.buttonShowZOrder.SetManualBounds(rect);
			rect.Offset(dx+5, 0);
			this.buttonShowTabIndex.SetManualBounds(rect);
		}


		protected IconButton				buttonShowGrid;
		protected IconButton				buttonShowExpand;
		protected IconButton				buttonShowZOrder;
		protected IconButton				buttonShowTabIndex;
	}
}
