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
	/// Données éditables pour un utilisateur de la comptabilité.
	/// </summary>
	public class UtilisateursEditionLine : AbstractEditionLine
	{
		public UtilisateursEditionLine(AbstractController controller)
			: base (controller)
		{
			this.datas.Add (ColumnType.Nom,        new EditionData (this.controller, this.ValidateNom));
			this.datas.Add (ColumnType.MotDePasse, new EditionData (this.controller, this.ValidateMotDePasse));
			this.datas.Add (ColumnType.Opérations, new EditionData (this.controller));
			this.datas.Add (ColumnType.Pièce,      new EditionData (this.controller, this.ValidatePièce));
		}


		#region Validators
		private void ValidateNom(EditionData data)
		{
			Validators.ValidateText (data, "Il manque le nom de l'utilisateur");
		}

		private void ValidateMotDePasse(EditionData data)
		{
			Validators.ValidateText (data, "Il manque le mot de passe");
		}

		private void ValidatePièce(EditionData data)
		{
			data.ClearError ();

			if (data.HasText)
			{
				var pièce = UtilisateursDataAccessor.GetPiècesGenerator (this.comptaEntity, data.Text);
				if (pièce == null)
				{
					data.Error = "Ce générateur de numéros de pièces n'existe pas";
				}
			}
		}
		#endregion


		public override void EntityToData(AbstractEntity entity)
		{
			var utilisateur = entity as ComptaUtilisateurEntity;

			this.SetText (ColumnType.Nom,        utilisateur.Nom);
			this.SetText (ColumnType.MotDePasse, utilisateur.MotDePasse);
			this.SetText (ColumnType.Pièce,      UtilisateursDataAccessor.GetPiècesGenerator (utilisateur));
		}

		public override void DataToEntity(AbstractEntity entity)
		{
			var utilisateur = entity as ComptaUtilisateurEntity;

			utilisateur.Nom        = this.GetText (ColumnType.Nom);
			utilisateur.MotDePasse = this.GetText (ColumnType.MotDePasse).ToSimpleText ();

			utilisateur.PiècesGenerator = UtilisateursDataAccessor.GetPiècesGenerator (this.comptaEntity, this.GetText (ColumnType.Pièce));
		}
	}
}