using NUnit.Framework;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.FormEngine
{
	[TestFixture]
	public class FormEngineTest
	{
		[SetUp]
		public void Initialize()
		{
			Document.Engine.Initialize();
			Widgets.Adorners.Factory.SetActive("LookMetal");
			Widget.Initialize();

			this.LoadResource();
		}
		
		[Test]
		public void AutomatedTestEnvironment()
		{
			//	Si ce test est exécuté avant les autres tests, ceux-ci ne bloquent pas
			//	dans l'interaction des diverses fenêtres. Utile si on fait un [Run] de
			//	tous les tests d'un coup.

			Window.RunningInAutomatedTestEnvironment = true;
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


		protected void LoadResource()
		{
			this.manager = new ResourceManager(typeof(FormEngineTest));
			this.manager.DefineDefaultModuleName("Demo5juin");

			this.accessor = new Support.ResourceAccessors.StructuredTypeResourceAccessor();
			this.accessor.Load(this.manager);

			this.collection = new Types.CollectionView(this.accessor.Collection);
		}


		protected ResourceManager manager;
		protected Support.ResourceAccessors.StructuredTypeResourceAccessor accessor;
		protected Types.CollectionView collection;
	}
}
