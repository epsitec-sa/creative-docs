//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.Helpers;
using Epsitec.Cresus.Assets.Server.NodesGetter;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.DataFillers
{
	public class AmortizationsObjectsTreeTableFiller : AbstractTreeTableFiller<SortableNode>
	{
		public AmortizationsObjectsTreeTableFiller(DataAccessor accessor, AbstractNodesGetter<SortableNode> nodesGetter)
			: base (accessor, nodesGetter)
		{
		}


		public override IEnumerable<ObjectField> Fields
		{
			get
			{
				yield return ObjectField.EventDate;
				yield return ObjectField.EventGlyph;
				yield return ObjectField.EventType;
				yield return ObjectField.MainValue;

				yield return ObjectField.AmortizationDetailsInitialValue;
				yield return ObjectField.AmortizationDetailsBaseValue;
				yield return ObjectField.AmortizationDetailsDeltaValue;
				yield return ObjectField.AmortizationDetailsForcedValue;

				//-yield return ObjectField.AmortizationDetailsDefRate;
				//-yield return ObjectField.AmortizationDetailsDefType;
				//-yield return ObjectField.AmortizationDetailsDefPeriodicity;
				//-yield return ObjectField.AmortizationDetailsDefProrataType;
				//-yield return ObjectField.AmortizationDetailsDefRound;
				//-yield return ObjectField.AmortizationDetailsDefResidual;

				//-yield return ObjectField.AmortizationDetailsProrataBeginDate;
				//-yield return ObjectField.AmortizationDetailsProrataEndDate;
				//-yield return ObjectField.AmortizationDetailsProrataValueDate;
				yield return ObjectField.AmortizationDetailsProrataNumerator;
				yield return ObjectField.AmortizationDetailsProrataDenominator;
				yield return ObjectField.AmortizationDetailsProrataQuotient;
			}
		}

		public override TreeTableColumnDescription[] Columns
		{
			get
			{
				var list = new List<TreeTableColumnDescription> ();

				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String,          70, "Date"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.Glyph,           20, ""));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String,         110, "Type"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.ComputedAmount, 120, "Valeur comptable"));

				list.Add (new TreeTableColumnDescription (TreeTableColumnType.Amount, 120, "Valeur initiale"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.Amount, 120, "Valeur de base"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.Amount, 120, "Delta"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.Amount, 120, "Valeur forcée"));

				//-list.Add (new TreeTableColumnDescription (TreeTableColumnType.Rate, 80, "Taux"));
				//-list.Add (new TreeTableColumnDescription (TreeTableColumnType.String, 80, "Type"));
				//-list.Add (new TreeTableColumnDescription (TreeTableColumnType.String, 100, "Périodicité"));
				//-list.Add (new TreeTableColumnDescription (TreeTableColumnType.String, 100, "Prorata"));
				//-list.Add (new TreeTableColumnDescription (TreeTableColumnType.Amount, 100, "Arrondi"));
				//-list.Add (new TreeTableColumnDescription (TreeTableColumnType.Amount, 120, "Valeur résiduelle"));

				list.Add (new TreeTableColumnDescription (TreeTableColumnType.Amount, 120, "Numérateur"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.Amount, 120, "Dénominateur"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.Rate, 120, "Prorata"));

				return list.ToArray ();
			}
		}

		public override TreeTableContentItem GetContent(int firstRow, int count, int selection)
		{
			var c11  = new TreeTableColumnItem<TreeTableCellString> ();
			var c12  = new TreeTableColumnItem<TreeTableCellGlyph> ();
			var c13  = new TreeTableColumnItem<TreeTableCellString> ();
			var c14  = new TreeTableColumnItem<TreeTableCellComputedAmount> ();

			var c21 = new TreeTableColumnItem<TreeTableCellDecimal> ();
			var c22 = new TreeTableColumnItem<TreeTableCellDecimal> ();
			var c23 = new TreeTableColumnItem<TreeTableCellDecimal> ();
			var c24 = new TreeTableColumnItem<TreeTableCellDecimal> ();

			//-var c32 = new TreeTableColumnItem<TreeTableCellDecimal> ();
			//-var c33 = new TreeTableColumnItem<TreeTableCellString> ();
			//-var c34 = new TreeTableColumnItem<TreeTableCellString> ();
			//-var c35 = new TreeTableColumnItem<TreeTableCellString> ();
			//-var c36 = new TreeTableColumnItem<TreeTableCellDecimal> ();
			//-var c37 = new TreeTableColumnItem<TreeTableCellDecimal> ();

			var c41 = new TreeTableColumnItem<TreeTableCellDecimal> ();
			var c42 = new TreeTableColumnItem<TreeTableCellDecimal> ();
			var c43 = new TreeTableColumnItem<TreeTableCellDecimal> ();

			for (int i=0; i<count; i++)
			{
				if (firstRow+i >= this.nodesGetter.Count)
				{
					break;
				}

				var node = this.nodesGetter[firstRow+i];
				var e    = this.DataObject.GetEvent (node.Guid);
				System.Diagnostics.Debug.Assert (e != null);

				var timestamp  = e.Timestamp;
				var eventType  = e.Type;

				{
					var date    = TypeConverters.DateToString (timestamp.Date);
					var glyph   = TimelineData.TypeToGlyph (eventType);
					var type    = DataDescriptions.GetEventDescription (eventType);
					var valeur1 = ObjectCalculator.GetObjectPropertyComputedAmount (this.DataObject, timestamp, ObjectField.MainValue, synthetic: false);

					var s11 = new TreeTableCellString         (true, date,    isSelected: (i == selection));
					var s12 = new TreeTableCellGlyph          (true, glyph,   isSelected: (i == selection));
					var s13 = new TreeTableCellString         (true, type,    isSelected: (i == selection));
					var s14 = new TreeTableCellComputedAmount (true, valeur1, isSelected: (i == selection));

					c11.AddRow (s11);
					c12.AddRow (s12);
					c13.AddRow (s13);
					c14.AddRow (s14);
				}

				{
					var j = ObjectCalculator.GetObjectPropertyDecimal (this.DataObject, timestamp, ObjectField.AmortizationDetailsInitialValue, synthetic: false);
					var b = ObjectCalculator.GetObjectPropertyDecimal (this.DataObject, timestamp, ObjectField.AmortizationDetailsBaseValue,    synthetic: false);
					var d = ObjectCalculator.GetObjectPropertyDecimal (this.DataObject, timestamp, ObjectField.AmortizationDetailsDeltaValue,   synthetic: false);
					var f = ObjectCalculator.GetObjectPropertyDecimal (this.DataObject, timestamp, ObjectField.AmortizationDetailsForcedValue,  synthetic: false);

					var s21 = new TreeTableCellDecimal (true, j, isSelected: (i == selection));
					var s22 = new TreeTableCellDecimal (true, b, isSelected: (i == selection));
					var s23 = new TreeTableCellDecimal (true, d, isSelected: (i == selection));
					var s24 = new TreeTableCellDecimal (true, f, isSelected: (i == selection));

					c21.AddRow (s21);
					c22.AddRow (s22);
					c23.AddRow (s23);
					c24.AddRow (s24);
				}

				{
					//-var taux   = ObjectCalculator.GetObjectPropertyDecimal (this.DataObject, timestamp, ObjectField.AmortizationDetailsDefRate,        synthetic: false);
					//-var type   = ObjectCalculator.GetObjectPropertyInt     (this.DataObject, timestamp, ObjectField.AmortizationDetailsDefType,        synthetic: false);
					//-var period = ObjectCalculator.GetObjectPropertyInt     (this.DataObject, timestamp, ObjectField.AmortizationDetailsDefPeriodicity, synthetic: false);
					//-var prorat = ObjectCalculator.GetObjectPropertyInt     (this.DataObject, timestamp, ObjectField.AmortizationDetailsDefProrataType, synthetic: false);
					//-var round  = ObjectCalculator.GetObjectPropertyDecimal (this.DataObject, timestamp, ObjectField.AmortizationDetailsDefRound,       synthetic: false);
					//-var residu = ObjectCalculator.GetObjectPropertyDecimal (this.DataObject, timestamp, ObjectField.AmortizationDetailsDefResidual,    synthetic: false);
					//-
					//-var t = EnumDictionaries.GetAmortizationTypeName (type);
					//-var c = EnumDictionaries.GetPeriodicityName (period);
					//-var r = EnumDictionaries.GetProrataTypeName (prorat);
					//-
					//-var s32 = new TreeTableCellDecimal (true, taux,   isSelected: (i == selection));
					//-var s33 = new TreeTableCellString  (true, t,      isSelected: (i == selection));
					//-var s34 = new TreeTableCellString  (true, c,      isSelected: (i == selection));
					//-var s35 = new TreeTableCellString  (true, r,      isSelected: (i == selection));
					//-var s36 = new TreeTableCellDecimal (true, round,  isSelected: (i == selection));
					//-var s37 = new TreeTableCellDecimal (true, residu, isSelected: (i == selection));
					//-
					//-c32.AddRow (s32);
					//-c33.AddRow (s33);
					//-c34.AddRow (s34);
					//-c35.AddRow (s35);
					//-c36.AddRow (s36);
					//-c37.AddRow (s37);
				}

				{
					var n = ObjectCalculator.GetObjectPropertyDecimal (this.DataObject, timestamp, ObjectField.AmortizationDetailsProrataNumerator,   synthetic: false);
					var d = ObjectCalculator.GetObjectPropertyDecimal (this.DataObject, timestamp, ObjectField.AmortizationDetailsProrataDenominator, synthetic: false);
					var q = ObjectCalculator.GetObjectPropertyDecimal (this.DataObject, timestamp, ObjectField.AmortizationDetailsProrataQuotient,    synthetic: false);

					var s41 = new TreeTableCellDecimal (true, n, isSelected: (i == selection));
					var s42 = new TreeTableCellDecimal (true, d, isSelected: (i == selection));
					var s43 = new TreeTableCellDecimal (true, q, isSelected: (i == selection));

					c41.AddRow (s41);
					c42.AddRow (s42);
					c43.AddRow (s43);
				}
			}

			var content = new TreeTableContentItem ();

			content.Columns.Add (c11);
			content.Columns.Add (c12);
			content.Columns.Add (c13);
			content.Columns.Add (c14);

			content.Columns.Add (c21);
			content.Columns.Add (c22);
			content.Columns.Add (c23);
			content.Columns.Add (c24);

			//-content.Columns.Add (c32);
			//-content.Columns.Add (c33);
			//-content.Columns.Add (c34);
			//-content.Columns.Add (c35);
			//-content.Columns.Add (c36);
			//-content.Columns.Add (c37);

			content.Columns.Add (c41);
			content.Columns.Add (c42);
			content.Columns.Add (c43);

			return content;
		}
	}
}
