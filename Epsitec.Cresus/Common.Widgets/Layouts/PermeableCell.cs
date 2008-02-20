//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Widgets.Layouts
{
	/// <summary>
	/// The <c>PermeableCell</c> structure associates a visual with grid-related
	/// positioning information.
	/// </summary>
	public struct PermeableCell
	{
		public PermeableCell(Visual visual, int column, int row, int columnSpan, int rowSpan)
		{
			this.visual     = visual;
			this.column     = column;
			this.row        = row;
			this.columnSpan = columnSpan;
			this.rowSpan    = rowSpan;
		}

		public Visual Visual
		{
			get
			{
				return this.visual;
			}
		}

		public int Column
		{
			get
			{
				return this.column;
			}
		}

		public int Row
		{
			get
			{
				return this.row;
			}
		}

		public int ColumnSpan
		{
			get
			{
				return this.columnSpan;
			}
		}

		public int RowSpan
		{
			get
			{
				return this.rowSpan;
			}
		}
		
		private Visual visual;
		private int column;
		private int row;
		private int columnSpan;
		private int rowSpan;
	}
}
