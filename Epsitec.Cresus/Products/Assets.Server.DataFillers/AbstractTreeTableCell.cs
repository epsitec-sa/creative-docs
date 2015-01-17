//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.DataFillers
{
	public abstract class AbstractTreeTableCell
	{
		public AbstractTreeTableCell(CellState cellState, string tooltip = null)
		{
			this.CellState = cellState;
			this.Tooltip   = tooltip;
		}

		public readonly CellState				CellState;
		public readonly string					Tooltip;


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

			if ((this.CellState & CellState.Dimmed) != 0)
			{
				buffer.Append (" Dimmed");
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
				case FieldType.Account:
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

				case FieldType.AmortizedAmount:
					var aa = ObjectProperties.GetObjectPropertyAmortizedAmount (obj, timestamp, userField.Field, synthetic: synthetic);
					return new TreeTableCellAmortizedAmount (aa, cellState);

				case FieldType.Date:
					var date = ObjectProperties.GetObjectPropertyDate (obj, timestamp, userField.Field, synthetic: synthetic);
					return new TreeTableCellDate (date, cellState);

				case FieldType.GuidPerson:
					var gp = ObjectProperties.GetObjectPropertyGuid (obj, timestamp, userField.Field, synthetic: synthetic);
					var person = PersonsLogic.GetSummary (accessor, gp);
					return new TreeTableCellString (person, cellState);

				case FieldType.GuidMethod:
					var ge = ObjectProperties.GetObjectPropertyGuid (obj, timestamp, userField.Field, synthetic: synthetic);
					var exp = MethodsLogic.GetSummary (accessor, ge);
					return new TreeTableCellString (exp, cellState);

				default:
					throw new System.InvalidOperationException (string.Format ("Unknown FieldType {0}", userField.Type.ToString ()));
			}
		}

		public static TreeTableColumnType GetColumnType(FieldType type)
		{
			switch (type)
			{
				case FieldType.String:
				case FieldType.Account:
				case FieldType.GuidPerson:
				case FieldType.GuidMethod:
					return TreeTableColumnType.String;

				case FieldType.Int:
					return TreeTableColumnType.Int;

				case FieldType.Decimal:
					return TreeTableColumnType.Decimal;

				case FieldType.ComputedAmount:
					return TreeTableColumnType.ComputedAmount;

				case FieldType.AmortizedAmount:
					return TreeTableColumnType.AmortizedAmount;

				case FieldType.Date:
					return TreeTableColumnType.Date;

				default:
					throw new System.InvalidOperationException (string.Format ("Unknown FieldType {0}", type.ToString ()));
			}
		}
	}
}
