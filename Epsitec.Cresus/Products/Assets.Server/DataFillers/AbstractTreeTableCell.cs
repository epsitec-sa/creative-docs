//	Copyright � 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
					var gp = ObjectProperties.GetObjectPropertyGuid (obj, timestamp, userField.Field, synthetic: synthetic);
					var person = PersonsLogic.GetSummary (accessor, gp);
					return new TreeTableCellString (person, cellState);

				case FieldType.GuidAccount:
					var ga = ObjectProperties.GetObjectPropertyGuid (obj, timestamp, userField.Field, synthetic: synthetic);
					var account = AccountsLogic.GetSummary (accessor, ga);
					return new TreeTableCellString (account, cellState);

				default:
					throw new System.InvalidOperationException (string.Format ("Unknown FieldType {0}", userField.Type.ToString ()));
			}
		}

		public static TreeTableColumnType GetColumnType(FieldType type)
		{
			switch (type)
			{
				case FieldType.String:
				case FieldType.GuidPerson:
				case FieldType.GuidAccount:
					return TreeTableColumnType.String;

				case FieldType.Int:
					return TreeTableColumnType.Int;

				case FieldType.Decimal:
					return TreeTableColumnType.Decimal;

				case FieldType.ComputedAmount:
					return TreeTableColumnType.ComputedAmount;

				case FieldType.Date:
					return TreeTableColumnType.Date;

				default:
					throw new System.InvalidOperationException (string.Format ("Unknown FieldType {0}", type.ToString ()));
			}
		}
	}
}
