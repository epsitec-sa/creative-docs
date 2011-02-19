using NUnit.Framework;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;

namespace Epsitec.Common.Widgets
{
	[TestFixture]
	public class RibbonTest
	{
		[SetUp] public void Initialize()
		{
			Epsitec.Common.Document.Engine.Initialize();
			Epsitec.Common.Widgets.Adorners.Factory.SetActive("LookMetal");
		}

		[Test]
		public void AutomatedTestEnvironment()
		{
			Epsitec.Common.Widgets.Window.RunningInAutomatedTestEnvironment = true;
		}

		[Test]
		public void CheckRibbonWidgets()
		{
			Window.RunInTestEnvironment(RibbonTest.CreateAdornerWidgets());
		}
		
		public static Window CreateAdornerWidgets()
		{
			Document.Engine.Initialize();

			Window window = new Window();
			
			window.ClientSize = new Size(500, 300);
			window.Text = "CheckRibbonWidgets";
			window.Name = "CheckRibbonWidgets";

			ToolTip tip = ToolTip.Default;
			tip.Behaviour = ToolTipBehaviour.Normal;

			//	Crée le widget pour permettre de changer d'adorner.
			AdornerTest.CreateListLook(window.Root, 10, 200, tip, 1);

			RibbonBook book = new RibbonBook();
			book.Dock = DockStyle.Top;
			book.TabIndex = 2;
			book.TabNavigationMode = TabNavigationMode.ActivateOnTab | TabNavigationMode.ForwardToChildren;
			window.Root.Children.Add(book);

			//	Crée l'onglet 1.
			RibbonPage page1 = new RibbonPage();
			page1.RibbonTitle = "Principal";
			page1.TabIndex = 1;
			page1.TabNavigationMode = TabNavigationMode.ActivateOnTab | TabNavigationMode.ForwardToChildren | TabNavigationMode.ForwardOnly;
			book.Pages.Add(page1);

			RibbonSection p1s1 = new RibbonSection();
			p1s1.Title = "Rouge";
			p1s1.PreferredWidth = 200;
			page1.Items.Add(p1s1);

			Button p1s1b1 = new Button(p1s1);
			p1s1b1.Text = "A";
			p1s1b1.PreferredWidth = 40;
			p1s1b1.Margins = new Margins(0, 0, 0, 0);
			p1s1b1.Dock = DockStyle.Left;

			Button p1s1b2 = new Button(p1s1);
			p1s1b2.Text = "B";
			p1s1b2.PreferredWidth = 40;
			p1s1b2.Margins = new Margins(5, 0, 0, 0);
			p1s1b2.Dock = DockStyle.Left;

			RibbonSection p1s2 = new RibbonSection();
			p1s2.Title = "Vert";
			p1s2.PreferredWidth = 100;
			page1.Items.Add(p1s2);

			RibbonSection p1s3 = new RibbonSection();
			p1s3.Title = "Bleu";
			p1s3.PreferredWidth = 100;
			page1.Items.Add(p1s3);

			//	Crée l'onglet 2.
			RibbonPage page2 = new RibbonPage();
			page2.RibbonTitle = "Edition";
			page2.TabIndex = 2;
			page2.TabNavigationMode = TabNavigationMode.ActivateOnTab | TabNavigationMode.ForwardToChildren | TabNavigationMode.ForwardOnly;
			book.Pages.Add(page2);

			RibbonSection p2s1 = new RibbonSection();
			p2s1.Title = "Lausanne";
			p2s1.PreferredWidth = 150;
			page2.Items.Add(p2s1);

			RibbonSection p2s2 = new RibbonSection();
			p2s2.Title = "Genève";
			p2s2.PreferredWidth = 100;
			page2.Items.Add(p2s2);

			//	Crée l'onglet 3.
			RibbonPage page3 = new RibbonPage();
			page3.RibbonTitle = "Edition";
			page3.TabIndex = 2;
			page3.TabNavigationMode = TabNavigationMode.ActivateOnTab | TabNavigationMode.ForwardToChildren | TabNavigationMode.ForwardOnly;
			book.Pages.Add(page3);

			RibbonSection p3s1 = new RibbonSection();
			p3s1.Title = "Lundi";
			p3s1.PreferredWidth = 80;
			page3.Items.Add(p3s1);

			RibbonSection p3s2 = new RibbonSection();
			p3s2.Title = "Mardi";
			p3s2.PreferredWidth = 80;
			page3.Items.Add(p3s2);

			RibbonSection p3s3 = new RibbonSection();
			p3s3.Title = "Mercredi";
			p3s3.PreferredWidth = 80;
			page3.Items.Add(p3s3);

			RibbonSection p3s4 = new RibbonSection();
			p3s4.Title = "Jeudi";
			p3s4.PreferredWidth = 80;
			page3.Items.Add(p3s4);

			RibbonSection p3s5 = new RibbonSection();
			p3s5.Title = "Vendredi";
			p3s5.PreferredWidth = 80;
			page3.Items.Add(p3s5);

			book.ActivePage = page1;

			Assert.IsFalse(window.IsVisible);
			Assert.IsFalse(window.Root.IsVisible);
			
			window.Show();
			
			Assert.IsTrue(window.IsVisible);
			Assert.IsTrue(window.Root.IsVisible);
			
			return window;
		}
	}
}
