using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
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

				Point wLoc = this.globalSettings.WindowLocation;
				Size wSize = this.globalSettings.WindowSize;
				if ( wLoc.IsEmpty )
				{
					ScreenInfo si = ScreenInfo.Find(new Point(0,0));
					Rectangle wa = si.WorkingArea;
					wLoc = wa.Center-wSize/2;
				}
				Rectangle wrect = new Rectangle(wLoc, wSize);

				this.window = new Window();
				this.window.MakeFramelessWindow();
				this.window.MakeTopLevelWindow();
				this.window.ClientSize = new Size(dx, dy);
				this.window.WindowLocation = new Point(wrect.Center.X-dx/2, wrect.Center.Y-dy/2);
				this.window.PreventAutoClose = true;
				this.window.Root.PaintForeground += new PaintEventHandler(this.HandleSplashPaintForeground);
				this.window.Owner = this.editor.Window;

				StaticText image = About.CreateWidgetSplash(this.window.Root, this.editor.InstallType);
				image.Clicked += new MessageEventHandler(this.HandleSplashImageClicked);

				this.splashTimer = new Timer();
				this.splashTimer.TimeElapsed += new EventHandler(this.HandleSplashTimerTimeElapsed);
				this.splashTimer.Delay = 10.0;
				this.splashTimer.Start();
			}

			this.window.Show();
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

			this.window.Hide();
			this.window.Dispose();
			this.window = null;

			this.splashTimer.Stop();
			this.splashTimer.TimeElapsed -= new EventHandler(this.HandleSplashTimerTimeElapsed);
			this.splashTimer.Dispose();
			this.splashTimer = null;
		}


		protected Timer						splashTimer;
	}
}
