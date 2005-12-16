using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.OpenType;
using Epsitec.Common.Document;

namespace Epsitec.App.DocumentEditor.Dialogs
{
	using GlobalSettings = Common.Document.Settings.GlobalSettings;

	/// <summary>
	/// Dialogue des informations sur le document.
	/// </summary>
	public class Splash : Abstract
	{
		public Splash(DocumentEditor editor) : base(editor)
		{
		}

		// Crée et montre la fenêtre du dialogue.
		public override void Show()
		{
			if ( this.window == null )
			{
				double dx = 400;
				double dy = 200;
				Rectangle mw = this.globalSettings.MainWindow;
				Rectangle wrect = new Rectangle(mw.Center.X-dx/2, mw.Center.Y-dy/2, dx, dy);

				this.window = new Window();
				this.window.MakeFramelessWindow();
				this.window.MakeTopLevelWindow();
				this.window.ClientSize = wrect.Size;
				this.window.WindowLocation = wrect.Location;
				this.window.PreventAutoClose = true;
				this.window.Root.PaintForeground += new PaintEventHandler(this.HandleSplashPaintForeground);
				this.window.Owner = this.editor.Window;

				StaticText image = About.CreateWidgetSplash(this.window.Root, this.editor.InstallType);
				image.Clicked += new MessageEventHandler(this.HandleSplashImageClicked);

				this.workInProgress = new StaticText(image);
				this.workInProgress.Bounds = new Rectangle(22, 50, 350, 20);

				this.splashTimer = new Timer();
				this.splashTimer.TimeElapsed += new EventHandler(this.HandleSplashTimerTimeElapsed);
				this.splashTimer.Delay = 10.0;
				this.splashTimer.Start();
				
				this.window.MakeLayeredWindow();
			}

			this.window.Show();
			
			Common.OpenType.FontIdentityCallback callback = new Common.OpenType.FontIdentityCallback(this.UpdateWorkInProgress);
			Common.Text.TextContext.InitializeFontCollection(callback);

			this.workInProgress.Text = "";  // efface le texte
		}


		private void UpdateWorkInProgress(Common.OpenType.FontIdentity fid)
		{
			string text = string.Format(Res.Strings.Dialog.Splash.WorkInProgress, fid.InvariantFaceName);
			this.workInProgress.Text = text;
		}

		private void HandleSplashPaintForeground(object sender, PaintEventArgs e)
		{
			WindowRoot root = sender as WindowRoot;
			double dx = root.Client.Width;
			double dy = root.Client.Height;
			Graphics graphics = e.Graphics;
			graphics.LineWidth = 1;
			graphics.AddRectangle(0.5, 0.5, dx-1, dy-1);
			graphics.RenderSolid(Color.FromBrightness(0));
		}

		private void HandleSplashImageClicked(object sender, MessageEventArgs e)
		{
			this.DeleteWindowSplash();
		}

		private void HandleSplashTimerTimeElapsed(object sender)
		{
			this.DeleteWindowSplash();
		}
		
		protected void DeleteWindowSplash()
		{
			if ( this.window == null )  return;

#if true
			this.window.WindowAnimationEnded += new Common.Support.EventHandler(this.HandleWindowAnimationEnded);
			this.window.AnimateHide(Common.Widgets.Animation.FadeOut);

			this.splashTimer.Stop();
			this.splashTimer.TimeElapsed -= new EventHandler(this.HandleSplashTimerTimeElapsed);
			this.splashTimer.Dispose();
			this.splashTimer = null;
#else
			this.window.Hide();
			this.window.Dispose();
			this.window = null;
			this.OnClosed();

			this.splashTimer.Stop();
			this.splashTimer.TimeElapsed -= new EventHandler(this.HandleSplashTimerTimeElapsed);
			this.splashTimer.Dispose();
			this.splashTimer = null;
#endif
		}

		private void HandleWindowAnimationEnded(object sender)
		{
			if ( this.window == null )  return;
			
			this.window.Hide();
			this.window.Dispose();
			this.window = null;
			this.OnClosed();
		}


		protected StaticText				workInProgress;
		protected Timer						splashTimer;
	}
}
