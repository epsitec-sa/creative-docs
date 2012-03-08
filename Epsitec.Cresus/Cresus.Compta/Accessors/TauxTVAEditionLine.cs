//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Accessors
{
	/// <summary>
	/// Données éditables pour un taux de TVA de la comptabilité.
	/// </summary>
	public class TauxTVAEditionLine : AbstractEditionLine
	{
		public TauxTVAEditionLine(AbstractController controller)
			: base (controller)
		{
			this.dataDict.Add (ColumnType.Nom,       new EditionData (this.controller, this.ValidateNom));
			this.dataDict.Add (ColumnType.DateDébut, new EditionData (this.controller, this.ValidateDate));
			this.dataDict.Add (ColumnType.DateFin,   new EditionData (this.controller, this.ValidateDate));
			this.dataDict.Add (ColumnType.Taux,      new EditionData (this.controller, this.ValidateTaux));
			this.dataDict.Add (ColumnType.ParDéfaut, new EditionData (this.controller));
		}


		#region Validators
		private void ValidateNom(EditionData data)
		{
			data.ClearError ();

			if (data.HasText)
			{
				var taux = this.compta.TauxTVA.Where (x => x.Nom == data.Text).FirstOrDefault ();
				if (taux == null)
				{
					return;
				}

				var himself = (this.controller.DataAccessor.JustCreated || this.controller.EditorController.Duplicate) ? null : this.controller.DataAccessor.GetEditionEntity (this.controller.DataAccessor.FirstEditedRow) as ComptaTauxTVAEntity;
				if (himself != null && himself.Nom == data.Text)
				{
					return;
				}

				data.Error = "Ce nom de taux de TVA existe déjà";
			}
			else
			{
				data.Error = "Il manque le nom du taux de TVA";
			}
		}

		private void ValidateDate(EditionData data)
		{
			Validators.ValidateDate (data, emptyAccepted: true);
		}

		private void ValidateTaux(EditionData data)
		{
			data.ClearError ();

			if (data.HasText)
			{
				decimal? taux = Converters.ParsePercent (data.Text);
				if (taux.HasValue)
				{
					data.Text = Converters.PercentToString (taux);
				}
				else
				{
					data.Error = "Le taux n'est pas correct";
				}
			}
			else
			{
				data.Error = "Il manque le taux";
			}
		}

		private void ValidateMontant(EditionData data)
		{
			Validators.ValidateMontant (data, emptyAccepted: true);
		}
		#endregion


		public override void EntityToData(AbstractEntity entity)
		{
			var tauxTVA = entity as ComptaTauxTVAEntity;

			this.SetText (ColumnType.Nom,       tauxTVA.Nom);
			this.SetText (ColumnType.DateDébut, tauxTVA.DateDébut.ToString ());
			this.SetText (ColumnType.DateFin,   tauxTVA.DateFin.ToString ());
			this.SetText (ColumnType.Taux,      Converters.PercentToString (tauxTVA.Taux));
			this.SetText (ColumnType.ParDéfaut, tauxTVA.ParDéfaut ? "1" : "0");
		}

		public override void DataToEntity(AbstractEntity entity)
		{
			var tauxTVA = entity as ComptaTauxTVAEntity;

			tauxTVA.Nom       = this.GetText (ColumnType.Nom);
			tauxTVA.Taux      = Converters.ParsePercent (this.GetText (ColumnType.Taux)).GetValueOrDefault ();
			tauxTVA.ParDéfaut = this.GetText (ColumnType.ParDéfaut) == "1";

			Date date;

			if (PériodesDataAccessor.ParseDate (this.GetText (ColumnType.DateDébut), out date))
			{
				tauxTVA.DateDébut = date;
			}

			if (PériodesDataAccessor.ParseDate (this.GetText (ColumnType.DateFin), out date))
			{
				tauxTVA.DateFin = date;
			}
		}
	}
}