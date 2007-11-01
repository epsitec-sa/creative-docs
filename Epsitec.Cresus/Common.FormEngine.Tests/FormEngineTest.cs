using System.Collections.Generic;

using NUnit.Framework;
using Epsitec.Common.FormEngine;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

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
			this.collection.MoveCurrentToFirst();
			CultureMap item = this.collection.CurrentItem as CultureMap;
			StructuredData data = item.GetCultureData("00");

			FormEngine engine = new FormEngine();
			Widget form = engine.CreateForm(data);

			Window window = new Window();
			
			window.ClientSize = new Size(400, 300);
			window.Text = "CheckFormEngine";
			window.Root.Padding = new Margins(10, 10, 10, 10);

			form.Anchor = AnchorStyles.TopLeft;
			window.Root.Children.Add(form);

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
			//	Charge les ressources 'Demo5juin'.
			this.manager = new ResourceManager(typeof(FormEngineTest));
			this.manager.DefineDefaultModuleName("Demo5juin");

			this.accessor = new Support.ResourceAccessors.StructuredTypeResourceAccessor();
			this.accessor.Load(this.manager);

			this.collection = new CollectionView(this.accessor.Collection);
		}


		protected ResourceManager manager;
		protected Support.ResourceAccessors.StructuredTypeResourceAccessor accessor;
		protected CollectionView collection;
	}
}
