//	Copyright © 2009-2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Graph.Adorners
{
	/// <summary>
	/// The <c>HorizontalAxisMode</c> enumeration defines how the values will
	/// be positioned on the horizontal axis.
	/// </summary>
	public enum HorizontalAxisMode
	{
		/// <summary>
		/// The horizontal axis is meaningless.
		/// </summary>
		None,

		/// <summary>
		/// The values are centered on the mark. This is the default for a
		/// line chart.
		/// </summary>
		Ticks,

		/// <summary>
		/// The values are represented between two consecutive marks. This
		/// is the default for a bar chart.
		/// </summary>
		Ranges
	}
}
