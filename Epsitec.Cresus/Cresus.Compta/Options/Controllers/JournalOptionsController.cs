//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Options.Data;
using Epsitec.Cresus.Compta.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Options.Controllers
{
	/// <summary>
	/// Ce contrôleur gère les options d'affichage du journal des écritures de la comptabilité.
	/// </summary>
	public class JournalOptionsController : AbstractOptionsController
	{
		public JournalOptionsController(AbstractController controller)
			: base (controller)
		{
		}


		public override void CreateUI(FrameBox parent, System.Action optionsChanged)
		{
			base.CreateUI (parent, optionsChanged);

			this.CreateJournalUI (this.mainFrame);
		}

		protected override bool HasBeginnerSpecialist
		{
			get
			{
				return false;
			}
		}

		protected override void OptionsChanged()
		{
			this.UpdateWidgets ();
			base.OptionsChanged ();
		}

		protected override void UpdateWidgets()
		{
			this.UpdateCombo ();
			this.UpdateSummary ();

			base.UpdateWidgets ();
		}


		public override void UpdateContent()
		{
			base.UpdateContent ();

			if (this.showPanel)
			{
				this.UpdateCombo ();
				this.UpdateSummary ();
			}
		}


		private void CreateJournalUI(FrameBox parent)
		{
			var frame = new FrameBox
			{
				Parent          = parent,
				PreferredHeight = 20,
				Dock            = DockStyle.Top,
				TabIndex        = ++this.tabIndex,
			};

			var label = new StaticText
			{
				Parent          = frame,
				Text            = "Journal",
				PreferredHeight = 20,
				Dock            = DockStyle.Left,
				Margins         = new Margins (0, 10, 0, 0),
			};

			label.PreferredWidth = label.GetBestFitSize ().Width;

			this.comboJournaux = new TextFieldCombo
			{
				Parent          = frame,
				PreferredWidth  = JournalOptionsController.JournauxWidth,
				PreferredHeight = 20,
				MenuButtonWidth = UIBuilder.ComboButtonWidth,
				IsReadOnly      = true,
				Dock            = DockStyle.Left,
				TabIndex        = ++this.tabIndex,
			};

			this.summary = new StaticText
			{
				Parent  = frame,
				Dock    = DockStyle.Fill,
				Margins = new Margins (20, 0, 0, 0),
			};

			this.UpdateWidgets ();

			this.comboJournaux.SelectedItemChanged += delegate
			{
				if (this.ignoreChanges.IsZero && this.comboJournaux.SelectedItemIndex != -1)
				{
					if (this.comboJournaux.SelectedItemIndex == this.compta.Journaux.Count)  // tous les journaux ?
					{
						this.Options.JournalId = 0;
					}
					else
					{
						this.Options.JournalId = this.compta.Journaux[this.comboJournaux.SelectedItemIndex].Id;
					}

					this.OptionsChanged ();
				}
			};
		}


		private void UpdateCombo()
		{
			using (this.ignoreChanges.Enter ())
			{
				this.comboJournaux.Items.Clear ();
				FormattedText sel = JournalOptionsController.AllJournaux;

				foreach (var journal in this.compta.Journaux)
				{
					this.comboJournaux.Items.Add (journal.Nom);

					if (journal.Id ==  this.Options.JournalId)
					{
						sel = journal.Nom;
					}
				}

				this.comboJournaux.Items.Add (JournalOptionsController.AllJournaux);
				this.comboJournaux.FormattedText = sel;
			}
		}

		private void UpdateSummary()
		{
			var journal = this.compta.Journaux.Where (x => x.Id == this.Options.JournalId).FirstOrDefault ();
			this.summary.Text = this.période.GetJournalSummary (journal);
		}


		private JournalOptions Options
		{
			get
			{
				return this.options as JournalOptions;
			}
		}


		private static readonly double JournauxWidth = 200;
		public static readonly string AllJournaux = "Tous les journaux";

		private TextFieldCombo			comboJournaux;
		private StaticText				summary;
	}
}
