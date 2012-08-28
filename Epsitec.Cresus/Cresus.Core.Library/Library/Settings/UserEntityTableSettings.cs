//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;

using Epsitec.Cresus.Core.Metadata;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Epsitec.Cresus.Core.Library.Settings
{
	public sealed class UserEntityTableSettings
	{
		public UserEntityTableSettings ()
		{
			this.sort = new List<EntityColumnSort> ();
		}

		public XElement Save(string xmlNodeName)
		{
			return new XElement (xmlNodeName);
		}


		public IList<EntityColumnSort> Sort
		{
			get
			{
				return this.sort;
			}
		}


		public static UserEntityTableSettings Restore(XElement xml)
		{
			return new UserEntityTableSettings ();
		}


		private static class Xml
		{
		}


		private readonly List<EntityColumnSort>	sort;
	}
}
