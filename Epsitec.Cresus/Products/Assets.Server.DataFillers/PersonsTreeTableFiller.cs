//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.DataFillers
{
	public class PersonsTreeTableFiller : AbstractTreeTableFiller<SortableNode>
	{
		public PersonsTreeTableFiller(DataAccessor accessor, INodeGetter<SortableNode> nodeGetter)
			: base (accessor, nodeGetter)
		{
			this.userFields = this.accessor.UserFieldsAccessor.GetUserFields (BaseType.PersonsUserFields).ToArray ();
		}


		public override SortingInstructions		DefaultSorting
		{
			get
			{
				if (this.userFields.Any ())
				{
					var field = this.userFields.First ().Field;
					return new SortingInstructions (field, SortedType.Ascending, ObjectField.Unknown, SortedType.None);
				}
				else
				{
					return SortingInstructions.Empty;
				}
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

				foreach (var userField in this.userFields)
				{
					var type = AbstractTreeTableCell.GetColumnType (userField.Type);
					columns.Add (new TreeTableColumnDescription (userField.Field, type, userField.ColumnWidth, userField.Name));
				}

				return columns.ToArray ();
			}
		}

		public override TreeTableContentItem GetContent(int firstRow, int count, int selection)
		{
			var content = new TreeTableContentItem ();

			foreach (var userField in userFields)
			{
				content.Columns.Add (new TreeTableColumnItem ());
			}

			for (int i=0; i<count; i++)
			{
				if (firstRow+i >= this.nodeGetter.Count)
				{
					break;
				}

				var node = this.nodeGetter[firstRow+i];
				var guid = node.Guid;
				var obj  = this.accessor.GetObject (BaseType.Persons, guid);

				var cellState = (i == selection) ? CellState.Selected : CellState.None;

				int columnRank = 0;
				foreach (var userField in userFields)
				{
					bool inputValue = (columnRank == 0);
					var cell = AbstractTreeTableCell.CreateTreeTableCell (this.accessor, obj, this.Timestamp, userField, inputValue, cellState);

					content.Columns[columnRank++].AddRow (cell);
				}
			}

			return content;
		}


		private readonly UserField[] userFields;
	}
}
