using NUnit.Framework;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;

namespace Epsitec.Common.Widgets
{
	[TestFixture]
	public class CustomWidgetTest
	{
		static CustomWidgetTest()
		{
		}
		
		[Test] public void AutomatedTestEnvironment()
		{
			Epsitec.Common.Widgets.Window.RunningInAutomatedTestEnvironment = true;
		}

		[Test] public void CheckCustomWidgetSimpleWindow()
		{
			Window window = new Window ();
			window.Text = "CheckCustomWidgetSimpleWindow";
			
			CustomWidget a = new CustomWidget ();
			CustomWidget b = new CustomWidget ();
			
			a.MouseCursor = MouseCursor.AsHand;
			b.MouseCursor = MouseCursor.AsIBeam;
			
			window.Root.Children.Add (a);
			window.Root.Children.Add (b);
			
			double x = window.ClientSize.Width - 90;

			a.Name = "A"; a.Margins = new Margins (0, 10, 0, 40); a.PreferredSize = new Size (80, 20); a.Text = "OK";     a.Anchor = AnchorStyles.BottomRight;
			b.Name = "B"; b.Margins = new Margins (0, 10, 0, 10); b.PreferredSize = new Size (80, 20); b.Text = "Cancel"; b.Anchor = AnchorStyles.BottomRight;
			
			a.Clicked += this.HandleWidgetClicked;
			b.Clicked += this.HandleWidgetClicked;
			a.Pressed += this.HandleWidgetPressed;
			b.Pressed += this.HandleWidgetPressed;
			a.Released += this.HandleWidgetReleased;
			b.Released += this.HandleWidgetReleased;
			a.Engaged += this.HandleWidgetEngaged;
			b.Engaged += this.HandleWidgetEngaged;
			a.Disengaged += this.HandleWidgetDisengaged;
			b.Disengaged += this.HandleWidgetDisengaged;
			a.StillEngaged += this.HandleWidgetStillEngaged;
			b.StillEngaged += this.HandleWidgetStillEngaged;
			
			a.AutoRepeat = true;
			b.AutoRepeat = false;
			
			window.Show ();
			Window.RunInTestEnvironment (window);
		}

		private void HandleWidgetClicked(object sender, MessageEventArgs e)
		{
			Widget widget = sender as Widget;
			
			System.Diagnostics.Debug.WriteLine ("Cliqué sur " + widget.Name + " avec le bouton " + e.Message.Button.ToString ());
		}

		private void HandleWidgetPressed(object sender, MessageEventArgs e)
		{
			Widget widget = sender as Widget;
			
			System.Diagnostics.Debug.WriteLine ("Souris pressée dans " + widget.Name + ", bouton " + e.Message.Button.ToString ());
		}
		
		private void HandleWidgetReleased(object sender, MessageEventArgs e)
		{
			Widget widget = sender as Widget;
			
			System.Diagnostics.Debug.WriteLine ("Souris relâchée dans " + widget.Name + ", bouton " + e.Message.Button.ToString ());
		}

		private void HandleWidgetEngaged(object sender)
		{
			Widget widget = sender as Widget;
			
			System.Diagnostics.Debug.WriteLine ("Pressé Widget " + widget.Name);
		}

		private void HandleWidgetDisengaged(object sender)
		{
			Widget widget = sender as Widget;
			
			System.Diagnostics.Debug.WriteLine ("Relâché Widget  " + widget.Name);
		}

		private void HandleWidgetStillEngaged(object sender)
		{
			Widget widget = sender as Widget;
			
			System.Diagnostics.Debug.WriteLine ("Toujours pressé Widget " + widget.Name);
		}
	}
	
