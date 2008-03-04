//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace Epsitec.ModuleRepository
{
	public static class ModuleRepositoryClient
	{
		static ModuleRepositoryClient()
		{
			BasicHttpBinding binding = new BasicHttpBinding ();
			EndpointAddress address = new EndpointAddress (new System.Uri ("http://localhost:8080/ModuleRepository"));
			ModuleRepositoryClient.factory = new ChannelFactory<IModuleRepositoryService> (binding, address);
		}

		public static IModuleRepositoryService GetService()
		{
			return ModuleRepositoryClient.factory.CreateChannel ();
		}


		private static ChannelFactory<IModuleRepositoryService> factory;
	}
}
