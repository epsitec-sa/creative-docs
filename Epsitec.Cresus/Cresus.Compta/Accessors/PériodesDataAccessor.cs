//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus;

using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta.Search.Data;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Accessors
{
	/// <summary>
	/// Gère l'accès aux périodes comptables de la comptabilité.
	/// </summary>
	public class PériodesDataAccessor : AbstractDataAccessor
	{
		public PériodesDataAccessor(AbstractController controller)
			: base (controller)
		{
			this.searchData = this.mainWindowController.GetSettingsSearchData ("Présentation.Périodes.Search");

			this.StartDefaultLine ();
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
				return this.comptaEntity.Périodes.Count;
			}
		}


		public override AbstractEntity GetEditionEntity(int row)
		{
			if (row < 0 || row >= this.comptaEntity.Périodes.Count)
			{
				return null;
			}
			else
			{
				return this.comptaEntity.Périodes[row];
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
				return this.comptaEntity.Périodes.IndexOf (entity as ComptaPériodeEntity);
			}
		}


		public override FormattedText GetText(int row, ColumnType column, bool all = false)
		{
			var périodes = comptaEntity.Périodes;

			if (row < 0 || row >= périodes.Count)
			{
				return FormattedText.Null;
			}

			var période = périodes[row];

			switch (column)
			{
				case ColumnType.Utilise:
					string icon = (période == this.mainWindowController.Période) ? "Button.RadioYes" : "Button.RadioNo";
					return string.Format (@"<img src=""{0}"" voff=""-5""/>", UIBuilder.GetResourceIconUri (icon));

				case ColumnType.DateDébut:
					return période.DateDébut.ToString () + " — " + période.DateFin.ToString ();

				case ColumnType.Titre:
					return période.Description;

				case ColumnType.Résumé:
					return période.GetJournalSummary (null);

				default:
					return FormattedText.Null;
			}
		}


		public override ColumnType ColumnForInsertionPoint
		{
			get
			{
				return ColumnType.DateDébut;
			}
		}

		public override int InsertionPointRow
		{
			get
			{
				var text = this.editionLine[0].GetText (ColumnType.DateDébut);

				Date date;
				PériodesDataAccessor.ParseDate (text, out date);

				if (!this.justCreated && this.firstEditedRow != -1 && this.countEditedRow != 0)
				{
					if (this.HasCorrectOrder (this.firstEditedRow, date))
					{
						return -1;
					}
				}

				return this.GetSortedRow (date);
			}
		}


		public override void InsertEditionLine(int index)
		{
			var newData = new PériodesEditionLine (this.controller);

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
			this.editionLine.Add (new PériodesEditionLine (this.controller));
			this.PrepareEditionLine (0);

			this.firstEditedRow = -1;
			this.countEditedRow = 1;

			this.isCreation = true;
			this.isModification = false;
			this.justCreated = false;
		}

		public override void StartModificationLine(int row)
		{
			this.editionLine.Clear ();

			this.firstEditedRow = row;
			this.countEditedRow = 0;

			if (row >= 0 && row < this.comptaEntity.Périodes.Count)
			{
				var data = new PériodesEditionLine (this.controller);
				var période = this.comptaEntity.Périodes[row];
				data.EntityToData (période);

				this.editionLine.Add (data);
				this.countEditedRow++;
			}

			this.initialCountEditedRow = this.countEditedRow;
			this.isCreation = false;
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
				var période = this.CreatePériode ();
				data.DataToEntity (période);

				int row = this.GetSortedRow (période.DateDébut);
				this.comptaEntity.Périodes.Insert (row, période);

				if (firstRow == -1)
				{
					firstRow = row;
				}
			}

			this.firstEditedRow = firstRow;
		}

		private void UpdateModificationData()
		{
			int row = this.firstEditedRow;

			var période = this.comptaEntity.Périodes[row];
			this.editionLine[0].DataToEntity (période);
		}


		public override FormattedText GetRemoveModificationLineError()
		{
			var période = this.comptaEntity.Périodes[this.firstEditedRow];
			if (période.Journal.Count != 0)
			{
				return "Cette période ne peut pas être supprimée,<br/>car elle contient des écritures.";
			}

			return FormattedText.Null;  // ok
		}

		public override FormattedText GetRemoveModificationLineQuestion()
		{
			var période = this.comptaEntity.Périodes[this.firstEditedRow];
			return string.Format ("Voulez-vous supprimer la période \"{0}\" ?", période.ShortTitle);
		}

		public override void RemoveModificationLine()
		{
			if (this.isModification)
			{
				for (int row = this.firstEditedRow+this.countEditedRow-1; row >= this.firstEditedRow; row--)
                {
					var période = this.comptaEntity.Périodes[row];
					this.DeletePériode (période);
					this.comptaEntity.Périodes.RemoveAt (row);
                }

				if (this.firstEditedRow >= this.comptaEntity.Périodes.Count)
				{
					this.firstEditedRow = this.comptaEntity.Périodes.Count-1;
				}
			}
		}


		private ComptaPériodeEntity CreatePériode()
		{
			this.controller.MainWindowController.SetDirty ();

			if (this.businessContext == null)
			{
				return new ComptaPériodeEntity ();
			}
			else
			{
				return this.businessContext.CreateEntity<ComptaPériodeEntity> ();
			}
		}

		private void DeletePériode(ComptaPériodeEntity période)
		{
			this.controller.MainWindowController.SetDirty ();

			if (this.businessContext == null)
			{
				// rien à faire
			}
			else
			{
				this.businessContext.DeleteEntity (période);
			}
		}


		private bool HasCorrectOrder(int row)
		{
			var périodes = this.comptaEntity.Périodes;
			var période = périodes[row];
			return this.HasCorrectOrder (row, période.DateDébut);
		}

		private bool HasCorrectOrder(int row, Date date)
		{
			var périodes = this.comptaEntity.Périodes;

			if (row > 0 && this.Compare (row-1, date) > 0)
			{
				return false;
			}

			if (row < périodes.Count-1 && this.Compare (row+1, date) < 0)
			{
				return false;
			}

			return true;
		}

		private int Compare(int row, Date date)
		{
			var périodes = this.comptaEntity.Périodes;
			var période = périodes[row];
			return période.DateDébut.CompareTo (date);
		}

		private int GetSortedRow(Date date)
		{
			var périodes = this.comptaEntity.Périodes;

			for (int row = périodes.Count-1; row >= 0; row--)
            {
				var période = périodes[row];

				if (période.DateDébut <= date)
				{
					return row+1;
				}
            }

			return 0;
		}


		public static bool ParseDate(FormattedText text, out Date date)
		{
			//	Transforme un texte en une date valide pour la comptabilité.
			System.DateTime d;

			if (System.DateTime.TryParse (text.ToSimpleText (), System.Threading.Thread.CurrentThread.CurrentCulture, System.Globalization.DateTimeStyles.AssumeLocal | System.Globalization.DateTimeStyles.AllowWhiteSpaces, out d))
			{
				date = new Date (d);
				return true;
			}
			else
			{
				date = Date.Today;
				return false;
			}
		}


		public static FormattedText GetPiècesGenerator(ComptaPériodeEntity période)
		{
			if (période.PiècesGenerator == null)
			{
				return FormattedText.Empty;
			}
			else
			{
				return période.PiècesGenerator.Nom;
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