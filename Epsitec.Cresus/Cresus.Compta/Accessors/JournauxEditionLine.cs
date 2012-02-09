//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Accessors
{
	/// <summary>
	/// Données éditables pour un journal de la comptabilité.
	/// </summary>
	public class JournauxEditionLine : AbstractEditionLine
	{
		public JournauxEditionLine(AbstractController controller)
			: base (controller)
		{
			this.datas.Add (ColumnType.Titre,   new EditionData (this.controller, this.ValidateTitle));
			this.datas.Add (ColumnType.Libellé, new EditionData (this.controller));
		}


		#region Validators
		private void ValidateTitle(EditionData data)
		{
			Validators.ValidateText (data, "Il manque le titre du journal");
		}
		#endregion


		public override void EntityToData(AbstractEntity entity)
		{
			var journal = entity as ComptaJournalEntity;

			this.SetText (ColumnType.Titre,   journal.Nom);
			this.SetText (ColumnType.Libellé, journal.Description);
		}

		public override void DataToEntity(AbstractEntity entity)
		{
			var journal = entity as ComptaJournalEntity;

			journal.Nom         = this.GetText (ColumnType.Titre);
			journal.Description = this.GetText (ColumnType.Libellé);
		}
	}
}