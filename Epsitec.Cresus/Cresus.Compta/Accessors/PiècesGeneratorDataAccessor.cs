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
	public class PiècesGeneratorDataAccessor : AbstractDataAccessor
	{
		public PiècesGeneratorDataAccessor(AbstractController controller)
			: base (controller)
		{
			this.searchData = this.mainWindowController.GetSettingsSearchData ("Présentation.PiècesGenerator.Search");
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
				return this.compta.PiècesGenerator.Count;
			}
		}


		public override AbstractEntity GetEditionEntity(int row)
		{
			if (row < 0 || row >= this.compta.PiècesGenerator.Count)
			{
				return null;
			}
			else
			{
				return this.compta.PiècesGenerator[row];
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
				return this.compta.PiècesGenerator.IndexOf (entity as ComptaPiècesGeneratorEntity);
			}
		}


		public override FormattedText GetText(int row, ColumnType column, bool all = false)
		{
			var pièces = compta.PiècesGenerator;

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

				case ColumnType.Suffixe:
					return pièce.Suffixe;

				case ColumnType.Format:
					return pièce.Format;

				case ColumnType.Numéro:
					return Converters.IntToString (pièce.Numéro);

				case ColumnType.Incrément:
					return Converters.IntToString (pièce.Incrément);

				case ColumnType.Exemple:
					return this.mainWindowController.PiècesGenerator.GetSample (pièce);

				case ColumnType.Résumé:
					return this.mainWindowController.PiècesGenerator.GetSummary (pièce);

				default:
					return FormattedText.Null;
			}
		}


		public override void InsertEditionLine(int index)
		{
			var newData = new PiècesGeneratorEditionLine (this.controller);

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
			this.editionLine.Add (new PiècesGeneratorEditionLine (this.controller));
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
			this.editionLine[line].SetText (ColumnType.Format,    Converters.IntToString (1));
			this.editionLine[line].SetText (ColumnType.Numéro,    Converters.IntToString (1));
			this.editionLine[line].SetText (ColumnType.Incrément, Converters.IntToString (1));
		}

		public override void StartModificationLine(int row)
		{
			this.editionLine.Clear ();

			this.firstEditedRow = row;
			this.countEditedRow = 0;

			if (row >= 0 && row < this.compta.PiècesGenerator.Count)
			{
				var data = new PiècesGeneratorEditionLine (this.controller);
				var pièce = this.compta.PiècesGenerator[row];
				data.EntityToData (pièce);

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
				var pièce = this.CreatePiècesGenerator ();
				data.DataToEntity (pièce);

				this.compta.PiècesGenerator.Add (pièce);

				if (firstRow == -1)
				{
					firstRow = this.compta.PiècesGenerator.Count-1;
				}
			}

			this.firstEditedRow = firstRow;
		}

		private void UpdateModificationData()
		{
			int row = this.firstEditedRow;

			var pièce = this.compta.PiècesGenerator[row];
			this.editionLine[0].DataToEntity (pièce);
		}


		public override FormattedText GetRemoveModificationLineError()
		{
			var pièce = this.compta.PiècesGenerator[this.firstEditedRow];
			return this.mainWindowController.PiècesGenerator.GetRemoveError (pièce);
		}

		public override FormattedText GetRemoveModificationLineQuestion()
		{
			var pièce = this.compta.PiècesGenerator[this.firstEditedRow];
			return string.Format ("Voulez-vous supprimer le générateur de numéros de pièces \"{0}\" ?", pièce.Nom);
		}

		public override void RemoveModificationLine()
		{
			if (this.isModification)
			{
				for (int row = this.firstEditedRow+this.countEditedRow-1; row >= this.firstEditedRow; row--)
                {
					var pièce = this.compta.PiècesGenerator[row];
					this.DeletePiècesGenerator (pièce);
					this.compta.PiècesGenerator.RemoveAt (row);
                }

				if (this.firstEditedRow >= this.compta.PiècesGenerator.Count)
				{
					this.firstEditedRow = this.compta.PiècesGenerator.Count-1;
				}
			}
		}


		public override bool MoveEditionLine(int direction)
		{
			if (this.IsMoveEditionLineEnable (direction))
			{
				var t1 = this.compta.PiècesGenerator[this.firstEditedRow];
				var t2 = this.compta.PiècesGenerator[this.firstEditedRow+direction];

				this.compta.PiècesGenerator[this.firstEditedRow] = t2;
				this.compta.PiècesGenerator[this.firstEditedRow+direction] = t1;

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


		private ComptaPiècesGeneratorEntity CreatePiècesGenerator()
		{
			this.controller.MainWindowController.SetDirty ();

			ComptaPiècesGeneratorEntity generator;

			if (this.businessContext == null)
			{
				generator = new ComptaPiècesGeneratorEntity ();
			}
			else
			{
				generator = this.businessContext.CreateEntity<ComptaPiècesGeneratorEntity> ();
			}

			generator.Numéro    = 1;
			generator.Incrément = 1;

			return generator;
		}

		private void DeletePiècesGenerator(ComptaPiècesGeneratorEntity generator)
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