//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta.Search.Data;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Accessors
{
	/// <summary>
	/// Gère l'accès aux utilisateurs de la comptabilité.
	/// </summary>
	public class UtilisateursDataAccessor : AbstractDataAccessor
	{
		public UtilisateursDataAccessor(AbstractController controller)
			: base (controller)
		{
			this.searchData = this.mainWindowController.GetSettingsSearchData ("Présentation.Utilisateurs.Search");
		}


		public override void UpdateFilter()
		{
			this.UpdateAfterOptionsChanged ();
		}

		public override void UpdateAfterOptionsChanged()
		{
		}


		public override int AllCount
		{
			get
			{
				return this.Count;
			}
		}

		public override int Count
		{
			get
			{
				return this.compta.Utilisateurs.Count;
			}
		}


		public override AbstractEntity GetEditionEntity(int row)
		{
			if (row < 0 || row >= this.compta.Utilisateurs.Count)
			{
				return null;
			}
			else
			{
				return this.compta.Utilisateurs[row];
			}
		}

		public override int GetEditionIndex(AbstractEntity entity)
		{
			if (entity == null)
			{
				return -1;
			}
			else
			{
				return this.compta.Utilisateurs.IndexOf (entity as ComptaUtilisateurEntity);
			}
		}


		public override FormattedText GetText(int row, ColumnType column, bool all = false)
		{
			var utilisateurs = compta.Utilisateurs;

			if (row < 0 || row >= utilisateurs.Count)
			{
				return FormattedText.Null;
			}

			var utilisateur = utilisateurs[row];

			switch (column)
			{
				case ColumnType.Utilisateur:
					return utilisateur.Utilisateur;

				case ColumnType.NomComplet:
					return utilisateur.NomComplet;

				case ColumnType.DateDébut:
					return Converters.DateToString (utilisateur.DateDébut);

				case ColumnType.DateFin:
					return Converters.DateToString (utilisateur.DateFin);

				case ColumnType.MotDePasse:
					return Strings.GetStandardPassword (utilisateur.MotDePasse);

				case ColumnType.Pièce:
					return UtilisateursDataAccessor.GetPiècesGenerator (utilisateur);

				case ColumnType.Résumé:
					return utilisateur.GetAccessSummary ();

				case ColumnType.Icône:
					return utilisateur.GetPasswordIcon ();

				default:
					return FormattedText.Null;
			}
		}


		public override void InsertEditionLine(int index)
		{
			var newData = new UtilisateursEditionLine (this.controller);

			if (index == -1)
			{
				this.editionLine.Add (newData);
			}
			else
			{
				this.editionLine.Insert (index, newData);
			}

			base.InsertEditionLine (index);
		}

		public override void StartCreationLine()
		{
			this.editionLine.Clear ();
			this.editionLine.Add (new UtilisateursEditionLine (this.controller));
			this.PrepareEditionLine (0);

			this.firstEditedRow = -1;
			this.countEditedRow = 1;

			this.isCreation = true;
			this.isModification = false;
			this.justCreated = false;

			this.controller.EditorController.UpdateFieldsEditionData ();
		}

		protected override void PrepareEditionLine(int line)
		{
			string s = null;
			Converters.SetPrésentationCommand (ref s, Res.Commands.Présentation.Open,    true);
			Converters.SetPrésentationCommand (ref s, Res.Commands.Présentation.Save,    true);
			Converters.SetPrésentationCommand (ref s, Res.Commands.Présentation.Print,   true);
			Converters.SetPrésentationCommand (ref s, Res.Commands.Présentation.Journal, true);
			Converters.SetPrésentationCommand (ref s, Res.Commands.Présentation.Balance, true);
			Converters.SetPrésentationCommand (ref s, Res.Commands.Présentation.Extrait, true);

			this.editionLine[line].SetText (ColumnType.Présentations, s);

			base.PrepareEditionLine (line);
		}

		public override void StartModificationLine(int row)
		{
			this.editionLine.Clear ();

			this.firstEditedRow = row;
			this.countEditedRow = 0;

			if (row >= 0 && row < this.compta.Utilisateurs.Count)
			{
				var data = new UtilisateursEditionLine (this.controller);
				var utilisateur = this.compta.Utilisateurs[row];
				data.EntityToData (utilisateur);

				this.editionLine.Add (data);
				this.countEditedRow++;
			}

			this.initialCountEditedRow = this.countEditedRow;
			this.isCreation = false;
			this.isModification = true;
			this.justCreated = false;

			this.controller.EditorController.UpdateFieldsEditionData ();
		}

		public override void UpdateEditionLine()
		{
			if (this.isModification)
			{
				this.UpdateModificationData ();
				this.justCreated = true;
				this.isCreation = true;
				this.isModification = false;
			}
			else
			{
				this.UpdateCreationData ();
				this.justCreated = true;
			}

			this.SearchUpdate ();
			this.controller.MainWindowController.SetDirty ();
		}

		private void UpdateCreationData()
		{
			int firstRow = -1;

			foreach (var data in this.editionLine)
			{
				var utilisateur = this.CreateUtilisateur ();
				data.DataToEntity (utilisateur);

				this.compta.Utilisateurs.Add (utilisateur);

				if (firstRow == -1)
				{
					firstRow = this.compta.Utilisateurs.Count-1;
				}
			}

			this.firstEditedRow = firstRow;
		}

		private void UpdateModificationData()
		{
			int row = this.firstEditedRow;

			var utilisateur = this.compta.Utilisateurs[row];
			this.editionLine[0].DataToEntity (utilisateur);
		}


		public override FormattedText GetRemoveModificationLineError()
		{
			var utilisateur = this.compta.Utilisateurs[this.firstEditedRow];
			if (utilisateur.Admin)
			{
				return "Il n'est pas possible de supprimer l'administrateur.";
			}
			else
			{
				return FormattedText.Null;  // ok
			}
		}

		public override FormattedText GetRemoveModificationLineQuestion()
		{
			var utilisateur = this.compta.Utilisateurs[this.firstEditedRow];
			return string.Format ("Voulez-vous supprimer l'utilisateur \"{0}\" ?", utilisateur.Utilisateur);
		}

		public override void RemoveModificationLine()
		{
			if (this.isModification)
			{
				for (int row = this.firstEditedRow+this.countEditedRow-1; row >= this.firstEditedRow; row--)
                {
					var utilisateur = this.compta.Utilisateurs[row];
					this.DeleteUtilisateur (utilisateur);
					this.compta.Utilisateurs.RemoveAt (row);
                }

				if (this.firstEditedRow >= this.compta.Utilisateurs.Count)
				{
					this.firstEditedRow = this.compta.Utilisateurs.Count-1;
				}
			}
		}


		public override bool MoveEditionLine(int direction)
		{
			if (this.IsMoveEditionLineEnable (direction))
			{
				var t1 = this.compta.Utilisateurs[this.firstEditedRow];
				var t2 = this.compta.Utilisateurs[this.firstEditedRow+direction];

				this.compta.Utilisateurs[this.firstEditedRow] = t2;
				this.compta.Utilisateurs[this.firstEditedRow+direction] = t1;

				this.firstEditedRow += direction;

				return true;
			}
			else
			{
				return false;
			}
		}

		public override bool IsMoveEditionLineEnable(int direction)
		{
			if (this.firstEditedRow == -1)
			{
				return false;
			}

			return this.firstEditedRow+direction >= 0 && this.firstEditedRow+direction < this.Count;
		}


		private ComptaUtilisateurEntity CreateUtilisateur()
		{
			this.controller.MainWindowController.SetDirty ();

			ComptaUtilisateurEntity utilisateur;

			if (this.businessContext == null)
			{
				utilisateur = new ComptaUtilisateurEntity ();
			}
			else
			{
				utilisateur = this.businessContext.CreateEntity<ComptaUtilisateurEntity> ();
			}

			return utilisateur;
		}

		private void DeleteUtilisateur(ComptaUtilisateurEntity utilisateur)
		{
			this.controller.MainWindowController.SetDirty ();

			if (this.businessContext == null)
			{
				// rien à faire
			}
			else
			{
				this.businessContext.DeleteEntity (utilisateur);
			}
		}


		public static FormattedText GetPiècesGenerator(ComptaUtilisateurEntity utilisateur)
		{
			if (utilisateur.PiècesGenerator == null)
			{
				return FormattedText.Empty;
			}
			else
			{
				return utilisateur.PiècesGenerator.Nom;
			}
		}

		public static ComptaPiècesGeneratorEntity GetPiècesGenerator(ComptaEntity compta, FormattedText pièce)
		{
			if (pièce.IsNullOrEmpty)
			{
				return null;
			}
			else
			{
				return compta.PiècesGenerator.Where (x => x.Nom == pièce).FirstOrDefault ();
			}
		}
	}
}