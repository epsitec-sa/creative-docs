using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Ribbons
{
	/// <summary>
	/// La classe Debug donne accès aux commandes provisoires de debug.
	/// </summary>
	[SuppressBundleSupport]
	public class Debug : Abstract
	{
		public Debug() : base()
		{
			this.title.Text = "Debug";

			this.buttonOthers = this.CreateMenuButton("", "Debug menu...", new MessageEventHandler(this.HandleOthersClicked));
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
			}
			
			base.Dispose(disposing);
		}

		// Retourne la largeur standard.
		public override double DefaultWidth
		{
			get
			{
				return 8 + 40;
			}
		}


		// Met à jour la géométrie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.buttonOthers == null )  return;

			double dx = this.buttonOthers.DefaultWidth;
			double dy = this.buttonOthers.DefaultHeight;

			Rectangle rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(0, dy);
			this.buttonOthers.Bounds = rect;
		}


		// Bouton pour ouvrir le menu des autres opérations.
		private void HandleOthersClicked(object sender, MessageEventArgs e)
		{
			GlyphButton button = sender as GlyphButton;
			if ( button == null )  return;
			Point pos = button.MapClientToScreen(new Point(0, 1));
			VMenu menu = this.BuildOthersMenu();
			if ( menu == null )  return;
			menu.Host = this;
			menu.ShowAsContextMenu(this.Window, pos);
		}

		// Construit le sous-menu des autres opérations.
		protected VMenu BuildOthersMenu()
		{
			VMenu menu = new VMenu();

			this.MenuAdd(menu, "y/n", "DebugBboxThin", "Show BBoxThin", "");
			this.MenuAdd(menu, "y/n", "DebugBboxGeom", "Show BBoxGeom", "");
			this.MenuAdd(menu, "y/n", "DebugBboxFull", "Show BBoxFull", "");
			this.MenuAdd(menu, "", "", "", "");
			this.MenuAdd(menu, "", "DebugDirty", "Make dirty", "F12");
			this.MenuAdd(menu, "", "", "", "");
			this.MenuAdd(menu, "", "ForceSaveAll", "Save and overwrite all", "");

			menu.AdjustSize();
			return menu;
		}

		
		protected GlyphButton				buttonOthers;
	}
}
