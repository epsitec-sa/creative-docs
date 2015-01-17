//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.WebCore.Server.Core;
using Epsitec.Cresus.WebCore.Server.NancyHosting;
using Epsitec.Cresus.WebCore.Server.Processors;

using Nancy;

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.WebCore.Server.NancyModules
{
	using Database = Core.Databases.Database;
	using Epsitec.Common.Support;

	/// <summary>
	/// Report Builder
	/// </summary>
	public class ReportingModule : AbstractAuthenticatedModule
	{
		public ReportingModule(CoreServer coreServer)
			: base (coreServer, "/reporting")
		{
			var instances  = InterfaceImplementationResolver<IReportingProcessor>.CreateInstances (coreServer);
			var processors = instances.Select (x => new KeyValuePair<string, IReportingProcessor> (x.Name, x));
			
			this.processors = new System.Collections.Concurrent.ConcurrentDictionary<string, IReportingProcessor> (processors);
			
			Get["/{processor}/{settings}/{report}"] =
				p => this.Execute (context => this.ProduceReport (context, p));
		}

		
		private Response ProduceReport(BusinessContext businessContext, dynamic parameters)
		{
			string processorName = parameters.processor;
			
			IReportingProcessor processor;

			if (this.processors.TryGetValue (processorName, out processor))
			{
				var path   = System.IO.Path.GetTempFileName ();
				
				//	The stream will be owned by Nancy through the Response object. Ensure that when
				//	it gets disposed, the file will be deleted.
				
				var stream = new System.IO.FileStream (path, System.IO.FileMode.Truncate,
					System.IO.FileAccess.ReadWrite, System.IO.FileShare.None, 16*1024,
					System.IO.FileOptions.DeleteOnClose);
				
				var reportName = processor.CreateReport (stream, businessContext, parameters);

				stream.Flush ();
				stream.Seek (0, System.IO.SeekOrigin.Begin);

				return CoreResponse.CreateStreamResponse (stream, reportName);
			}
			else
			{
				return CoreResponse.Failure ("Générateur de rapports", string.Format ("Le générateur {0} n'a pas été trouvé.", processorName));
			}
		}

		private readonly ConcurrentDictionary<string, IReportingProcessor> processors;
	}
}