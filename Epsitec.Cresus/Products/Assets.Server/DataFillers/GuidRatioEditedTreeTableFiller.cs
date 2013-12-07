//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.Helpers;
using Epsitec.Cresus.Assets.Server.NodesGetter;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.DataFillers
{
	/// <summary>
	/// Permet de remplir un TreeTable avec la liste des GuidRatio en édition.
	/// </summary>
	public class GuidRatioEditedTreeTableFiller : AbstractTreeTableFiller<ObjectField>
	{
		public GuidRatioEditedTreeTableFiller(DataAccessor accessor, AbstractNodesGetter<ObjectField> nodesGetter)
			: base (accessor, nodesGetter)
		{
		}


		public override IEnumerable<ObjectField> Fields
		{
			get
			{
				yield return ObjectField.EventGlyph;
				yield return ObjectField.Nom;
				yield return ObjectField.Description;
			}
		}

		public override TreeTableColumnDescription[] Columns
		{
			get
			{
				var list = new List<TreeTableColumnDescription> ();

				list.Add (new TreeTableColumnDescription (TreeTableColumnType.Glyph,   20, ""));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String, 400, "Groupe"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String,  60, "Ratio"));

				return list.ToArray ();
			}
		}

		public override TreeTableContentItem GetContent(int firstRow, int count, int selection)
		{
			var c0 = new TreeTableColumnItem<TreeTableCellGlyph> ();
			var c1 = new TreeTableColumnItem<TreeTableCellString> ();
			var c2 = new TreeTableColumnItem<TreeTableCellString> ();

			for (int i=0; i<count; i++)
			{
				if (firstRow+i >= this.nodesGetter.Count)
				{
					break;
				}

				var field = this.nodesGetter[firstRow+i];
				var node  = this.accessor.EditionAccessor.GetFieldGuidRatio (field);
				var state = this.accessor.EditionAccessor.GetEditionPropertyState (field);
				var isSel = (i == selection);

				var glyph = isSel ? TimelineGlyph.FilledCircle : TimelineGlyph.Empty;
				var group = GroupsLogic.GetFullName (this.accessor, node.Guid);
				var ratio = TypeConverters.RateToString (node.Ratio);

				if (string.IsNullOrEmpty (group))
				{
					group = "<i>Nouveau</i>";
				}

				bool isEvent = state == PropertyState.Single;  // true -> fond bleu

				var s0 = new TreeTableCellGlyph  (true, glyph, isEvent: isEvent);
				var s1 = new TreeTableCellString (true, group, isEvent: isEvent, isSelected: isSel);
				var s2 = new TreeTableCellString (true, ratio, isEvent: isEvent, isSelected: isSel);

				c0.AddRow (s0);
				c1.AddRow (s1);
				c2.AddRow (s2);
			}

			var content = new TreeTableContentItem ();

			content.Columns.Add (c0);
			content.Columns.Add (c1);
			content.Columns.Add (c2);

			return content;
		}
	}
}
