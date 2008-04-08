//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Reporting.DataItems
{
	class EntityDataItem : DataView.DataItem
	{
		public EntityDataItem(DataViewContext context, AbstractEntity entity)
		{
			this.entity = entity;
			this.DataView = new DataView (context);
		}

		public override object ObjectValue
		{
			get
			{
				return this.entity;
			}
		}

		private readonly AbstractEntity entity;
	}
}
