//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;
using Epsitec.Common.Types;

namespace Epsitec.Common.UI
{
	/// <summary>
	/// The <c>Panel</c> class is used as the (local) root in a widget tree
	/// built by the dynamic user interface designer.
	/// </summary>
	public class Panel : Widgets.AbstractGroup
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:Panel"/> class.
		/// </summary>
		public Panel()
		{
		}

		/// <summary>
		/// Gets or sets the data source.
		/// </summary>
		/// <value>The data source.</value>
		public DataSourceCollection DataSource
		{
			get
			{
				return this.dataSource;
			}
			set
			{
				if (this.dataSource != value)
				{
					if (this.dataSource != null)
					{
						DataObject.ClearDataContext (this);
					}
					
					this.dataSource = value;

					if (this.dataSource != null)
					{
						DataObject.SetDataContext (this, new Binding (this.dataSource));
					}
				}
			}
		}
		
		/// <summary>
		/// Fills the serialization context <c>ExternalMap</c> property.
		/// </summary>
		/// <param name="context">The serialization context.</param>
		public void FillSerializationContext(Types.Serialization.Context context)
		{
			context.ExternalMap.Record ("DataSource", this.dataSource);
		}

		
		private DataSourceCollection dataSource;
	}
}
