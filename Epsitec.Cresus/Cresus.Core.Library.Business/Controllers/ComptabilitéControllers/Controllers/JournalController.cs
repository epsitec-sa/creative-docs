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
	/// Ce contrôleur gère le journal des écritures de la comptabilité.
	/// </summary>
	public class JournalController : AbstractController<JournalColumn, ComptabilitéEcritureEntity>
	{
		public JournalController(TileContainer tileContainer, ComptabilitéEntity comptabilitéEntity)
			: base (tileContainer, comptabilitéEntity)
		{
			this.dataAccessor = new JournalAccessor (this.comptabilitéEntity);

			this.columnMappers = new List<AbstractColumnMapper<JournalColumn>> ();
			foreach (var mapper in this.ColumnMappers)
			{
				this.columnMappers.Add (mapper);
			}
		}


		protected override FormattedText GetArrayText(int row, int column)
		{
			//	Retourne le texte contenu dans une cellule.
			var mapper = this.columnMappers[column];
			return this.dataAccessor.GetText (row, mapper.Column);
		}


		protected override void CreateFooter(FrameBox parent)
		{
			this.footerController = new JournalFooterController (this.tileContainer, this.comptabilitéEntity, this.dataAccessor, this.columnMappers, this.arrayController);
			this.footerController.CreateUI (parent, this.UpdateArrayContent);
		}

		protected override void FinalizeFooter(FrameBox parent)
		{
			this.footerController.FinalizeUI (parent);
		}


		#region Column Mappers
		private IEnumerable<ColumnMapper> ColumnMappers
		{
			get
			{
				yield return new ColumnMapper (JournalColumn.Date,    this.ValidateDate,    0.20, ContentAlignment.MiddleLeft,  "Date",    "Date de l'écriture");
				yield return new ColumnMapper (JournalColumn.Débit,   this.ValidateCompte,  0.25, ContentAlignment.MiddleLeft,  "Débit",   "Numéro ou nom du compte à débiter");
				yield return new ColumnMapper (JournalColumn.Crédit,  this.ValidateCompte,  0.25, ContentAlignment.MiddleLeft,  "Crédit",  "Numéro ou nom du compte à créditer");
				yield return new ColumnMapper (JournalColumn.Pièce,   null,                 0.20, ContentAlignment.MiddleLeft,  "Pièce",   "Numéro de la pièce comptable correspondant à l'écriture");
				yield return new ColumnMapper (JournalColumn.Libellé, this.ValidateLibellé, 0.80, ContentAlignment.MiddleLeft,  "Libellé", "Libellé de l'écriture");
				yield return new ColumnMapper (JournalColumn.Montant, this.ValidateMontant, 0.25, ContentAlignment.MiddleRight, "Montant", "Montant de l'écriture");
			}
		}

		private class ColumnMapper : AbstractColumnMapper<JournalColumn>
		{
			public ColumnMapper(JournalColumn column, ValidateFunction validate, double relativeWidth, ContentAlignment alignment, FormattedText description, FormattedText tooltip)
				: base (column, validate, relativeWidth, alignment, description, tooltip)
			{
			}
		}
		#endregion


		#region Validators
		private FormattedText ValidateDate(JournalColumn column, ref FormattedText text)
		{
			Date? date;
			var accessor = this.dataAccessor as JournalAccessor;

			if (this.comptabilitéEntity.ParseDate (text, out date) && date.HasValue)
			{
				text = date.ToString ();
				return FormattedText.Empty;
			}
			else
			{
				var b = (this.comptabilitéEntity.BeginDate.HasValue) ? this.comptabilitéEntity.BeginDate.Value.ToString () : "?";
				var e = (this.comptabilitéEntity.  EndDate.HasValue) ? this.comptabilitéEntity.  EndDate.Value.ToString () : "?";

				return string.Format ("La date est incorrecte<br/>Elle devrait être comprise entre {0} et {1}", b, e);
			}
		}

		private FormattedText ValidateCompte(JournalColumn column, ref FormattedText text)
		{
			if (text.IsNullOrEmpty)
			{
				return "Il manque le numéro du compte";
			}

			if (text == JournalAccessor.multi)
			{
				return FormattedText.Empty;
			}

			var n = PlanComptableAccessor.GetCompteNuméro (text);
			var compte = this.comptabilitéEntity.PlanComptable.Where (x => x.Numéro == n).FirstOrDefault ();

			if (compte == null)
			{
				return "Ce compte n'existe pas";
			}

			if (compte.Type != TypeDeCompte.Normal)
			{
				return "Ce compte n'a pas le type \"Normal\"";
			}

			text = n;
			return FormattedText.Empty;
		}

		private FormattedText ValidateLibellé(JournalColumn column, ref FormattedText text)
		{
			if (text.IsNullOrEmpty)
			{
				return "Il manque le libellé";
			}
			else
			{
				return FormattedText.Empty;
			}
		}

		private FormattedText ValidateMontant(JournalColumn column, ref FormattedText text)
		{
			if (text.IsNullOrEmpty)
			{
				return "Il manque le montant";
			}

			decimal montant;
			if (decimal.TryParse (text.ToSimpleText (), out montant))
			{
				text = montant.ToString ("0.00");
				return FormattedText.Empty;
			}
			else
			{
				return "Le montant n'est pas correct";
			}
		}
		#endregion


	}
}
