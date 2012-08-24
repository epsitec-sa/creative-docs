//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;
using Epsitec.Cresus.DataLayer.Expressions;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;

namespace Epsitec.Cresus.Core.Data.Metadata
{
	/// <summary>
	/// The <c>EntityColumnFilter</c> class defines the filtering conditions for an
	/// <see cref="EntityColumn"/>.
	/// </summary>
	public class EntityColumnFilter
	{
		public EntityColumnFilter()
		{
		}
		
		
		public XElement Save(string xmlNodeName)
		{
			return new XElement (xmlNodeName);
		}

		public static EntityColumnFilter Restore(XElement xml)
		{
			if (xml == null)
			{
				return null;
			}

			return new EntityColumnFilter ();
		}

		
		#region Strings Class

		private static class Strings
		{
		}

		#endregion
	}
}
