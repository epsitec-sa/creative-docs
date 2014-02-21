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
		public AbstractTreeTableCell(bool isValid, bool isSelected = false, bool isEvent = false, bool isError = false, bool isUnavailable = false)
		{
			this.IsValid       = isValid;
			this.IsSelected    = isSelected;
			this.IsEvent       = isEvent;
			this.IsError       = isError;
			this.IsUnavailable = isUnavailable;
		}

		public readonly bool					IsValid;
		public readonly bool					IsSelected;
		public readonly bool					IsEvent;
		public readonly bool					IsError;
		public readonly bool					IsUnavailable;


		public override string ToString()
		{
			var buffer = new System.Text.StringBuilder ();

			if (!this.IsValid)
			{
				buffer.Append (" Invalid");
			}

			if (this.IsSelected)
			{
				buffer.Append (" Selected");
			}

			if (this.IsEvent)
			{
				buffer.Append (" Event");
			}

			if (this.IsError)
			{
				buffer.Append (" Error");
			}

			if (this.IsUnavailable)
			{
				buffer.Append (" Unavailable");
			}

			return buffer.ToString ();
		}


		public static void AddColumnDescription(List<TreeTableColumnDescription> columns, IEnumerable<UserField> userFields)
		{
			foreach (var userField in userFields)
			{
				var type = AbstractTreeTableCell.GetColumnType (userField.Type);
				columns.Add (new TreeTableColumnDescription (type, userField.ColumnWidth, userField.Name));
			}
		}

		public static AbstractTreeTableCell CreateTreeTableCell(DataObject obj, Timestamp? timestamp,
			UserField userField, bool inputValue,
			bool isValid = true, bool isSelected = false, bool isEvent = false,
			bool isError = false, bool isUnavailable = false)
		{
			switch (userField.Type)
			{
				case FieldType.String:
					var text = AssetCalculator.GetObjectPropertyString (obj, timestamp, userField.Field, inputValue);
					return new TreeTableCellString (isValid, text, isSelected, isEvent, isError, isUnavailable);

				case FieldType.Int:
					var i = AssetCalculator.GetObjectPropertyInt (obj, timestamp, userField.Field, inputValue);
					return new TreeTableCellInt (isValid, i, isSelected, isEvent, isError, isUnavailable);

				case FieldType.Decimal:
					var d = AssetCalculator.GetObjectPropertyDecimal (obj, timestamp, userField.Field, inputValue);
					return new TreeTableCellDecimal (isValid, d, isSelected, isEvent, isError, isUnavailable);

				case FieldType.ComputedAmount:
					var ca = AssetCalculator.GetObjectPropertyComputedAmount (obj, timestamp, userField.Field, inputValue);
					return new TreeTableCellComputedAmount (isValid, ca, isSelected, isEvent, isError, isUnavailable);

				case FieldType.Date:
					var date = AssetCalculator.GetObjectPropertyDate (obj, timestamp, userField.Field, inputValue);
					return new TreeTableCellDate (isValid, date, isSelected, isEvent, isError, isUnavailable);

				default:
					throw new System.InvalidOperationException (string.Format ("Unknown FieldType {0}", userField.Type.ToString ()));
			}
		}

		private static TreeTableColumnType GetColumnType(FieldType type)
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

				default:
					throw new System.InvalidOperationException (string.Format ("Unknown FieldType {0}", type.ToString ()));
			}
		}
	}
}
