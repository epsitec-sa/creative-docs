//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
