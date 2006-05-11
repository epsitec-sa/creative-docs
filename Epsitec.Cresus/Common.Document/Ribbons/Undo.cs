using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Ribbons
{
	/// <summary>
	/// La classe Undo gère les commandes undo/redo.
	/// </summary>
	public class Undo : Abstract
	{
		public Undo() : base()
		{
			this.Title = Res.Strings.Action.UndoMain;
			this.PreferredWidth = 8 + 22*1.5*2;

			this.buttonUndo = this.CreateIconButton("Undo", "Large");
			this.buttonRedo = this.CreateIconButton("Redo", "Large");
			this.buttonList = this.CreateMenuButton ("UndoRedoList", Res.Strings.Action.UndoRedoList, new MessageEventHandler (this.HandleListPressed));
			
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

			if ( this.buttonUndo == null )  return;

			double dx = this.buttonUndo.PreferredWidth;
			double dy = this.buttonUndo.PreferredHeight;

			Rectangle rect = this.UsefulZone;
			rect.Width  = dx*1.5;
			rect.Height = dy*1.5;
			rect.Offset(0, dy*0.5);
			this.buttonUndo.SetManualBounds(rect);
			rect.Offset(dx*1.5, 0);
			this.buttonRedo.SetManualBounds(rect);

			rect = this.UsefulZone;
			rect.Width  = dx*1.5*2;
			rect.Height = dy*0.5;
			this.buttonList.SetManualBounds(rect);
		}


		private void HandleListPressed(object sender, MessageEventArgs e)
		{
			//	Bouton pour ouvrir la liste cliqué.
			if ( this.document.Modifier.ActiveViewer.IsCreating )
			{
				return;
			}

			if ( !this.document.Modifier.OpletQueue.CanUndo &&
				 !this.document.Modifier.OpletQueue.CanRedo )
			{
				return;
			}

			GlyphButton button = sender as GlyphButton;
			if ( button == null )  return;
			VMenu menu = this.document.Modifier.CreateUndoRedoMenu(null);
			menu.Host = this;
			menu.MinWidth = button.ActualWidth;
			TextFieldCombo.AdjustComboSize(button, menu);
			menu.ShowAsComboList(button, Point.Zero, button);
		}


		protected IconButton				buttonUndo;
		protected IconButton				buttonRedo;
		protected GlyphButton				buttonList;
	}
}
