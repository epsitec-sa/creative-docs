//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Debug;
using Epsitec.Common.Dialogs;
using Epsitec.Common.Drawing;
using Epsitec.Common.IO;
using Epsitec.Common.Printing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Dialogs
{
	/// <summary>
	/// Dialogue pour monter les lignes importées d'un fichier V11.
	/// </summary>
	class V11ImportDialog : AbstractDialog
	{
		public V11ImportDialog(CoreApplication application, List<V11.V11AbstractLine> records)
		{
			this.IsApplicationWindow = true;  // pour avoir les boutons Minimize/Maximize/Close !

			this.application = application;
			this.records     = records;
		}


		protected override Window CreateWindow()
		{
			Window window = new Window ();

			this.SetupWindow  (window);
			this.SetupWidgets (window);
			this.SetupEvents  (window);

			window.AdjustWindowSize ();

			return window;
		}

		protected void SetupWindow(Window window)
		{
			this.OwnerWindow = this.application.Window;

			window.Icon = this.application.Window.Icon;
			window.Text = "Contenu du fichier importé";
			window.ClientSize = new Size (800, 400);
			window.Root.WindowStyles = WindowStyles.DefaultDocumentWindow;  // pour avoir les boutons Minimize/Maximize/Close !

			window.WindowCloseClicked += delegate
			{
				this.OnDialogClosed ();
				this.CloseDialog ();
			};
		}

		protected void SetupWidgets(Window window)
		{
			this.table = new CellTable
			{
				Parent = window.Root,
				StyleH = CellArrayStyles.ScrollNorm | CellArrayStyles.Header | CellArrayStyles.Mobile | CellArrayStyles.Separator,
				StyleV = CellArrayStyles.ScrollNorm | CellArrayStyles.Separator | CellArrayStyles.SelectLine,
				Anchor = AnchorStyles.All,
				Margins = new Margins (10, 10, 10, 40),
			};

			this.footer = new FrameBox
			{
				Parent = window.Root,
				PreferredHeight = 20,
				Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
				Margins = new Margins (10, 10, 0, 10),
			};

			this.closeButton = new Button ()
			{
				Parent = this.footer,
				Text = "Fermer",
				ButtonStyle = Common.Widgets.ButtonStyle.DefaultAcceptAndCancel,
				Dock = DockStyle.Right,
				TabIndex = 1,
			};

			this.UpdateTable ();
		}

		protected void SetupEvents(Window window)
		{
			this.closeButton.Clicked += delegate
			{
				this.CloseDialog ();
			};
		}


		private void UpdateTable()
		{
			this.table.SetArraySize (18, this.records.Count);

			this.table.SetWidthColumn (0, 50);
			this.table.SetWidthColumn (1, 50);
			this.table.SetWidthColumn (2, 90);
			this.table.SetWidthColumn (3, 90);
			this.table.SetWidthColumn (4, 80);
			this.table.SetWidthColumn (5, 50);
			this.table.SetWidthColumn (6, 30);
			this.table.SetWidthColumn (7, 80);
			this.table.SetWidthColumn (8, 190);

			this.table.SetWidthColumn (9, 70);
			this.table.SetWidthColumn (10, 70);
			this.table.SetWidthColumn (11, 70);

			this.table.SetWidthColumn (12, 80);
			this.table.SetWidthColumn (13, 30);
			this.table.SetWidthColumn (14, 80);
			this.table.SetWidthColumn (15, 30);

			this.table.SetWidthColumn (16, 50);
			this.table.SetWidthColumn (17, 30);

			this.table.SetHeaderTextH (0, "Type");
			this.table.SetHeaderTextH (1, "Genre");
			this.table.SetHeaderTextH (2, "Remise");
			this.table.SetHeaderTextH (3, "Code");
			this.table.SetHeaderTextH (4, "Origine");
			this.table.SetHeaderTextH (5, "BVR");
			this.table.SetHeaderTextH (6, "");
			this.table.SetHeaderTextH (7, "N° client");
			this.table.SetHeaderTextH (8, "N° référence");

			this.table.SetHeaderTextH (9, "Dépôt");
			this.table.SetHeaderTextH (10, "Traitement");
			this.table.SetHeaderTextH (11, "Crédit");

			this.table.SetHeaderTextH (12, "Montant");
			this.table.SetHeaderTextH (13, "");
			this.table.SetHeaderTextH (14, "Taxe");
			this.table.SetHeaderTextH (15, "");

			this.table.SetHeaderTextH (16, "Rejet");
			this.table.SetHeaderTextH (17, "Nb");

			for (int row = 0; row < this.records.Count; row++)
			{
				this.UpdateTableFillRow (row);
				this.UpdateTableContentRow (row);
			}
		}

		private void UpdateTableFillRow(int row)
		{
			for (int column=0; column<this.table.Columns; column++)
			{
				if (this.table[column, row].IsEmpty)
				{
					var st = new StaticText ();
					st.ContentAlignment = (column == 12 || column == 14) ? ContentAlignment.MiddleRight : ContentAlignment.MiddleLeft;
					st.Dock = DockStyle.Fill;
					st.Margins = new Margins (4, 4, 0, 0);
					this.table[column, row].Insert (st);
				}
			}
		}

		private void UpdateTableContentRow(int row)
		{
			//	Met à jour le contenu d'une ligne de la table.
			V11.V11AbstractLine line = this.records[row];

			this.UpdateTableContentCell (row, 1, line.GenreTransaction.ToString ());
			this.UpdateTableContentCell (row, 2, line.GenreRemise.ToString ());
			this.UpdateTableContentCell (row, 6, line.MonnaieTransaction);
			this.UpdateTableContentCell (row, 7, line.FormatedNoClient);

			if (line is V11.V11RecordLine)
			{
				var record = line as V11.V11RecordLine;

				this.UpdateTableContentCell (row, 0, "Ligne");
				this.UpdateTableContentCell (row, 3, record.CodeTransaction.ToString ());
				this.UpdateTableContentCell (row, 4, record.Origine.ToString ());
				this.UpdateTableContentCell (row, 5, record.BVRTransaction.ToString ());
				this.UpdateTableContentCell (row, 8, record.FormatedNoRéférence);

				this.UpdateTableContentCell (row, 9, Misc.GetDateShortDescription (record.DateDépot));
				this.UpdateTableContentCell (row, 10, Misc.GetDateShortDescription (record.DateTraitement));
				this.UpdateTableContentCell (row, 11, Misc.GetDateShortDescription (record.DateCrédit));

				this.UpdateTableContentCell (row, 12, Misc.PriceToString (line.Montant));
				this.UpdateTableContentCell (row, 13, line.MonnaieMontant);
				this.UpdateTableContentCell (row, 14, Misc.PriceToString (line.Taxes));
				this.UpdateTableContentCell (row, 15, line.MonnaieTaxes);

				this.UpdateTableContentCell (row, 16, record.CodeRejet.ToString ());
			}

			if (line is V11.V11TotalLine)
			{
				var total = line as V11.V11TotalLine;

				this.UpdateTableContentCell (row, 0, "<b>Total</b>");
				//?this.UpdateTableContentCell (row, 8, total.CleTri);
				this.UpdateTableContentCell (row, 10, Misc.GetDateShortDescription (total.DateEtablissement));

				this.UpdateTableContentCell (row, 12, string.Concat ("<b>", Misc.PriceToString (line.Montant), "</b>"));
				this.UpdateTableContentCell (row, 13, line.MonnaieMontant);
				this.UpdateTableContentCell (row, 14, string.Concat ("<b>", Misc.PriceToString (line.Taxes), "</b>"));
				this.UpdateTableContentCell (row, 15, line.MonnaieTaxes);

				this.UpdateTableContentCell (row, 17, total.NbTransactions.ToString ());
			}

			this.table.SelectRow (row, false);
		}

		private void UpdateTableContentCell(int row, int column, string text)
		{
			StaticText st = this.table[column, row].Children[0] as StaticText;
			st.Text = text;
		}


		protected override void OnDialogClosed()
		{
			base.OnDialogClosed ();
		}



		private readonly CoreApplication				application;
		private readonly List<V11.V11AbstractLine>		records;

		private CellTable								table;
		private FrameBox								footer;
		private Button									closeButton;
	}
}
