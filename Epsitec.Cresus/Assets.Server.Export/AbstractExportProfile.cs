//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.Export
{
	public abstract class AbstractExportProfile
	{
		public abstract void Serialize(System.Xml.XmlWriter writer, string name);
	}
}