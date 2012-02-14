//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Options.Data;

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

			new StaticText
			{
				Parent          = frame,
				Text            = "Choix du journal à utiliser",
				PreferredWidth  = 140,
				PreferredHeight = 20,
				Dock            = DockStyle.Left,
			};

			this.comboJournaux = new TextFieldCombo
			{
				Parent          = frame,
				PreferredWidth  = JournalOptionsController.JournauxWidth,
				PreferredHeight = 20,
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
					if (this.comboJournaux.SelectedItemIndex == this.comptaEntity.Journaux.Count)  // tous les journaux ?
					{
						this.Options.Journal = null;
					}
					else
					{
						this.Options.Journal = this.comptaEntity.Journaux[this.comboJournaux.SelectedItemIndex];
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

				foreach (var journal in this.comptaEntity.Journaux)
				{
					this.comboJournaux.Items.Add (journal.Nom);
				}

				this.comboJournaux.Items.Add (JournalOptionsController.AllJournaux);

				this.comboJournaux.FormattedText = (this.Options.Journal == null) ? JournalOptionsController.AllJournaux : this.Options.Journal.Nom;
			}
		}

		private void UpdateSummary()
		{
			this.summary.Text = this.périodeEntity.GetJournalSummary (this.Options.Journal);
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
