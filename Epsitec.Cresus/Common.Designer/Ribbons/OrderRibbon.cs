using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Ribbons
{
	/// <summary>
	/// La classe Order permet de choisir l'ordre de la sélection.
	/// </summary>
	public class OrderRibbon : AbstractRibbon
	{
		public OrderRibbon(DesignerApplication designerApplication) : base(designerApplication)
		{
			this.Title = Res.Strings.Ribbon.Section.Order;
			this.PreferredWidth = 8 + 22*2;

			this.buttonDownAll = this.CreateIconButton("PanelOrderDownAll");
			this.buttonDownOne = this.CreateIconButton("PanelOrderDownOne");
			this.buttonUpOne   = this.CreateIconButton("PanelOrderUpOne");
			this.buttonUpAll   = this.CreateIconButton("PanelOrderUpAll");
			
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

			if ( this.buttonDownAll == null )  return;

			double dx = this.buttonDownAll.PreferredWidth;
			double dy = this.buttonDownAll.PreferredHeight;

			Rectangle rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(0, dy+5);
			this.buttonDownAll.SetManualBounds(rect);
			rect.Offset(dx, 0);
			this.buttonUpAll.SetManualBounds(rect);

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			this.buttonDownOne.SetManualBounds(rect);
			rect.Offset(dx, 0);
			this.buttonUpOne.SetManualBounds(rect);
		}


		protected IconButton				buttonDownAll;
		protected IconButton				buttonDownOne;
		protected IconButton				buttonUpOne;
		protected IconButton				buttonUpAll;
	}
}
