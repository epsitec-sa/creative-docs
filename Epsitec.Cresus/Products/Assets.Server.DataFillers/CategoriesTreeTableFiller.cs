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
	public class CategoriesTreeTableFiller : AbstractTreeTableFiller<SortableNode>
	{
		public CategoriesTreeTableFiller(DataAccessor accessor, INodeGetter<SortableNode> nodeGetter)
			: base (accessor, nodeGetter)
		{
			this.argumentFields = new List<ObjectField> ();
			this.argumentTypes  = new List<ArgumentType> ();
		}


		public override SortingInstructions		DefaultSorting
		{
			get
			{
				return new SortingInstructions (ObjectField.Name, SortedType.Ascending, ObjectField.Unknown, SortedType.None);
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

				columns.Add (new TreeTableColumnDescription (ObjectField.Name,        TreeTableColumnType.String,  180, Res.Strings.CategoriesTreeTableFiller.Name.ToString ()));
				columns.Add (new TreeTableColumnDescription (ObjectField.Number,      TreeTableColumnType.String,   50, Res.Strings.CategoriesTreeTableFiller.Number.ToString ()));
				columns.Add (new TreeTableColumnDescription (ObjectField.MethodGuid,  TreeTableColumnType.String,  180, Res.Strings.CategoriesTreeTableFiller.Method.ToString ()));
				columns.Add (new TreeTableColumnDescription (ObjectField.Periodicity, TreeTableColumnType.String,  100, Res.Strings.CategoriesTreeTableFiller.Periodicity.ToString ()));

				this.argumentFields.Clear ();
				this.argumentTypes.Clear ();

				foreach (var argument in ArgumentsLogic.GetSortedArguments (this.accessor))
				{
					var name = ObjectProperties.GetObjectPropertyString (argument, null, ObjectField.Name);
					var type = (ArgumentType) ObjectProperties.GetObjectPropertyInt (argument, null, ObjectField.ArgumentType);
					var field = (ObjectField) ObjectProperties.GetObjectPropertyInt (argument, null, ObjectField.ArgumentField);

					switch (type)
					{
						case ArgumentType.Decimal:
							columns.Add (new TreeTableColumnDescription (field, TreeTableColumnType.Decimal, 100, name));
							break;

						case ArgumentType.Amount:
							columns.Add (new TreeTableColumnDescription (field, TreeTableColumnType.Amount, 100, name));
							break;

						case ArgumentType.Rate:
							columns.Add (new TreeTableColumnDescription (field, TreeTableColumnType.Rate, 100, name));
							break;

						case ArgumentType.Int:
							columns.Add (new TreeTableColumnDescription (field, TreeTableColumnType.Int, 100, name));
							break;

						case ArgumentType.Date:
							columns.Add (new TreeTableColumnDescription (field, TreeTableColumnType.Date, 100, name));
							break;

						case ArgumentType.String:
							columns.Add (new TreeTableColumnDescription (field, TreeTableColumnType.String, 100, name));
							break;
					}

					this.argumentFields.Add (field);
					this.argumentTypes.Add (type);
				}

				foreach (var field in DataAccessor.AccountFields)
				{
					columns.Add (new TreeTableColumnDescription (field, TreeTableColumnType.String, 150, DataDescriptions.GetObjectFieldDescription (field)));
				}

				return columns.ToArray ();
			}
		}

		public override TreeTableContentItem GetContent(int firstRow, int count, int selection)
		{
			var content = new TreeTableContentItem ();

			int columnCount = 4 + this.argumentFields.Count + DataAccessor.AccountFields.Count ();
			for (int i=0; i<columnCount; i++)
			{
				content.Columns.Add (new TreeTableColumnItem ());
			}

			for (int i=0; i<count; i++)
			{
				if (firstRow+i >= this.nodeGetter.Count)
				{
					break;
				}

				var node  = this.nodeGetter[firstRow+i];
				var guid  = node.Guid;
				var obj   = this.accessor.GetObject (BaseType.Categories, guid);

				var name   = ObjectProperties.GetObjectPropertyString  (obj, this.Timestamp, ObjectField.Name, inputValue: true);
				var number = ObjectProperties.GetObjectPropertyString  (obj, this.Timestamp, ObjectField.Number);
				var mg     = ObjectProperties.GetObjectPropertyGuid    (obj, this.Timestamp, ObjectField.MethodGuid);
				var period = ObjectProperties.GetObjectPropertyInt     (obj, this.Timestamp, ObjectField.Periodicity);

				var m = MethodsLogic.GetSummary (this.accessor, mg);
				var c = EnumDictionaries.GetPeriodicityName (period);

				var cellState = (i == selection) ? CellState.Selected : CellState.None;

				var cell11 = new TreeTableCellString  (name,   cellState);
				var cell12 = new TreeTableCellString  (number, cellState);
				var cell13 = new TreeTableCellString  (m,      cellState);
				var cell14 = new TreeTableCellString  (c,      cellState);

				int columnRank = 0;

				content.Columns[columnRank++].AddRow (cell11);
				content.Columns[columnRank++].AddRow (cell12);
				content.Columns[columnRank++].AddRow (cell13);
				content.Columns[columnRank++].AddRow (cell14);

				for (int a=0; a<this.argumentFields.Count; a++)
				{
					switch (this.argumentTypes[a])
					{
						case ArgumentType.Decimal:
						case ArgumentType.Amount:
						case ArgumentType.Rate:
							{
								var value = ObjectProperties.GetObjectPropertyDecimal (obj, this.Timestamp, this.argumentFields[a]);
								var cell = new TreeTableCellDecimal (value, cellState);
								content.Columns[columnRank++].AddRow (cell);
								break;
							}

						case ArgumentType.Int:
							{
								var value = ObjectProperties.GetObjectPropertyInt (obj, this.Timestamp, this.argumentFields[a]);
								var cell = new TreeTableCellInt (value, cellState);
								content.Columns[columnRank++].AddRow (cell);
								break;
							}

						case ArgumentType.Date:
							{
								var value = ObjectProperties.GetObjectPropertyDate (obj, this.Timestamp, this.argumentFields[a]);
								var cell = new TreeTableCellDate (value, cellState);
								content.Columns[columnRank++].AddRow (cell);
								break;
							}

						case ArgumentType.String:
							{
								var value = ObjectProperties.GetObjectPropertyString (obj, this.Timestamp, this.argumentFields[a]);
								var cell = new TreeTableCellString (value, cellState);
								content.Columns[columnRank++].AddRow (cell);
								break;
							}
					}
				}

				foreach (var field in DataAccessor.AccountFields)
				{
					var cell = new TreeTableCellString (this.GetAccount (obj, field), cellState);
					content.Columns[columnRank++].AddRow (cell);
				}
			}

			return content;
		}

		private string GetAccount(DataObject obj, ObjectField field)
		{
			return ObjectProperties.GetObjectPropertyString (obj, this.Timestamp, field);
		}


		private List<ObjectField>				argumentFields;
		private List<ArgumentType>				argumentTypes;
	}
}
