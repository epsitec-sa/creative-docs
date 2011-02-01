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

				var group = new FrameBox
				{
					Parent = header,
					Dock = DockStyle.Left,
				};

				this.fullEnableButton = new RadioButton
				{
					Parent = group,
					Text = "Trace complète",
					PreferredWidth = 120,
					AutoToggle = false,
					Dock = DockStyle.Top,
				};

				this.shortEnableButton = new RadioButton
				{
					Parent = group,
					Text = "Trace réduite",
					PreferredWidth = 120,
					AutoToggle = false,
					Dock = DockStyle.Top,
				};

				this.disableButton = new RadioButton
				{
					Parent = group,
					Text = "Plus de trace",
					PreferredWidth = 120,
					AutoToggle = false,
					Dock = DockStyle.Top,
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

			this.clearButton.Clicked += delegate
			{
				this.ClearTable ();
			};

			this.UpdateRadio ();
			this.UpdateTable ();
		}


		private void FullEnable()
		{
			this.LogMode = Database.Logging.LogMode.Extended;
			this.UpdateRadio ();
		}

		private void ShortEnable()
		{
			this.LogMode = Database.Logging.LogMode.Basic;
			this.UpdateRadio ();
		}

		private void Disable()
		{
			this.LogMode = Database.Logging.LogMode.Stopped;
			this.UpdateRadio ();
		}

		private void UpdateRadio()
		{
			this.fullEnableButton.ActiveState  = (this.LogMode == Database.Logging.LogMode.Extended) ? ActiveState.Yes : ActiveState.No;
			this.shortEnableButton.ActiveState = (this.LogMode == Database.Logging.LogMode.Basic   ) ? ActiveState.Yes : ActiveState.No;
			this.disableButton.ActiveState     = (this.LogMode == Database.Logging.LogMode.Stopped ) ? ActiveState.Yes : ActiveState.No;
		}


		private void UpdateTable()
		{
			var db = this.application.Data.DataInfrastructure.DbInfrastructure;

			int count = (db.QueryLog == null) ? 0 : db.QueryLog.GetNbEntries ();

			this.table.SetArraySize (4, count);
			this.table.SetWidthColumn (0, 120);
			this.table.SetWidthColumn (1,  80);
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
					ContentAlignment = ContentAlignment.MiddleRight,
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
			text.FormattedText = LoggingTabPage.GetDuration (query.Duration);

			// TODO: remplir les autres colonnes
		}

		private void ClearTable()
		{
			var db = this.application.Data.DataInfrastructure.DbInfrastructure;

			db.QueryLog.Clear ();
			this.UpdateTable ();
		}


		private static string GetDuration(System.TimeSpan duration)
		{
			// Un Tick vaut 100 nanosecondes.
			return string.Concat ((duration.Ticks/10).ToString (), " us");
		}

		private LogMode LogMode
		{
			get
			{
				var db = this.application.Data.DataInfrastructure.DbInfrastructure;

				if (db.QueryLog == null)
				{
					return Database.Logging.LogMode.Stopped;
				}
				else
				{
					return db.QueryLog.Mode;
				}
			}
			set
			{
				var db = this.application.Data.DataInfrastructure.DbInfrastructure;

				if (value == Database.Logging.LogMode.Stopped)
				{
					db.DisableLogging ();
				}
				else
				{
					db.EnableLogging ();
					db.QueryLog.Mode = value;
				}
			}
		}


		private RadioButton			fullEnableButton;
		private RadioButton			shortEnableButton;
		private RadioButton			disableButton;
		private Button				clearButton;
		private CellTable			table;
	}
}
