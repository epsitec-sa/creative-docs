//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Compta.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta
{
	/// <summary>
	/// Ce contrôleur gère la ExtraitDeCompte de vérification de la comptabilité.
	/// </summary>
	public class ExtraitDeCompteController : AbstractController
	{
		public ExtraitDeCompteController(CoreApp app, BusinessContext businessContext, ComptabilitéEntity comptabilitéEntity, List<AbstractController> controllers)
			: base (app, businessContext, comptabilitéEntity, controllers)
		{
			this.dataAccessor = new ExtraitDeCompteDataAccessor (this.businessContext, this.comptabilitéEntity);
			this.InitializeColumnMapper ();
		}


		protected override void CreateOptions(FrameBox parent)
		{
			this.optionsController = new ExtraitDeCompteOptionsController (this.comptabilitéEntity, this.dataAccessor.AccessorOptions as ExtraitDeCompteOptions);
			this.optionsController.CreateUI (parent, this.OptinsChanged);
		}

		protected override void FinalizeOptions(FrameBox parent)
		{
			this.optionsController.FinalizeUI (parent);
		}

		protected override void OptinsChanged()
		{
			this.InitializeColumnMapper ();
			this.UpdateArray ();

			base.OptinsChanged ();
		}

		protected override void FinalUpdate()
		{
			base.FinalUpdate ();
		}

		public override void UpdateData()
		{
		}


		protected override FormattedText GetArrayText(int row, int column)
		{
			//	Retourne le texte contenu dans une cellule.
			var mapper = this.columnMappers[column];
			var text = this.dataAccessor.GetText (row, mapper.Column);
			var data = this.dataAccessor.GetReadOnlyData (row) as ExtraitDeCompteData;

			if (mapper.Column == ColumnType.Solde &&
				row == this.dataAccessor.Count-2)  // total sur l'avant-dernière ligne ?
			{
				text = text.ApplyBold ();
			}

			return data.Typo (text);
		}


		protected override IEnumerable<ColumnMapper> ColumnMappers
		{
			get
			{
				yield return new ColumnMapper (ColumnType.Date,      0.20, ContentAlignment.MiddleLeft, "Date");
				yield return new ColumnMapper (ColumnType.CP,        0.20, ContentAlignment.MiddleLeft,  "C/P");
				yield return new ColumnMapper (ColumnType.Pièce,     0.20, ContentAlignment.MiddleLeft,  "Pièce");
				yield return new ColumnMapper (ColumnType.Libellé,   0.60, ContentAlignment.MiddleLeft,  "Libellé");
				yield return new ColumnMapper (ColumnType.Débit,     0.20, ContentAlignment.MiddleRight, "Débit");
				yield return new ColumnMapper (ColumnType.Crédit,    0.20, ContentAlignment.MiddleRight, "Crédit");
				yield return new ColumnMapper (ColumnType.Solde,     0.20, ContentAlignment.MiddleRight, "Solde");

				if ((this.dataAccessor.AccessorOptions as ExtraitDeCompteOptions).HasGraphics)
				{
					yield return new ColumnMapper (ColumnType.SoldeGraphique, 0.20, ContentAlignment.MiddleRight, "");
				}
			}
		}
	}
}
