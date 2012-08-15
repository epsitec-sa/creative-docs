//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Core.Data.Extraction
{
	/// <summary>
	/// The <c>EntityDataMetadata</c> class is a collection of <see cref="EntityDataColum"/>
	/// instances.
	/// </summary>
	public sealed class EntityDataMetadata
	{
		internal EntityDataMetadata(IEnumerable<EntityDataColumn> columns)
		{
			this.columns = columns.ToArray ();
		}

		
		public int							TotalColumnCount
		{
			get
			{
				return this.columns.Length;
			}
		}

		private readonly EntityDataColumn[] columns;
	}
}