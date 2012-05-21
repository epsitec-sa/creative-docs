using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Ribbons
{
	/// <summary>
	/// La classe Global correspond au menu fichiers globaux.
	/// </summary>
	public class GlobalRibbon : AbstractRibbon
	{
		public GlobalRibbon(DesignerApplication designerApplication)
			: base (designerApplication)
		{
			this.Title = Res.Strings.Ribbon.Section.Global;
			this.PreferredWidth = 8 + 22*1.5*3;

			this.buttonInitialMessage = this.CreateIconButton ("InitialMessage", "Large");
			this.buttonSaveAll        = this.CreateIconButton ("SaveAll",        "Large");
			this.buttonSaveAllBitmaps = this.CreateIconButton ("SaveAllBitmaps", "Large");
			
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

			if (this.buttonSaveAll == null)  return;

			double dx = this.buttonSaveAll.PreferredWidth;
			double dy = this.buttonSaveAll.PreferredHeight;

			Rectangle rect = this.UsefulZone;
			rect.Width  = dx*1.5;
			rect.Height = dy*1.5;
			rect.Offset (0, dy*0.5);
			this.buttonInitialMessage.SetManualBounds (rect);
			rect.Offset (dx*1.5, 0);
			this.buttonSaveAll.SetManualBounds (rect);
			rect.Offset (dx*1.5, 0);
			this.buttonSaveAllBitmaps.SetManualBounds (rect);
		}


		protected IconButton				buttonInitialMessage;
		protected IconButton				buttonSaveAll;
		protected IconButton				buttonSaveAllBitmaps;
	}
}
