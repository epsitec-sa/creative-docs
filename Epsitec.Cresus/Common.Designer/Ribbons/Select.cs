using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Ribbons
{
	/// <summary>
	/// La classe Select permet de gérer la sélection.
	/// </summary>
	public class Select : Abstract
	{
		public Select() : base()
		{
			this.Title = Res.Strings.Ribbon.Section.Select;
			this.PreferredWidth = 8 + 22*1.5*2 + 4 + 22*2;

			this.buttonCreate    = this.CreateIconButton("Create", "Large");
			this.buttonDuplicate = this.CreateIconButton("Duplicate", "Large");
			this.buttonDelete    = this.CreateIconButton("Delete");
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


		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();

			if ( this.buttonDelete == null )  return;

			double dx = this.buttonDelete.PreferredWidth;
			double dy = this.buttonDelete.PreferredHeight;

			Rectangle rect = this.UsefulZone;
			rect.Width  = dx*1.5;
			rect.Height = dy*1.5;
			rect.Offset(0, dy*0.5);
			this.buttonCreate.SetManualBounds(rect);
			rect.Offset(dx*1.5, 0);
			this.buttonDuplicate.SetManualBounds(rect);

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(dx*1.5*2+4, dy+5);
			this.buttonDelete.SetManualBounds(rect);
			rect.Offset(dx, 0);
			this.buttonUp.SetManualBounds(rect);

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(dx*1.5*2+4+dx, 0);
			this.buttonDown.SetManualBounds(rect);
		}


		protected IconButton				buttonCreate;
		protected IconButton				buttonDuplicate;
		protected IconButton				buttonDelete;
		protected IconButton				buttonUp;
		protected IconButton				buttonDown;
	}
}
