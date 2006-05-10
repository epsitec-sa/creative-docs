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
		public File() : base()
		{
			this.Title = Res.Strings.Ribbon.Section.File;
			this.PreferredWidth = 8 + 22*1.5*2 + 4 + 22*1;

			this.buttonOpen   = this.CreateIconButton("Open", "Large");
			this.buttonSave   = this.CreateIconButton("Save", "Large");
			this.buttonNew    = this.CreateIconButton("New");
			this.buttonSaveAs = this.CreateIconButton("SaveAs");
			
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
			this.buttonOpen.SetManualBounds(rect);
			rect.Offset(dx*1.5, 0);
			this.buttonSave.SetManualBounds(rect);

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(dx*1.5*2+4, dy+5);
			this.buttonNew.SetManualBounds(rect);

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(dx*1.5*2+4, 0);
			this.buttonSaveAs.SetManualBounds(rect);
		}


		protected IconButton				buttonOpen;
		protected IconButton				buttonSave;
		protected IconButton				buttonNew;
		protected IconButton				buttonSaveAs;
	}
}
