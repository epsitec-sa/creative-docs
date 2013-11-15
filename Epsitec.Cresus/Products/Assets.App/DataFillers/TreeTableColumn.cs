//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.Widgets;

namespace Epsitec.Cresus.Assets.App.DataFillers
{
	public class TreeTableColumn
	{
		public TreeTableColumn()
		{
			this.rows = new List<object> ();
		}


		public System.Type Type
		{
			get
			{
				if (this.rows.Count == 0)
				{
					return null;
				}
				else
				{
					return this.rows[0].GetType ();
				}
			}
		}


		public IEnumerable<TreeTableCellComputedAmount> ComputedAmountRows
		{
			get
			{
				return this.rows.OfType<TreeTableCellComputedAmount> ();
			}
		}

		public IEnumerable<TreeTableCellDate> DateRows
		{
			get
			{
				return this.rows.OfType<TreeTableCellDate> ();
			}
		}

		public IEnumerable<TreeTableCellDecimal> DecimalRows
		{
			get
			{
				return this.rows.OfType<TreeTableCellDecimal> ();
			}
		}

		public IEnumerable<TreeTableCellGlyph> GlyphRows
		{
			get
			{
				return this.rows.OfType<TreeTableCellGlyph> ();
			}
		}

		public IEnumerable<TreeTableCellGuid> GuidRows
		{
			get
			{
				return this.rows.OfType<TreeTableCellGuid> ();
			}
		}

		public IEnumerable<TreeTableCellInt> IntRows
		{
			get
			{
				return this.rows.OfType<TreeTableCellInt> ();
			}
		}

		public IEnumerable<TreeTableCellString> StringRows
		{
			get
			{
				return this.rows.OfType<TreeTableCellString> ();
			}
		}

		public IEnumerable<TreeTableCellTree> TreeRows
		{
			get
			{
				return this.rows.OfType<TreeTableCellTree> ();
			}
		}


		public void AddRow(object value)
		{
			this.rows.Add (value);
		}


		private readonly List<object> rows;
	}
}
