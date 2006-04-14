using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Ribbons
{
	/// <summary>
	/// La classe Access permet de choisir comment accéder aux ressources.
	/// </summary>
	[SuppressBundleSupport]
	public class Access : Abstract
	{
		public Access() : base()
		{
			this.title.Text = Res.Strings.Ribbon.Section.Access;

			this.buttonFilter = this.CreateIconButton("Filter");
			this.buttonSearch = this.CreateIconButton("Search");

			this.separator1 = new IconSeparator(this);

			this.buttonAccessFirst = this.CreateIconButton("AccessFirst");
			this.buttonAccessLast = this.CreateIconButton("AccessLast");

			this.buttonAccessPrev = this.CreateIconButton("AccessPrev");
			this.buttonAccessNext = this.CreateIconButton("AccessNext");

			this.separator2 = new IconSeparator(this);

			this.buttonWarningPrev = this.CreateIconButton("WarningPrev");
			this.buttonWarningNext = this.CreateIconButton("WarningNext");

			this.buttonModificationPrev = this.CreateIconButton("ModificationPrev");
			this.buttonModificationNext = this.CreateIconButton("ModificationNext");
			this.buttonModificationClear = this.CreateIconButton("ModificationClear");

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
				return 8 + 22*1 + this.separatorWidth + 22*2 + this.separatorWidth + 22*3;
			}
		}


		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();

			if ( this.buttonFilter == null )  return;

			double dx = this.buttonFilter.DefaultWidth;
			double dy = this.buttonFilter.DefaultHeight;

			Rectangle rect = this.UsefulZone;
			rect.Left += dx*1;
			rect.Width = this.separatorWidth;
			this.separator1.Bounds = rect;

			rect = this.UsefulZone;
			rect.Left += dx*1 + this.separatorWidth + dx*2;
			rect.Width = this.separatorWidth;
			this.separator2.Bounds = rect;

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(0, dy+5);
			this.buttonFilter.Bounds = rect;
			rect.Offset(dx+this.separatorWidth, 0);
			this.buttonAccessFirst.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonAccessPrev.Bounds = rect;
			rect.Offset(dx+this.separatorWidth, 0);
			this.buttonWarningPrev.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonModificationPrev.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonModificationClear.Bounds = rect;

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			this.buttonSearch.Bounds = rect;
			rect.Offset(dx+this.separatorWidth, 0);
			this.buttonAccessLast.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonAccessNext.Bounds = rect;
			rect.Offset(dx+this.separatorWidth, 0);
			this.buttonWarningNext.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonModificationNext.Bounds = rect;
		}


		protected IconButton				buttonFilter;
		protected IconButton				buttonSearch;
		protected IconSeparator				separator1;
		protected IconButton				buttonAccessFirst;
		protected IconButton				buttonAccessLast;
		protected IconButton				buttonAccessPrev;
		protected IconButton				buttonAccessNext;
		protected IconSeparator				separator2;
		protected IconButton				buttonWarningPrev;
		protected IconButton				buttonWarningNext;
		protected IconButton				buttonModificationPrev;
		protected IconButton				buttonModificationNext;
		protected IconButton				buttonModificationClear;
	}
}
