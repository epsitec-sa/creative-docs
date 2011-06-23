//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.SummaryControllers
{
	public class SummaryUnitOfMeasureGroupViewController : SummaryViewController<UnitOfMeasureGroupEntity>
	{
#if true
		protected override void CreateBricks(Bricks.BrickWall<UnitOfMeasureGroupEntity> wall)
		{
			wall.AddBrick (x => x);
		}
#else
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

		protected void CreateUIMain(TileDataItems data)
		{
			data.Add (
				new TileDataItem
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

		private void CreateUIUnits(TileDataItems data)
		{
			data.Add (
				new TileDataItem
				{
					AutoGroup    = true,
					Name		 = "UnitOfMeasures",
					IconUri		 = "Data.UnitOfMeasure",
					Title		 = TextFormatter.FormatText ("Unités du groupe"),
					CompactTitle = TextFormatter.FormatText ("Unités du groupe"),
					Text		 = CollectionTemplate.DefaultEmptyText,
				});

			var template = new CollectionTemplate<UnitOfMeasureEntity> ("UnitOfMeasures", this.BusinessContext);

			template.DefineText        (x => x.GetCompactSummary ());
			template.DefineCompactText (x => x.GetCompactSummary ());

			data.Add (this.CreateCollectionAccessor (template, x => x.Units));
		}
#endif
	}
}
