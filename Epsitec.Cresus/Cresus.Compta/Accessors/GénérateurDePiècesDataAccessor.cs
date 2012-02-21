//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Search.Data;
using Epsitec.Cresus.Compta.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Accessors
{
	/// <summary>
	/// Gère l'accès aux générateurs de numéros de pièces de la comptabilité.
	/// </summary>
	public class GénérateurDePiècesDataAccessor : AbstractDataAccessor
	{
		public GénérateurDePiècesDataAccessor(AbstractController controller)
			: base (controller)
		{
			this.searchData = this.mainWindowController.GetSettingsSearchData ("Présentation.Pièces.Search");

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
				return this.comptaEntity.GénérateurDePièces.Count;
			}
		}


		public override AbstractEntity GetEditionEntity(int row)
		{
			if (row < 0 || row >= this.comptaEntity.GénérateurDePièces.Count)
			{
				return null;
			}
			else
			{
				return this.comptaEntity.GénérateurDePièces[row];
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
				return this.comptaEntity.GénérateurDePièces.IndexOf (entity as ComptaGénérateurDePiècesEntity);
			}
		}


		public override FormattedText GetText(int row, ColumnType column, bool all = false)
		{
			var pièces = comptaEntity.GénérateurDePièces;

			if (row < 0 || row >= pièces.Count)
			{
				return FormattedText.Null;
			}

			var pièce = pièces[row];

			switch (column)
			{
				case ColumnType.Nom:
					return pièce.Nom;

				case ColumnType.Préfixe:
					return pièce.Préfixe;

				case ColumnType.Numéro:
					return Converters.IntToString (pièce.Numéro);

				case ColumnType.Suffixe:
					return pièce.Suffixe;

				case ColumnType.SépMilliers:
					return pièce.SépMilliers;

				case ColumnType.Digits:
					return (pièce.Digits == 0) ? FormattedText.Empty : Converters.IntToString (pièce.Digits);

				case ColumnType.Incrément:
					return Converters.IntToString (pièce.Incrément);

				default:
					return FormattedText.Null;
			}
		}


		public override void InsertEditionLine(int index)
		{
			var newData = new GénérateurDePiècesEditionLine (this.controller);

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
			this.editionLine.Add (new GénérateurDePiècesEditionLine (this.controller));
			this.PrepareEditionLine (0);

			this.firstEditedRow = -1;
			this.countEditedRow = 1;

			this.isModification = false;
			this.justCreated = false;
		}

		protected override void PrepareEditionLine(int line)
		{
			this.editionLine[line].SetText (ColumnType.Numéro,    Converters.IntToString (1));
			this.editionLine[line].SetText (ColumnType.Incrément, Converters.IntToString (1));
		}

		public override void StartModificationLine(int row)
		{
			this.editionLine.Clear ();

			this.firstEditedRow = row;
			this.countEditedRow = 0;

			if (row >= 0 && row < this.comptaEntity.GénérateurDePièces.Count)
			{
				var data = new GénérateurDePiècesEditionLine (this.controller);
				var pièce = this.comptaEntity.GénérateurDePièces[row];
				data.EntityToData (pièce);

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
				var pièce = this.CreateGénérateurDePièces ();
				data.DataToEntity (pièce);

				this.comptaEntity.GénérateurDePièces.Add (pièce);

				if (firstRow == -1)
				{
					firstRow = this.comptaEntity.GénérateurDePièces.Count-1;
				}
			}

			this.firstEditedRow = firstRow;
		}

		private void UpdateModificationData()
		{
			int row = this.firstEditedRow;

			var pièce = this.comptaEntity.GénérateurDePièces[row];
			this.editionLine[0].DataToEntity (pièce);
		}


		public override FormattedText GetRemoveModificationLineQuestion()
		{
			var pièce = this.comptaEntity.GénérateurDePièces[this.firstEditedRow];
			return string.Format ("Voulez-vous supprimer le générateur de numéros de pièces \"{0}\" ?", pièce.Nom);
		}

		public override void RemoveModificationLine()
		{
			if (this.isModification)
			{
				for (int row = this.firstEditedRow+this.countEditedRow-1; row >= this.firstEditedRow; row--)
                {
					var pièce = this.comptaEntity.GénérateurDePièces[row];
					this.DeleteGénérateurDePièces (pièce);
					this.comptaEntity.GénérateurDePièces.RemoveAt (row);
                }

				if (this.firstEditedRow >= this.comptaEntity.GénérateurDePièces.Count)
				{
					this.firstEditedRow = this.comptaEntity.GénérateurDePièces.Count-1;
				}
			}
		}


		public override bool MoveEditionLine(int direction)
		{
			if (this.IsMoveEditionLineEnable (direction))
			{
				var t1 = this.comptaEntity.GénérateurDePièces[this.firstEditedRow];
				var t2 = this.comptaEntity.GénérateurDePièces[this.firstEditedRow+direction];

				this.comptaEntity.GénérateurDePièces[this.firstEditedRow] = t2;
				this.comptaEntity.GénérateurDePièces[this.firstEditedRow+direction] = t1;

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


		private ComptaGénérateurDePiècesEntity CreateGénérateurDePièces()
		{
			this.controller.MainWindowController.SetDirty ();

			ComptaGénérateurDePiècesEntity generator;

			if (this.businessContext == null)
			{
				generator = new ComptaGénérateurDePiècesEntity ();
			}
			else
			{
				generator = this.businessContext.CreateEntity<ComptaGénérateurDePiècesEntity> ();
			}

			generator.Numéro    = 1;
			generator.Incrément = 1;

			return generator;
		}

		private void DeleteGénérateurDePièces(ComptaGénérateurDePiècesEntity generator)
		{
			this.controller.MainWindowController.SetDirty ();

			if (this.businessContext == null)
			{
				// rien à faire
			}
			else
			{
				this.businessContext.DeleteEntity (generator);
			}
		}
	}
}