using NUnit.Framework;
using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Tests
{
	[TestFixture]
	public class AdornerTest
	{
		[Test] public void CheckAdornerButton()
		{
			WindowFrame window = new WindowFrame();
			
			window.ClientSize = new System.Drawing.Size(300, 200);
			window.Text = "CheckAdorner";

			Button a = new Button();
			a.Name = "A";
			a.Location = new Point(10, 10);
			a.Size = new Size(75, 24);
			a.Text = "O<m>K</m>";
			a.ButtonStyle = ButtonStyle.DefaultActive;
			a.Anchor = AnchorStyles.Bottom|AnchorStyles.Right;
			window.Root.Children.Add(a);

			Button b = new Button();
			b.Name = "B";
			b.Location = new Point(95, 10);
			b.Size = new Size(75, 24);
			b.Text = "<m>A</m>nnuler";
			b.Anchor = AnchorStyles.Bottom|AnchorStyles.Right;
			window.Root.Children.Add(b);

			Button c = new Button();
			c.Name = "C";
			c.Location = new Point(95+85, 10);
			c.Size = new Size(75, 24);
			c.Text = "Ai<m>d</m>e";
			c.SetEnabled(false);
			c.Anchor = AnchorStyles.Bottom|AnchorStyles.Right;
			window.Root.Children.Add(c);

			window.FocusedWidget = a;

			window.Show();
		}

		[Test] public void CheckAdornerBase()
		{
			WindowFrame window = new WindowFrame();
			
			window.ClientSize = new System.Drawing.Size(300, 200);
			window.Text = "CheckAdorner";
			window.Root.PaintForeground += new PaintEventHandler(CheckPaint_Paint1);

			window.Show();
		}

		private void CheckPaint_Paint1(object sender, PaintEventArgs e)
		{
			IAdorner adorner = Widgets.Adorner.Factory.Active;
			Widgets.Adorner.Factory.SetActive("Default");

			Rectangle rect = new Rectangle(10, 10, 60, 30);
			WidgetState state = WidgetState.Enabled;
			Direction shadow = Direction.Up;
			Direction dir = Direction.Up;
			adorner.PaintArrow(e.Graphics, rect, state, shadow, dir);

			rect.Offset(70, 0);
			adorner.PaintButtonBackground(e.Graphics, rect, state, shadow, ButtonStyle.Normal);

			rect.Offset(70, 0);
			adorner.PaintButtonBackground(e.Graphics, rect, state, shadow, ButtonStyle.Flat);

			rect.Offset(-70, 40);
			rect.Offset(0.5, 0.5);
			adorner.PaintButtonBackground(e.Graphics, rect, state, shadow, ButtonStyle.Normal);
		}
	}
}
