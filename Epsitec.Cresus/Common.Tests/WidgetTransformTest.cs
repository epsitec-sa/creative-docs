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
			try { System.Diagnostics.Debug.WriteLine (""); } 
			catch { }
		}
		
		[Test] public void CheckWidgetTransform()
		{
			WindowFrame window = new WindowFrame ();
			window.Text = "CheckWidgetTransform";
			
			TransformWidget a = new TransformWidget ();	a.Text = "A"; a.Bounds = new Rectangle (10, 10, 200, 200); a.SetClientZoom (1);
			TransformWidget b = new TransformWidget (); b.Text = "B"; b.Bounds = new Rectangle (10, 10, 160, 120); b.SetClientAngle (90);
			TransformWidget c = new TransformWidget (); c.Text = "C"; c.Bounds = new Rectangle (10, 10,  60,  40);
			
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
			this.internal_state |= InternalState.AutoFocus;
			this.internal_state |= InternalState.AutoEngage;
			this.internal_state |= InternalState.Focusable;
			this.internal_state |= InternalState.Engageable;
			this.internal_state |= InternalState.AutoToggle;
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
			double size = 15;
			
			graphics.AddText (5, 5, this.Text, font, size);
			graphics.RenderSolid (color_text);
		}
		
		protected override void ProcessMessage(Message message, Point pos)
		{
			System.Diagnostics.Debug.WriteLine ("Message " + message.ToString () + " in " + this.Text);
			message.Consumer = this;
		}
	}
}
