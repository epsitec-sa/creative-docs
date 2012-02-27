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
	/// Données éditables pour un journal de la comptabilité.
	/// </summary>
	public class JournauxEditionLine : AbstractEditionLine
	{
		public JournauxEditionLine(AbstractController controller)
			: base (controller)
		{
			this.datas.Add (ColumnType.Titre,   new EditionData (this.controller, this.ValidateTitle));
			this.datas.Add (ColumnType.Libellé, new EditionData (this.controller));
			this.datas.Add (ColumnType.Pièce,   new EditionData (this.controller, this.ValidatePièce));
		}


		#region Validators
		private void ValidateTitle(EditionData data)
		{
			data.ClearError ();

			if (data.HasText)
			{
				var journal = this.comptaEntity.Journaux.Where (x => x.Nom == data.Text).FirstOrDefault ();
				if (journal == null)
				{
					return;
				}

				var himself = (this.controller.DataAccessor.JustCreated || this.controller.EditorController.Duplicate) ? null : this.controller.DataAccessor.GetEditionEntity (this.controller.DataAccessor.FirstEditedRow) as ComptaJournalEntity;
				if (himself != null && himself.Nom == data.Text)
				{
					return;
				}

				data.Error = "Ce nom de journal existe déjà";
			}
			else
			{
				data.Error = "Il manque le nom du journal";
			}
		}

		private void ValidatePièce(EditionData data)
		{
			data.ClearError ();

			if (data.HasText)
			{
				var pièce = JournauxDataAccessor.GetPiècesGenerator (this.comptaEntity, data.Text);
				if (pièce == null)
				{
					data.Error = "Ce générateur de numéros de pièces n'existe pas";
				}
			}
		}
		#endregion


		public override void EntityToData(AbstractEntity entity)
		{
			var journal = entity as ComptaJournalEntity;

			this.SetText (ColumnType.Titre,   journal.Nom);
			this.SetText (ColumnType.Libellé, journal.Description);
			this.SetText (ColumnType.Pièce, JournauxDataAccessor.GetPiècesGenerator (journal));
		}

		public override void DataToEntity(AbstractEntity entity)
		{
			var journal = entity as ComptaJournalEntity;

			journal.Nom         = this.GetText (ColumnType.Titre);
			journal.Description = this.GetText (ColumnType.Libellé);
			journal.PiècesGenerator = JournauxDataAccessor.GetPiècesGenerator (this.comptaEntity, this.GetText (ColumnType.Pièce));
		}
	}
}