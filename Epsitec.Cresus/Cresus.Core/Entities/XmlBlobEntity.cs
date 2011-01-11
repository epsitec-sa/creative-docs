//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Cresus.Core.Business.Accounting;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class XmlBlobEntity
	{
		//	Donne accès au champ 'Data' sous sa forme effective d'un XElement.
		public XElement XmlData
		{
			get
			{
				using (var stream = new System.IO.MemoryStream (this.Data))
				{
					return XElement.Load (stream);
				}
			}
			set
			{
				using (var stream = new System.IO.MemoryStream ())
				{
					value.Save (stream);
					this.Data = stream.GetBuffer ();
				}
			}
		}
	}
}
