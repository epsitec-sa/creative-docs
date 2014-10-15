//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.DataFillers;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	/// <summary>
	/// Montre quelques exemples d'écritures, en fonction des comptes définis par
	/// l'objet en édition.
	/// </summary>
	public class EntrySamples
	{
		public EntrySamples(DataAccessor accessor, System.DateTime? forcedDate)
		{
			this.accessor   = accessor;
			this.forcedDate = forcedDate;

			this.entries = new Entries (this.accessor);
		}

		public void Dispose()
		{
			this.entries.Dispose ();
		}


		public void CreateUI(Widget parent)
		{
			const int margins = 5;

			var box = new FrameBox
			{
				Parent    = parent,
				Dock      = DockStyle.Top,
				Margins   = new Margins (0, 0, 10, 0),
				Padding   = new Margins (margins),
				BackColor = ColorManager.WindowBackgroundColor,
			};

			new StaticText
			{
				Parent           = box,
				Text             = Res.Strings.EntrySamples.Title.ToString (),
				ContentAlignment = ContentAlignment.MiddleCenter,
				Dock             = DockStyle.Top,
				Margins          = new Margins (0, 0, 0, margins),
			};

			int totalRows = EntrySamples.Scenarios.Count ();

			this.treeTable = new TreeTable (EntrySamples.rowHeight, EntrySamples.rowHeight, 0)
			{
				Parent          = box,
				Dock            = DockStyle.Top,
				PreferredHeight = EntrySamples.rowHeight * (1+totalRows) + AbstractScroller.DefaultBreadth,
			};

			int w12 = EntrySamples.accountWidth;
			int w3  = AbstractView.editionWidth - 20 - AbstractView.scrollerDefaultBreadth - margins*2 - w12*2;
			
			var c1 = new TreeTableColumnDescription (ObjectField.EntryDebitAccount,  TreeTableColumnType.String, w12, Res.Strings.EntryController.Debit.ToString ());
			var c2 = new TreeTableColumnDescription (ObjectField.EntryCreditAccount, TreeTableColumnType.String, w12, Res.Strings.EntryController.Credit.ToString ());
			var c3 = new TreeTableColumnDescription (ObjectField.EntryTitle,         TreeTableColumnType.String, w3,  Res.Strings.EntryController.Title.ToString ());

			var columns = new TreeTableColumnDescription[] { c1, c2, c3 };
			this.treeTable.SetColumns (columns, SortingInstructions.Default, 0, "EntrySamples");
		}

		public void Update()
		{
			int totalRows = EntrySamples.Scenarios.Count ();

			for (int c=0; c<3; c++)
			{
				var columnItem = new TreeTableColumnItem ();

				for (int r=0; r<totalRows; r++)
				{
					string text, tooltip;
					this.GetContent (r, c, out text, out tooltip);
					columnItem.AddRow (new TreeTableCellString (text, CellState.None, tooltip));
				}

				this.treeTable.SetColumnCells (c, columnItem);
			}
		}


		private void GetContent(int row, int column, out string text, out string tooltip)
		{
			//	Retourne le contenu d'une cellule du tableau.
			var scenario = EntrySamples.GetScenario (row);
			this.entries.GetSample (scenario, column, out text, out tooltip);

			if (column == 0 || column == 1)  // colonne débit ou crédit ?
			{
				var baseType = this.accessor.Mandat.GetAccountsBase (this.EffectiveDate);
				var account = AccountsLogic.GetSummary (this.accessor, baseType, text);

				if (!string.IsNullOrEmpty (account))
				{
					tooltip = string.Concat (tooltip, "<br/>", account);
				}
			}
		}

		private System.DateTime EffectiveDate
		{
			get
			{
				if (this.forcedDate.HasValue)  // y a-t-il une date forcée ?
				{
					//	Si oui, elle prend le dessus.
					return this.forcedDate.Value;
				}
				else
				{
					return this.accessor.EditionAccessor.EventDate;
				}
			}
		}


		private static EntryScenario GetScenario(int row)
		{
			//	Retourne le scénario à utiliser pour une ligne donnée.
			var s = EntrySamples.Scenarios.ToArray ();

			if (row >= 0 && row < s.Length)
			{
				return s[row];
			}
			else
			{
				return EntryScenario.None;
			}
		}

		private static IEnumerable<EntryScenario> Scenarios
		{
			//	Enumère les scénarios montrés en exemple, dans l'ordre souhaité.
			get
			{
				yield return EntryScenario.Purchase;
				yield return EntryScenario.AmortizationAuto;
				yield return EntryScenario.AmortizationExtra;
				yield return EntryScenario.Increase;
				yield return EntryScenario.Decrease;
				yield return EntryScenario.Sale;
			}
		}


		private const int rowHeight    = 18;
		private const int accountWidth = 110;

		private readonly DataAccessor			accessor;
		private readonly System.DateTime?		forcedDate;
		private readonly Entries				entries;

		private TreeTable						treeTable;
	}
}
