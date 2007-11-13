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

			if (name == "Facture")
			{
				width *= 2;
			}
			
			if (name == "Tree")
			{
				width *= 2;
			}
			
			window.ClientSize = new Size(width, height);
			window.Text = string.Concat("CheckFormEngine-", name);
			window.Root.Padding = new Margins(10, 10, 10, 10);

			Widget form = this.CreateForm(name);
			if (form != null)
			{
				form.Dock = DockStyle.Fill;
				window.Root.Children.Add(form);
			}

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

				fields.Add(this.CreateField("[63083]", Color.Empty, FieldDescription.SeparatorType.Normal, 10, 2));  // Rue
				fields.Add(this.CreateField("[63093]", Color.Empty, FieldDescription.SeparatorType.Normal, 2, 1));  // Numéro
				fields.Add(this.CreateField("[630A3]", Color.Empty, FieldDescription.SeparatorType.Normal, 6, 1));  // Case
				fields.Add(this.CreateSeparator(FieldDescription.FieldType.Line));
				fields.Add(this.CreateField("[630C3]", Color.Empty, FieldDescription.SeparatorType.Append, 3, 1));  // Npa
				fields.Add(this.CreateField("[630B3]", Color.Empty, FieldDescription.SeparatorType.Normal, 7, 1));  // Ville
				fields.Add(this.CreateSeparator(FieldDescription.FieldType.Line));
				fields.Add(this.CreateField("[630D3]", Color.Empty, FieldDescription.SeparatorType.Normal, 10, 1));  // Etat
				fields.Add(this.CreateField("[630E3]", Color.Empty, FieldDescription.SeparatorType.Normal, 10, 1));  // Pays
				fields.Add(this.CreateSeparator(FieldDescription.FieldType.Line));
			}

			if (name == "Facture")
			{
				entityId = Druid.Parse("[63021]");  // Facture

				fields.Add(this.CreateBoxBegin(ContainerLayoutMode.VerticalFlow, 5, 5, Color.Empty, FrameState.All, 1, 7));
				fields.Add(this.CreateField("[630A2]", Color.Empty, FieldDescription.SeparatorType.Normal, 5, 1));  // Numéro
				fields.Add(this.CreateField("[630C2]", Color.Empty, FieldDescription.SeparatorType.Normal, 6, 1));  // DateTravail
				fields.Add(this.CreateField("[630D2]", Color.Empty, FieldDescription.SeparatorType.Normal, 6, 1));  // DateFacture
				fields.Add(this.CreateField("[630E2]", Color.Empty, FieldDescription.SeparatorType.Normal, 6, 1));  // DateEcheance

				fields.Add(this.CreateSeparator(FieldDescription.FieldType.Title));
				fields.Add(this.CreateField("[630B2].[630S2]", Color.Empty, FieldDescription.SeparatorType.Normal, 10, 1));  // Affaire.Client
				fields.Add(this.CreateField("[630B2].[630T2]", Color.Empty, FieldDescription.SeparatorType.Normal, 10, 5));  // Affaire.Désignation
				fields.Add(this.CreateSeparator(FieldDescription.FieldType.Title));
				fields.Add(this.CreateField("[630B2].[63003].[63043].[63053]", Color.Empty, FieldDescription.SeparatorType.Normal, 10, 1));  // Affaire.Paiements.Valeur.PrixSimple.Montant
				fields.Add(this.CreateSeparator(FieldDescription.FieldType.Title));
				fields.Add(this.CreateField("[630B2].[63003].[63043].[63063].[630A]", Color.Empty, FieldDescription.SeparatorType.Normal, 10, 1));  // Affaire.Paiements.Valeur.PrixSimple.Monnaie.Désignation
				fields.Add(this.CreateField("[630B2].[63003].[63043].[63063].[630B]", Color.Empty, FieldDescription.SeparatorType.Normal, 10, 1));  // Affaire.Paiements.Valeur.PrixSimple.Monnaie.TauxChangeVersChf
				fields.Add(this.CreateBoxEnd());

				fields.Add(this.CreateBoxBegin(ContainerLayoutMode.VerticalFlow, 5, 5, Color.Empty, FrameState.All, 1, 3));
				fields.Add(this.CreateSeparator(FieldDescription.FieldType.Title));
				fields.Add(this.CreateField("[630B2].[63013].[63053]", Color.Empty, FieldDescription.SeparatorType.Append, 4, 1));  // Affaire.SoldeDû.Montant
				fields.Add(this.CreateField("[630B2].[63013].[63063].[630A]", Color.Empty, FieldDescription.SeparatorType.Normal, 4, 1));  // Affaire.SoldeDû.Monnaie.Designation

				fields.Add(this.CreateSeparator(FieldDescription.FieldType.Title));
				fields.Add(this.CreateField("[630L2].[630M].[630H]", Color.FromRgb(1, 0.5, 0.5), FieldDescription.SeparatorType.Normal, 4, 1));  // TotalFacturé.Prix.Ht
				fields.Add(this.CreateSeparator(FieldDescription.FieldType.Title));
				fields.Add(this.CreateField("[630L2].[630N]", Color.FromRgb(1, 0.9, 0.5), FieldDescription.SeparatorType.Normal, 4, 1));  // TotalFacturé.TotalHt
				fields.Add(this.CreateField("[630L2].[630O]", Color.FromRgb(1, 0.9, 0.5), FieldDescription.SeparatorType.Normal, 4, 1));  // TotalFacturé.TotalTtc
				fields.Add(this.CreateField("[630L2].[630P]", Color.FromRgb(1, 0.9, 0.5), FieldDescription.SeparatorType.Normal, 4, 1));  // TotalFacturé.TotalTva

				//	Pour tester. Cela n'a pas de sens de mettre un titre suivi d'aucun champ. Doit générer un simple trait.
				fields.Add(this.CreateSeparator(FieldDescription.FieldType.Title));
				fields.Add(this.CreateBoxEnd());
			}

			if (name == "Affaire")
			{
				entityId = Druid.Parse("[63051]");  // Affaire

				fields.Add(this.CreateField("[630S2]", Color.Empty, FieldDescription.SeparatorType.Normal, 5, 1));  // Client
				fields.Add(this.CreateField("[630T2]", Color.Empty, FieldDescription.SeparatorType.Normal, 10, 5));  // Désignation

				fields.Add(this.CreateBoxBegin(ContainerLayoutMode.VerticalFlow, 5, 10, Color.Empty, FrameState.All, 1, 10));
				fields.Add(this.CreateSeparator(FieldDescription.FieldType.Title));
				fields.Add(this.CreateField("[630V2].[630M2]", Color.Empty, FieldDescription.SeparatorType.Normal, 2, 1));  // Rappels.Nième
				fields.Add(this.CreateField("[630V2].[630N2]", Color.Empty, FieldDescription.SeparatorType.Normal, 10, 1));  // Rappels.Texte
				fields.Add(this.CreateBoxEnd());

				fields.Add(this.CreateSeparator(FieldDescription.FieldType.Title));
				fields.Add(this.CreateField("[63013].[63053]", Color.FromRgb(1, 0.9, 0.5), FieldDescription.SeparatorType.Normal, 4, 1));  // SoldeDû.Montant
			}

			if (name == "Tree")
			{
				entityId = Druid.Parse("[63021]");  // Facture

				fields.Add(this.CreateBoxBegin(ContainerLayoutMode.VerticalFlow, 0, 0, Color.Empty, FrameState.None, 0, 7));
				fields.Add(this.CreateBoxBegin(ContainerLayoutMode.VerticalFlow, 5, 10, Color.Empty, FrameState.All, 1, 10));
				fields.Add(this.CreateField("[630A2]", Color.Empty, FieldDescription.SeparatorType.Normal, 5, 1));  // Numéro

				fields.Add(this.CreateSeparator(FieldDescription.FieldType.Title));
				fields.Add(this.CreateField("[630B2].[63013].[63053]", Color.Empty, FieldDescription.SeparatorType.Append, 4, 1));  // Affaire.SoldeDû.Montant
				fields.Add(this.CreateField("[630B2].[63013].[63063].[630A]", Color.Empty, FieldDescription.SeparatorType.Normal, 4, 1));  // Affaire.SoldeDû.Monnaie.Designation
				fields.Add(this.CreateBoxEnd());

				fields.Add(this.CreateBoxBegin(ContainerLayoutMode.VerticalFlow, 5, 10, Color.FromRgb(0.6, 0.8, 0.8), FrameState.All, 5, 10));
				List<FieldDescription> subFields = new List<FieldDescription>();
				subFields.Add(this.CreateSeparator(FieldDescription.FieldType.Title));
				subFields.Add(this.CreateField("[630B2].[630S2]", Color.Empty, FieldDescription.SeparatorType.Normal, 10, 1));  // Affaire.Client
				subFields.Add(this.CreateField("[630B2].[630T2]", Color.Empty, FieldDescription.SeparatorType.Normal, 10, 5));  // Affaire.Désignation
				subFields.Add(this.CreateSeparator(FieldDescription.FieldType.Title));
				subFields.Add(this.CreateField("[630B2].[63003].[63043].[63053]", Color.Empty, FieldDescription.SeparatorType.Normal, 10, 1));  // Affaire.Paiements.Valeur.PrixSimple.Montant
				subFields.Add(this.CreateSeparator(FieldDescription.FieldType.Title));
				subFields.Add(this.CreateField("[630B2].[63003].[63043].[63063].[630A]", Color.Empty, FieldDescription.SeparatorType.Normal, 10, 1));  // Affaire.Paiements.Valeur.PrixSimple.Monnaie.Désignation
				subFields.Add(this.CreateField("[630B2].[63003].[63043].[63063].[630B]", Color.Empty, FieldDescription.SeparatorType.Normal, 10, 1));  // Affaire.Paiements.Valeur.PrixSimple.Monnaie.TauxChangeVersChf
				fields.Add(this.CreateNode(subFields));
				fields.Add(this.CreateBoxEnd());
				fields.Add(this.CreateBoxEnd());

				fields.Add(this.CreateBoxBegin(ContainerLayoutMode.VerticalFlow, 5, 10, Color.Empty, FrameState.Left, 1, 3));
				fields.Add(this.CreateSeparator(FieldDescription.FieldType.Title));
				fields.Add(this.CreateField("[630L2].[630M].[630H]", Color.FromRgb(1, 0.5, 0.5), FieldDescription.SeparatorType.Normal, 4, 1));  // TotalFacturé.Prix.Ht
				fields.Add(this.CreateSeparator(FieldDescription.FieldType.Title));
				fields.Add(this.CreateField("[630L2].[630N]", Color.FromRgb(1, 0.9, 0.5), FieldDescription.SeparatorType.Normal, 4, 1));  // TotalFacturé.TotalHt
				fields.Add(this.CreateField("[630L2].[630O]", Color.FromRgb(1, 0.9, 0.5), FieldDescription.SeparatorType.Normal, 4, 1));  // TotalFacturé.TotalTtc
				fields.Add(this.CreateField("[630L2].[630P]", Color.FromRgb(1, 0.9, 0.5), FieldDescription.SeparatorType.Normal, 4, 1));  // TotalFacturé.TotalTva
				fields.Add(this.CreateBoxEnd());
			}

			List<FieldDescription> flat = Arrange.Develop(fields);

			string err = Arrange.Check(flat);
			if (err == null)
			{
				FormEngine engine = new FormEngine(this.manager);
				return engine.CreateForm(entityId, flat);
			}
			else
			{
				System.Console.WriteLine(string.Format("Erreur {0}", err));
				return null;
			}
		}


		protected FieldDescription CreateNode(List<FieldDescription> descriptions)
		{
			FieldDescription field = new FieldDescription(FieldDescription.FieldType.Node);

			field.SetNode(descriptions);
			
			return field;
		}

		protected FieldDescription CreateSeparator(FieldDescription.FieldType type)
		{
			FieldDescription field = new FieldDescription(type);

			return field;
		}

		protected FieldDescription CreateBoxBegin(ContainerLayoutMode mode, double margins, double padding, Color backColor, FrameState frame, double width, int columns)
		{
			FieldDescription field = new FieldDescription(FieldDescription.FieldType.BoxBegin);

			field.ContainerLayoutMode = mode;
			field.ContainerMargins = new Margins(margins, margins, margins, margins);
			field.ContainerPadding = new Margins(padding, padding, padding, padding);
			field.ContainerBackColor = backColor;
			field.ContainerFrameState = frame;
			field.ContainerFrameWidth = width;
			field.ColumnsRequired = columns;

			return field;
		}

		protected FieldDescription CreateBoxEnd()
		{
			FieldDescription field = new FieldDescription(FieldDescription.FieldType.BoxEnd);

			return field;
		}

		protected FieldDescription CreateField(string listDruids, Color backColor, FieldDescription.SeparatorType separator, int columns, int rows)
		{
			FieldDescription field = new FieldDescription(FieldDescription.FieldType.Field);

			field.SetFields(listDruids);
			field.BackColor = backColor;
			field.Separator = separator;
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
