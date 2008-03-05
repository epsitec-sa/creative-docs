//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace Epsitec.ModuleRepository
{
	/// <summary>
	/// The <c>ModuleRepositoryClient</c> class manages the access to the
	/// proxy implementing <see cref="IModuleRepositoryService"/>.
	/// </summary>
	public static class ModuleRepositoryClient
	{
		#region Private Static Initializer

		static ModuleRepositoryClient()
		{
			BasicHttpBinding binding = new BasicHttpBinding ();
			EndpointAddress address = new EndpointAddress (new System.Uri ("http://localhost:8080/ModuleRepository"));
			ModuleRepositoryClient.factory = new ChannelFactory<IModuleRepositoryService> (binding, address);
		}

		#endregion

		/// <summary>
		/// Gets and opens a proxy to the service.
		/// </summary>
		/// <returns>The proxy implementing <see cref="IModuleRepositoryService"/>.</returns>
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

		/// <summary>
		/// Closes the service.
		/// </summary>
		/// <param name="service">The proxy obtained through <see cref="GetService"/>.</param>
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
