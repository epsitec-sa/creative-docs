//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Widgets.Layouts
{
	/// <summary>
	/// ILayout.
	/// </summary>
	public interface ILayoutEngine
	{
		void UpdateLayout(Visual container, Drawing.Rectangle rect, IEnumerable<Visual> children);
	}
}