	public class CustomWidget : Widget
	{
		public CustomWidget()
		{
			this.AutoFocus  = true;
			this.AutoEngage = true;
			this.AutoToggle = true;
			
			this.InternalState |= WidgetInternalState.Focusable;
			this.InternalState |= WidgetInternalState.Engageable;
		}
		
		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clip_rect)
		{
			Path path = new Path ();
			
			double dx = this.Client.Size.Width;
			double dy = this.Client.Size.Height;
			
			if (this.highlight)
			{
				graphics.ScaleTransform (1.03, 1.03, dx/2, dy/2);
			}
			
			path.MoveTo (5, 0);
			path.LineTo (dx-5, 0);
			path.CurveTo (dx, 0, dx, 5);
			path.LineTo (dx, dy-5);
			path.CurveTo (dx, dy, dx-5, dy);
			path.LineTo (5, dy);
			path.CurveTo (0, dy, 0, dy-5);
			path.LineTo (0, 5);
			path.CurveTo (0, 0, 5, 0);
			path.Close ();
			
			Color color_text = this.IsFocused ? Color.FromRgb (0.0, 0.0, 0.5) : Color.FromRgb (0.0, 0.0, 0.0);
			Color color_back = this.IsFocused ? Color.FromRgb (0.8, 0.8, 1.0) : Color.FromRgb (1.0, 1.0, 1.0);
			
			graphics.SolidRenderer.Color = color_back;
			graphics.Rasterizer.AddSurface (path);
			graphics.RenderSolid ();
			
			double x = 10;
			double y = dy * 0.25;
			
			Font   font = Font.GetFont ("Tahoma", "Regular");
			double size = dy * 0.8;
			
			graphics.SolidRenderer.Color = color_text;
			graphics.AddText (x, y, this.Text, font, size);
			graphics.Rasterizer.AddOutline (path, 0.6);
			graphics.RenderSolid ();
			
			if (this.ActiveState == ActiveState.Yes)
			{
				graphics.SolidRenderer.Color = Color.FromRgb (1.0, 0, 0);
				graphics.AddText (2, 2, dx-4, dy-4, "A", font, size, ContentAlignment.MiddleRight);
				graphics.RenderSolid ();
			}
			
			if (this.IsEngaged)
			{
				graphics.SolidRenderer.Color = Color.FromRgb (0, 0, 1.0);
				graphics.AddText (2, 2, dx-4, dy-4, "E  ", font, size, ContentAlignment.MiddleRight);
				graphics.RenderSolid ();
			}
			
			if (this.IsSelected)
			{
				graphics.SolidRenderer.Color = Color.FromRgb (0, 0.5, 0);
				graphics.AddText (2, 2, dx-4, dy-4, "S    ", font, size, ContentAlignment.MiddleRight);
				graphics.RenderSolid ();
			}
		}
		
		public override Margins GetShapeMargins()
		{
			double growth = System.Math.Max (this.Client.Size.Width, this.Client.Size.Height) * 0.03;
			double margin = 0.3 + (this.highlight ? growth : 0);

			return new Drawing.Margins (margin, margin, margin, margin);
		}

		
		protected override void ProcessMessage(Message message, Point pos)
		{
			switch (message.MessageType)
			{
				case MessageType.MouseEnter:
					this.highlight = true;
					this.Invalidate ();
					break;
				
				case MessageType.MouseDown:
					break;
				
				case MessageType.MouseUp:
					break;
				
				case MessageType.MouseLeave:
					this.Invalidate ();
					this.highlight = false;
					break;
				
				case MessageType.KeyPress:
					System.Diagnostics.Debug.WriteLine ("Key pressed: " + message.KeyChar + " in " + this.Name);
					
					if (message.IsAltPressed)
					{
						return;
					}
					if (message.IsControlPressed == false)
					{
						switch (message.KeyCode)
						{
							case KeyCode.Escape:
								this.Window.ClearFocusedWidget ();
								break;
							case KeyCode.Space:
								this.Toggle ();
								break;
						}
					}
					
					break;
			}
			
			message.Consumer = this;
		}
		
		protected override bool ProcessShortcut(Shortcut shortcut)
		{
			System.Diagnostics.Debug.WriteLine ("Shortcut pressed: " + shortcut.ToString () + " in " + this.Name);
			
			return base.ProcessShortcut (shortcut);
		}
		
		
		
		protected bool					highlight;
	}
}
