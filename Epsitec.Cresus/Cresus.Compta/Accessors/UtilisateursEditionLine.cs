//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
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
			this.dataDict.Add (ColumnType.Utilisateur,     new EditionData (this.controller, this.ValidateUtilisateur));
			this.dataDict.Add (ColumnType.NomComplet,      new EditionData (this.controller, this.ValidateNomComplet));
			this.dataDict.Add (ColumnType.DateDébut,       new EditionData (this.controller, this.ValidateDate));
			this.dataDict.Add (ColumnType.DateFin,         new EditionData (this.controller, this.ValidateDate));
			this.dataDict.Add (ColumnType.MotDePasse,      new EditionData (this.controller, this.ValidateMotDePasse));
			this.dataDict.Add (ColumnType.Pièce,           new EditionData (this.controller, this.ValidatePièce));
			this.dataDict.Add (ColumnType.IdentitéWindows, new EditionData (this.controller));
			this.dataDict.Add (ColumnType.Désactivé,       new EditionData (this.controller));
		}


		#region Validators
		private void ValidateUtilisateur(EditionData data)
		{
			data.ClearError ();

			if (data.HasText)
			{
				var utilisateur = this.compta.Utilisateurs.Where (x => x.Utilisateur == data.Text).FirstOrDefault ();
				if (utilisateur == null)
				{
					return;
				}

				var himself = (this.controller.DataAccessor.JustCreated || this.controller.EditorController.Duplicate) ? null : this.controller.DataAccessor.GetEditionEntity (this.controller.DataAccessor.FirstEditedRow) as ComptaUtilisateurEntity;
				if (himself != null && himself.Utilisateur == data.Text)
				{
					return;
				}

				data.Error = "Ce nom d'utilisateur existe déjà";
			}
			else
			{
				data.Error = "Il manque le nom de l'utilisateur";
			}
		}

		private void ValidateNomComplet(EditionData data)
		{
			Validators.ValidateText (data, "Il manque le nom complet");
		}

		private void ValidateDate(EditionData data)
		{
			Validators.ValidateDate (this.période, data, emptyAccepted: true);

			if (!data.HasError && this.IsAdmin && data.HasText)
			{
				data.Error = "L'administrateur ne peut pas avoir de date de validité";
			}
		}

		private void ValidateMotDePasse(EditionData data)
		{
			data.ClearError ();  // toujours ok (pour l'instant)
		}

		private void ValidatePièce(EditionData data)
		{
			data.ClearError ();

			if (data.HasText)
			{
				var pièce = UtilisateursDataAccessor.GetPiècesGenerator (this.compta, data.Text);
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

			this.SetText (ColumnType.Utilisateur,     utilisateur.Utilisateur);
			this.SetText (ColumnType.NomComplet,      utilisateur.NomComplet);
			this.SetText (ColumnType.DateDébut,       Converters.DateToString (utilisateur.DateDébut));
			this.SetText (ColumnType.DateFin,         Converters.DateToString (utilisateur.DateFin));
			this.SetText (ColumnType.MotDePasse,      utilisateur.MotDePasse);
			this.SetText (ColumnType.Pièce,           UtilisateursDataAccessor.GetPiècesGenerator (utilisateur));
			this.SetText (ColumnType.Admin,           utilisateur.Admin ? "admin" : "");
			this.SetText (ColumnType.Présentations,   utilisateur.Présentations);
			this.SetText (ColumnType.IdentitéWindows, utilisateur.IdentitéWindows ? "1":"0");
			this.SetText (ColumnType.Désactivé,       utilisateur.Désactivé ? "1":"0");
		}

		public override void DataToEntity(AbstractEntity entity)
		{
			var utilisateur = entity as ComptaUtilisateurEntity;

			utilisateur.Utilisateur     = this.GetText (ColumnType.Utilisateur);
			utilisateur.NomComplet      = this.GetText (ColumnType.NomComplet);
			utilisateur.DateDébut       = Converters.ParseDate (this.GetText (ColumnType.DateDébut));
			utilisateur.DateFin         = Converters.ParseDate (this.GetText (ColumnType.DateFin));
			utilisateur.MotDePasse      = this.GetText (ColumnType.MotDePasse).ToString ();
			utilisateur.PiècesGenerator = UtilisateursDataAccessor.GetPiècesGenerator (this.compta, this.GetText (ColumnType.Pièce));
			utilisateur.Présentations   = this.GetText (ColumnType.Présentations).ToString ();
			utilisateur.IdentitéWindows = this.GetText (ColumnType.IdentitéWindows) == "1";
			utilisateur.Désactivé       = this.GetText (ColumnType.Désactivé) == "1";
		}


		public bool IsAdmin
		{
			get
			{
				string s = this.GetText (ColumnType.Admin).ToString ();
				return s == "admin";
			}
		}

		public void SetPrésenttion(Command cmd, bool state)
		{
			string s = this.GetText (ColumnType.Présentations).ToString ();
			Converters.SetPrésentationCommand (ref s, cmd, state);
			this.SetText (ColumnType.Présentations, s);
		}

		public bool HasPrésentation(Command cmd)
		{
			string s = this.GetText (ColumnType.Présentations).ToString ();
			return Converters.ContainsPrésentationCommand (s, cmd);
		}
	}
}