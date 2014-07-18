﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.Views;
using Epsitec.Cresus.Assets.Core.Helpers;
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
				return new SortingInstructions (ObjectField.Description, SortedType.Ascending, ObjectField.Unknown, SortedType.None);
			}
		}

		public override int						DefaultDockToLeftCount
		{
			get
			{
				return 1;
			}
		}

		public override TreeTableColumnDescription[] Columns
		{
			get
			{
				var columns = new List<TreeTableColumnDescription> ();

				columns.Add (new TreeTableColumnDescription (ObjectField.WarningGlyph,       TreeTableColumnType.Icon,    32, "Vue"));
				columns.Add (new TreeTableColumnDescription (ObjectField.WarningObject,      TreeTableColumnType.String, 200, "Description"));
				columns.Add (new TreeTableColumnDescription (ObjectField.WarningDate,        TreeTableColumnType.String,  80, "Date"));
				columns.Add (new TreeTableColumnDescription (ObjectField.WarningField,       TreeTableColumnType.String, 150, "Champ"));
				columns.Add (new TreeTableColumnDescription (ObjectField.WarningDescription, TreeTableColumnType.String, 300, "Message"));

				return columns.ToArray ();
			}
		}

		public override TreeTableContentItem GetContent(int firstRow, int count, int selection)
		{
			var content = new TreeTableContentItem ();

			for (int i=0; i<5; i++)
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
				var obj = this.accessor.GetObject (warning.BaseType, warning.ObjectGuid);
				var e = obj.GetEvent (warning.EventGuid);

				var cellState = (i == selection) ? CellState.Selected : CellState.None;

				string icon  = warning.ViewIcon;
				string text  = this.GetObjectSummary (warning.BaseType, obj);
				string date  = TypeConverters.DateToString (e.Timestamp.Date);
				string field = DataDescriptions.GetObjectFieldDescription (warning.Field);
				string desc  = warning.Description;

				var cell1 = new TreeTableCellString (icon,  cellState);
				var cell2 = new TreeTableCellString (text,  cellState);
				var cell3 = new TreeTableCellString (date,  cellState);
				var cell4 = new TreeTableCellString (field, cellState);
				var cell5 = new TreeTableCellString (desc,  cellState);

				int columnRank = 0;

				content.Columns[columnRank++].AddRow (cell1);
				content.Columns[columnRank++].AddRow (cell2);
				content.Columns[columnRank++].AddRow (cell3);
				content.Columns[columnRank++].AddRow (cell4);
				content.Columns[columnRank++].AddRow (cell5);
			}

			return content;
		}

		private string GetObjectSummary(BaseType baseType, DataObject obj)
		{
			switch (baseType.Kind)
			{
				case BaseTypeKind.Assets:
					return AssetsLogic.GetSummary (this.accessor, obj.Guid, null);

				case BaseTypeKind.Categories:
					return CategoriesLogic.GetSummary (this.accessor, obj.Guid);

				default:
					return null;
			}
		}
	}
}