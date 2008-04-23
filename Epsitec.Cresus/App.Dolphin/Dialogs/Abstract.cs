//	Copyright © 2003-2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.App.Dolphin.Dialogs
{
	/// <summary>
	/// Classe de base.
	/// </summary>
	public abstract class Abstract
	{
		public Abstract(DolphinApplication application)
		{
			this.application = application;
			this.window = null;
		}

		public virtual void Show()
		{
			//	Crée et montre la fenêtre du dialogue.
		}

		public virtual void Hide()
		{
			//	Cache la fenêtre du dialogue.
			if (this.window != null)
			{
				this.window.Hide();
			}
		}

		public virtual void Save()
		{
			//	Enregistre la position de la fenêtre du dialogue.
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

		protected void WindowSave(string name)
		{
			//	Sauve la fenêtre.
		}

		protected Rectangle CurrentBounds
		{
			//	Donne les frontières de l'application.
			get
			{
				return new Rectangle(this.application.Window.WindowLocation, this.application.Window.WindowSize);
			}
		}


		protected virtual void CloseWindow()
		{
			//	Ferme la fenêtre du dialogue.
			this.application.Window.MakeActive();
			this.window.Hide();
			this.OnClosed();
		}

		
		protected virtual void OnClosed()
		{
			if ( this.Closed != null )
			{
				this.Closed(this);
			}
		}

		protected static void HandleLinkHypertextClicked(object sender, MessageEventArgs e)
		{
			Widget widget = sender as Widget;
			System.Diagnostics.Process.Start(widget.Hypertext);
		}

		
		public event Epsitec.Common.Support.EventHandler		Closed;

		
		protected DolphinApplication			application;
		protected Window						window;
	}
}
