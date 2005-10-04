//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets.Layouts
{
	/// <summary>
	/// ILayout.
	/// </summary>
	public interface ILayout
	{
		void UpdateLayout(Visual container, System.Collections.ICollection children);
	}
}
