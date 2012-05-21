//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Support
{
	/// <summary>
	///	The <c>IDataBroker</c> interface is used to create items which belong to
	/// collections of <see cref="StructuredData"/> items in the context of a
	/// given <see cref="CultureMap"/>.
	/// </summary>
	public interface IDataBroker
	{
		Types.StructuredData CreateData(CultureMap container);
	}
}
