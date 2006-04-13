using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Ribbons
{
	/// <summary>
	/// La classe Select permet de gérer la sélection.
	/// </summary>
	[SuppressBundleSupport]
	public class Select : Abstract
	{
		public Select() : base()
		{
			this.title.Text = Res.Strings.Ribbon.Section.Select;

			this.buttonDelete    = this.CreateIconButton("Delete", "Large");
			this.buttonDuplicate = this.CreateIconButton("Duplicate", "Large");
			this.buttonUp        = this.CreateIconButton("Up");
			this.buttonDown      = this.CreateIconButton("Down");

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

			if ( this.buttonDelete == null )  return;

			double dx = this.buttonDelete.DefaultWidth;
			double dy = this.buttonDelete.DefaultHeight;

			Rectangle rect = this.UsefulZone;
			rect.Width  = dx*1.5;
			rect.Height = dy*1.5;
			rect.Offset(0, dy*0.5);
			this.buttonDelete.Bounds = rect;
			rect.Offset(dx*1.5, 0);
			this.buttonDuplicate.Bounds = rect;

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(dx*1.5*2+4, dy+5);
			this.buttonUp.Bounds = rect;

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(dx*1.5*2+4, 0);
			this.buttonDown.Bounds = rect;
		}


		protected IconButton				buttonDelete;
		protected IconButton				buttonDuplicate;
		protected IconButton				buttonUp;
		protected IconButton				buttonDown;
	}
}
