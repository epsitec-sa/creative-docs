using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Ribbons
{
	/// <summary>
	/// La classe Undo permet de gérer les groupes.
	/// </summary>
	[SuppressBundleSupport]
	public class Undo : Abstract
	{
		public Undo() : base()
		{
			this.title.Text = "Undo";

			this.buttonUndo = this.CreateIconButton("Undo", Misc.Icon("Undo2"), Res.Strings.Action.Undo);
			this.buttonRedo = this.CreateIconButton("Redo", Misc.Icon("Redo2"), Res.Strings.Action.Redo);

			this.buttonList = new GlyphButton(this);
			this.buttonList.ButtonStyle = ButtonStyle.ToolItem;
			this.buttonList.GlyphShape = GlyphShape.Menu;
			this.buttonList.Clicked += new MessageEventHandler(this.HandleListClicked);
			ToolTip.Default.SetToolTip(this.buttonList, Res.Strings.Action.UndoRedoList);
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
				return 8 + 22*1.5*2;
			}
		}


		// Met à jour la géométrie.
		protected override void UpdateClientGeometry()
		{
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


		// Bouton pour ouvrir la liste cliqué.
		private void HandleListClicked(object sender, MessageEventArgs e)
		{
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
