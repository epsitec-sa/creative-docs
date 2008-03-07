using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Ribbons
{
	/// <summary>
	/// La classe File correspond au menu fichiers.
	/// </summary>
	public class File : Abstract
	{
		public File(DesignerApplication designerApplication) : base(designerApplication)
		{
			this.Title = Res.Strings.Ribbon.Section.File;
			this.PreferredWidth = 8 + 22*1.5*4;

			this.buttonNew    = this.CreateIconButton("New", "Large");
			this.buttonOpen   = this.CreateIconButton("Open", "Large");
			this.buttonCheck  = this.CreateIconButton("Check", "Large");
			this.buttonSave   = this.CreateIconButton("Save", "Large");
			
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

			if ( this.buttonNew == null )  return;

			double dx = this.buttonNew.PreferredWidth;
			double dy = this.buttonNew.PreferredHeight;

			Rectangle rect = this.UsefulZone;
			rect.Width  = dx*1.5;
			rect.Height = dy*1.5;
			rect.Offset(0, dy*0.5);
			this.buttonNew.SetManualBounds(rect);
			rect.Offset(dx*1.5, 0);
			this.buttonOpen.SetManualBounds(rect);
			rect.Offset(dx*1.5, 0);
			this.buttonCheck.SetManualBounds(rect);
			rect.Offset(dx*1.5, 0);
			this.buttonSave.SetManualBounds(rect);
		}


		protected IconButton				buttonNew;
		protected IconButton				buttonOpen;
		protected IconButton				buttonCheck;
		protected IconButton				buttonSave;
	}
}
