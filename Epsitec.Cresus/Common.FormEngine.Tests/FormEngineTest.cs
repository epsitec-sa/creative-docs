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

		[Test]
		public void CheckFormEngineTree()
		{
			this.CreateWindow("Tree");
		}


		protected void CreateWindow(string name)
		{
			Window window = new Window();

			double width = 400;
			double height = 600;

			if (name == "Facture" || name == "Tree")
			{
				width *= 2;
			}
			
			window.ClientSize = new Size(width, height);
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
			Druid entityId = Druid.Empty;
			List<FieldDescription> fields = new List<FieldDescription>();

			if (name == "Adresse")
			{
				entityId = Druid.Parse("[63081]");  // Adresse

				fields.Add(this.CreateSeparator(FieldDescription.FieldType.Line, "First"));
				fields.Add(this.CreateField("[63083]", Color.Empty, FieldDescription.SeparatorType.Normal, 9, 2, "First"));  // Rue
				fields.Add(this.CreateField("[63093]", Color.Empty, FieldDescription.SeparatorType.Normal, 2, 1, "First"));  // Numéro
				fields.Add(this.CreateField("[630A3]", Color.Empty, FieldDescription.SeparatorType.Normal, 5, 1, "First"));  // Case
				fields.Add(this.CreateSeparator(FieldDescription.FieldType.Line, "First"));
				fields.Add(this.CreateField("[630C3]", Color.Empty, FieldDescription.SeparatorType.Append, 2, 1, "First"));  // Npa
				fields.Add(this.CreateField("[630B3]", Color.Empty, FieldDescription.SeparatorType.Normal, 6, 1, "First"));  // Ville
				fields.Add(this.CreateSeparator(FieldDescription.FieldType.Line, "First"));
				fields.Add(this.CreateField("[630D3]", Color.Empty, FieldDescription.SeparatorType.Normal, 9, 1, "First"));  // Etat
				fields.Add(this.CreateField("[630E3]", Color.Empty, FieldDescription.SeparatorType.Normal, 9, 1, "First"));  // Pays
			}

			if (name == "Facture")
			{
				entityId = Druid.Parse("[63021]");  // Facture

				fields.Add(this.CreateField("[630A2]", Color.Empty, FieldDescription.SeparatorType.Normal, 4, 1, "First"));  // Numéro
				fields.Add(this.CreateField("[630C2]", Color.Empty, FieldDescription.SeparatorType.Normal, 5, 1, "First"));  // DateTravail
				fields.Add(this.CreateField("[630D2]", Color.Empty, FieldDescription.SeparatorType.Normal, 5, 1, "First"));  // DateFacture
				fields.Add(this.CreateField("[630E2]", Color.Empty, FieldDescription.SeparatorType.Normal, 5, 1, "First"));  // DateEcheance

				fields.Add(this.CreateSeparator(FieldDescription.FieldType.Title, "First"));
				fields.Add(this.CreateField("[630B2].[630S2]", Color.Empty, FieldDescription.SeparatorType.Normal, 9, 1, "First"));  // Affaire.Client
				fields.Add(this.CreateField("[630B2].[630T2]", Color.Empty, FieldDescription.SeparatorType.Normal, 9, 5, "First"));  // Affaire.Désignation
				fields.Add(this.CreateSeparator(FieldDescription.FieldType.Title, "First"));
				fields.Add(this.CreateField("[630B2].[63003].[63043].[63053]", Color.Empty, FieldDescription.SeparatorType.Normal, 9, 1, "First"));  // Affaire.Paiements.Valeur.PrixSimple.Montant
				fields.Add(this.CreateSeparator(FieldDescription.FieldType.Title, "First"));
				fields.Add(this.CreateField("[630B2].[63003].[63043].[63063].[630A]", Color.Empty, FieldDescription.SeparatorType.Normal, 9, 1, "First"));  // Affaire.Paiements.Valeur.PrixSimple.Monnaie.Désignation
				fields.Add(this.CreateField("[630B2].[63003].[63043].[63063].[630B]", Color.Empty, FieldDescription.SeparatorType.Normal, 9, 1, "First"));  // Affaire.Paiements.Valeur.PrixSimple.Monnaie.TauxChangeVersChf
				
				fields.Add(this.CreateSeparator(FieldDescription.FieldType.Title, "Second"));
				fields.Add(this.CreateField("[630B2].[63013].[63053]", Color.Empty, FieldDescription.SeparatorType.Append, 3, 1, "Second"));  // Affaire.SoldeDû.Montant
				fields.Add(this.CreateField("[630B2].[63013].[63063].[630A]", Color.Empty, FieldDescription.SeparatorType.Normal, 3, 1, "Second"));  // Affaire.SoldeDû.Monnaie.Designation
				
				fields.Add(this.CreateSeparator(FieldDescription.FieldType.Title, "Second"));
				fields.Add(this.CreateField("[630L2].[630M].[630H]", Color.FromRgb(1, 0.5, 0.5), FieldDescription.SeparatorType.Normal, 3, 1, "Second"));  // TotalFacturé.Prix.Ht
				fields.Add(this.CreateSeparator(FieldDescription.FieldType.Title, "Second"));
				fields.Add(this.CreateField("[630L2].[630N]", Color.FromRgb(1, 0.9, 0.5), FieldDescription.SeparatorType.Normal, 3, 1, "Second"));  // TotalFacturé.TotalHt
				fields.Add(this.CreateField("[630L2].[630O]", Color.FromRgb(1, 0.9, 0.5), FieldDescription.SeparatorType.Normal, 3, 1, "Second"));  // TotalFacturé.TotalTtc
				fields.Add(this.CreateField("[630L2].[630P]", Color.FromRgb(1, 0.9, 0.5), FieldDescription.SeparatorType.Normal, 3, 1, "Second"));  // TotalFacturé.TotalTva

				//	Pour tester. Cela n'a pas de sens de mettre un titre suivi d'aucun champ. Doit générer un simple trait.
				fields.Add(this.CreateSeparator(FieldDescription.FieldType.Title, "Second"));
			}

			if (name == "Affaire")
			{
				entityId = Druid.Parse("[63051]");  // Affaire

				fields.Add(this.CreateField("[630S2]", Color.Empty, FieldDescription.SeparatorType.Normal, 4, 1, "First"));  // Client
				fields.Add(this.CreateField("[630T2]", Color.Empty, FieldDescription.SeparatorType.Normal, 9, 5, "First"));  // Désignation
				fields.Add(this.CreateSeparator(FieldDescription.FieldType.Title, "First"));
				fields.Add(this.CreateField("[63013].[63053]", Color.FromRgb(1, 0.9, 0.5), FieldDescription.SeparatorType.Normal, 3, 1, "First"));  // SoldeDû.Montant
			}

			if (name == "Tree")
			{
				entityId = Druid.Parse("[63021]");  // Facture

				fields.Add(this.CreateField("[630A2]", Color.Empty, FieldDescription.SeparatorType.Normal, 4, 1, "First"));  // Numéro

				fields.Add(this.CreateSeparator(FieldDescription.FieldType.Title, "First"));
				fields.Add(this.CreateField("[630B2].[63013].[63053]", Color.Empty, FieldDescription.SeparatorType.Append, 3, 1, "First"));  // Affaire.SoldeDû.Montant
				fields.Add(this.CreateField("[630B2].[63013].[63063].[630A]", Color.Empty, FieldDescription.SeparatorType.Normal, 3, 1, "First"));  // Affaire.SoldeDû.Monnaie.Designation
				
				List<FieldDescription> subFields = new List<FieldDescription>();
				subFields.Add(this.CreateSeparator(FieldDescription.FieldType.Title, "First"));
				subFields.Add(this.CreateField("[630B2].[630S2]", Color.Empty, FieldDescription.SeparatorType.Normal, 9, 1, "First"));  // Affaire.Client
				subFields.Add(this.CreateField("[630B2].[630T2]", Color.Empty, FieldDescription.SeparatorType.Normal, 9, 5, "First"));  // Affaire.Désignation
				subFields.Add(this.CreateSeparator(FieldDescription.FieldType.Title, "First"));
				subFields.Add(this.CreateField("[630B2].[63003].[63043].[63053]", Color.Empty, FieldDescription.SeparatorType.Normal, 9, 1, "First"));  // Affaire.Paiements.Valeur.PrixSimple.Montant
				subFields.Add(this.CreateSeparator(FieldDescription.FieldType.Title, "First"));
				subFields.Add(this.CreateField("[630B2].[63003].[63043].[63063].[630A]", Color.Empty, FieldDescription.SeparatorType.Normal, 9, 1, "First"));  // Affaire.Paiements.Valeur.PrixSimple.Monnaie.Désignation
				subFields.Add(this.CreateField("[630B2].[63003].[63043].[63063].[630B]", Color.Empty, FieldDescription.SeparatorType.Normal, 9, 1, "First"));  // Affaire.Paiements.Valeur.PrixSimple.Monnaie.TauxChangeVersChf
				fields.Add(this.CreateNode(subFields));
				
				fields.Add(this.CreateSeparator(FieldDescription.FieldType.Title, "Second"));
				fields.Add(this.CreateField("[630L2].[630M].[630H]", Color.FromRgb(1, 0.5, 0.5), FieldDescription.SeparatorType.Normal, 3, 1, "Second"));  // TotalFacturé.Prix.Ht
				fields.Add(this.CreateSeparator(FieldDescription.FieldType.Title, "Second"));
				fields.Add(this.CreateField("[630L2].[630N]", Color.FromRgb(1, 0.9, 0.5), FieldDescription.SeparatorType.Normal, 3, 1, "Second"));  // TotalFacturé.TotalHt
				fields.Add(this.CreateField("[630L2].[630O]", Color.FromRgb(1, 0.9, 0.5), FieldDescription.SeparatorType.Normal, 3, 1, "Second"));  // TotalFacturé.TotalTtc
				fields.Add(this.CreateField("[630L2].[630P]", Color.FromRgb(1, 0.9, 0.5), FieldDescription.SeparatorType.Normal, 3, 1, "Second"));  // TotalFacturé.TotalTva

				//	Pour tester. Cela n'a pas de sens de mettre un titre suivi d'aucun champ. Doit générer un simple trait.
				fields.Add(this.CreateSeparator(FieldDescription.FieldType.Title, "Second"));
			}

			FormEngine engine = new FormEngine(this.manager);
			return engine.CreateForm(entityId, fields);
		}


		protected FieldDescription CreateNode(List<FieldDescription> descriptions)
		{
			FieldDescription field = new FieldDescription(FieldDescription.FieldType.Node);

			field.SetNode(descriptions);
			
			return field;
		}

		protected FieldDescription CreateSeparator(FieldDescription.FieldType type, string container)
		{
			FieldDescription field = new FieldDescription(type);

			field.Container = container;
			
			return field;
		}

		protected FieldDescription CreateField(string listDruids, Color backColor, FieldDescription.SeparatorType separator, int columns, int rows, string container)
		{
			FieldDescription field = new FieldDescription(FieldDescription.FieldType.Field);

			field.SetFields(listDruids);
			field.BackColor = backColor;
			field.Separator = separator;
			field.ColumnsRequired = columns;
			field.RowsRequired = rows;
			field.Container = container;
			
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
