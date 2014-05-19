//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.DataFillers
{
	/// <summary>
	/// Permet de remplir un TreeTable avec une liste d'erreurs ou de messages.
	/// </summary>
	public class ErrorsTreeTableFiller : AbstractTreeTableFiller<Error>
	{
		public ErrorsTreeTableFiller(DataAccessor accessor, INodeGetter<Error> nodeGetter)
			: base (accessor, nodeGetter)
		{
		}


		public override IEnumerable<ObjectField> Fields
		{
			get
			{
				yield return ObjectField.Name;
				yield return ObjectField.Description;
			}
		}

		public override TreeTableColumnDescription[] Columns
		{
			get
			{
				var columns = new List<TreeTableColumnDescription> ();

				columns.Add (new TreeTableColumnDescription (TreeTableColumnType.String, 150, "Objet"));
				columns.Add (new TreeTableColumnDescription (TreeTableColumnType.String, 300, "Message"));

				return columns.ToArray ();
			}
		}

		public override TreeTableContentItem GetContent(int firstRow, int count, int selection)
		{
			var content = new TreeTableContentItem ();

			for (int i=0; i<2; i++)
			{
				content.Columns.Add (new TreeTableColumnItem ());
			}

			for (int i=0; i<count; i++)
			{
				if (firstRow+i >= this.nodeGetter.Count)
				{
					break;
				}

				var error = this.nodeGetter[firstRow+i];
				bool isError = !error.IsMessage;

				var name    = ErrorDescription.GetErrorObject (this.accessor, error);
				var message = ErrorDescription.GetErrorDescription (error);

				var cellState = (i == selection) ? CellState.Selected : CellState.None;
				cellState |= (isError ? CellState.Error : CellState.None);

				var cell1 = new TreeTableCellString (name,    cellState);
				var cell2 = new TreeTableCellString (message, cellState);

				int columnRank = 0;

				content.Columns[columnRank++].AddRow (cell1);
				content.Columns[columnRank++].AddRow (cell2);
			}

			return content;
		}
	}
}
