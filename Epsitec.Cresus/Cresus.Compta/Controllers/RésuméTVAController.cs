//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta.Options.Data;
using Epsitec.Cresus.Compta.Options.Controllers;
using Epsitec.Cresus.Compta.Permanents;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Controllers
{
	/// <summary>
	/// Ce contrôleur gère le résumé TVA de la comptabilité.
	/// </summary>
	public class RésuméTVAController : AbstractController
	{
		public RésuméTVAController(ComptaApplication app, BusinessContext businessContext, MainWindowController mainWindowController)
			: base (app, businessContext, mainWindowController)
		{
			this.dataAccessor = new RésuméTVADataAccessor (this);
		}


		protected override void CreateTopOptions(FrameBox parent)
		{
			this.optionsController = new RésuméTVAOptionsController (this);
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

			this.UpdateArrayContent ();
			this.FilterUpdateTopToolbar ();
			this.UpdateViewSettings ();
		}

		public override ControllerType ControllerType
		{
			get
			{
				return Controllers.ControllerType.RésuméTVA;
			}
		}


		protected override FormattedText GetArrayText(int row, ColumnType columnType)
		{
			//	Retourne le texte contenu dans une cellule.
			var text = this.dataAccessor.GetText (row, columnType);
			var data = this.dataAccessor.GetReadOnlyData (row) as RésuméTVAData;

			if (columnType == ColumnType.Solde)
			{
				if (!data.NeverFiltered)
				{
					text = FormattedText.Empty;
				}
			}

			return data.Typo (text);
		}


		#region Context menu
		protected override VMenu ContextMenu
		{
			//	Retourne le menu contextuel à utiliser.
			get
			{
				var menu = new VMenu ();

				this.PutContextMenuJournal (menu);
				this.PutContextMenuExtrait (menu);

				return menu;
			}
		}

		private void PutContextMenuJournal(VMenu menu)
		{
			var data = this.dataAccessor.GetReadOnlyData (this.arrayController.SelectedRow) as RésuméTVAData;
			var écriture = data.Entity as ComptaEcritureEntity;

			var item = this.PutContextMenuItem (menu, Présentations.GetIcon (ControllerType.Journal), "Montre dans le journal", écriture != null);

			item.Clicked += delegate
			{
				var présentation = this.mainWindowController.ShowPrésentation (ControllerType.Journal);

				int row = (présentation.DataAccessor as JournalDataAccessor).GetIndexOf (écriture);
				if (row != -1)
				{
					présentation.SelectedArrayLine = row;
				}
			};
		}

		private void PutContextMenuExtrait(VMenu menu)
		{
			var data = this.dataAccessor.GetReadOnlyData (this.arrayController.SelectedRow) as RésuméTVAData;

			var item = this.PutContextMenuItem (menu, Présentations.GetIcon (ControllerType.ExtraitDeCompte), string.Format ("Extrait du compte {0}", data.Numéro), !data.Numéro.IsNullOrEmpty);

			item.Clicked += delegate
			{
				var présentation = this.mainWindowController.ShowPrésentation (ControllerType.ExtraitDeCompte);

				var options = présentation.DataAccessor.Options as ExtraitDeCompteOptions;
				options.NuméroCompte = data.Numéro;

				présentation.UpdateAfterChanged ();
			};
		}
		#endregion


		protected override IEnumerable<ColumnMapper> InitialColumnMappers
		{
			get
			{
				yield return new ColumnMapper (ColumnType.Compte,     0.20, ContentAlignment.MiddleLeft,  "Compte");
				yield return new ColumnMapper (ColumnType.CodeTVA,    0.20, ContentAlignment.MiddleLeft,  "Code TVA");
				yield return new ColumnMapper (ColumnType.TauxTVA,    0.20, ContentAlignment.MiddleLeft,  "Taux");
				yield return new ColumnMapper (ColumnType.Date,       0.20, ContentAlignment.MiddleLeft,  "Date");
				yield return new ColumnMapper (ColumnType.Pièce,      0.20, ContentAlignment.MiddleLeft,  "Pièce");
				yield return new ColumnMapper (ColumnType.Compte2,    0.20, ContentAlignment.MiddleLeft,  "Compte");
				yield return new ColumnMapper (ColumnType.Titre,      1.00, ContentAlignment.MiddleLeft,  "Code TVA / Titre du compte");
				yield return new ColumnMapper (ColumnType.Montant,    0.20, ContentAlignment.MiddleRight, "Montant HT");
				yield return new ColumnMapper (ColumnType.MontantTVA, 0.20, ContentAlignment.MiddleRight, "TVA");
				yield return new ColumnMapper (ColumnType.Différence, 0.20, ContentAlignment.MiddleRight, "Diff.");
			}
		}

		protected override void UpdateColumnMappers()
		{
			var options = this.dataAccessor.Options as RésuméTVAOptions;

			this.SetColumnParameters (ColumnType.Compte,     (!options.MontreEcritures && !options.ParCodesTVA) || (options.MontreEcritures && options.ParCodesTVA), "Compte");
			this.SetColumnParameters (ColumnType.CodeTVA,    (!options.MontreEcritures && options.ParCodesTVA) || (options.MontreEcritures && !options.ParCodesTVA), "Code TVA");
			this.SetColumnParameters (ColumnType.TauxTVA,    options.MontreEcritures ||  options.ParCodesTVA, "Taux");
			this.SetColumnParameters (ColumnType.Date,       options.MontreEcritures, "Date");
			this.SetColumnParameters (ColumnType.Pièce,      options.MontreEcritures, "Pièce");
			this.SetColumnParameters (ColumnType.Compte2,    !options.MontreEcritures &&  options.ParCodesTVA, "Compte");
			this.SetColumnParameters (ColumnType.Différence, options.MontreEcritures, "Diff.");

			if (options.MontreEcritures)
			{
				this.SetColumnDescription (ColumnType.Titre, options.ParCodesTVA ? "Code TVA / Libellé écriture" : "Compte / Libellé écriture");
			}
			else
			{
				this.SetColumnDescription (ColumnType.Titre, options.ParCodesTVA ? "Titre du compte" : "Code TVA / Titre du compte");
			}

			this.SetColumnDescription (ColumnType.Montant, options.MontantTTC ? "Montant TTC" : "Montant HT");
		}
	}
}
