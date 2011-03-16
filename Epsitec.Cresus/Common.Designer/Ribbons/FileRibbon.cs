using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Ribbons
{
	/// <summary>
	/// La classe File correspond au menu fichiers.
	/// </summary>
	public class FileRibbon : AbstractRibbon
	{
		public FileRibbon(DesignerApplication designerApplication) : base(designerApplication)
		{
			this.Title = Res.Strings.Ribbon.Section.File;
			this.PreferredWidth = 8 + 22*1.5*4 + 4 + 22*1;

			this.buttonOpen           = this.CreateIconButton ("Open", "Large");
			this.buttonInitialMessage = this.CreateIconButton ("InitialMessage", "Large");
			this.buttonCheck          = this.CreateIconButton ("Check", "Large");
			this.buttonSave           = this.CreateIconButton ("Save", "Large");
			this.buttonNew            = this.CreateIconButton ("New");
			this.buttonRecycle        = this.CreateIconButton ("Recycle");
			
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

			if ( this.buttonOpen == null )  return;

			double dx = this.buttonOpen.PreferredWidth;
			double dy = this.buttonOpen.PreferredHeight;

			Rectangle rect = this.UsefulZone;
			rect.Width  = dx*1.5;
			rect.Height = dy*1.5;
			rect.Offset (0, dy*0.5);
			this.buttonOpen.SetManualBounds (rect);
			rect.Offset (dx*1.5, 0);
			this.buttonInitialMessage.SetManualBounds (rect);
			rect.Offset (dx*1.5, 0);
			this.buttonCheck.SetManualBounds (rect);
			rect.Offset (dx*1.5, 0);
			this.buttonSave.SetManualBounds (rect);

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset (dx*1.5*4+4, dy+5);
			this.buttonNew.SetManualBounds (rect);
			rect.Offset (0, -dx-5);
			this.buttonRecycle.SetManualBounds (rect);
		}


		protected IconButton				buttonOpen;
		protected IconButton				buttonInitialMessage;
		protected IconButton				buttonCheck;
		protected IconButton				buttonSave;
		protected IconButton				buttonNew;
		protected IconButton				buttonRecycle;
	}
}
