//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
		IEnumerable<PermeableCell> GetChildren(int column, int row);
		bool GetGlobalGridSpan(out int columnSpan, out int rowSpan);
	}
}
