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
using Epsitec.Cresus.Core.Widgets.Tiles;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Business.Finance;
using Epsitec.Cresus.Core.Business.Finance.Comptabilité;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.ComptabilitéControllers
{
	/// <summary>
	/// Ce contrôleur gère le pied de page pour l'édition de la comptabilité.
	/// </summary>
	public class PlanComptableFooterController : AbstractFooterController<PlanComptableColumn, ComptabilitéCompteEntity>
	{
		public PlanComptableFooterController(TileContainer tileContainer, ComptabilitéEntity comptabilitéEntity, AbstractDataAccessor<PlanComptableColumn, ComptabilitéCompteEntity> dataAccessor, List<AbstractColumnMapper<PlanComptableColumn>> columnMappers, ArrayController<ComptabilitéCompteEntity> arrayController)
			: base (tileContainer, comptabilitéEntity, dataAccessor, columnMappers, arrayController)
		{
		}


		public override void CreateUI(FrameBox parent, System.Action updateArrayContentAction)
		{
			this.footerContainers.Clear ();
			this.footerFields.Clear ();
			this.footerValidatedTexts.Clear ();

			var footerFrame = new FrameBox
			{
				Parent  = parent,
				Dock    = DockStyle.Bottom,
				Margins = new Margins (0, 0, 0, 0),
			};

			int columnCount = this.columnMappers.Count;

			for (int column = 0; column < columnCount; column++)
			{
				var mapper = this.columnMappers[column];

				Widget container;
				AbstractTextField field;

				if (mapper.Column == PlanComptableColumn.Catégorie)
				{
					var marshaler = Marshaler.Create<CatégorieDeCompte> (() => this.Catégorie, x => this.Catégorie = x);
					IEnumerable<EnumKeyValues<CatégorieDeCompte>> possibleItems = EnumKeyValues.FromEnum<CatégorieDeCompte> ();

					UIBuilder.CreateAutoCompleteTextField<CatégorieDeCompte> (footerFrame, marshaler, possibleItems, out container, out field);
					container.TabIndex = column+1;
					field.Index = column;

					field.TextChanged += delegate
					{
						this.FooterTextChanged (field.Index, field.FormattedText);
					};
				}
				else if (mapper.Column == PlanComptableColumn.Type)
				{
					var marshaler = Marshaler.Create<TypeDeCompte> (() => this.Type, x => this.Type = x);
					IEnumerable<EnumKeyValues<TypeDeCompte>> possibleItems = EnumKeyValues.FromEnum<TypeDeCompte> ();

					UIBuilder.CreateAutoCompleteTextField<TypeDeCompte> (footerFrame, marshaler, possibleItems, out container, out field);
					container.TabIndex = column+1;
					field.Index = column;

					field.TextChanged += delegate
					{
						this.FooterTextChanged (field.Index, field.FormattedText);
					};
				}
				else if (mapper.Column == PlanComptableColumn.TVA)
				{
					var marshaler = Marshaler.Create<VatCode> (() => this.TVA, x => this.TVA = x);
					IEnumerable<EnumKeyValues<VatCode>> possibleItems = EnumKeyValues.FromEnum<VatCode> ();

					UIBuilder.CreateAutoCompleteTextField<VatCode> (footerFrame, marshaler, possibleItems, out container, out field);
					container.TabIndex = column+1;
					field.Index = column;

					field.TextChanged += delegate
					{
						this.FooterTextChanged (field.Index, field.FormattedText);
					};
				}
				else if (mapper.Column == PlanComptableColumn.Groupe)
				{
					var comptes = this.comptabilitéEntity.PlanComptable.Where (x => x.Type == TypeDeCompte.Groupe).OrderBy (x => x.Numéro);
					var marshaler = Marshaler.Create<FormattedText> (() => this.CompteGroupe, x => this.CompteGroupe = x);
					UIBuilder.CreateAutoCompleteTextField (footerFrame, comptes, marshaler, out container, out field);
					container.TabIndex = column+1;
					field.Index = column;

					field.TextChanged += delegate
					{
						this.FooterTextChanged (field.Index, field.FormattedText);
					};
				}
				else if (mapper.Column == PlanComptableColumn.CompteOuvBoucl)
				{
					var comptes = this.comptabilitéEntity.PlanComptable.Where (x => x.Type == TypeDeCompte.Normal && x.Catégorie == CatégorieDeCompte.Exploitation).OrderBy (x => x.Numéro);
					var marshaler = Marshaler.Create<FormattedText> (() => this.CompteOuvBoucl, x => this.CompteOuvBoucl = x);
					UIBuilder.CreateAutoCompleteTextField (footerFrame, comptes, marshaler, out container, out field);
					container.TabIndex = column+1;
					field.Index = column;

					field.TextChanged += delegate
					{
						this.FooterTextChanged (field.Index, field.FormattedText);
					};
				}
				else
				{
					field = new TextField
					{
						Parent   = footerFrame,
						Dock     = DockStyle.Left,
						Margins  = new Margins (0, 1, 0, 0),
						Index    = column,
						TabIndex = column+1,
					};

					field.TextChanged += delegate
					{
						this.FooterTextChanged (field.Index, field.FormattedText);
					};

					container = field;
				}

				this.footerContainers.Add (container);
				this.footerFields.Add (field);
				this.footerValidatedTexts.Add (FormattedText.Empty);
			}

			base.CreateUI (parent, updateArrayContentAction);
		}

		protected override FormattedText GetOperationDescription(bool modify)
		{
			return modify ? "Modification d'un compte :" : "Création d'un compte :";
		}


		protected override void CreateEntity(bool silent)
		{
			var compte = this.tileContainer.Controller.DataContext.CreateEntity<ComptabilitéCompteEntity> ();
			this.dataAccessor.Add (compte);
			this.UpdateEntity (silent, compte);
		}

		protected override void FinalUpdateEntities()
		{
			this.PlanComptableUpdate ();
		}


		public override void UpdateFooterContent()
		{
			this.arrayController.IgnoreChanged = true;

			int columnCount = this.columnMappers.Count;
			int row = this.arrayController.SelectedRow;

			if (row == -1 || row >= this.dataAccessor.Count)
			{
				for (int column = 0; column < columnCount; column++)
				{
					var mapper = this.columnMappers[column];

					if (mapper.Column == PlanComptableColumn.Type)
					{
						this.footerFields[column].FormattedText = "Normal";
					}
					else if (mapper.Column == PlanComptableColumn.IndexOuvBoucl)
					{
						this.footerFields[column].FormattedText = "1";
					}
					else
					{
						this.footerFields[column].FormattedText = null;
					}
				}
			}
			else
			{
				for (int column = 0; column < columnCount; column++)
				{
					var mapper = this.columnMappers[column];

					this.footerFields[column].FormattedText = this.dataAccessor.GetText (row, mapper.Column);
				}
			}

			this.FooterValidate ();

			this.arrayController.IgnoreChanged = false;
		}


		private CatégorieDeCompte Catégorie
		{
			get
			{
				var compte = this.arrayController.SelectedEntity;

				if (compte == null)
				{
					return CatégorieDeCompte.Inconnu;
				}
				else
				{
					return compte.Catégorie;
				}
			}
			set
			{
				var compte = this.arrayController.SelectedEntity;

				if (compte != null)
				{
					compte.Catégorie = value;
				}
			}
		}

		private TypeDeCompte Type
		{
			get
			{
				var compte = this.arrayController.SelectedEntity;

				if (compte == null)
				{
					return TypeDeCompte.Normal;
				}
				else
				{
					return compte.Type;
				}
			}
			set
			{
				var compte = this.arrayController.SelectedEntity;

				if (compte != null)
				{
					compte.Type = value;
				}
			}
		}

		private VatCode TVA
		{
			get
			{
				var compte = this.arrayController.SelectedEntity;

				if (compte == null)
				{
					return VatCode.None;
				}
				else
				{
					//?return compte.TVA;
					return VatCode.None;
				}
			}
			set
			{
				var compte = this.arrayController.SelectedEntity;

				if (compte != null)
				{
					//?compte.TVA = value;
				}
			}
		}

		private FormattedText CompteGroupe
		{
			get
			{
				var compte = this.arrayController.SelectedEntity;

				if (compte != null && compte.Groupe != null)
				{
					return PlanComptableAccessor.GetCompteDescription (compte.Groupe);
				}

				return FormattedText.Empty;
			}
			set
			{
				var compte = this.arrayController.SelectedEntity;

				if (compte != null)
				{
					compte.Groupe = PlanComptableAccessor.GetCompteEntity (this.comptabilitéEntity, value);
				}
			}
		}

		private FormattedText CompteOuvBoucl
		{
			get
			{
				var compte = this.arrayController.SelectedEntity;

				if (compte != null && compte.CompteOuvBoucl != null)
				{
					return PlanComptableAccessor.GetCompteDescription (compte.CompteOuvBoucl);
				}

				return FormattedText.Empty;
			}
			set
			{
				var compte = this.arrayController.SelectedEntity;

				if (compte != null)
				{
					compte.CompteOuvBoucl = PlanComptableAccessor.GetCompteEntity (this.comptabilitéEntity, value);
				}
			}
		}


		public void PlanComptableUpdate()
		{
			foreach (var compte in this.comptabilitéEntity.PlanComptable)
			{
				this.comptabilitéEntity.UpdateNiveauCompte (compte);
			}
		}
	}
}
