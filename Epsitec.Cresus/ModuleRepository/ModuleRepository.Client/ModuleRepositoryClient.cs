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
			NetTcpBinding tcpBinding = new NetTcpBinding (SecurityMode.Transport, true);
			
			tcpBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
			tcpBinding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

			EndpointAddress address = new EndpointAddress (new System.Uri ("net.tcp://svn.opac.ch:31415/ModuleRepository"));
			ModuleRepositoryClient.factory = new ChannelFactory<IModuleRepositoryService> (tcpBinding, address);
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
		/// Reopens the service if needs to be reopened.
		/// </summary>
		/// <param name="service">The service.</param>
		public static void ReopenService(ref IModuleRepositoryService service)
		{
			if (service == null)
			{
				throw new System.ArgumentNullException ("service");
			}
			else
			{
				System.ServiceModel.Channels.IChannel channel = service as System.ServiceModel.Channels.IChannel;

				System.Diagnostics.Debug.Assert (channel != null);
				
				if (channel.State != CommunicationState.Opened)
				{
					if (channel.State != CommunicationState.Closed)
					{
						channel.Close ();
					}

					service = ModuleRepositoryClient.GetService ();
					channel = service as System.ServiceModel.Channels.IChannel;
					
					System.Diagnostics.Debug.Assert (channel.State == CommunicationState.Opened);
				}
			}
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
