//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;
using Epsitec.Common.Types;

namespace Epsitec.Common.UI
{
	public class Panel : Widgets.AbstractGroup
	{
		public Panel()
		{
		}

		public DataSourceCollection DataSource
		{
			get
			{
				return this.dataSource;
			}
			set
			{
				this.dataSource = value;
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
