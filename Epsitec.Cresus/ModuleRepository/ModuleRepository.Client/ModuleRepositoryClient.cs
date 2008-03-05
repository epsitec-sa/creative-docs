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
			IModuleRepositoryService service = ModuleRepositoryClient.factory.CreateChannel ();
			System.ServiceModel.Channels.IChannel channel = service as System.ServiceModel.Channels.IChannel;

			System.Diagnostics.Debug.Assert (channel != null);
			System.Diagnostics.Debug.Assert (channel.State == CommunicationState.Created);
			channel.Open ();
			System.Diagnostics.Debug.Assert (channel.State == CommunicationState.Opened);

			return service;
		}

		public static void CloseService(IModuleRepositoryService service)
		{
			System.ServiceModel.Channels.IChannel channel = service as System.ServiceModel.Channels.IChannel;

			System.Diagnostics.Debug.Assert (channel != null);
			System.Diagnostics.Debug.Assert (channel.State != CommunicationState.Closed);
			channel.Close ();
			System.Diagnostics.Debug.Assert (channel.State == CommunicationState.Closed);
		}


		private static ChannelFactory<IModuleRepositoryService> factory;
	}
}
