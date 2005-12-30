using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Ribbons
{
	/// <summary>
	/// La classe Undo g�re les commandes undo/redo.
	/// </summary>
	[SuppressBundleSupport]
	public class Undo : Abstract
	{
		public Undo() : base()
		{
			this.title.Text = Res.Strings.Action.UndoMain;

			this.buttonUndo = this.CreateIconButton("Undo", "Big");
			this.buttonRedo = this.CreateIconButton("Redo", "Big");
			this.buttonList = this.CreateMenuButton("UndoRedoList", Res.Strings.Action.UndoRedoList, new MessageEventHandler(this.HandleListClicked));
			
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
				return 8 + 22*1.5*2;
			}
		}


		protected override void UpdateClientGeometry()
		{
			//	Met � jour la g�om�trie.
			base.UpdateClientGeometry();

			if ( this.buttonUndo == null )  return;

			double dx = this.buttonUndo.DefaultWidth;
			double dy = this.buttonUndo.DefaultHeight;

			Rectangle rect = this.UsefulZone;
			rect.Width  = dx*1.5;
			rect.Height = dy*1.5;
			rect.Offset(0, dy*0.5);
			this.buttonUndo.Bounds = rect;
			rect.Offset(dx*1.5, 0);
			this.buttonRedo.Bounds = rect;

			rect = this.UsefulZone;
			rect.Width  = dx*1.5*2;
			rect.Height = dy*0.5;
			this.buttonList.Bounds = rect;
		}


		private void HandleListClicked(object sender, MessageEventArgs e)
		{
			//	Bouton pour ouvrir la liste cliqu�.
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
			Point pos = button.MapClientToScreen(new Point(0, 1));
			VMenu menu = this.document.Modifier.CreateUndoRedoMenu(null);
			menu.Host = this;
			menu.ShowAsContextMenu(this.Window, pos);
		}


		protected IconButton				buttonUndo;
		protected IconButton				buttonRedo;
		protected GlyphButton				buttonList;
	}
}
