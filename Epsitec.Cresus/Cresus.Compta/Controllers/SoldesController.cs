//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta.Options.Data;
using Epsitec.Cresus.Compta.Options.Controllers;
using Epsitec.Cresus.Compta.Permanents;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Controllers
{
	/// <summary>
	/// Ce contrôleur gère les soldes de la comptabilité.
	/// </summary>
	public class SoldesController : AbstractController
	{
		public SoldesController(ComptaApplication app, BusinessContext businessContext, MainWindowController mainWindowController)
			: base (app, businessContext, mainWindowController)
		{
			this.dataAccessor = new SoldesDataAccessor (this);
		}


		protected override int ArrayLineHeight
		{
			get
			{
				var options = this.dataAccessor.Options as SoldesOptions;

				if (options.HasSideBySideGraph)
				{
					var accessor = this.dataAccessor as SoldesDataAccessor;
					return System.Math.Max (2+accessor.ColumnCount*6, 14);
				}
				else
				{
					return 14;
				}
			}
		}


		protected override void CreateTopOptions(FrameBox parent)
		{
			this.ShowOptionsPanel = true;

			this.optionsController = new SoldesOptionsController (this);
			this.optionsController.CreateUI (parent, this.OptionsChanged);
			this.optionsController.ShowPanel = this.ShowOptionsPanel;

			this.UpdateColumnMappers ();
		}

		protected override void OptionsChanged()
		{
			this.dataAccessor.UpdateAfterOptionsChanged ();
			this.ClearHilite ();
			this.UpdateColumnMappers ();
			this.UpdateArray ();

			this.UpdateArrayContent ();
			this.FilterUpdateTopToolbar ();
			this.UpdateViewSettings ();
		}

		public override ControllerType ControllerType
		{
			get
			{
				return Controllers.ControllerType.Soldes;
			}
		}


		protected override FormattedText GetArrayText(int row, ColumnType columnType)
		{
			//	Retourne le texte contenu dans une cellule.
			var text = this.dataAccessor.GetText (row, columnType);
			var data = this.dataAccessor.GetReadOnlyData (row) as SoldesData;

			var options = this.dataAccessor.Options as SoldesOptions;

			if (columnType >= ColumnType.Solde1 &&
				columnType <= ColumnType.Solde12)
			{
				var value = Converters.ParseMontant (text);
				if (!data.NeverFiltered && value.GetValueOrDefault () == 0)
				{
					text = FormattedText.Empty;
				}
			}

			return data.Typo (text);
		}


		protected override IEnumerable<ColumnMapper> InitialColumnMappers
		{
			get
			{
				yield return new ColumnMapper (ColumnType.Titre,   0.20, ContentAlignment.MiddleLeft, "t");
				yield return new ColumnMapper (ColumnType.Solde1,  0.20, ContentAlignment.MiddleRight, "");
				yield return new ColumnMapper (ColumnType.Solde2,  0.20, ContentAlignment.MiddleRight, "");
				yield return new ColumnMapper (ColumnType.Solde3,  0.20, ContentAlignment.MiddleRight, "");
				yield return new ColumnMapper (ColumnType.Solde4,  0.20, ContentAlignment.MiddleRight, "");
				yield return new ColumnMapper (ColumnType.Solde5,  0.20, ContentAlignment.MiddleRight, "");
				yield return new ColumnMapper (ColumnType.Solde6,  0.20, ContentAlignment.MiddleRight, "");
				yield return new ColumnMapper (ColumnType.Solde7,  0.20, ContentAlignment.MiddleRight, "");
				yield return new ColumnMapper (ColumnType.Solde8,  0.20, ContentAlignment.MiddleRight, "");
				yield return new ColumnMapper (ColumnType.Solde9,  0.20, ContentAlignment.MiddleRight, "");
				yield return new ColumnMapper (ColumnType.Solde10, 0.20, ContentAlignment.MiddleRight, "");
				yield return new ColumnMapper (ColumnType.Solde11, 0.20, ContentAlignment.MiddleRight, "");
				yield return new ColumnMapper (ColumnType.Solde12, 0.20, ContentAlignment.MiddleRight, "");
			}
		}

		protected override void UpdateColumnMappers()
		{
			var options = this.dataAccessor.Options as SoldesOptions;

			//	Cache toutes les colonnes des soldes.
			for (int i = 0; i < 12; i++)
			{
				if (i < options.SoldesColumns.Count)
				{
					this.SetColumnParameters (ColumnType.Solde1+i, true, options.SoldesColumns[i].Description);
				}
				else
				{
					this.SetColumnParameters (ColumnType.Solde1+i, false, FormattedText.Null);
				}
			}

		}
	}
}
