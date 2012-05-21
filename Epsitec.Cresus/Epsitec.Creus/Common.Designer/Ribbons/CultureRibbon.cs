using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Ribbons
{
	/// <summary>
	/// La classe Culture permet de gérer les cultures.
	/// </summary>
	public class CultureRibbon : AbstractRibbon
	{
		public CultureRibbon(DesignerApplication designerApplication) : base(designerApplication)
		{
			this.Title = Res.Strings.Ribbon.Section.Culture;
			this.PreferredWidth = 8 + 22*2;

			this.buttonNewCulture    = this.CreateIconButton("NewCulture");
			this.buttonDeleteCulture = this.CreateIconButton("DeleteCulture");
			
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

			if ( this.buttonNewCulture == null )  return;

			double dx = this.buttonNewCulture.PreferredWidth;
			double dy = this.buttonNewCulture.PreferredHeight;

			Rectangle rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(0, dy+5);
			this.buttonNewCulture.SetManualBounds(rect);
			rect.Offset(dx, 0);
			this.buttonDeleteCulture.SetManualBounds(rect);
		}


		protected IconButton				buttonNewCulture;
		protected IconButton				buttonDeleteCulture;
	}
}
