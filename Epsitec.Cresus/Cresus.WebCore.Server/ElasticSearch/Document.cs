using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nest;
using Newtonsoft.Json;

namespace Epsitec.Cresus.WebCore.Server.ElasticSearch
{
	[ElasticType (IdProperty = "DocumentId")]
	class Document
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
	}
}
