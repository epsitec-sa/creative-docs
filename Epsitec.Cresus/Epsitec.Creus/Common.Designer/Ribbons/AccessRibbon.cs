using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Ribbons
{
	/// <summary>
	/// La classe Access permet de choisir comment accéder aux ressources.
	/// </summary>
	public class AccessRibbon : AbstractRibbon
	{
		public AccessRibbon(DesignerApplication designerApplication) : base(designerApplication)
		{
			this.Title = Res.Strings.Ribbon.Section.Access;
			this.PreferredWidth = 8 + 22*1.5*1 + 4 + 22*3 + this.separatorWidth + 22*2 + 4 + 22*1.5*1;

			this.buttonSearch = this.CreateIconButton("Search", "Large");

			this.buttonSearchPrev = this.CreateIconButton("SearchPrev");
			this.buttonSearchNext = this.CreateIconButton("SearchNext");

			this.buttonAccessFirst = this.CreateIconButton("AccessFirst");
			this.buttonAccessLast = this.CreateIconButton("AccessLast");

			this.buttonAccessPrev = this.CreateIconButton("AccessPrev");
			this.buttonAccessNext = this.CreateIconButton("AccessNext");

			this.separator1 = new IconSeparator(this);

			this.buttonModificationPrev  = this.CreateIconButton("ModificationPrev");
			this.buttonModificationNext  = this.CreateIconButton("ModificationNext");
			this.buttonModificationAll   = this.CreateIconButton("ModificationAll");
			this.buttonModificationClear = this.CreateIconButton("ModificationClear", "Large");

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

			if ( this.buttonSearch == null )  return;

			double dx = this.buttonSearch.PreferredWidth;
			double dy = this.buttonSearch.PreferredHeight;

			Rectangle rect = this.UsefulZone;
			rect.Left += dx*1.5*1+4 + dx*3;
			rect.Width = this.separatorWidth;
			this.separator1.SetManualBounds(rect);

			rect = this.UsefulZone;
			rect.Width  = dx*1.5;
			rect.Height = dy*1.5;
			rect.Offset(0, dy*0.5);
			this.buttonSearch.SetManualBounds(rect);
			rect.Offset(dx*1.5+8+dx*5+this.separatorWidth, 0);
			this.buttonModificationClear.SetManualBounds(rect);

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(dx*1.5*1+4, dy+5);
			this.buttonSearchPrev.SetManualBounds(rect);
			rect.Offset(dx, 0);
			this.buttonAccessFirst.SetManualBounds(rect);
			rect.Offset(dx, 0);
			this.buttonAccessPrev.SetManualBounds(rect);
			rect.Offset(dx+this.separatorWidth, 0);
			this.buttonModificationPrev.SetManualBounds(rect);
			rect.Offset(dx, 0);
			this.buttonModificationAll.SetManualBounds(rect);

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(dx*1.5*1+4, 0);
			this.buttonSearchNext.SetManualBounds(rect);
			rect.Offset(dx, 0);
			this.buttonAccessLast.SetManualBounds(rect);
			rect.Offset(dx, 0);
			this.buttonAccessNext.SetManualBounds(rect);
			rect.Offset(dx+this.separatorWidth, 0);
			this.buttonModificationNext.SetManualBounds(rect);
		}


		protected IconButton				buttonSearch;
		protected IconButton				buttonSearchPrev;
		protected IconButton				buttonSearchNext;
		protected IconButton				buttonAccessFirst;
		protected IconButton				buttonAccessLast;
		protected IconButton				buttonAccessPrev;
		protected IconButton				buttonAccessNext;
		protected IconSeparator				separator1;
		protected IconButton				buttonModificationPrev;
		protected IconButton				buttonModificationNext;
		protected IconButton				buttonModificationAll;
		protected IconButton				buttonModificationClear;
	}
}
