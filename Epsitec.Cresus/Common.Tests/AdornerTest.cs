using NUnit.Framework;
using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Tests
{
	[TestFixture]
	public class AdornerTest
	{
		[Test] public void CheckAdorner()
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
		}
	}
}
