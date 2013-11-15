//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.DataFillers;
using Epsitec.Cresus.Assets.Server.NodesGetter;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public class HistoryAccessor
	{
		public HistoryAccessor(DataAccessor accessor, BaseType baseType, Guid objectGuid, Timestamp? timestamp, ObjectField field)
		{
			this.accessor    = accessor;
			this.baseType    = baseType;
			this.objectField = field;
			this.fieldType   = DataAccessor.GetFieldType (field);
			this.obj         = this.accessor.GetObject (this.baseType, objectGuid);
			this.timestamp   = timestamp;

			this.getter = new HistoryNodesGetter
			{
				DataObject = this.obj,
				Field      = this.objectField,
				Timestamp  = this.timestamp,
			};
			this.getter.UpdateData ();

			this.filler = new HistoryTreeTableFiller (this.accessor, this.getter, this.objectField)
			{
				DataObject = this.obj,
			};
		}


		public HistoryTreeTableFiller			Filler
		{
			get
			{
				return this.filler;
			}
		}


		public int								RowsCount
		{
			get
			{
				return this.getter.Count;
			}
		}

		public int								SelectedRow
		{
			get
			{
				int sel = 0;

				foreach (var node in this.getter.Nodes)
				{
					var e = this.obj.GetEvent (node.Guid);
					if (e.Timestamp == this.timestamp)
					{
						return sel;
					}

					sel++;
				}

				return -1;
			}
		}

		public int								ColumnsWidth
		{
			get
			{
				return HistoryTreeTableFiller.DateColumnWidth
					 + HistoryTreeTableFiller.GlyphColumnWidth
					 + HistoryTreeTableFiller.GetValueColumnWidth (this.objectField);
			}
		}


		public Timestamp? GetTimestamp(int row)
		{
			var node = this.getter[row];
			if (!node.IsEmpty)
			{
				var e = this.obj.GetEvent (node.Guid);
				if (e != null)
				{
					return e.Timestamp;
				}
			}

			return null;
		}


		private readonly DataAccessor								accessor;
		private readonly BaseType									baseType;
		private readonly ObjectField								objectField;
		private readonly FieldType									fieldType;
		private readonly DataObject									obj;
		private readonly Timestamp?									timestamp;

		private readonly HistoryNodesGetter							getter;
		private readonly HistoryTreeTableFiller						filler;
	}
}
