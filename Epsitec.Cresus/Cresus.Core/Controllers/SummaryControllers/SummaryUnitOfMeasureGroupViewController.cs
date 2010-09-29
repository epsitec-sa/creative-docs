//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Business;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.SummaryControllers
{
	public class SummaryUnitOfMeasureGroupViewController : SummaryViewController<Entities.UnitOfMeasureGroupEntity>
	{
		public SummaryUnitOfMeasureGroupViewController(string name, Entities.UnitOfMeasureGroupEntity entity)
			: base (name, entity)
		{
		}


		protected override void CreateUI()
		{
			using (var builder = new UIBuilder (this))
			{
				using (var data = TileContainerController.Setup (builder))
				{
					this.CreateUIMain  (data);
					this.CreateUIUnits (data);
				}
			}
		}

		protected void CreateUIMain(SummaryDataItems data)
		{
			data.Add (
				new SummaryData
				{
					Name				= "UnitOfMeasureGroup",
					IconUri				= "Data.UnitOfMeasureGroup",
					Title				= TextFormatter.FormatText ("Groupe d'unités de mesure"),
					CompactTitle		= TextFormatter.FormatText ("Groupe d'unités"),
					TextAccessor		= this.CreateAccessor (x => x.GetSummary ()),
					CompactTextAccessor = this.CreateAccessor (x => x.GetCompactSummary ()),
					EntityMarshaler		= this.CreateEntityMarshaler (),
				});
		}

		private void CreateUIUnits(SummaryDataItems data)
		{
			data.Add (
				new SummaryData
				{
					AutoGroup    = true,
					Name		 = "UnitOfMeasures",
					IconUri		 = "Data.UnitOfMeasure",
					Title		 = TextFormatter.FormatText ("Unités du groupe"),
					CompactTitle = TextFormatter.FormatText ("Unités du groupe"),
					Text		 = CollectionTemplate.DefaultEmptyText,
				});

			var template = new CollectionTemplate<UnitOfMeasureEntity> ("UnitOfMeasures", data.Controller, this.DataContext);

			template.DefineText        (x => x.GetCompactSummary ());
			template.DefineCompactText (x => x.GetCompactSummary ());

			data.Add (this.CreateCollectionAccessor (template, x => x.Units));
		}
	}
}
