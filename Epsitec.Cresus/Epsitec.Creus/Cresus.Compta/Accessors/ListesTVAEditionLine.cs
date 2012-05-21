//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Fields.Controllers;
using Epsitec.Cresus.Compta.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Accessors
{
	/// <summary>
	/// Données éditables pour un taux de TVA de la comptabilité.
	/// </summary>
	public class ListesTVAEditionLine : AbstractEditionLine
	{
		public ListesTVAEditionLine(AbstractController controller)
			: base (controller)
		{
			this.dataDict.Add (ColumnType.Nom,  new EditionData (this.controller, this.ValidateNom));
			this.dataDict.Add (ColumnType.Taux, new EditionData (this.controller));
		}


		#region Validators
		private void ValidateNom(EditionData data)
		{
			data.ClearError ();

			if (data.HasText)
			{
				var taux = this.compta.ListesTVA.Where (x => x.Nom == data.Text).FirstOrDefault ();
				if (taux == null)
				{
					return;
				}

				var himself = (this.controller.DataAccessor.JustCreated || this.controller.EditorController.Duplicate) ? null : this.controller.DataAccessor.GetEditionEntity (this.controller.DataAccessor.FirstEditedRow) as ComptaListeTVAEntity;
				if (himself != null && himself.Nom == data.Text)
				{
					return;
				}

				data.Error = "Ce nom de liste de taux existe déjà";
			}
			else
			{
				data.Error = "Il manque le nom de la liste de taux";
			}
		}
		#endregion


		public override void EntityToData(AbstractEntity entity)
		{
			var liste = entity as ComptaListeTVAEntity;

			this.SetText (ColumnType.Nom, liste.Nom);
			this.TauxToArray (liste);
		}

		public override void DataToEntity(AbstractEntity entity)
		{
			var liste = entity as ComptaListeTVAEntity;

			liste.Nom = this.GetText (ColumnType.Nom);
			this.ArrayToTaux (liste);
		}

		private void TauxToArray(ComptaListeTVAEntity listeTVA)
		{
			//	Transfert la liste des entités de taux dans un tableau.
			//	ComptaListeTVA -> DataArray
			var array = this.GetArray (ColumnType.Taux);
			array.Clear ();

			int row = 0;
			foreach (var taux in listeTVA.Taux)
			{
				array.SetBool    (row, ColumnType.ParDéfaut, taux == listeTVA.TauxParDéfaut);
				array.SetDate    (row, ColumnType.Date, taux.DateDébut);
				array.SetPercent (row, ColumnType.Taux, taux.Taux);

				row++;
			}
		}

		private void ArrayToTaux(ComptaListeTVAEntity listeTVA)
		{
			//	Transfert le tableau des taux dans la liste des entités.
			//	DataArray -> ComptaListeTVA
			var array = this.GetArray (ColumnType.Taux);
			listeTVA.Taux.Clear ();  // efface la liste actuelle

			//	Recrée la liste des ComptaTauxTVAEntity.
			for (int row = 0; row < array.RowCount; row++)
			{
				var taux = new ComptaTauxTVAEntity
				{
					DateDébut = array.GetDate    (row, ColumnType.Date).GetValueOrDefault (),
					Taux      = array.GetPercent (row, ColumnType.Taux).GetValueOrDefault (),
				};

				if (array.GetBool (row, ColumnType.ParDéfaut))
				{
					listeTVA.TauxParDéfaut = taux;
				}

				listeTVA.Taux.Add (taux);
			}
		}
	}
}