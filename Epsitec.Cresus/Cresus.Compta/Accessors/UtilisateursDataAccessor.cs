//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Entities;
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

			this.StartCreationLine ();
		}


		public override void FilterUpdate()
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
				return this.comptaEntity.Utilisateurs.Count;
			}
		}


		public override AbstractEntity GetEditionEntity(int row)
		{
			if (row < 0 || row >= this.comptaEntity.Utilisateurs.Count)
			{
				return null;
			}
			else
			{
				return this.comptaEntity.Utilisateurs[row];
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
				return this.comptaEntity.Utilisateurs.IndexOf (entity as ComptaUtilisateurEntity);
			}
		}


		public override FormattedText GetText(int row, ColumnType column, bool all = false)
		{
			var utilisateurs = comptaEntity.Utilisateurs;

			if (row < 0 || row >= utilisateurs.Count)
			{
				return FormattedText.Null;
			}

			var utilisateur = utilisateurs[row];

			switch (column)
			{
				case ColumnType.Nom:
					return utilisateur.Nom;

				case ColumnType.MotDePasse:
					return utilisateur.MotDePasse;

				case ColumnType.Pièce:
					return UtilisateursDataAccessor.GetPièce (utilisateur);

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

			this.countEditedRow = this.editionLine.Count;
		}

		public override void StartCreationLine()
		{
			this.editionLine.Clear ();
			this.editionLine.Add (new UtilisateursEditionLine (this.controller));
			this.PrepareEditionLine (0);

			this.firstEditedRow = -1;
			this.countEditedRow = 1;

			this.isModification = false;
			this.justCreated = false;
		}

		public override void StartModificationLine(int row)
		{
			this.editionLine.Clear ();

			this.firstEditedRow = row;
			this.countEditedRow = 0;

			if (row >= 0 && row < this.comptaEntity.Utilisateurs.Count)
			{
				var data = new UtilisateursEditionLine (this.controller);
				var utilisateur = this.comptaEntity.Utilisateurs[row];
				data.EntityToData (utilisateur);

				this.editionLine.Add (data);
				this.countEditedRow++;
			}

			this.initialCountEditedRow = this.countEditedRow;
			this.isModification = true;
			this.justCreated = false;
		}

		public override void UpdateEditionLine()
		{
			if (this.isModification)
			{
				this.UpdateModificationData ();
				this.justCreated = false;
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

				this.comptaEntity.Utilisateurs.Add (utilisateur);

				if (firstRow == -1)
				{
					firstRow = this.comptaEntity.Utilisateurs.Count-1;
				}
			}

			this.firstEditedRow = firstRow;
		}

		private void UpdateModificationData()
		{
			int row = this.firstEditedRow;

			var utilisateur = this.comptaEntity.Utilisateurs[row];
			this.editionLine[0].DataToEntity (utilisateur);
		}


		public override FormattedText GetRemoveModificationLineQuestion()
		{
			var utilisateur = this.comptaEntity.Utilisateurs[this.firstEditedRow];
			return string.Format ("Voulez-vous supprimer l'utilisateur \"{0}\" ?", utilisateur.Nom);
		}

		public override void RemoveModificationLine()
		{
			if (this.isModification)
			{
				for (int row = this.firstEditedRow+this.countEditedRow-1; row >= this.firstEditedRow; row--)
                {
					var utilisateur = this.comptaEntity.Utilisateurs[row];
					this.DeleteUtilisateur (utilisateur);
					this.comptaEntity.Utilisateurs.RemoveAt (row);
                }

				if (this.firstEditedRow >= this.comptaEntity.Utilisateurs.Count)
				{
					this.firstEditedRow = this.comptaEntity.Utilisateurs.Count-1;
				}
			}
		}


		public override bool MoveEditionLine(int direction)
		{
			if (this.IsMoveEditionLineEnable (direction))
			{
				var t1 = this.comptaEntity.Utilisateurs[this.firstEditedRow];
				var t2 = this.comptaEntity.Utilisateurs[this.firstEditedRow+direction];

				this.comptaEntity.Utilisateurs[this.firstEditedRow] = t2;
				this.comptaEntity.Utilisateurs[this.firstEditedRow+direction] = t1;

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


		public static FormattedText GetPièce(ComptaUtilisateurEntity utilisateur)
		{
			if (utilisateur.GénérateurDePièces == null)
			{
				return FormattedText.Empty;
			}
			else
			{
				return utilisateur.GénérateurDePièces.Nom;
			}
		}

		public static ComptaPièceEntity GetPièce(ComptaEntity compta, FormattedText pièce)
		{
			if (pièce.IsNullOrEmpty)
			{
				return null;
			}
			else
			{
				return compta.Pièces.Where (x => x.Nom == pièce).FirstOrDefault ();
			}
		}
	}
}