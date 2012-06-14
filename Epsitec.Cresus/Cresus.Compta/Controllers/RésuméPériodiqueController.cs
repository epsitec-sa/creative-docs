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
	/// Ce contrôleur gère le résumé périodique de la comptabilité.
	/// </summary>
	public class RésuméPériodiqueController : AbstractController
	{
		public RésuméPériodiqueController(ComptaApplication app, BusinessContext businessContext, MainWindowController mainWindowController)
			: base (app, businessContext, mainWindowController)
		{
			this.dataAccessor = new RésuméPériodiqueDataAccessor (this);
		}


		protected override int ArrayLineHeight
		{
			get
			{
				var options = this.dataAccessor.Options as RésuméPériodiqueOptions;

				if (options.HasSideBySideGraph)
				{
					var accessor = this.dataAccessor as RésuméPériodiqueDataAccessor;
					return System.Math.Max (2+accessor.ColumnCount*6, 14);
				}
				else
				{
					return 14;
				}
			}
		}


		protected override void CreateTopOptions(FrameBox parent)
		{
			this.optionsController = new RésuméPériodiqueOptionsController (this);
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
				return Controllers.ControllerType.RésuméPériodique;
			}
		}


		protected override FormattedText GetArrayText(int row, ColumnType columnType)
		{
			//	Retourne le texte contenu dans une cellule.
			var text = this.dataAccessor.GetText (row, columnType);
			var data = this.dataAccessor.GetReadOnlyData (row) as RésuméPériodiqueData;

			var options = this.dataAccessor.Options as RésuméPériodiqueOptions;
			int niveau = this.dataAccessor.FilterData.GetBeginnerNiveau (data.Niveau);

			if (columnType == ColumnType.Titre)
			{
				for (int i = 0; i < niveau; i++)
				{
					text = FormattedText.Concat (UIBuilder.leftIndentText, text);
				}
			}
			else if (columnType >= ColumnType.Solde1 &&
					 columnType <= ColumnType.Solde12)
			{
				var value = Converters.ParseMontant (text);
				if (!data.NeverFiltered && options.ZeroDisplayedInWhite && value.GetValueOrDefault () == 0)
				{
					text = FormattedText.Empty;
				}

				if (!text.IsNullOrEmpty ())
				{
					for (int i = 0; i < niveau; i++)
					{
						text = FormattedText.Concat (text, UIBuilder.rightIndentText);
					}
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
			var data = this.dataAccessor.GetReadOnlyData (this.arrayController.SelectedRow) as RésuméPériodiqueData;
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
			var data = this.dataAccessor.GetReadOnlyData (this.arrayController.SelectedRow) as RésuméPériodiqueData;

			var item = this.PutContextMenuItem (menu, Présentations.GetIcon (ControllerType.ExtraitDeCompte), string.Format ("Extrait du compte {0}", data.Numéro), !data.Numéro.IsNullOrEmpty ());

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
				yield return new ColumnMapper (ColumnType.Numéro,         0.20, ContentAlignment.MiddleLeft,  "Numéro");
				yield return new ColumnMapper (ColumnType.Titre,          1.00, ContentAlignment.MiddleLeft,  "Titre du compte");
																          
				yield return new ColumnMapper (ColumnType.Solde1,         0.20, ContentAlignment.MiddleRight, "");
				yield return new ColumnMapper (ColumnType.Solde2,         0.20, ContentAlignment.MiddleRight, "");
				yield return new ColumnMapper (ColumnType.Solde3,         0.20, ContentAlignment.MiddleRight, "");
				yield return new ColumnMapper (ColumnType.Solde4,         0.20, ContentAlignment.MiddleRight, "");
				yield return new ColumnMapper (ColumnType.Solde5,         0.20, ContentAlignment.MiddleRight, "");
				yield return new ColumnMapper (ColumnType.Solde6,         0.20, ContentAlignment.MiddleRight, "");
				yield return new ColumnMapper (ColumnType.Solde7,         0.20, ContentAlignment.MiddleRight, "");
				yield return new ColumnMapper (ColumnType.Solde8,         0.20, ContentAlignment.MiddleRight, "");
				yield return new ColumnMapper (ColumnType.Solde9,         0.20, ContentAlignment.MiddleRight, "");
				yield return new ColumnMapper (ColumnType.Solde10,        0.20, ContentAlignment.MiddleRight, "");
				yield return new ColumnMapper (ColumnType.Solde11,        0.20, ContentAlignment.MiddleRight, "");
				yield return new ColumnMapper (ColumnType.Solde12,        0.20, ContentAlignment.MiddleRight, "");
				yield return new ColumnMapper (ColumnType.SoldeGraphique, 0.40, ContentAlignment.MiddleRight, "", hideForSearch: true);

				yield return new ColumnMapper (ColumnType.Solde,          0.20, ContentAlignment.MiddleRight, "Solde",      show: false);
				yield return new ColumnMapper (ColumnType.Catégorie,      0.20, ContentAlignment.MiddleLeft,  "Catégorie",  show: false);
				yield return new ColumnMapper (ColumnType.Profondeur,     0.20, ContentAlignment.MiddleLeft,  "Profondeur", show: false);
			}
		}

		protected override void UpdateColumnMappers()
		{
			var options = this.dataAccessor.Options as RésuméPériodiqueOptions;

			//	Cache toutes les colonnes des soldes.
			for (int i = 0; i < 12; i++)
			{
				this.SetColumnParameters (ColumnType.Solde1+i, false, FormattedText.Null);
			}

			//	Montre les colonnes des soldes requises et détermine leurs titres.
			RésuméPériodiqueDataAccessor.ColumnsProcess (this.période, options, (index, dateDébut, dateFin) =>
			{
				this.SetColumnParameters (ColumnType.Solde1+index, true, Dates.GetMonthShortDescription (dateDébut, dateFin));
			});

			this.ShowHideColumn (ColumnType.SoldeGraphique, options.HasStackedGraph || options.HasSideBySideGraph);
		}
	}
}
