//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.ServiceModel;

namespace Epsitec.ModuleRepository
{
	[ServiceContract]
	public interface IModuleRepositoryService
	{
		[OperationContract]
		int GetNewModuleId(string moduleName, string developerName);
		
		[OperationContract]
		bool RecycleModuleId(int moduleId, string developerName);

		[OperationContract]
		ModuleDirectory CreateEmptyModule(int moduleId, string moduleLayerPrefix, string sourceNamespace);
	}
}
