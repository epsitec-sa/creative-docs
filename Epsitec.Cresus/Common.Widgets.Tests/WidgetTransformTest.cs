using NUnit.Framework;
using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Tests
{
	[TestFixture]
	public class WidgetTransformTest
	{
		static WidgetTransformTest()
		{
		}
		
		[Test] public void CheckWidgetTransform()
		{
			Window window = new Window ();
			window.Text = "CheckWidgetTransform";
			
			TransformWidget a = new TransformWidget ();	a.Text = "A"; a.Bounds = new Rectangle (10, 10, 270, 200);
			TransformWidget b = new TransformWidget (); b.Text = "B"; b.Bounds = new Rectangle (20, 20, 230, 160); b.SetClientAngle (90);
			TransformWidget c = new TransformWidget (); c.Text = "C"; c.Bounds = new Rectangle (20, 20, 120,  60); c.SetClientAngle (-90);
			
			a.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.TopAndBottom;
			b.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.TopAndBottom;
			c.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.Bottom;
			
			window.Root.Children.Add (a);
			a.Children.Add (b);
			b.Children.Add (c);
			
			window.Show ();
		}
	}
	
	public class TransformWidget : Widget
	{
		public TransformWidget()
		{
			this.internalState |= InternalState.AutoFocus;
			this.internalState |= InternalState.AutoEngage;
			this.internalState |= InternalState.Focusable;
			this.internalState |= InternalState.Engageable;
			this.internalState |= InternalState.AutoToggle;
		}
		
		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clip_rect)
		{
			double dx = this.Client.Width;
			double dy = this.Client.Height;
			
			Color color_text = this.IsFocused ? Color.FromRGB (0.0, 0.0, 0.5) : Color.FromRGB (0.0, 0.0, 0.0);
			Color color_back = this.IsFocused ? Color.FromRGB (0.8, 0.8, 1.0) : Color.FromRGB (1.0, 1.0, 1.0);
			
			graphics.AddFilledRectangle (1, 1, dx - 2, dy - 2);
			graphics.RenderSolid (color_back);
			graphics.AddRectangle (1, 1, dx-2, dy-2);
			graphics.RenderSolid (Color.FromRGB (0, 0, 0.6));
			
			Font   font = Font.GetFont ("Tahoma", "Regular");
			double size = 12;
			
			switch (this.RootAngle)
			{
				case 0:
				case 90:
				case 180:
				case 270:
					break;
			}
			
			graphics.AddText (5, 5, this.Text + " " + this.RootAngle.ToString () + "�" + " " + this.RootDirection.ToString (), font, size);
			graphics.RenderSolid (color_text);
		}
		
		protected override void ProcessMessage(Message message, Point pos)
		{
			System.Diagnostics.Debug.WriteLine ("Message " + message.ToString () + " in " + this.Text + " at " + pos.ToString ());
			message.Consumer = this;
		}
	}
}
