using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Dialogs
{
	/// <summary>
	/// Classe de base.
	/// </summary>
	public abstract class Abstract
	{
		public Abstract(DesignerApplication mainWindow)
		{
			this.mainWindow = mainWindow;
			this.parentWindow = mainWindow.Window;
		}

		public virtual void Show()
		{
			//	Cr�e et montre la fen�tre du dialogue.
		}

		public virtual void Hide()
		{
			//	Cache la fen�tre du dialogue.
			if ( this.window != null )
			{
				this.window.Hide();
			}
		}


		protected void WindowInit(string name, double dx, double dy)
		{
			//	Initialise la fen�tre, � partir de la taille int�rieure,
			//	c'est-�-dire sans le cadre.
			this.WindowInit(name, dx, dy, false);
		}

		protected void WindowInit(string name, double dx, double dy, bool resizable)
		{
			this.window.ClientSize = new Size(dx, dy);
			dx = this.window.WindowSize.Width;
			dy = this.window.WindowSize.Height;  // taille avec le cadre

			Rectangle cb = this.CurrentBounds;
			Rectangle rect = new Rectangle(cb.Center.X-dx/2, cb.Center.Y-dy/2, dx, dy);
			this.window.WindowBounds = rect;
		}

		protected Rectangle CurrentBounds
		{
			//	Donne les fronti�res de l'application.
			get
			{
				return new Rectangle(this.parentWindow.WindowLocation, this.parentWindow.WindowSize);
			}
		}


		protected virtual void OnClosed()
		{
			if ( this.Closed != null )
			{
				this.Closed(this);
			}
		}

		public event Support.EventHandler		Closed;


		protected DesignerApplication					mainWindow;
		protected Window						parentWindow;
		protected Window						window;
		protected bool							ignoreChanged = false;
	}
}
