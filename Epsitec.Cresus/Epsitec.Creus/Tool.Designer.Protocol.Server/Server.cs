//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace Epsitec.Designer.Protocol
{
	/// <summary>
	/// The <c>Server</c> class is used to implement the <see cref="INavigator"/>
	/// service through a named pipes binding.
	/// </summary>
	public sealed class Server : System.IDisposable
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Server"/> class.
		/// </summary>
		/// <param name="enableMetadata">If set to <c>true</c>, enables metadata
		/// publishing through a MEX service endpoint.</param>
		public Server(bool enableMetadata)
		{
			this.serviceHost = new ServiceHost (typeof (NavigatorService));
			NetNamedPipeBinding binding = new NetNamedPipeBinding (NetNamedPipeSecurityMode.None);

			if (enableMetadata)
			{
				ServiceMetadataBehavior beheavior = new ServiceMetadataBehavior ();
				this.serviceHost.Description.Behaviors.Add (beheavior);
				this.serviceHost.AddServiceEndpoint (typeof (IMetadataExchange), MetadataExchangeBindings.CreateMexNamedPipeBinding (), string.Concat (Addresses.DesignerAddress, "/mex"));
			}

			this.serviceHost.AddServiceEndpoint (typeof (INavigator), binding, Addresses.DesignerAddress);
			this.isReady = true;
		}

		/// <summary>
		/// Opens the server: the service host will listen to incoming requests
		/// until <see cref="M:Close"/> is called.
		/// </summary>
		public void Open()
		{
			if (this.isDisposed)
			{
				throw new System.ObjectDisposedException ("Server");
			}
			if (!this.isReady)
			{
				throw new System.InvalidOperationException ("Server not properly initialized");
			}
			if (this.isOpen == false)
			{
				this.serviceHost.Open ();
				this.isOpen = true;
			}
		}

		/// <summary>
		/// Shuts down the server.
		/// </summary>
		public void Close()
		{
			if (this.isDisposed)
			{
				throw new System.ObjectDisposedException ("Server");
			}
			if (!this.isReady)
			{
				throw new System.InvalidOperationException ("Server not properly initialized");
			}
			if (this.isOpen)
			{
				this.serviceHost.Close ();
				this.isOpen = false;
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
			if (this.isOpen)
			{
				this.Close ();
			}

			this.isDisposed = true;
		}

		#endregion


		private readonly ServiceHost serviceHost;
		
		private bool isReady;
		private bool isOpen;
		private bool isDisposed;
	}
}
