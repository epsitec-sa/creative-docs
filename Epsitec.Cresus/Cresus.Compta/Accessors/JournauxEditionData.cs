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
	/// Données éditables pour un journal de la comptabilité.
	/// </summary>
	public class JournauxEditionData : AbstractEditionData
	{
		public JournauxEditionData(AbstractController controller)
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
				case ColumnType.Titre:
					error = this.ValidateTitle (ref text);
					break;
			}

			this.SetText (columnType, text);
			this.errors[columnType] = error;
		}


		#region Validators
		private FormattedText ValidateTitle(ref FormattedText text)
		{
			if (text.IsNullOrEmpty)
			{
				return "Il manque le titre du journal";
			}

			return FormattedText.Empty;
		}
		#endregion


		public override void EntityToData(AbstractEntity entity)
		{
			var journal = entity as ComptaJournalEntity;

			this.SetText (ColumnType.Titre,   journal.Name);
			this.SetText (ColumnType.Libellé, journal.Description);
		}

		public override void DataToEntity(AbstractEntity entity)
		{
			var journal = entity as ComptaJournalEntity;

			journal.Name        = this.GetText (ColumnType.Titre);
			journal.Description = this.GetText (ColumnType.Libellé);
		}
	}
}