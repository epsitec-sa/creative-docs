using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Document;

namespace Epsitec.App.DocumentEditor.Dialogs
{
	using GlobalSettings = Common.Document.Settings.GlobalSettings;

	/// <summary>
	/// Classe de base.
	/// </summary>
	public abstract class Abstract
	{
		public Abstract(DocumentEditor editor)
		{
			this.editor = editor;
			this.globalSettings = editor.GlobalSettings;
			this.window = null;
		}

		// Cr�e et montre la fen�tre du dialogue.
		public virtual void Show()
		{
		}

		// Cache la fen�tre du dialogue.
		public virtual void Hide()
		{
			if ( this.window != null )
			{
				this.window.Hide();
			}
		}

		// Enregistre la position de la fen�tre du dialogue.
		public virtual void Save()
		{
		}

		// Reconstruit le dialogue.
		public virtual void Rebuild()
		{
		}


		// Initialise la fen�tre, � partir de la taille int�rieure,
		// c'est-�-dire sans le cadre.
		protected void WindowInit(string name, double dx, double dy)
		{
			this.WindowInit(name, dx, dy, false);
		}

		protected void WindowInit(string name, double dx, double dy, bool resizable)
		{
			this.window.ClientSize = new Size(dx, dy);
			dx = this.window.WindowSize.Width;
			dy = this.window.WindowSize.Height;  // taille avec le cadre

			Point location = new Point();
			Size size = new Size();
			if ( this.globalSettings.GetWindowBounds(name, ref location, ref size) )
			{
				if ( resizable )
				{
					this.window.ClientSize = size;
					dx = this.window.WindowSize.Width;
					dy = this.window.WindowSize.Height;  // taille avec le cadre
				}

				Rectangle rect = new Rectangle(location, new Size(dx, dy));
				rect = GlobalSettings.WindowClip(rect);
				this.window.WindowBounds = rect;
			}
			else
			{
				Rectangle cb = this.CurrentBounds;
				Rectangle rect = new Rectangle(cb.Center.X-dx/2, cb.Center.Y-dy/2, dx, dy);
				this.window.WindowBounds = rect;
			}
		}

		// Sauve la fen�tre.
		protected void WindowSave(string name)
		{
			if ( this.window == null )  return;
			this.globalSettings.SetWindowBounds(name, this.window.WindowLocation, this.window.ClientSize);
		}

		// Donne les fronti�res de l'application.
		protected Rectangle CurrentBounds
		{
			get
			{
				if ( this.editor.Window == null )
				{
					return this.globalSettings.MainWindow;
				}
				else
				{
					return new Rectangle(this.editor.Window.WindowLocation, this.editor.Window.WindowSize);
				}
			}
		}


		protected DocumentEditor				editor;
		protected GlobalSettings				globalSettings;
		protected Window						window;
	}
}
