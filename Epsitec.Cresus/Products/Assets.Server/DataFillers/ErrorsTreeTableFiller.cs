//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
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
		public ErrorsTreeTableFiller(DataAccessor accessor, AbstractNodesGetter<Error> nodesGetter)
			: base (accessor, nodesGetter)
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
				var list = new List<TreeTableColumnDescription> ();

				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String, 150, "Objet"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String, 300, "Message"));

				return list.ToArray ();
			}
		}

		public override TreeTableContentItem GetContent(int firstRow, int count, int selection)
		{
			var c1 = new TreeTableColumnItem<TreeTableCellString> ();
			var c2 = new TreeTableColumnItem<TreeTableCellString> ();

			for (int i=0; i<count; i++)
			{
				if (firstRow+i >= this.nodesGetter.Count)
				{
					break;
				}

				var error = this.nodesGetter[firstRow+i];
				bool isError = !error.IsMessage;

				var nom     = ErrorDescription.GetErrorObject (this.accessor, error);
				var message = ErrorDescription.GetErrorDescription (error);

				var s1 = new TreeTableCellString (true, nom,     isSelected: (i == selection), isError: isError);
				var s2 = new TreeTableCellString (true, message, isSelected: (i == selection), isError: isError);

				c1.AddRow (s1);
				c2.AddRow (s2);
			}

			var content = new TreeTableContentItem ();

			content.Columns.Add (c1);
			content.Columns.Add (c2);

			return content;
		}
	}
}
