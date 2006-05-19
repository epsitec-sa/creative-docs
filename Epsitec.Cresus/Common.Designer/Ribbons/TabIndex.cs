using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Ribbons
{
	/// <summary>
	/// La classe TabIndex permet de choisir l'ordre de la sélection.
	/// </summary>
	public class TabIndex : Abstract
	{
		public TabIndex() : base()
		{
			this.Title = Res.Strings.Ribbon.Section.TabIndex;
			this.PreferredWidth = 8 + 22*1.5*2 + 4 + 22*1;

			this.buttonLast  = this.CreateIconButton("TabIndexLast", "Large");
			this.buttonFirst = this.CreateIconButton("TabIndexFirst", "Large");
			this.buttonNext  = this.CreateIconButton("TabIndexNext");
			this.buttonPrev  = this.CreateIconButton("TabIndexPrev");
			
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

			if ( this.buttonFirst == null )  return;

			double dx = this.buttonFirst.PreferredWidth;
			double dy = this.buttonFirst.PreferredHeight;

			Rectangle rect = this.UsefulZone;
			rect.Width  = dx*1.5;
			rect.Height = dy*1.5;
			rect.Offset(0, dy*0.5+5);
			this.buttonFirst.SetManualBounds(rect);
			rect.Offset(dx*1.5, -dy*0.5-5);
			this.buttonLast.SetManualBounds(rect);

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(dx*1.5*2+4, dy+5);
			this.buttonNext.SetManualBounds(rect);
			rect.Offset(0, -dy-5);
			this.buttonPrev.SetManualBounds(rect);
		}


		protected IconButton				buttonFirst;
		protected IconButton				buttonPrev;
		protected IconButton				buttonNext;
		protected IconButton				buttonLast;
	}
}
