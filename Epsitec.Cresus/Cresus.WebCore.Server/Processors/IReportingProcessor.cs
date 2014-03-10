//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.WebCore.Server.Core;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.WebCore.Server.Processors
{
	public interface IReportingProcessor : IName
	{
		string CreateReport(System.IO.Stream stream, WorkerApp workerApp, BusinessContext businessContext, dynamic parameters);
	}
}

