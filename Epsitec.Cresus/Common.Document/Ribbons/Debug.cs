using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Ribbons
{
	/// <summary>
	/// La classe Debug donne acc�s aux commandes provisoires de debug.
	/// </summary>
	public class Debug : Abstract
	{
		public Debug() : base()
		{
			this.title.Text = "Debug";

			this.buttonOthers = this.CreateMenuButton ("", "Debug menu...", new MessageEventHandler (this.HandleOthersPressed));
			
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
				return 8 + 40;
			}
		}


		protected override void UpdateClientGeometry()
		{
			//	Met � jour la g�om�trie.
			base.UpdateClientGeometry();

			if ( this.buttonOthers == null )  return;

			double dx = this.buttonOthers.PreferredWidth;
			double dy = this.buttonOthers.PreferredHeight;

			Rectangle rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(0, dy);
			this.buttonOthers.SetManualBounds(rect);
		}


		private void HandleOthersPressed(object sender, MessageEventArgs e)
		{
			//	Bouton pour ouvrir le menu des autres op�rations.
			GlyphButton button = sender as GlyphButton;
			if ( button == null )  return;
			VMenu menu = this.BuildOthersMenu();
			if ( menu == null )  return;
			menu.Host = this;
			menu.MinWidth = button.ActualWidth;
			TextFieldCombo.AdjustComboSize(button, menu);
			menu.ShowAsComboList(button, Point.Zero, button);
		}

		protected VMenu BuildOthersMenu()
		{
			//	Construit le sous-menu des autres op�rations.
			VMenu menu = new VMenu();

			this.MenuAdd(menu, "", "ResDesignerBuild", "Ressources Designer (build)", "");
			this.MenuAdd(menu, "", "ResDesignerTranslate", "Ressources Designer (translate)", "");
			this.MenuAdd(menu, "", "", "", "");
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
