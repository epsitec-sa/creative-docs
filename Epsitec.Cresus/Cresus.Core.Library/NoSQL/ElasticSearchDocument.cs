using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nest;
using Newtonsoft.Json;

namespace Epsitec.Cresus.Core.NoSQL
{
	[ElasticType (IdProperty = "DocumentId")]
	public class ElasticSearchDocument
	{
		//DocumentId will be used as id, no need to index it as a property
		[JsonIgnore]
		public string DocumentId
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public string Text
		{
			get;
			set;
		}


		public string DatasetId
		{
			get;
			set;
		}

		public string EntityId
		{
			get;
			set;
		}	
	}
}
