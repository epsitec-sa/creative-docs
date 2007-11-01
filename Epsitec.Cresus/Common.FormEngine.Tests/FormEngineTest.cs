using NUnit.Framework;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.FormEngine
{
	[TestFixture]
	public class FormEngineTest
	{
		[SetUp] public void Initialize()
		{
			Document.Engine.Initialize();
			Widgets.Adorners.Factory.SetActive("LookMetal");
			Widget.Initialize();
		}

		[Test]
		public void CheckFormEngine()
		{
			Window window = new Window();
			
			window.ClientSize = new Size(400, 300);
			window.Text = "CheckFormEngine";
			window.Root.Padding = new Margins(10, 10, 10, 10);

			Button a = new Button();
			a.PreferredWidth = 75;
			a.Text = "OK";
			a.ButtonStyle = ButtonStyle.DefaultAccept;
			a.Anchor = AnchorStyles.BottomRight;
			window.Root.Children.Add(a);

			window.Show();
			Window.RunInTestEnvironment(window);
		}
	}
}
