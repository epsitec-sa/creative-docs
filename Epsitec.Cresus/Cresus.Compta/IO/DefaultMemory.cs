//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Support;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta.Search.Data;
using Epsitec.Cresus.Compta.Options.Data;
using Epsitec.Cresus.Compta.Memory.Data;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.IO
{
	/// <summary>
	/// Cette classe s'occupe de créer les styles par défaut.
	/// </summary>
	public class DefaultMemory
	{
		public DefaultMemory(MainWindowController mainWindowController)
		{
			this.mainWindowController = mainWindowController;
			this.compta               = mainWindowController.Compta;
		}


		public void CreateDefaultMemory()
		{
			this.CreateDefaultMemoryJournal         ("Journal");
			this.CreateDefaultMemoryPlanComptable   ("PlanComptable");
			this.CreateDefaultMemoryBalance         ("Balance");
			this.CreateDefaultMemoryExtraitDeCompte ("ExtraitDeCompte");
			this.CreateDefaultMemoryBilan           ("Bilan");
			this.CreateDefaultMemoryPP              ("PP");
			this.CreateDefaultMemoryExploitation    ("Exploitation");
			this.CreateDefaultMemoryBudgets         ("Budgets");
			this.CreateDefaultMemoryJournaux        ("Journaux");
			this.CreateDefaultMemoryLibellés        ("Libellés");
			this.CreateDefaultMemoryModèles         ("Modèles");
			this.CreateDefaultMemoryPériodes        ("Périodes");
		}


		private void CreateDefaultMemoryJournal(string nomPrésentation)
		{
			var list = this.mainWindowController.GetMemoryList (this.GetKey (nomPrésentation, "Memory"));

			bool searchExist  = true;
			bool filterExist  = true;
			bool optionsExist = true;

			{
				var memory = this.CreateMemoryData (list, DefaultMemory.defaultName, searchExist, filterExist, optionsExist);

				memory.Options = new JournalOptions ();
				memory.Options.SetComptaEntity (this.compta);
			}

			int year = Date.Today.Year;

			{
				var memory = this.CreateMemoryData (list, "Premier trimestre", searchExist, filterExist, optionsExist);
				this.SearchAdaptDate (memory.Filter, new Date (year, 1, 1), new Date (year, 3, 31));

				memory.Options = new JournalOptions ();
				memory.Options.SetComptaEntity (this.compta);
			}

			{
				var memory = this.CreateMemoryData (list, "Deuxième trimestre", searchExist, filterExist, optionsExist);
				this.SearchAdaptDate (memory.Filter, new Date (year, 4, 1), new Date (year, 6, 30));

				memory.Options = new JournalOptions ();
				memory.Options.SetComptaEntity (this.compta);
			}

			{
				var memory = this.CreateMemoryData (list, "Troisième trimestre", searchExist, filterExist, optionsExist);
				this.SearchAdaptDate (memory.Filter, new Date (year, 7, 1), new Date (year, 9, 30));

				memory.Options = new JournalOptions ();
				memory.Options.SetComptaEntity (this.compta);
			}

			{
				var memory = this.CreateMemoryData (list, "Quatrième trimestre", searchExist, filterExist, optionsExist);
				this.SearchAdaptDate (memory.Filter, new Date (year, 10, 1), new Date (year, 12, 31));

				memory.Options = new JournalOptions ();
				memory.Options.SetComptaEntity (this.compta);
			}

			this.Select<JournalOptions> (list, nomPrésentation);
		}

		private void CreateDefaultMemoryPlanComptable(string nomPrésentation)
		{
			var list = this.mainWindowController.GetMemoryList (this.GetKey (nomPrésentation, "Memory"));

			bool searchExist  = true;
			bool filterExist  = true;
			bool optionsExist = false;

			{
				var memory = this.CreateMemoryData (list, DefaultMemory.defaultName, searchExist, filterExist, optionsExist);
			}

			{
				var memory = this.CreateMemoryData (list, "Actifs", searchExist, filterExist, optionsExist);
				this.SearchAdaptCatégorie (memory.Filter, CatégorieDeCompte.Actif);
			}

			{
				var memory = this.CreateMemoryData (list, "Passifs", searchExist, filterExist, optionsExist);
				this.SearchAdaptCatégorie (memory.Filter, CatégorieDeCompte.Passif);
			}

			{
				var memory = this.CreateMemoryData (list, "Charges", searchExist, filterExist, optionsExist);
				this.SearchAdaptCatégorie (memory.Filter, CatégorieDeCompte.Charge);
			}

			{
				var memory = this.CreateMemoryData (list, "Produits", searchExist, filterExist, optionsExist);
				this.SearchAdaptCatégorie (memory.Filter, CatégorieDeCompte.Produit);
			}

			{
				var memory = this.CreateMemoryData (list, "Exploitations", searchExist, filterExist, optionsExist);
				this.SearchAdaptCatégorie (memory.Filter, CatégorieDeCompte.Exploitation);
			}

			this.Select (list, nomPrésentation);
		}

		private void CreateDefaultMemoryBalance(string nomPrésentation)
		{
			var list = this.mainWindowController.GetMemoryList (this.GetKey (nomPrésentation, "Memory"));

			bool searchExist  = true;
			bool filterExist  = true;
			bool optionsExist = true;

			{
				var memory = this.CreateMemoryData (list, DefaultMemory.defaultName, searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (memory.Filter);
			}

			{
				var memory = this.CreateMemoryData (list, "Vue d'ensemble (niveau 1)", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (memory.Filter);
				this.SearchAddProfondeur (memory.Filter, 1, 1);
			}

			{
				var memory = this.CreateMemoryData (list, "Vue d'ensemble (niveaux 1 et 2)", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (memory.Filter);
				this.SearchAddProfondeur (memory.Filter, 1, 2);
			}

			{
				var memory = this.CreateMemoryData (list, "Vue d'ensemble (niveaux 1 à 3)", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (memory.Filter);
				this.SearchAddProfondeur (memory.Filter, 1, 3);
			}

			this.Select<BalanceOptions> (list, nomPrésentation);
		}

		private void CreateDefaultMemoryExtraitDeCompte(string nomPrésentation)
		{
			var list = this.mainWindowController.GetMemoryList (this.GetKey (nomPrésentation, "Memory"));

			bool searchExist  = true;
			bool filterExist  = true;
			bool optionsExist = true;

			{
				var memory = this.CreateMemoryData (list, DefaultMemory.defaultName, searchExist, filterExist, optionsExist);
			}

			this.Select<ExtraitDeCompteOptions> (list, nomPrésentation);
		}

		private void CreateDefaultMemoryBilan(string nomPrésentation)
		{
			var list = this.mainWindowController.GetMemoryList (this.GetKey (nomPrésentation, "Memory"));

			bool searchExist  = true;
			bool filterExist  = true;
			bool optionsExist = true;

			{
				var memory = this.CreateMemoryData (list, DefaultMemory.defaultName, searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (memory.Filter);

				memory.Options = new BilanOptions ()
				{
					HideZero    = true,
					HasGraphics = false,
				};
				memory.Options.SetComptaEntity (this.compta);
			}

			{
				var memory = this.CreateMemoryData (list, "Vue d'ensemble (niveau 1)", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (memory.Filter);
				this.SearchAddProfondeur (memory.Filter, 1, 1);

				memory.Options = new BilanOptions ()
				{
					HideZero    = true,
					HasGraphics = false,
				};
				memory.Options.SetComptaEntity (this.compta);
			}

			{
				var memory = this.CreateMemoryData (list, "Vue d'ensemble (niveaux 1 et 2)", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (memory.Filter);
				this.SearchAddProfondeur (memory.Filter, 1, 2);

				memory.Options = new BilanOptions ()
				{
					HideZero    = true,
					HasGraphics = false,
				};
				memory.Options.SetComptaEntity (this.compta);
			}

			{
				var memory = this.CreateMemoryData (list, "Vue d'ensemble (niveaux 1 à 3)", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (memory.Filter);
				this.SearchAddProfondeur (memory.Filter, 1, 3);

				memory.Options = new BilanOptions ()
				{
					HideZero    = true,
					HasGraphics = false,
				};
				memory.Options.SetComptaEntity (this.compta);
			}

			{
				var memory = this.CreateMemoryData (list, "Précédent graphique", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (memory.Filter);

				memory.Options = new BilanOptions ()
				{
					HideZero              = true,
					HasGraphics           = true,
					ComparisonEnable      = true,
					ComparisonShowed      = ComparisonShowed.PériodePrécédente,
					ComparisonDisplayMode = ComparisonDisplayMode.Graphique,
				};
				memory.Options.SetComptaEntity (this.compta);
			}

			this.Select<BilanOptions> (list, nomPrésentation);
		}

		private void CreateDefaultMemoryPP(string nomPrésentation)
		{
			var list = this.mainWindowController.GetMemoryList (this.GetKey (nomPrésentation, "Memory"));

			bool searchExist  = true;
			bool filterExist  = true;
			bool optionsExist = true;

			{
				var memory = this.CreateMemoryData (list, DefaultMemory.defaultName, searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (memory.Filter);

				memory.Options = new PPOptions ()
				{
					HideZero    = true,
					HasGraphics = false,
				};
				memory.Options.SetComptaEntity (this.compta);
			}

			{
				var memory = this.CreateMemoryData (list, "Vue d'ensemble (niveau 1)", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (memory.Filter);
				this.SearchAddProfondeur (memory.Filter, 1, 1);

				memory.Options = new PPOptions ()
				{
					HideZero    = true,
					HasGraphics = false,
				};
				memory.Options.SetComptaEntity (this.compta);
			}

			{
				var memory = this.CreateMemoryData (list, "Vue d'ensemble (niveaux 1 et 2)", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (memory.Filter);
				this.SearchAddProfondeur (memory.Filter, 1, 2);

				memory.Options = new PPOptions ()
				{
					HideZero    = true,
					HasGraphics = false,
				};
				memory.Options.SetComptaEntity (this.compta);
			}

			{
				var memory = this.CreateMemoryData (list, "Vue d'ensemble (niveaux 1 à 3)", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (memory.Filter);
				this.SearchAddProfondeur (memory.Filter, 1, 3);

				memory.Options = new PPOptions ()
				{
					HideZero    = true,
					HasGraphics = false,
				};
				memory.Options.SetComptaEntity (this.compta);
			}

			{
				var memory = this.CreateMemoryData (list, "Budget graphique", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (memory.Filter);

				memory.Options = new PPOptions ()
				{
					HideZero              = true,
					HasGraphics           = true,
					ComparisonEnable      = true,
					ComparisonShowed      = ComparisonShowed.Budget,
					ComparisonDisplayMode = ComparisonDisplayMode.Graphique,
				};
				memory.Options.SetComptaEntity (this.compta);
			}

			this.Select<PPOptions> (list, nomPrésentation);
		}

		private void CreateDefaultMemoryExploitation(string nomPrésentation)
		{
			var list = this.mainWindowController.GetMemoryList (this.GetKey (nomPrésentation, "Memory"));

			bool searchExist  = true;
			bool filterExist  = true;
			bool optionsExist = true;

			{
				var memory = this.CreateMemoryData (list, DefaultMemory.defaultName, searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (memory.Filter);
			}

			this.Select<ExploitationOptions> (list, nomPrésentation);
		}

		private void CreateDefaultMemoryBudgets(string nomPrésentation)
		{
			var list = this.mainWindowController.GetMemoryList (this.GetKey (nomPrésentation, "Memory"));

			bool searchExist  = true;
			bool filterExist  = true;
			bool optionsExist = false;

			{
				var memory = this.CreateMemoryData (list, DefaultMemory.defaultName, searchExist, filterExist, optionsExist);
			}

			this.Select (list, nomPrésentation);
		}

		private void CreateDefaultMemoryJournaux(string nomPrésentation)
		{
			var list = this.mainWindowController.GetMemoryList (this.GetKey (nomPrésentation, "Memory"));

			bool searchExist  = true;
			bool filterExist  = false;
			bool optionsExist = false;

			{
				var memory = this.CreateMemoryData (list, DefaultMemory.defaultName, searchExist, filterExist, optionsExist);
			}

			this.Select (list, nomPrésentation);
		}

		private void CreateDefaultMemoryLibellés(string nomPrésentation)
		{
			var list = this.mainWindowController.GetMemoryList (this.GetKey (nomPrésentation, "Memory"));

			bool searchExist  = true;
			bool filterExist  = false;
			bool optionsExist = false;

			{
				var memory = this.CreateMemoryData (list, DefaultMemory.defaultName, searchExist, filterExist, optionsExist);
			}

			this.Select (list, nomPrésentation);
		}

		private void CreateDefaultMemoryModèles(string nomPrésentation)
		{
			var list = this.mainWindowController.GetMemoryList (this.GetKey (nomPrésentation, "Memory"));

			bool searchExist  = true;
			bool filterExist  = false;
			bool optionsExist = false;

			{
				var memory = this.CreateMemoryData (list, DefaultMemory.defaultName, searchExist, filterExist, optionsExist);
			}

			this.Select (list, nomPrésentation);
		}

		private void CreateDefaultMemoryPériodes(string nomPrésentation)
		{
			var list = this.mainWindowController.GetMemoryList (this.GetKey (nomPrésentation, "Memory"));

			bool searchExist  = true;
			bool filterExist  = false;
			bool optionsExist = false;

			{
				var memory = this.CreateMemoryData (list, DefaultMemory.defaultName, searchExist, filterExist, optionsExist);
			}

			this.Select (list, nomPrésentation);
		}


		private MemoryData CreateMemoryData(MemoryList list, FormattedText name, bool searchExist, bool filterExist, bool optionsExist)
		{
			var memory = new MemoryData ()
			{
				Name        = name,
				Readonly    = true,
				Permanent   = true,
				Search      = new SearchData (),
				Filter      = new SearchData (),
				ShowSearch  = searchExist  ? ShowPanelMode.Nop : ShowPanelMode.DoesNotExist,
				ShowFilter  = filterExist  ? ShowPanelMode.Nop : ShowPanelMode.DoesNotExist,
				ShowOptions = optionsExist ? ShowPanelMode.Nop : ShowPanelMode.DoesNotExist,
			};

			list.List.Add (memory);

			return memory;
		}


		private void SearchAdaptForNonZero(SearchData data)
		{
			var tab = data.TabsData[0];

			tab.SearchText.FromText = Converters.MontantToString (0);
			tab.SearchText.Mode     = SearchMode.WholeContent;
			tab.SearchText.Invert   = true;
			tab.Column              = ColumnType.Solde;
		}

		private void SearchAddProfondeur(SearchData data, int minProfondeur, int maxProfondeur)
		{
			var tab = new SearchTabData();
			data.TabsData.Add (tab);

			tab.SearchText.FromText = Converters.IntToString (minProfondeur);
			tab.SearchText.ToText   = Converters.IntToString (maxProfondeur);
			tab.SearchText.Mode     = SearchMode.Interval;
			tab.Column              = ColumnType.Profondeur;

			data.OrMode = false;
		}

		private void SearchAdaptDate(SearchData data, Date début, Date fin)
		{
			var tab = data.TabsData[0];

			tab.SearchText.FromText = Converters.DateToString (début);
			tab.SearchText.ToText   = Converters.DateToString (fin);
			tab.SearchText.Mode     = SearchMode.Interval;
			tab.Column              = ColumnType.Date;
		}

		private void SearchAdaptCatégorie(SearchData data, CatégorieDeCompte catégorie)
		{
			var tab = data.TabsData[0];

			tab.SearchText.FromText = Converters.CatégoriesToString (catégorie);
			tab.SearchText.Mode     = SearchMode.Fragment;
			tab.Column              = ColumnType.Catégorie;
		}


		private void Select<T>(MemoryList list, string nomPrésentation)
			where T : AbstractOptions, new ()
		{
			this.Select (list, nomPrésentation);

			if (list.Selected.Options != null)
			{
				list.Selected.Options.CopyTo (this.mainWindowController.GetSettingsOptions<T> (this.GetKey (nomPrésentation, "Options"), this.compta));
			}
		}

		private void Select(MemoryList list, string nomPrésentation)
		{
			list.SelectedIndex = 0;  // sélectionne "Réglages standards"

			if (list.Selected.Search != null)
			{
				list.Selected.Search.CopyTo (this.mainWindowController.GetSettingsSearchData (this.GetKey (nomPrésentation, "Search")));
			}

			if (list.Selected.Filter != null)
			{
				list.Selected.Filter.CopyTo (this.mainWindowController.GetSettingsSearchData (this.GetKey (nomPrésentation, "Filter")));
			}
		}

		private string GetKey(string nomPrésentation, string typeName)
		{
			//	Retourne par exemple "Présentation.Journal.Search".
			return "Présentation." + nomPrésentation + "." + typeName;
		}


		private readonly static FormattedText				defaultName = "Réglages standards";

		private readonly MainWindowController				mainWindowController;
		private readonly ComptaEntity						compta;
	}
}
