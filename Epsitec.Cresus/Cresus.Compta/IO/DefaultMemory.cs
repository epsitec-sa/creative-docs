//	Copyright � 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
	/// Cette classe s'occupe de cr�er les styles par d�faut.
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
			this.CreateDefaultMemoryLibell�s        ("Libell�s");
			this.CreateDefaultMemoryMod�les         ("Mod�les");
			this.CreateDefaultMemoryP�riodes        ("P�riodes");
		}


		private void CreateDefaultMemoryJournal(string nomPr�sentation)
		{
			var list = this.mainWindowController.GetMemoryList (this.GetKey (nomPr�sentation, "Memory"));

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
				var memory = this.CreateMemoryData (list, "Deuxi�me trimestre", searchExist, filterExist, optionsExist);
				this.SearchAdaptDate (memory.Filter, new Date (year, 4, 1), new Date (year, 6, 30));

				memory.Options = new JournalOptions ();
				memory.Options.SetComptaEntity (this.compta);
			}

			{
				var memory = this.CreateMemoryData (list, "Troisi�me trimestre", searchExist, filterExist, optionsExist);
				this.SearchAdaptDate (memory.Filter, new Date (year, 7, 1), new Date (year, 9, 30));

				memory.Options = new JournalOptions ();
				memory.Options.SetComptaEntity (this.compta);
			}

			{
				var memory = this.CreateMemoryData (list, "Quatri�me trimestre", searchExist, filterExist, optionsExist);
				this.SearchAdaptDate (memory.Filter, new Date (year, 10, 1), new Date (year, 12, 31));

				memory.Options = new JournalOptions ();
				memory.Options.SetComptaEntity (this.compta);
			}

			this.Select<JournalOptions> (list, nomPr�sentation);
		}

		private void CreateDefaultMemoryPlanComptable(string nomPr�sentation)
		{
			var list = this.mainWindowController.GetMemoryList (this.GetKey (nomPr�sentation, "Memory"));

			bool searchExist  = true;
			bool filterExist  = true;
			bool optionsExist = false;

			{
				var memory = this.CreateMemoryData (list, DefaultMemory.defaultName, searchExist, filterExist, optionsExist);
			}

			{
				var memory = this.CreateMemoryData (list, "Actifs", searchExist, filterExist, optionsExist);
				this.SearchAdaptCat�gorie (memory.Filter, Cat�gorieDeCompte.Actif);
			}

			{
				var memory = this.CreateMemoryData (list, "Passifs", searchExist, filterExist, optionsExist);
				this.SearchAdaptCat�gorie (memory.Filter, Cat�gorieDeCompte.Passif);
			}

			{
				var memory = this.CreateMemoryData (list, "Charges", searchExist, filterExist, optionsExist);
				this.SearchAdaptCat�gorie (memory.Filter, Cat�gorieDeCompte.Charge);
			}

			{
				var memory = this.CreateMemoryData (list, "Produits", searchExist, filterExist, optionsExist);
				this.SearchAdaptCat�gorie (memory.Filter, Cat�gorieDeCompte.Produit);
			}

			{
				var memory = this.CreateMemoryData (list, "Exploitations", searchExist, filterExist, optionsExist);
				this.SearchAdaptCat�gorie (memory.Filter, Cat�gorieDeCompte.Exploitation);
			}

			this.Select (list, nomPr�sentation);
		}

		private void CreateDefaultMemoryBalance(string nomPr�sentation)
		{
			var list = this.mainWindowController.GetMemoryList (this.GetKey (nomPr�sentation, "Memory"));

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
				var memory = this.CreateMemoryData (list, "Vue d'ensemble (niveaux 1 � 3)", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (memory.Filter);
				this.SearchAddProfondeur (memory.Filter, 1, 3);
			}

			this.Select<BalanceOptions> (list, nomPr�sentation);
		}

		private void CreateDefaultMemoryExtraitDeCompte(string nomPr�sentation)
		{
			var list = this.mainWindowController.GetMemoryList (this.GetKey (nomPr�sentation, "Memory"));

			bool searchExist  = true;
			bool filterExist  = true;
			bool optionsExist = true;

			{
				var memory = this.CreateMemoryData (list, DefaultMemory.defaultName, searchExist, filterExist, optionsExist);
			}

			this.Select<ExtraitDeCompteOptions> (list, nomPr�sentation);
		}

		private void CreateDefaultMemoryBilan(string nomPr�sentation)
		{
			var list = this.mainWindowController.GetMemoryList (this.GetKey (nomPr�sentation, "Memory"));

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
				var memory = this.CreateMemoryData (list, "Vue d'ensemble (niveaux 1 � 3)", searchExist, filterExist, optionsExist);
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
				var memory = this.CreateMemoryData (list, "Pr�c�dent graphique", searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (memory.Filter);

				memory.Options = new BilanOptions ()
				{
					HideZero              = true,
					HasGraphics           = true,
					ComparisonEnable      = true,
					ComparisonShowed      = ComparisonShowed.P�riodePr�c�dente,
					ComparisonDisplayMode = ComparisonDisplayMode.Graphique,
				};
				memory.Options.SetComptaEntity (this.compta);
			}

			this.Select<BilanOptions> (list, nomPr�sentation);
		}

		private void CreateDefaultMemoryPP(string nomPr�sentation)
		{
			var list = this.mainWindowController.GetMemoryList (this.GetKey (nomPr�sentation, "Memory"));

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
				var memory = this.CreateMemoryData (list, "Vue d'ensemble (niveaux 1 � 3)", searchExist, filterExist, optionsExist);
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

			this.Select<PPOptions> (list, nomPr�sentation);
		}

		private void CreateDefaultMemoryExploitation(string nomPr�sentation)
		{
			var list = this.mainWindowController.GetMemoryList (this.GetKey (nomPr�sentation, "Memory"));

			bool searchExist  = true;
			bool filterExist  = true;
			bool optionsExist = true;

			{
				var memory = this.CreateMemoryData (list, DefaultMemory.defaultName, searchExist, filterExist, optionsExist);
				this.SearchAdaptForNonZero (memory.Filter);
			}

			this.Select<ExploitationOptions> (list, nomPr�sentation);
		}

		private void CreateDefaultMemoryBudgets(string nomPr�sentation)
		{
			var list = this.mainWindowController.GetMemoryList (this.GetKey (nomPr�sentation, "Memory"));

			bool searchExist  = true;
			bool filterExist  = true;
			bool optionsExist = false;

			{
				var memory = this.CreateMemoryData (list, DefaultMemory.defaultName, searchExist, filterExist, optionsExist);
			}

			this.Select (list, nomPr�sentation);
		}

		private void CreateDefaultMemoryJournaux(string nomPr�sentation)
		{
			var list = this.mainWindowController.GetMemoryList (this.GetKey (nomPr�sentation, "Memory"));

			bool searchExist  = true;
			bool filterExist  = false;
			bool optionsExist = false;

			{
				var memory = this.CreateMemoryData (list, DefaultMemory.defaultName, searchExist, filterExist, optionsExist);
			}

			this.Select (list, nomPr�sentation);
		}

		private void CreateDefaultMemoryLibell�s(string nomPr�sentation)
		{
			var list = this.mainWindowController.GetMemoryList (this.GetKey (nomPr�sentation, "Memory"));

			bool searchExist  = true;
			bool filterExist  = false;
			bool optionsExist = false;

			{
				var memory = this.CreateMemoryData (list, DefaultMemory.defaultName, searchExist, filterExist, optionsExist);
			}

			this.Select (list, nomPr�sentation);
		}

		private void CreateDefaultMemoryMod�les(string nomPr�sentation)
		{
			var list = this.mainWindowController.GetMemoryList (this.GetKey (nomPr�sentation, "Memory"));

			bool searchExist  = true;
			bool filterExist  = false;
			bool optionsExist = false;

			{
				var memory = this.CreateMemoryData (list, DefaultMemory.defaultName, searchExist, filterExist, optionsExist);
			}

			this.Select (list, nomPr�sentation);
		}

		private void CreateDefaultMemoryP�riodes(string nomPr�sentation)
		{
			var list = this.mainWindowController.GetMemoryList (this.GetKey (nomPr�sentation, "Memory"));

			bool searchExist  = true;
			bool filterExist  = false;
			bool optionsExist = false;

			{
				var memory = this.CreateMemoryData (list, DefaultMemory.defaultName, searchExist, filterExist, optionsExist);
			}

			this.Select (list, nomPr�sentation);
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

		private void SearchAdaptDate(SearchData data, Date d�but, Date fin)
		{
			var tab = data.TabsData[0];

			tab.SearchText.FromText = Converters.DateToString (d�but);
			tab.SearchText.ToText   = Converters.DateToString (fin);
			tab.SearchText.Mode     = SearchMode.Interval;
			tab.Column              = ColumnType.Date;
		}

		private void SearchAdaptCat�gorie(SearchData data, Cat�gorieDeCompte cat�gorie)
		{
			var tab = data.TabsData[0];

			tab.SearchText.FromText = Converters.Cat�goriesToString (cat�gorie);
			tab.SearchText.Mode     = SearchMode.Fragment;
			tab.Column              = ColumnType.Cat�gorie;
		}


		private void Select<T>(MemoryList list, string nomPr�sentation)
			where T : AbstractOptions, new ()
		{
			this.Select (list, nomPr�sentation);

			if (list.Selected.Options != null)
			{
				list.Selected.Options.CopyTo (this.mainWindowController.GetSettingsOptions<T> (this.GetKey (nomPr�sentation, "Options"), this.compta));
			}
		}

		private void Select(MemoryList list, string nomPr�sentation)
		{
			list.SelectedIndex = 0;  // s�lectionne "R�glages standards"

			if (list.Selected.Search != null)
			{
				list.Selected.Search.CopyTo (this.mainWindowController.GetSettingsSearchData (this.GetKey (nomPr�sentation, "Search")));
			}

			if (list.Selected.Filter != null)
			{
				list.Selected.Filter.CopyTo (this.mainWindowController.GetSettingsSearchData (this.GetKey (nomPr�sentation, "Filter")));
			}
		}

		private string GetKey(string nomPr�sentation, string typeName)
		{
			//	Retourne par exemple "Pr�sentation.Journal.Search".
			return "Pr�sentation." + nomPr�sentation + "." + typeName;
		}


		private readonly static FormattedText				defaultName = "R�glages standards";

		private readonly MainWindowController				mainWindowController;
		private readonly ComptaEntity						compta;
	}
}
