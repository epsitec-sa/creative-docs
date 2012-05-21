using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Ribbons
{
	/// <summary>
	/// La classe Debug donne accès aux commandes provisoires de debug.
	/// </summary>
	public class Debug : Abstract
	{
		public Debug() : base()
		{
			this.Title = "Debug";
			this.PreferredWidth = 8 + 40;

			this.buttonOthers = this.CreateMenuButton("", "Debug menu...", this.HandleOthersPressed);
			
//			this.UpdateClientGeometry();
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
			//	Bouton pour ouvrir le menu des autres opérations.
			GlyphButton button = sender as GlyphButton;
			if ( button == null )  return;
			VMenu menu = this.BuildOthersMenu();
			if ( menu == null )  return;
			menu.Host = this;
			menu.MinWidth = button.ActualWidth;
			TextFieldCombo.AdjustComboSize(button, menu, false);
			menu.ShowAsComboList(button, Point.Zero, button);
		}

		protected VMenu BuildOthersMenu()
		{
			//	Construit le sous-menu des autres opérations.
			VMenu menu = new VMenu();

#if false
			this.MenuAdd(menu, "", "ResDesignerBuild", "Ressources Designer (build)", "");
			this.MenuAdd(menu, "", "ResDesignerTranslate", "Ressources Designer (translate)", "");
			this.MenuAdd(menu, "", "", "", "");
#endif
			this.MenuAdd(menu, "y/n", "DebugBboxThin", "Show BBoxThin", "");
			this.MenuAdd(menu, "y/n", "DebugBboxGeom", "Show BBoxGeom", "");
			this.MenuAdd(menu, "y/n", "DebugBboxFull", "Show BBoxFull", "");
			this.MenuAdd(menu, "", "", "", "");
			this.MenuAdd(menu, "", "DebugDirty", "Make dirty", "F12");
			this.MenuAdd(menu, "", "", "", "");
			this.MenuAdd(menu, "", "ForceSaveAll", "Save and overwrite all", "");
			this.MenuAdd(menu, "", "OverwriteAll", "Open, overwrite and close all", "");

			menu.AdjustSize();
			return menu;
		}

		
		protected GlyphButton				buttonOthers;
	}
}
