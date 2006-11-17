//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// The <c>ILogicalTree</c> interface defines the relationship of an object
	/// with its logical parent widget.
	/// </summary>
	public interface ILogicalTree
	{
		Visual Parent
		{
			get;
		}
	}
}
