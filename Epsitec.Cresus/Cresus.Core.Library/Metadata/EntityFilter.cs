//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;

namespace Epsitec.Cresus.Core.Metadata
{
	public class EntityFilter
	{
		public EntityFilter()
		{
			this.columns = new List<ColumnRef<EntityColumnFilter>> ();
		}


		public IList<ColumnRef<EntityColumnFilter>> Columns
		{
			get
			{
				return this.columns;
			}
		}


		public XElement Save(string xmlNodeName)
		{
			throw new System.NotImplementedException ();
		}

		public static EntityFilter Restore(XElement xml)
		{
			throw new System.NotImplementedException ();
		}
		
		
		private readonly List<ColumnRef<EntityColumnFilter>> columns;
	}
}
