//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Xml.Linq;

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <c>IXmlNodeClass</c> interface 
	/// </summary>
	public interface IXmlNodeClass
	{
		XElement Serialize();
	}
}
