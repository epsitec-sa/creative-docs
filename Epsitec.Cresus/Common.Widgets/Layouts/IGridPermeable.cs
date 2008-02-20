//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Widgets.Layouts
{
	/// <summary>
	/// The <c>IGridPermeable</c> interface is implemented by visuals which are
	/// considered as "transparent" when processing a grid layout.
	/// </summary>
	public interface IGridPermeable
	{
		IEnumerable<PermeableCell> GetChildren(int column, int row, int columnSpan, int rowSpan);
		bool UpdateGridSpan(ref int columnSpan, ref int rowSpan);
	}
}
