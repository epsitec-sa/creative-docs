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
			this.PreferredWidth = 8 + 22*4;

			this.buttonFirst = this.CreateIconButton("TabIndexFirst");
			this.buttonPrev  = this.CreateIconButton("TabIndexPrev");
			this.buttonNext  = this.CreateIconButton("TabIndexNext");
			this.buttonLast  = this.CreateIconButton("TabIndexLast");
			
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
			rect.Width  = dx;
			rect.Height = dy;
			//?rect.Offset(0, dy+5);
			this.buttonFirst.SetManualBounds(rect);
			rect.Offset(dx, 0);
			this.buttonPrev.SetManualBounds(rect);
			rect.Offset(dx, 0);
			this.buttonNext.SetManualBounds(rect);
			rect.Offset(dx, 0);
			this.buttonLast.SetManualBounds(rect);
		}


		protected IconButton				buttonFirst;
		protected IconButton				buttonPrev;
		protected IconButton				buttonNext;
		protected IconButton				buttonLast;
	}
}
