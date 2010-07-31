//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.BusinessLogic;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.SummaryControllers
{
	public class SummaryUnitOfMeasureViewController : SummaryViewController<Entities.UnitOfMeasureEntity>
	{
		public SummaryUnitOfMeasureViewController(string name, Entities.UnitOfMeasureEntity entity)
			: base (name, entity)
		{
		}


		protected override void CreateUI(TileContainer container)
		{
			this.TileContainerController = new TileContainerController (this, container);
			var data = this.TileContainerController.DataItems;

			data.Add (
				new SummaryData
				{
					Name				= "UnitOfMeasure",
					IconUri				= "Data.UnitOfMeasure",
					Title				= UIBuilder.FormatText ("Unité de mesure"),
					CompactTitle		= UIBuilder.FormatText ("Unité"),
					TextAccessor		= Accessor.Create (this.EntityGetter, x => UIBuilder.FormatText ("Nom: ", x.Name, "\n", "Code: ", x.Code, "\n", "Catégorie: ", GetCategory (x), "\n", "Valeurs: ", GetFactors (x))),
					CompactTextAccessor = Accessor.Create (this.EntityGetter, x => UIBuilder.FormatText (x.Name, "(", x.Code, ")")),
					EntityMarshaler		= this.EntityMarshaler,
				});

			this.TileContainerController.GenerateTiles ();
		}


		private static string GetCategory(Entities.UnitOfMeasureEntity unit)
		{
			foreach (var item in Enumerations.GetGetAllPossibleItemsUnitOfMeasureCategory ())
			{
				if (item.Key == unit.Category)
				{
					return UIBuilder.FormatText (item.Values).ToSimpleText ();
				}
			}

			return null;
		}

		private static string GetFactors(Entities.UnitOfMeasureEntity unit)
		{
			return string.Format ("÷{0}, ×{1}, ±{2}", unit.DivideRatio.ToString (), unit.MultiplyRatio.ToString (), unit.SmallestIncrement.ToString ());
		}
	}
}
