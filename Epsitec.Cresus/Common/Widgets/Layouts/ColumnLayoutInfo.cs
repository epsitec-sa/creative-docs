//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Widgets.Layouts
{
	/// <summary>
	/// The <c>ColumnLayoutInfo</c> structure defines the layout of a column: how it should
	/// behave (<see cref="ColumnDefinition"/>) and what size if occupies (<see cref="ColumnMeasure"/>).
	/// </summary>
	public struct ColumnLayoutInfo
	{
		public ColumnLayoutInfo(ColumnDefinition definition)
		{
			this.definition = definition;
			this.measure    = new ColumnMeasure ();
		}


		public ColumnDefinition					Definition
		{
			get
			{
				return this.definition;
			}
		}

		public ColumnMeasure					Measure
		{
			get
			{
				return this.measure;
			}
		}

		
		private readonly ColumnDefinition		definition;
		private readonly ColumnMeasure			measure;
	}
}
