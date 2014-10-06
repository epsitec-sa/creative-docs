//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.Views;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.DataFillers;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.DataFillers
{
	/// <summary>
	/// Permet de remplir un TreeTable avec une liste d'erreurs ou de messages.
	/// </summary>
	public class WarningsTreeTableFiller : AbstractTreeTableFiller<Warning>
	{
		public WarningsTreeTableFiller(DataAccessor accessor, INodeGetter<Warning> nodeGetter)
			: base (accessor, nodeGetter)
		{
		}


		public override SortingInstructions		DefaultSorting
		{
			get
			{
				return new SortingInstructions (ObjectField.WarningViewGlyph, SortedType.Descending, ObjectField.WarningObject, SortedType.Ascending);
			}
		}

		public override int						DefaultDockToLeftCount
		{
			get
			{
				return 2;
			}
		}

		public override TreeTableColumnDescription[] Columns
		{
			get
			{
				var columns = new List<TreeTableColumnDescription> ();

				columns.Add (new TreeTableColumnDescription (ObjectField.WarningViewGlyph,   TreeTableColumnType.Icon,    50, Res.Strings.DataFillers.WarningsTreeTable.Glyph.Title.ToString (), Res.Strings.DataFillers.WarningsTreeTable.Glyph.Tooltip.ToString ()));
				columns.Add (new TreeTableColumnDescription (ObjectField.WarningObject,      TreeTableColumnType.String, 200, Res.Strings.DataFillers.WarningsTreeTable.Object.Title.ToString ()));
				columns.Add (new TreeTableColumnDescription (ObjectField.WarningDate,        TreeTableColumnType.Date,    70, Res.Strings.DataFillers.WarningsTreeTable.Date.Title.ToString (), Res.Strings.DataFillers.WarningsTreeTable.Date.Tooltip.ToString ()));
				columns.Add (new TreeTableColumnDescription (ObjectField.WarningEventGlyph,  TreeTableColumnType.Glyph,   20, "",                                                               Res.Strings.DataFillers.WarningsTreeTable.EventGlyph.Tooltip.ToString ()));
				columns.Add (new TreeTableColumnDescription (ObjectField.WarningField,       TreeTableColumnType.String, 150, Res.Strings.DataFillers.WarningsTreeTable.Field.Title.ToString ()));
				columns.Add (new TreeTableColumnDescription (ObjectField.WarningDescription, TreeTableColumnType.String, 400, Res.Strings.DataFillers.WarningsTreeTable.Description.Title.ToString ()));

				return columns.ToArray ();
			}
		}

		public override TreeTableContentItem GetContent(int firstRow, int count, int selection)
		{
			var content = new TreeTableContentItem ();

			for (int i=0; i<6; i++)
			{
				content.Columns.Add (new TreeTableColumnItem ());
			}

			for (int i=0; i<count; i++)
			{
				if (firstRow+i >= this.nodeGetter.Count)
				{
					break;
				}

				var warning = this.nodeGetter[firstRow+i];

				Timestamp?       timestamp = null;
				System.DateTime? date      = null;
				EventType        eventType = EventType.Unknown;

				var obj = this.accessor.GetObject (warning.BaseType, warning.ObjectGuid);

				if (obj != null)
				{
					var e = obj.GetEvent (warning.EventGuid);

					if (e != null)
					{
						timestamp = e.Timestamp;
						date = timestamp.Value.Date;
						eventType = e.Type;
					}
				}

				var cellState = (i == selection) ? CellState.Selected : CellState.None;

				string icon  = StaticDescriptions.GetViewTypeIcon (StaticDescriptions.GetViewTypeKind (warning.BaseType.Kind));
				string text  = UniversalLogic.GetObjectSummary (this.accessor, warning.BaseType, obj, timestamp);
				var    glyph = TimelineData.TypeToGlyph (eventType);
				string field = UserFieldsLogic.GetFieldName (this.accessor, WarningsTreeTableFiller.GetUserFieldsBaseType (warning.BaseType), warning.Field);
				string desc  = warning.Description;

				var cell1 = new TreeTableCellString (icon,  cellState);
				var cell2 = new TreeTableCellString (text,  cellState);
				var cell3 = new TreeTableCellDate   (date,  cellState);
				var cell4 = new TreeTableCellGlyph  (glyph, cellState);
				var cell5 = new TreeTableCellString (field, cellState);
				var cell6 = new TreeTableCellString (desc,  cellState);

				int columnRank = 0;

				content.Columns[columnRank++].AddRow (cell1);
				content.Columns[columnRank++].AddRow (cell2);
				content.Columns[columnRank++].AddRow (cell3);
				content.Columns[columnRank++].AddRow (cell4);
				content.Columns[columnRank++].AddRow (cell5);
				content.Columns[columnRank++].AddRow (cell6);
			}

			return content;
		}

		private static BaseType GetUserFieldsBaseType(BaseType baseType)
		{
			switch (baseType.Kind)
			{
				case BaseTypeKind.Assets:
				case BaseTypeKind.AssetsUserFields:
					return BaseType.AssetsUserFields;

				case BaseTypeKind.Persons:
				case BaseTypeKind.PersonsUserFields:
					return BaseType.PersonsUserFields;

				default:
					return baseType;
			}
		}
	}
}
