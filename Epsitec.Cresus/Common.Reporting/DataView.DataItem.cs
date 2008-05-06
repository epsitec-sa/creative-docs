//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Reporting
{
	public partial class DataView
	{
		/// <summary>
		/// The <c>DataItem</c> class implements the common base class for every
		/// data item implementation in the <c>DataItems</c> sub-namespace.
		/// </summary>
		public abstract class DataItem : IDataItem
		{
			public DataItem()
			{
			}

			/// <summary>
			/// Gets or sets the id for this item.
			/// </summary>
			/// <value>The id.</value>
			public string Id
			{
				get;
				set;
			}

			/// <summary>
			/// Gets or sets the parent data view.
			/// </summary>
			/// <value>The parent data view.</value>
			public DataView ParentDataView
			{
				get;
				set;
			}

			/// <summary>
			/// Gets or sets the containg data view.
			/// </summary>
			/// <value>The data view.</value>
			public DataView DataView
			{
				get
				{
					return this.view;
				}
				set
				{
					if (this.view != value)
					{
						if (this.view != null)
						{
							this.view.self = null;
						}

						this.view = value;

						if (this.view != null)
						{
							this.view.self = this;
						}
					}
				}
			}

			/// <summary>
			/// Gets the object value cast to the <see cref="IValueStore"/>
			/// interface.
			/// </summary>
			/// <value>The value store.</value>
			public IValueStore ValueStore
			{
				get
				{
					return this.ObjectValue as IValueStore;
				}
			}

			/// <summary>
			/// Gets the columns definiton, stored as a vector setting.
			/// </summary>
			/// <value>The columns.</value>
			public virtual Settings.VectorSetting Columns
			{
				get
				{
					return null;
				}
			}
			
			public virtual bool IsCollection
			{
				get
				{
					return false;
				}
			}

			public virtual object GetValue(int index)
			{
				throw new System.NotImplementedException ();
			}

			public virtual string GetFirstChildId()
			{
				return null;
			}
			
			public virtual string GetNextChildId(string childId)
			{
				return null;
			}

			public virtual string GetPreviousChildId(string childId)
			{
				return null;
			}

			public virtual DataItem GetVirtualItem(VirtualNodeType virtualNodeType)
			{
				throw new System.NotImplementedException ();
			}
			
			#region IDataItem Members

			/// <summary>
			/// Gets the raw object value.
			/// </summary>
			/// <value>The raw object value.</value>
			public abstract object ObjectValue
			{
				get;
			}

			public virtual string Value
			{
				get
				{
					throw new System.NotImplementedException ();
				}
			}

			public virtual int Count
			{
				get
				{
					throw new System.NotImplementedException ();
				}
			}

			public virtual DataItemClass ItemClass
			{
				get;
				set;
			}

			public virtual DataItemType ItemType
			{
				get
				{
					throw new System.NotImplementedException ();
				}
			}

			public virtual INamedType DataType
			{
				get
				{
					throw new System.NotImplementedException ();
				}
			}

			#endregion

			private DataView view;
		}
	}
}
