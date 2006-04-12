using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Ribbons
{
	/// <summary>
	/// La classe File correspond au menu fichiers.
	/// </summary>
	[SuppressBundleSupport]
	public class File : Abstract
	{
		public File() : base()
		{
			this.title.Text = Res.Strings.Ribbon.Section.File;

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

		public override double DefaultWidth
		{
			//	Retourne la largeur standard.
			get
			{
				return 8 + 22*1.5*2 + 4 + 22*1;
			}
		}


		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();

			if ( this.buttonNew == null )  return;

			double dx = this.buttonNew.DefaultWidth;
			double dy = this.buttonNew.DefaultHeight;

			Rectangle rect = this.UsefulZone;
			rect.Width  = dx*1.5;
			rect.Height = dy*1.5;
			rect.Offset(0, dy*0.5);
			this.buttonOpen.Bounds = rect;
			rect.Offset(dx*1.5, 0);
			this.buttonSave.Bounds = rect;

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(dx*1.5*2+4, dy+5);
			this.buttonNew.Bounds = rect;

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(dx*1.5*2+4, 0);
			this.buttonSaveAs.Bounds = rect;
		}


		protected IconButton				buttonOpen;
		protected IconButton				buttonSave;
		protected IconButton				buttonNew;
		protected IconButton				buttonSaveAs;
	}
}
