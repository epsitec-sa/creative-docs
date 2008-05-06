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

		/// <summary>
		/// Gets the columns definiton, stored as a vector setting.
		/// </summary>
		/// <value>The columns.</value>
		public Settings.VectorSetting Columns
		{
			get
			{
				return this.vectorSetting;
			}
		}


		public override string GetNextChildId(string childId)
		{
			if (this.GenerateColumnIds ())
			{
				bool found = false;

				foreach (string id in this.columnIds)
				{
					if (found)
					{
						return id;
					}
					found = (id == childId);
				}
			}

			return null;
		}

		public override string GetPrevChildId(string childId)
		{
			if (this.GenerateColumnIds ())
			{
				string prev = null;

				foreach (string id in this.columnIds)
				{
					if (id == childId)
					{
						return prev;
					}
					prev = id;
				}
			}

			return null;
		}


		private bool GenerateColumnIds()
		{
			if (this.columnIds == null)
			{
				Settings.VectorSetting setting = this.Columns;

				IStructuredData     data     = this.entity;
				IEnumerable<string> fieldIds = data.GetValueIds ();

				if (setting != null)
				{
					this.columnIds = setting.CreateList (fieldIds);
				}
				else
				{
					this.columnIds = new List<string> (fieldIds);
				}
			}
			
			return true;
		}


		private readonly AbstractEntity entity;
		private Settings.VectorSetting vectorSetting;
		private List<string> columnIds;
	}
}
