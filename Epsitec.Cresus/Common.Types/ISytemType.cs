//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>ISystemType</c> interface gives access to a <c>SystemType</c>
	/// property.
	/// </summary>
	public interface ISystemType
	{
		/// <summary>
		/// Gets the system type described by this object.
		/// </summary>
		/// <value>The system type described by this object.</value>
		System.Type SystemType
		{
			get;
		}
	}
}
