//	Copyright � 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epsitec.Cresus.Assets.Server.SimpleEngine
{
	public class DataLinkProperty : AbstractDataProperty
	{
		public DataLinkProperty(ObjectField field, DataObject value)
			: base (field)
		{
			this.Value = value;
		}

		public readonly DataObject Value;
	}
}