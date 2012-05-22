//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus;

using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Search.Data;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Accessors
{
	/// <summary>
	/// Gère l'accès aux libellés usuels de la comptabilité.
	/// </summary>
	public class LibellésDataAccessor : AbstractDataAccessor
	{
		public LibellésDataAccessor(AbstractController controller)
			: base (controller)
		{
			this.viewSettingsList = this.mainWindowController.GetViewSettingsList (controller.ViewSettingsName);
			this.searchData       = this.mainWindowController.GetSettingsSearchData (controller.SearchName);
		}


		public override void UpdateFilter()
		{
			this.UpdateAfterOptionsChanged ();
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
				return this.compta.Libellés.Count;
			}
		}


		public override AbstractEntity GetEditionEntity(int row)
		{
			if (row < 0 || row >= this.compta.Libellés.Count)
			{
				return null;
			}
			else
			{
				return this.compta.Libellés[row];
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
				return this.compta.Libellés.IndexOf (entity as ComptaLibelléEntity);
			}
		}


		public override FormattedText GetText(int row, ColumnType column, bool all = false)
		{
			var libellés = compta.Libellés;

			if (row < 0 || row >= libellés.Count)
			{
				return FormattedText.Null;
			}

			var libellé = libellés[row];

			switch (column)
			{
				case ColumnType.Permanent:
					return libellé.Permanent ? LibellésDataAccessor.Permanent : LibellésDataAccessor.Volatile;

				case ColumnType.Libellé:
					return libellé.Libellé;

				default:
					return FormattedText.Null;
			}
		}


		public override void InsertEditionLine(int index)
		{
			var newData = new LibellésEditionLine (this.controller);

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
			this.editionLine.Add (new LibellésEditionLine (this.controller));
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
			this.editionLine[line].SetText (ColumnType.Permanent, "1");

			base.PrepareEditionLine (line);
		}

		public override void StartModificationLine(int row)
		{
			this.editionLine.Clear ();

			this.firstEditedRow = row;
			this.countEditedRow = 0;

			if (row >= 0 && row < this.compta.Libellés.Count)
			{
				var data = new LibellésEditionLine (this.controller);
				var libellé = this.compta.Libellés[row];
				data.EntityToData (libellé);

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
				var libellé = this.CreateLibellé ();
				data.DataToEntity (libellé);

				this.compta.Libellés.Insert (0, libellé);

				if (firstRow == -1)
				{
					firstRow = 0;
				}
			}

			this.firstEditedRow = firstRow;
		}

		private void UpdateModificationData()
		{
			int row = this.firstEditedRow;

			var libellé = this.compta.Libellés[row];
			this.editionLine[0].DataToEntity (libellé);
		}


		public override FormattedText GetRemoveModificationLineQuestion()
		{
			var libellé = this.compta.Libellés[this.firstEditedRow];
			return string.Format ("Voulez-vous supprimer le libellé \"{0}\" ?", libellé.Libellé);
		}

		public override void RemoveModificationLine()
		{
			if (this.isModification)
			{
				for (int row = this.firstEditedRow+this.countEditedRow-1; row >= this.firstEditedRow; row--)
                {
					var libellé = this.compta.Libellés[row];
					this.DeleteLibellé (libellé);
					this.compta.Libellés.RemoveAt (row);
                }

				if (this.firstEditedRow >= this.compta.Libellés.Count)
				{
					this.firstEditedRow = this.compta.Libellés.Count-1;
				}
			}
		}


		public override bool MoveEditionLine(int direction)
		{
			if (this.IsMoveEditionLineEnable (direction))
			{
				var t1 = this.compta.Libellés[this.firstEditedRow];
				var t2 = this.compta.Libellés[this.firstEditedRow+direction];

				this.compta.Libellés[this.firstEditedRow] = t2;
				this.compta.Libellés[this.firstEditedRow+direction] = t1;

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


		private ComptaLibelléEntity CreateLibellé()
		{
			this.controller.MainWindowController.SetDirty ();

			if (this.businessContext == null)
			{
				return new ComptaLibelléEntity ();
			}
			else
			{
				return this.businessContext.CreateEntity<ComptaLibelléEntity> ();
			}
		}

		private void DeleteLibellé(ComptaLibelléEntity libellé)
		{
			this.controller.MainWindowController.SetDirty ();

			if (this.businessContext == null)
			{
				// rien à faire
			}
			else
			{
				this.businessContext.DeleteEntity (libellé);
			}
		}


		private static readonly FormattedText	Permanent = Core.TextFormatter.FormatText ("Oui").ApplyBold ();
		private static readonly FormattedText	Volatile  = Core.TextFormatter.FormatText ("Non");
	}
}