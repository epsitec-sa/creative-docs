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
	public class ListesTVAEditionLine : AbstractEditionLine
	{
		public ListesTVAEditionLine(AbstractController controller)
			: base (controller)
		{
			this.dataDict.Add (ColumnType.Nom,  new EditionData (this.controller, this.ValidateNom));
			this.dataDict.Add (ColumnType.Taux, new EditionData (this.controller, this.ValidateTaux));
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

				data.Error = "Ce nom de taux de TVA existe déjà";
			}
			else
			{
				data.Error = "Il manque le nom du taux de TVA";
			}
		}

		private void ValidateTaux(EditionData data)
		{
			data.ClearError ();

			if (!data.Texts.Any ())
			{
				data.Error = "Il faut donner au moins un taux";
			}
		}
		#endregion


		public override void EntityToData(AbstractEntity entity)
		{
			var liste = entity as ComptaListeTVAEntity;

			this.SetText (ColumnType.Nom, liste.Nom);
			this.GetTaux (liste);
		}

		public override void DataToEntity(AbstractEntity entity)
		{
			var liste = entity as ComptaListeTVAEntity;

			liste.Nom = this.GetText (ColumnType.Nom);
			this.SetTaux (liste);
		}

		private void GetTaux(ComptaListeTVAEntity listeTVA)
		{
			//	Parameters <- ensemble des taux définis
			var parameters = this.GetParameters (ColumnType.Taux);
			parameters.Clear ();

			foreach (var taux in this.compta.TauxTVA)
			{
				parameters.Add (taux.Nom.ToString ());
			}

			//	Texts <- taux utilisés par le codeTVA
			var texts = this.GetTexts (ColumnType.Taux);
			texts.Clear ();

			foreach (var taux in listeTVA.Taux)
			{
				texts.Add (taux.Nom.ToString ());
			}
		}

		private void SetTaux(ComptaListeTVAEntity listeTVA)
		{
			listeTVA.Taux.Clear ();

			var texts = this.GetTexts (ColumnType.Taux);
			foreach (var text in texts)
			{
				var taux = this.compta.TauxTVA.Where (x => x.Nom == text).FirstOrDefault ();

				if (taux != null && !listeTVA.Taux.Contains (taux))
				{
					listeTVA.Taux.Add (taux);
				}
			}
		}
	}
}