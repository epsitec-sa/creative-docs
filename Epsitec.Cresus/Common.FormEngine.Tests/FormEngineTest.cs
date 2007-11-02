using System.Collections.Generic;

using NUnit.Framework;
using Epsitec.Common.FormEngine;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

namespace Epsitec.Common.FormEngine
{
	/// <summary>
	/// Projet de test de Common.FormEngine.
	/// </summary>
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

			Widget form = this.CreateForm();
			form.Dock = DockStyle.Fill;
			window.Root.Children.Add(form);

			Button a = new Button();
			a.Text = "OK";
			a.ButtonStyle = ButtonStyle.DefaultAccept;
			a.Dock = DockStyle.Bottom;
			window.Root.Children.Add(a);

			window.Show();
			Window.RunInTestEnvironment(window);
		}


		protected Widget CreateForm()
		{
			this.collection.MoveCurrentToFirst();
			CultureMap item = this.collection.CurrentItem as CultureMap;
			Druid itemId = item.Id;

			List<FieldDescription> fields = new List<FieldDescription>();
#if false
			itemId = Druid.Parse("[63081]");  // Adresse
			fields.Add(this.CreateField("[63083]"));  // Rue
			fields.Add(this.CreateField("[630C3]"));  // Npa
			fields.Add(this.CreateField("[630B3]"));  // Ville
#endif
#if true
			itemId = Druid.Parse ("[63021]"); // Facture
			fields.Add(this.CreateField("[630A2]"));  // Numéro
			fields.Add(this.CreateField("[630B2].[630S2]"));  // Affaire.Client
			fields.Add(this.CreateField("[630B2].[63013]"));  // Affaire.SoldeDû
			fields.Add(this.CreateField("[630L2]"));  // TotalFacturé
#endif

			System.Console.Out.WriteLine("Génère l'interface pour le DRUID {0}", itemId);

			FormEngine engine = new FormEngine(this.manager);
			return engine.CreateForm(itemId, fields);
		}

		protected FieldDescription CreateField(string text)
		{
			FieldDescription field = new FieldDescription();

			string[] list = text.Split('.');
			foreach (string d in list)
			{
				Druid id = Druid.Parse(d);
				field.FieldsIds.Add(id);
			}

			return field;
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
