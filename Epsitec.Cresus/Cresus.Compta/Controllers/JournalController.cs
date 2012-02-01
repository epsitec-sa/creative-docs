//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Controllers
{
	/// <summary>
	/// Ce contrôleur gère le journal des écritures de la comptabilité.
	/// </summary>
	public class JournalController : AbstractController
	{
		public JournalController(Application app, BusinessContext businessContext, ComptaEntity comptaEntity, MainWindowController mainWindowController)
			: base (app, businessContext, comptaEntity, mainWindowController)
		{
			this.dataAccessor = new JournalDataAccessor (this);
		}


		protected override void CreateOptions(FrameBox parent)
		{
			this.optionsController = new JournalOptionsController (this);
			this.optionsController.CreateUI (parent, this.OptionsChanged);
			this.optionsController.ShowPanel = this.ShowOptionsPanel;

			this.UpdateColumnMappers ();
		}

		protected override void OptionsChanged()
		{
			this.dataAccessor.UpdateAfterOptionsChanged ();
			this.ClearHilite ();
			this.UpdateColumnMappers ();
			this.UpdateArray ();
			this.footerController.UpdateFooterContent ();
			this.UpdateArrayContent ();
			this.UpdateTitle ();
			this.FilterUpdateTopToolbar ();
		}

		protected override void UpdateTitle()
		{
			var journal = (this.dataAccessor.AccessorOptions as JournalOptions).Journal;

			if (journal == null)  // tous les journaux ?
			{
				var name = TextFormatter.FormatText (JournalOptionsController.AllJournaux).ApplyFontColor (Color.FromName ("Red"));
				this.SetTitle (name);
			}
			else
			{
				this.SetTitle (TextFormatter.FormatText ("Journal", journal.Name));
			}
		}


		public override bool HasShowSearchPanel
		{
			get
			{
				return true;
			}
		}

		public override bool HasShowFilterPanel
		{
			get
			{
				return true;
			}
		}

		public override bool HasShowOptionsPanel
		{
			get
			{
				return true;
			}
		}

		public override bool HasShowInfoPanel
		{
			get
			{
				return true;
			}
		}


		protected override FormattedText GetArrayText(int row, ColumnType columnType)
		{
			//	Retourne le texte contenu dans une cellule.
			return this.dataAccessor.GetText (row, columnType);
		}


		protected override void CreateFooter(FrameBox parent)
		{
			this.footerController = new JournalFooterController (this);
			this.footerController.CreateUI (parent, this.UpdateArrayContent);
			this.footerController.ShowInfoPanel = this.ShowInfoPanel;
		}


		protected override IEnumerable<ColumnMapper> InitialColumnMappers
		{
			get
			{
				yield return new ColumnMapper (ColumnType.Date,    0.20, ContentAlignment.MiddleLeft,  "Date",    "Date de l'écriture");
				yield return new ColumnMapper (ColumnType.Débit,   0.25, ContentAlignment.MiddleLeft,  "Débit",   "Numéro ou nom du compte à débiter");
				yield return new ColumnMapper (ColumnType.Crédit,  0.25, ContentAlignment.MiddleLeft,  "Crédit",  "Numéro ou nom du compte à créditer");
				yield return new ColumnMapper (ColumnType.Pièce,   0.20, ContentAlignment.MiddleLeft,  "Pièce",   "Numéro de la pièce comptable correspondant à l'écriture");
				yield return new ColumnMapper (ColumnType.Libellé, 0.80, ContentAlignment.MiddleLeft,  "Libellé", "Libellé de l'écriture");
				yield return new ColumnMapper (ColumnType.Montant, 0.25, ContentAlignment.MiddleRight, "Montant", "Montant de l'écriture");
				yield return new ColumnMapper (ColumnType.Journal, 0.25, ContentAlignment.MiddleLeft,  "Journal", "Journal auquel appartient l'écriture");
			}
		}

		protected override void UpdateColumnMappers()
		{
			var options = this.dataAccessor.AccessorOptions as JournalOptions;

			//?this.ShowHideColumn (ColumnType.Journal, this.optionsController != null && (this.optionsController.Options as JournalOptions).Journal == null);
			this.ShowHideColumn (ColumnType.Journal, options != null && options.Journal == null);  // tous les journaux ?
		}

#if false
		private void CreateManyEcritures()
		{
			var débit  = this.comptaEntity.PlanComptable.Where(x => x.Numéro == "1000").FirstOrDefault ();
			var crédit = this.comptaEntity.PlanComptable.Where(x => x.Numéro == "1020").FirstOrDefault ();

			for (int i = 0; i < 10000; i++)
			{
				var écriture = this.businessContext.DataContext.CreateEntity<ComptaEcritureEntity> ();

				écriture.Date    = new Date (2011, 3, (i/1000)%31+1);
				écriture.Débit   = débit;
				écriture.Crédit  = crédit;
				écriture.Pièce   = "Test";
				écriture.Libellé = string.Format ("Virement {0}", i.ToString ());
				écriture.Montant = i%100+1;

				this.comptaEntity.Journal.Add (écriture);
			}
		}
#endif
	}
}
