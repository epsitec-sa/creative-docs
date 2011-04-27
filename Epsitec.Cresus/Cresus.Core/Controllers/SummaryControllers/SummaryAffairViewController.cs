//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.SummaryControllers
{
	public class SummaryAffairViewController : SummaryViewController<AffairEntity>
	{
		protected override void CreateBricks(Bricks.BrickWall<AffairEntity> wall)
		{
			wall.AddBrick (x => x);

			wall.AddBrick (x => x.Documents)
				.Attribute (BrickMode.HideAddButton)
				.Attribute (BrickMode.HideRemoveButton)
				.Attribute (BrickMode.DefaultToSummarySubview)
				.Name ("DocMetadata")
				.Icon ("Data.Document")
				.Title ("Document lié")
				.TitleCompact ("Documents liés")
				.Template ()
				  .Title (x => x.GetCompactSummary ())
				  .Text (x => x.GetSummary ())
				  .TextCompact (x => x.GetCompactSummary ())
				.End ();

			wall.AddBrick (x => x.Comments)
				.Template ();
		}
		private void CreateUIAffair(TileDataItems data)
		{
			data.Add (
				new TileDataItem
				{
					Name				= "Affair",
					IconUri				= "Data.Affair",
					Title				= TextFormatter.FormatText ("Affaire"),
					CompactTitle		= TextFormatter.FormatText ("Affaire"),
					TextAccessor		= this.CreateAccessor (x => x.GetSummary ()),
					CompactTextAccessor = this.CreateAccessor (x => x.GetCompactSummary ()),
					EntityMarshaler		= this.CreateEntityMarshaler (),
				});
		}

		private void CreateUIDocuments(TileDataItems data)
		{
			data.Add (
				new TileDataItem
				{
					AutoGroup        = false,
					Name		     = "DocMetadata",
					IconUri		     = "Data.Document",
					Title		     = TextFormatter.FormatText ("Document lié"),
					CompactTitle     = TextFormatter.FormatText ("Documents liés"),
					Text		     = CollectionTemplate.DefaultEmptyText,
					HideAddButton    = true,
					DefaultMode      = ViewControllerMode.Summary,
				});

			var template = new CollectionTemplate<DocumentMetadataEntity> ("DocMetadata", this.BusinessContext);

			template.DefineTitle       (x => x.GetCompactSummary ());
			template.DefineText        (x => x.GetSummary ());
			template.DefineCompactText (x => x.GetCompactSummary ());

			data.Add (this.CreateCollectionAccessor (template, x => x.Documents));
		}
	}
}
