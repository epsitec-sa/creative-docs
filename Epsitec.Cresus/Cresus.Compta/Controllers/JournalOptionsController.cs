//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Controllers
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

			this.UpdateWidgets ();
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
				this.UpdateSummary ();
			}
		}


		private void CreateJournalUI(FrameBox parent)
		{
			this.frame = new FrameBox
			{
				Parent          = parent,
				PreferredHeight = 20,
				Dock            = DockStyle.Top,
			};

			new StaticText
			{
				Parent          = this.frame,
				Text            = "Choix du journal à utiliser",
				PreferredWidth  = 140,
				PreferredHeight = 20,
				Dock            = DockStyle.Left,
			};

			this.comboJournaux = new TextFieldCombo
			{
				Parent          = this.frame,
				PreferredWidth  = JournalOptionsController.JournauxWidth,
				PreferredHeight = 20,
				IsReadOnly      = true,
				Dock            = DockStyle.Left,
			};

			this.summary = new StaticText
			{
				Parent  = this.frame,
				Dock    = DockStyle.Fill,
				Margins = new Margins (20, 0, 0, 0),
			};

			this.UpdateCombo ();

			this.comboJournaux.SelectedItemChanged += delegate
			{
				if (!this.ignoreChange && this.comboJournaux.SelectedItemIndex != -1)
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
			this.comboJournaux.Items.Clear ();

			foreach (var journal in this.comptaEntity.Journaux)
			{
				this.comboJournaux.Items.Add (journal.Nom);
			}

			this.comboJournaux.Items.Add (JournalOptionsController.AllJournaux);

			this.ignoreChange = true;
			this.comboJournaux.FormattedText = (this.Options.Journal == null) ? JournalOptionsController.AllJournaux : this.Options.Journal.Nom;
			this.ignoreChange = false;
		}

		private void UpdateSummary()
		{
			var summary = this.périodeEntity.GetJournalSummary (this.Options.Journal);

			this.summary.Text = summary;
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

		private FrameBox				frame;
		private TextFieldCombo			comboJournaux;
		private StaticText				summary;
	}
}
