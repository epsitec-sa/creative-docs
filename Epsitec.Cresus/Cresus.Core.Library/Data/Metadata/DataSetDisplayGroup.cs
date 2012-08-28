//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Data.Metadata
{
	public sealed class DataSetDisplayGroup
	{
		public DataSetDisplayGroup(Druid id)
		{
			this.id = id;
			this.dataSets = new List<DataSetMetadata> ();
		}

		public DataSetDisplayGroup(Druid id, IEnumerable<DataSetMetadata> dataSets)
		{
			this.id = id;
			this.dataSets = new List<DataSetMetadata> (dataSets);
		}


		public Druid							Id
		{
			get
			{
				return this.id;
			}
		}
		
		public Caption							Caption
		{
			get
			{
				if (this.id.IsEmpty)
				{
					return null;
				}
				
				return SafeResourceResolver.Instance.GetCaption (this.id);
			}
		}

		public IList<DataSetMetadata>			DataSets
		{
			get
			{
				return this.dataSets;
			}
		}

		
		private readonly Druid					id;
		private readonly List<DataSetMetadata>	dataSets;
	}
}
