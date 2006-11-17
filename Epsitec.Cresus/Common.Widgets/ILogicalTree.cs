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
		/// <summary>
		/// Gets the logical parent of the object.
		/// </summary>
		/// <value>The logical parent.</value>
		Visual Parent
		{
			get;
		}
	}
}
