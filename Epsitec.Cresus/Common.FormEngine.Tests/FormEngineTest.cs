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
		public void CheckFormEngineAdresse()
		{
			this.CreateWindow("Adresse");
		}

		[Test]
		public void CheckFormEngineFacture()
		{
			this.CreateWindow("Facture");
		}

		[Test]
		public void CheckFormEngineAffaire()
		{
			this.CreateWindow("Affaire");
		}


		protected void CreateWindow(string name)
		{
			Window window = new Window();
			
			window.ClientSize = new Size(400, 600);
			window.Text = string.Concat("CheckFormEngine-", name);
			window.Root.Padding = new Margins(10, 10, 10, 10);

			Widget form = this.CreateForm(name);
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

		protected Widget CreateForm(string name)
		{
			this.collection.MoveCurrentToFirst();
			CultureMap item = this.collection.CurrentItem as CultureMap;
			Druid itemId = item.Id;

			List<FieldDescription> fields = new List<FieldDescription>();

			if (name == "Adresse")
			{
				itemId = Druid.Parse("[63081]");  // Adresse
				fields.Add(this.CreateField("[63083]", Color.Empty, FieldDescription.SeparatorType.Normal, 9, 2));  // Rue
				fields.Add(this.CreateField("[63093]", Color.Empty, FieldDescription.SeparatorType.Normal, 2, 1));  // Numéro
				fields.Add(this.CreateField("[630A3]", Color.Empty, FieldDescription.SeparatorType.Line, 5, 1));  // Case
				fields.Add(this.CreateField("[630C3]", Color.Empty, FieldDescription.SeparatorType.Append, 2, 1));  // Npa
				fields.Add(this.CreateField("[630B3]", Color.Empty, FieldDescription.SeparatorType.Line, 6, 1));  // Ville
				fields.Add(this.CreateField("[630D3]", Color.Empty, FieldDescription.SeparatorType.Normal, 9, 1));  // Etat
				fields.Add(this.CreateField("[630E3]", Color.Empty, FieldDescription.SeparatorType.Normal, 9, 1));  // Pays
			}

			if (name == "Facture")
			{
				itemId = Druid.Parse("[63021]"); // Facture
				fields.Add(this.CreateField("[630A2]", Color.Empty, FieldDescription.SeparatorType.Normal, 4, 1));  // Numéro
				fields.Add(this.CreateField("[630C2]", Color.Empty, FieldDescription.SeparatorType.Normal, 5, 1));  // DateTravail
				fields.Add(this.CreateField("[630D2]", Color.Empty, FieldDescription.SeparatorType.Normal, 5, 1));  // DateFacture
				fields.Add(this.CreateField("[630E2]", Color.Empty, FieldDescription.SeparatorType.Line, 5, 1));  // DateEcheance
				fields.Add(this.CreateField("[630B2].[630S2]", Color.Empty, FieldDescription.SeparatorType.Normal, 9, 1));  // Affaire.Client
				fields.Add(this.CreateField("[630B2].[630T2]", Color.Empty, FieldDescription.SeparatorType.Normal, 9, 5));  // Affaire.Désignation
				fields.Add(this.CreateField("[630B2].[63013].[63053]", Color.Empty, FieldDescription.SeparatorType.Append, 3, 1));  // Affaire.SoldeDû.Montant
				fields.Add(this.CreateField("[630B2].[63013].[63063].[630A]", Color.Empty, FieldDescription.SeparatorType.Line, 3, 1));  // Affaire.SoldeDû.Monnaie.Designation
				fields.Add(this.CreateField("[630L2].[630M].[630H]", Color.FromRgb(1, 0.5, 0.5), FieldDescription.SeparatorType.Normal, 3, 1));  // TotalFacturé.Prix.Ht
				fields.Add(this.CreateField("[630L2].[630N]", Color.FromRgb(1, 0.9, 0.5), FieldDescription.SeparatorType.Normal, 3, 1));  // TotalFacturé.TotalHt
				fields.Add(this.CreateField("[630L2].[630O]", Color.FromRgb(1, 0.9, 0.5), FieldDescription.SeparatorType.Normal, 3, 1));  // TotalFacturé.TotalTtc
				fields.Add(this.CreateField("[630L2].[630P]", Color.FromRgb(1, 0.9, 0.5), FieldDescription.SeparatorType.Normal, 3, 1));  // TotalFacturé.TotalTva
			}

			if (name == "Affaire")
			{
				itemId = Druid.Parse("[63051]"); // Affaire
				fields.Add(this.CreateField("[630S2]", Color.Empty, FieldDescription.SeparatorType.Normal, 4, 1));  // Client
				fields.Add(this.CreateField("[630T2]", Color.Empty, FieldDescription.SeparatorType.Extend, 9, 5));  // Désignation
				fields.Add(this.CreateField("[63013].[63053]", Color.FromRgb(1, 0.9, 0.5), FieldDescription.SeparatorType.Normal, 3, 1));  // SoldeDû.Montant
			}

			System.Console.Out.WriteLine("Génère l'interface pour le DRUID {0}", itemId);

			FormEngine engine = new FormEngine(this.manager);
			return engine.CreateForm(itemId, fields);
		}

		protected FieldDescription CreateField(string listDruids, Color backColor, FieldDescription.SeparatorType separator, int columns, int rows)
		{
			FieldDescription field = new FieldDescription(listDruids);

			field.BackColor = backColor;
			field.BottomSeparator = separator;
			field.ColumnsRequired = columns;
			field.RowsRequired = rows;
			
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
