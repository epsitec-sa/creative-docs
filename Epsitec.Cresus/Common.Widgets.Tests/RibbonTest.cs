using NUnit.Framework;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;

namespace Epsitec.Common.Widgets
{
	[TestFixture]
	public class RibbonTest
	{
		[SetUp] public void Initialise()
		{
			Epsitec.Common.Document.Engine.Initialise();
			Epsitec.Common.Widgets.Adorners.Factory.SetActive("LookMetal");
		}

		[Test] public void CheckRibbonWidgets()
		{
			Window.RunInTestEnvironment(RibbonTest.CreateAdornerWidgets());
		}
		
		public static Window CreateAdornerWidgets()
		{
			Document.Engine.Initialise();

			Window window = new Window();
			
			window.ClientSize = new Size(500, 300);
			window.Text = "CheckRibbonWidgets";
			window.Name = "CheckRibbonWidgets";

			ToolTip tip = new ToolTip();
			tip.Behaviour = ToolTipBehaviour.Normal;

			AdornerTest.CreateListLook(window.Root, 10, 10, tip, 1);


			RibbonBook tab = new RibbonBook();
			tab.Anchor = AnchorStyles.All;
			tab.Margins = new Margins(100, 10, 10, 10);
			tab.TabIndex = 2;
			tab.TabNavigation = Widget.TabNavigationMode.ActivateOnTab | Widget.TabNavigationMode.ForwardToChildren;
			window.Root.Children.Add(tab);

			window.ForceLayout();

			//	Crée l'onglet 1.
			RibbonPage page1 = new RibbonPage();
			page1.RibbonTitle = "Principal";
			page1.TabIndex = 1;
			page1.TabNavigation = Widget.TabNavigationMode.ActivateOnTab | Widget.TabNavigationMode.ForwardToChildren | Widget.TabNavigationMode.ForwardOnly;
			tab.Items.Add(page1);

			//	Crée l'onglet 2.
			RibbonPage page2 = new RibbonPage();
			page2.RibbonTitle = "Edition";
			page2.TabIndex = 2;
			page2.TabNavigation = Widget.TabNavigationMode.ActivateOnTab | Widget.TabNavigationMode.ForwardToChildren | Widget.TabNavigationMode.ForwardOnly;
			tab.Items.Add(page2);

			tab.ActivePage = page1;

			Assert.IsFalse(window.IsVisible);
			Assert.IsFalse(window.Root.IsVisible);
			
			window.Show();
			
			Assert.IsTrue(window.IsVisible);
			Assert.IsTrue(window.Root.IsVisible);
			
			return window;
		}
	}
}
