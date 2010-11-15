//	Copyright © 2004-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>IReadOnly</c> interface is used to check if an object is read
	/// only or not.
	/// </summary>
	public interface IReadOnly
	{
		bool IsReadOnly
		{
			get;
		}
	}
}
