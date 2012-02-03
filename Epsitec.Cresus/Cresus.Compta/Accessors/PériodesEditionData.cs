//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Accessors
{
	/// <summary>
	/// Données éditables pour une période comptable de la comptabilité.
	/// </summary>
	public class PériodesEditionData : AbstractEditionData
	{
		public PériodesEditionData(AbstractController controller)
			: base (controller)
		{
		}


		public override void Validate(ColumnType columnType)
		{
			//	Valide le contenu d'une colonne, en adaptant éventuellement son contenu.
			var text = this.GetText (columnType);
			var error = FormattedText.Null;

			switch (columnType)
            {
				case ColumnType.DateDébut:
					error = this.ValidateDate (ref text);
					break;

				case ColumnType.DateFin:
					error = this.ValidateDate (ref text);
					break;
			}

			this.SetText (columnType, text);
			this.errors[columnType] = error;
		}


		#region Validators
		private FormattedText ValidateDate(ref FormattedText text)
		{
			if (text.IsNullOrEmpty)
			{
				return "Il manque la date";
			}

			Date date;
			if (PériodesDataAccessor.ParseDate (text, out date))
			{
				return FormattedText.Empty;
			}
			else
			{
				return "La date est incorrecte";
			}
		}
		#endregion


		public override void EntityToData(AbstractEntity entity)
		{
			var période = entity as ComptaPériodeEntity;

			this.SetText (ColumnType.DateDébut, période.DateDébut.ToString ());
			this.SetText (ColumnType.DateFin,   période.DateFin.ToString ());
			this.SetText (ColumnType.Titre,     période.Description);
		}

		public override void DataToEntity(AbstractEntity entity)
		{
			var période = entity as ComptaPériodeEntity;

			Date date;

			if (PériodesDataAccessor.ParseDate (this.GetText (ColumnType.DateDébut), out date))
			{
				période.DateDébut = date;
			}

			if (PériodesDataAccessor.ParseDate (this.GetText (ColumnType.DateFin), out date))
			{
				période.DateFin = date;
			}

			période.Description = this.GetText (ColumnType.Titre);
		}
	}
}