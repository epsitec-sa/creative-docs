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
	public class EventsAssetsTreeTableFiller : AbstractTreeTableFiller<SortableNode>
	{
		public EventsAssetsTreeTableFiller(DataAccessor accessor, INodeGetter<SortableNode> nodeGetter)
			: base (accessor, nodeGetter)
		{
		}


		public override SortingInstructions		DefaultSorting
		{
			get
			{
				if (this.UserFields.Any ())
				{
					var field = this.UserFields.First ().Field;
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

				foreach (var userField in this.UserFields)
				{
					TreeTableColumnType type;
				
					if (userField.Field == ObjectField.EventGlyph)
					{
						type = TreeTableColumnType.Glyph;
					}
					else
					{
						type = AbstractTreeTableCell.GetColumnType (userField.Type);
					}

					columns.Add (new TreeTableColumnDescription (userField.Field, type, userField.ColumnWidth, userField.Name));
				}

				return columns.ToArray ();
			}
		}

		public override TreeTableContentItem GetContent(int firstRow, int count, int selection)
		{
			var content = new TreeTableContentItem ();

			foreach (var userField in this.UserFields)
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
				var e    = this.DataObject.GetEvent (node.Guid);

				if (e == null)
				{
					//	Il peut arriver qu'on effectue ce code suite à un redimensionnement
					//	de la fenêtre, alors que les données ne sont pas à jour. Si un événement
					//	a été supprimé, il aura une valeur nulle et il faut stopper la boucle
					//	sans asset et sans signaler un quelconque problème !
					break;
				}

				var timestamp = e.Timestamp;
				var eventType = e.Type;

				var cellState = (i == selection) ? CellState.Selected : CellState.None;

				int columnRank = 0;

				foreach (var userField in this.UserFields)
				{
					AbstractTreeTableCell cell;

					if (userField.Field == ObjectField.EventDate)
					{
						cell = new TreeTableCellDate (timestamp.Date, cellState);
					}
					else if (userField.Field == ObjectField.EventGlyph)
					{
						var mode  = AssetsLogic.IsAmortizationEnded (this.DataObject, e);
						var glyph = TimelineData.TypeToGlyph (eventType, mode);
						cell = new TreeTableCellGlyph (glyph, cellState);
					}
					else if (userField.Field == ObjectField.EventType)
					{
						var type = DataDescriptions.GetEventDescription (eventType);
						cell = new TreeTableCellString (type, cellState);
					}
					else
					{
						cell = AbstractTreeTableCell.CreateTreeTableCell (this.accessor, this.DataObject, timestamp, userField, false, cellState, synthetic: false);
					}

					content.Columns[columnRank++].AddRow (cell);
				}
			}

			return content;
		}

		private IEnumerable<UserField> UserFields
		{
			get
			{
				yield return new UserField (0, Res.Strings.EventsAssetsTreeTableFiller.Date.ToString (), ObjectField.EventDate,  FieldType.Date,   false,  70, null, null, null, 0, null);
				yield return new UserField (1, "",                                                       ObjectField.EventGlyph, FieldType.String, false,  20, null, null, null, 0, null);
				yield return new UserField (2, Res.Strings.EventsAssetsTreeTableFiller.Type.ToString (), ObjectField.EventType,  FieldType.String, false, 110, null, null, null, 0, null);

				foreach (var userField in AssetsLogic.GetUserFields (this.accessor))
				{
					yield return userField;

					if (userField.Field == ObjectField.MainValue)
					{
						//	ObjectField.MainValueDelta permet d'obtenir la même valeur que ObjectField.MainValue, mais
						//	on obtient la différence (-Amortization), plutôt que la valeur finale (FinalAmount).
						yield return new UserField (-1, DataDescriptions.GetObjectFieldDescription (ObjectField.MainValueDelta), ObjectField.MainValueDelta, FieldType.AmortizedAmount, false, 120, null, null, null, 0, null);
					}
				}
			}
		}
	}
}
