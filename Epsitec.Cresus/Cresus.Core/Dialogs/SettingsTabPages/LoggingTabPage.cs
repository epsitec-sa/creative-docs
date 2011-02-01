//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Debug;
using Epsitec.Common.Dialogs;
using Epsitec.Common.Drawing;
using Epsitec.Common.IO;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Database.Logging;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Dialogs.SettingsTabPages
{
	/// <summary>
	/// Onglet de SettingsDialog pour gérer le DbInfrastructure.Logging de la base de données.
	/// </summary>
	public class LoggingTabPage : AbstractSettingsTabPage
	{
		public LoggingTabPage(CoreApplication application)
			: base (application)
		{
		}


		public override void AcceptChangings()
		{
		}

		public override void CreateUI(Widget parent)
		{
			var frame = new FrameBox
			{
				Parent = parent,
				Dock = DockStyle.Fill,
				Margins = new Margins (10),
			};

			{
				var header = new FrameBox
				{
					Parent = frame,
					Dock = DockStyle.Top,
					Margins = new Margins (0, 0, 0, 10),
				};

				this.fullEnableButton = new Button
				{
					Parent = header,
					Text = "Trace complète",
					PreferredWidth = 120,
					Dock = DockStyle.Left,
					Margins = new Margins (0, 1, 0, 0),
				};

				this.shortEnableButton = new Button
				{
					Parent = header,
					Text = "Trace réduite",
					PreferredWidth = 120,
					Dock = DockStyle.Left,
					Margins = new Margins (0, 10, 0, 0),
				};

				this.disableButton = new Button
				{
					Parent = header,
					Text = "Plus de trace",
					PreferredWidth = 120,
					Dock = DockStyle.Left,
					Margins = new Margins (0, 50, 0, 0),
				};

				this.updateButton = new Button
				{
					Parent = header,
					Text = "Met à jour la table",
					PreferredWidth = 120,
					Dock = DockStyle.Left,
					Margins = new Margins (0, 1, 0, 0),
				};

				this.clearButton = new Button
				{
					Parent = header,
					Text = "Vide la table",
					PreferredWidth = 120,
					Dock = DockStyle.Left,
					Margins = new Margins (0, 10, 0, 0),
				};
			}

			this.table = new CellTable
			{
				Parent = frame,
				StyleH = CellArrayStyles.Separator | CellArrayStyles.Header,
				StyleV = CellArrayStyles.ScrollNorm | CellArrayStyles.Separator | CellArrayStyles.SelectLine,
				Dock = DockStyle.Fill,
			};

			//	Connection des événements.
			this.fullEnableButton.Clicked += delegate
			{
				this.FullEnable ();
			};

			this.shortEnableButton.Clicked += delegate
			{
				this.ShortEnable ();
			};

			this.disableButton.Clicked += delegate
			{
				this.Disable ();
			};

			this.updateButton.Clicked += delegate
			{
				this.UpdateTable ();
			};

			this.clearButton.Clicked += delegate
			{
				this.ClearTable ();
			};

			this.UpdateTable ();
		}


		private void FullEnable()
		{
			var db = this.application.Data.DataInfrastructure.DbInfrastructure;

			db.EnableLogging ();
			db.QueryLog.Mode = Database.Logging.LogMode.Extended;
		}

		private void ShortEnable()
		{
			var db = this.application.Data.DataInfrastructure.DbInfrastructure;

			db.EnableLogging ();
			db.QueryLog.Mode = Database.Logging.LogMode.Basic;
		}

		private void Disable()
		{
			var db = this.application.Data.DataInfrastructure.DbInfrastructure;

			db.DisableLogging ();
		}

		private void UpdateTable()
		{
			var db = this.application.Data.DataInfrastructure.DbInfrastructure;

			int count = (db.QueryLog == null) ? 0 : db.QueryLog.GetNbEntries ();

			this.table.SetArraySize (4, count);
			this.table.SetWidthColumn (0, 120);
			this.table.SetWidthColumn (1, 100);
			this.table.SetWidthColumn (2, 100);
			this.table.SetWidthColumn (3, 400);

			this.table.SetHeaderTextH (0, "Début");
			this.table.SetHeaderTextH (1, "Durée");
			this.table.SetHeaderTextH (2, "Opération");
			this.table.SetHeaderTextH (3, "Détails");

			for (int row=0; row<count; row++)
			{
				this.TableFillRow (row);
				this.TableUpdateRow (row, db.QueryLog.GetEntry (row));
			}
		}

		private void TableFillRow(int row)
		{
			//	Peuple une ligne de la table, si nécessaire.
			if (this.table[0, row].IsEmpty)
			{
				var text = new StaticText
				{
					ContentAlignment = ContentAlignment.MiddleLeft,
					Dock = DockStyle.Fill,
					Margins = new Margins (4, 4, 0, 0),
				};

				this.table[0, row].Insert (text);
			}

			if (this.table[1, row].IsEmpty)
			{
				var text = new StaticText
				{
					ContentAlignment = ContentAlignment.MiddleLeft,
					Dock = DockStyle.Fill,
					Margins = new Margins (4, 4, 0, 0),
				};

				this.table[1, row].Insert (text);
			}

			if (this.table[2, row].IsEmpty)
			{
				var text = new StaticText
				{
					ContentAlignment = ContentAlignment.MiddleLeft,
					Dock = DockStyle.Fill,
					Margins = new Margins (4, 4, 0, 0),
				};

				this.table[2, row].Insert (text);
			}

			if (this.table[3, row].IsEmpty)
			{
				var text = new StaticText
				{
					ContentAlignment = ContentAlignment.MiddleLeft,
					Dock = DockStyle.Fill,
					Margins = new Margins (4, 4, 0, 0),
				};

				this.table[3, row].Insert (text);
			}
		}

		private void TableUpdateRow(int row, Query query)
		{
			//	Met à jour le contenu d'une ligne de la table.
			StaticText text;

			text = this.table[0, row].Children[0] as StaticText;
			text.FormattedText = query.StartTime.ToString ();

			text = this.table[1, row].Children[0] as StaticText;
			text.FormattedText = query.Duration.ToString ();

			// TODO: remplir les autres colonnes
		}

		private void ClearTable()
		{
			var db = this.application.Data.DataInfrastructure.DbInfrastructure;

			db.QueryLog.Clear ();
			this.UpdateTable ();
		}



		private Button			fullEnableButton;
		private Button			shortEnableButton;
		private Button			disableButton;
		private Button			updateButton;
		private Button			clearButton;
		private CellTable		table;
	}
}
