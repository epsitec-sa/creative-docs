//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Reporting.DataItems
{
	/// <summary>
	/// The <c>EntityDataItem</c> class represents items which map to
	/// entities, visible as a vector of data items.
	/// </summary>
	class EntityDataItem : DataView.DataItem
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EntityDataItem"/> class.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="entity">The entity.</param>
		public EntityDataItem(DataViewContext context, AbstractEntity entity)
		{
			this.entity = entity;
			this.DataView = new DataView (context);
		}

		/// <summary>
		/// Gets the raw object value.
		/// </summary>
		/// <value>The raw object value.</value>
		public override object ObjectValue
		{
			get
			{
				return this.entity;
			}
		}

		/// <summary>
		/// Gets the type of the item.
		/// </summary>
		/// <value>Always <c>DataItemType.Vector</c>.</value>
		public override DataItemType ItemType
		{
			get
			{
				return DataItemType.Vector;
			}
		}

		private readonly AbstractEntity entity;
	}
}
