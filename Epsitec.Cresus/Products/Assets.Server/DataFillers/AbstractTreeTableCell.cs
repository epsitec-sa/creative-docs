//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.DataFillers
{
	public abstract class AbstractTreeTableCell
	{
		public AbstractTreeTableCell(CellState cellState)
		{
			this.CellState = cellState;
		}

		public readonly CellState				CellState;


		public override string ToString()
		{
			var buffer = new System.Text.StringBuilder ();

			if ((this.CellState & CellState.Selected) != 0)
			{
				buffer.Append (" Selected");
			}

			if ((this.CellState & CellState.Event) != 0)
			{
				buffer.Append (" Event");
			}

			if ((this.CellState & CellState.Error) != 0)
			{
				buffer.Append (" Error");
			}

			if ((this.CellState & CellState.Unavailable) != 0)
			{
				buffer.Append (" Unavailable");
			}

			return buffer.ToString ();
		}


		public static void AddColumnDescription(List<TreeTableColumnDescription> columns, IEnumerable<UserField> userFields, bool treeFirst = false)
		{
			//	Ajoute les descriptifs de colonnes pour une liste de rubriques utilisateur.
			int columnRank = 0;

			foreach (var userField in userFields)
			{
				TreeTableColumnType type;

				if (treeFirst && columnRank == 0)
				{
					type = TreeTableColumnType.Tree;
				}
				else
				{
					type = AbstractTreeTableCell.GetColumnType (userField.Type);
				}

				columns.Add (new TreeTableColumnDescription (type, userField.ColumnWidth, userField.Name));
				columnRank++;
			}
		}

		public static AbstractTreeTableCell CreateTreeTableCell(DataAccessor accessor, DataObject obj, Timestamp? timestamp,
			UserField userField, bool inputValue, CellState cellState, bool synthetic = true)
		{
			//	Retourne le contenu d'une cellule pour une rubrique utilisateur.
			switch (userField.Type)
			{
				case FieldType.String:
					var text = ObjectProperties.GetObjectPropertyString (obj, timestamp, userField.Field, synthetic: synthetic, inputValue: inputValue);
					return new TreeTableCellString (text, cellState);

				case FieldType.Int:
					var i = ObjectProperties.GetObjectPropertyInt (obj, timestamp, userField.Field, synthetic: synthetic);
					return new TreeTableCellInt (i, cellState);

				case FieldType.Decimal:
					var d = ObjectProperties.GetObjectPropertyDecimal (obj, timestamp, userField.Field, synthetic: synthetic);
					return new TreeTableCellDecimal (d, cellState);

				case FieldType.ComputedAmount:
					var ca = ObjectProperties.GetObjectPropertyComputedAmount (obj, timestamp, userField.Field, synthetic: synthetic);
					return new TreeTableCellComputedAmount (ca, cellState);

				case FieldType.Date:
					var date = ObjectProperties.GetObjectPropertyDate (obj, timestamp, userField.Field, synthetic: synthetic);
					return new TreeTableCellDate (date, cellState);

				case FieldType.GuidPerson:
					var guid = ObjectProperties.GetObjectPropertyGuid (obj, timestamp, userField.Field, synthetic: synthetic);
					var person = PersonsLogic.GetSummary (accessor, guid);
					return new TreeTableCellString (person, cellState);

				default:
					throw new System.InvalidOperationException (string.Format ("Unknown FieldType {0}", userField.Type.ToString ()));
			}
		}

		public static TreeTableColumnType GetColumnType(FieldType type)
		{
			switch (type)
			{
				case FieldType.String:
					return TreeTableColumnType.String;

				case FieldType.Int:
					return TreeTableColumnType.Int;

				case FieldType.Decimal:
					return TreeTableColumnType.Decimal;

				case FieldType.ComputedAmount:
					return TreeTableColumnType.ComputedAmount;

				case FieldType.Date:
					return TreeTableColumnType.Date;

				case FieldType.GuidPerson:
					return TreeTableColumnType.String;

				default:
					throw new System.InvalidOperationException (string.Format ("Unknown FieldType {0}", type.ToString ()));
			}
		}
	}
}
