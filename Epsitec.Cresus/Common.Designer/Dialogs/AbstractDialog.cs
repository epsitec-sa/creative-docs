using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Dialogs
{
	/// <summary>
	/// Classe de base.
	/// </summary>
	public abstract class AbstractDialog
	{
		public AbstractDialog(DesignerApplication designerApplication)
		{
			this.designerApplication = designerApplication;
			this.parentWindow = designerApplication.Window;
		}

		public virtual void Show()
		{
			//	Crée et montre la fenêtre du dialogue.
		}

		public virtual void Hide()
		{
			//	Cache la fenêtre du dialogue.
			if ( this.window != null )
			{
				this.window.Hide();
			}
		}


		protected void WindowInit(string name, double dx, double dy)
		{
			//	Initialise la fenêtre, à partir de la taille intérieure,
			//	c'est-à-dire sans le cadre.
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
			//	Donne les frontières de l'application.
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


		protected DesignerApplication			designerApplication;
		protected Window						parentWindow;
		protected Window						window;
		protected bool							ignoreChanged = false;
	}
}
