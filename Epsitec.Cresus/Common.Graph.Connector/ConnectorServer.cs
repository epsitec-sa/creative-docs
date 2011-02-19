//	Copyright © 2009, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace Epsitec.Cresus.Graph
{
	/// <summary>
	/// The <c>Server</c> class is used to implement the <see cref="IConnector"/>
	/// service through a named pipes binding.
	/// </summary>
	public sealed class ConnectorServer : System.IDisposable
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ConnectorServer"/> class.
		/// </summary>
		/// <param name="enableMetadata">If set to <c>true</c>, enables metadata
		/// publishing through a MEX service endpoint.</param>
		public ConnectorServer(bool enableMetadata)
		{
			this.serviceHost = new ServiceHost (typeof (ConnectorService));
			NetNamedPipeBinding binding = new NetNamedPipeBinding (NetNamedPipeSecurityMode.None)
			{
				MaxBufferSize = Connector.MaxMessageSize,
				MaxReceivedMessageSize = Connector.MaxMessageSize,
			};

			var quotas = binding.ReaderQuotas;
			quotas.MaxStringContentLength = Connector.MaxMessageSize;

			if (enableMetadata)
			{
				ServiceMetadataBehavior beheavior = new ServiceMetadataBehavior ();
				this.serviceHost.Description.Behaviors.Add (beheavior);
				this.serviceHost.AddServiceEndpoint (typeof (IMetadataExchange), MetadataExchangeBindings.CreateMexNamedPipeBinding (), string.Concat (ConnectorServer.Address, "/mex"));
			}

			this.serviceHost.AddServiceEndpoint (typeof (IConnector), binding, ConnectorServer.Address);
			this.isReady = true;
		}

		/// <summary>
		/// Gets the address of this endpoint.
		/// </summary>
		/// <value>The address.</value>
		private static string Address
		{
			get
			{
				return ConnectorServer.GetAddress (System.Diagnostics.Process.GetCurrentProcess ().Id);
			}
		}

		
		/// <summary>
		/// Opens the server: the service host will listen to incoming requests
		/// until <see cref="M:Close"/> is called.
		/// </summary>
		public void Open(SendDataCallback sendDataCallback)
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
				
				ConnectorService.DefineSendDataCallback (sendDataCallback);

//-				System.Diagnostics.Debug.WriteLine ("Listening on " + ConnectorServer.Address);
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

		/// <summary>
		/// Gets the address of the endpoint for a given process id.
		/// </summary>
		/// <param name="processId">The process id.</param>
		/// <returns>The address.</returns>
		public static string GetAddress(int processId)
		{
			return string.Concat ("net.pipe://localhost/epsitec/", processId.ToString ("X8"), ".IConnector");
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
