//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>IName</c> interface returns the name of an object.
	/// </summary>
	public interface IName
	{
		/// <summary>
		/// Gets the name of the object.
		/// </summary>
		/// <value>The name of the object.</value>
		string Name
		{
			get;
		}
	}
}
