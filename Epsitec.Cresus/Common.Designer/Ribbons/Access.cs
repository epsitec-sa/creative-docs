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

			this.buttonAccessFirst = this.CreateIconButton("AccessFirst");
			this.buttonAccessPrev = this.CreateIconButton("AccessPrev");
			this.buttonAccessNext = this.CreateIconButton("AccessNext");
			this.buttonAccessLast = this.CreateIconButton("AccessLast");
			this.buttonFilter = this.CreateIconButton("Filter");
			this.buttonSearch = this.CreateIconButton("Search");
			
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
				return 8 + 22*4;
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
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(0, dy+5);
			this.buttonAccessFirst.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonAccessPrev.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonAccessNext.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonAccessLast.Bounds = rect;

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			this.buttonFilter.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonSearch.Bounds = rect;
		}


		protected IconButton				buttonAccessFirst;
		protected IconButton				buttonAccessPrev;
		protected IconButton				buttonAccessNext;
		protected IconButton				buttonAccessLast;
		protected IconButton				buttonFilter;
		protected IconButton				buttonSearch;
	}
}
