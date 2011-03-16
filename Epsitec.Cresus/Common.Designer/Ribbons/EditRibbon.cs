using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Ribbons
{
	/// <summary>
	/// La classe Select permet de gérer la sélection.
	/// </summary>
	public class EditRibbon : AbstractRibbon
	{
		public EditRibbon(DesignerApplication designerApplication) : base(designerApplication)
		{
			this.Title = Res.Strings.Ribbon.Section.Edit;
			this.PreferredWidth = 8 + 22*1.5*6;

			this.buttonLocked    = this.CreateIconButton("EditLocked", "Large");
			this.buttonOk        = this.CreateIconButton("EditOk", "Large");
			this.buttonCancel    = this.CreateIconButton("EditCancel", "Large");
			this.buttonDelete    = this.CreateIconButton("Delete", "Large");
			this.buttonCreate    = this.CreateIconButton("Create", "Large");
			this.buttonDuplicate = this.CreateIconButton("Duplicate", "Large");

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
			this.buttonLocked.SetManualBounds(rect);
			rect.Offset(dx*1.5, 0);
			this.buttonOk.SetManualBounds(rect);
			rect.Offset(dx*1.5, 0);
			this.buttonCancel.SetManualBounds(rect);
			rect.Offset(dx*1.5, 0);
			this.buttonDelete.SetManualBounds(rect);
			rect.Offset(dx*1.5, 0);
			this.buttonCreate.SetManualBounds(rect);
			rect.Offset(dx*1.5, 0);
			this.buttonDuplicate.SetManualBounds(rect);
		}


		protected IconButton				buttonLocked;
		protected IconButton				buttonOk;
		protected IconButton				buttonCancel;
		protected IconButton				buttonDelete;
		protected IconButton				buttonCreate;
		protected IconButton				buttonDuplicate;
	}
}
