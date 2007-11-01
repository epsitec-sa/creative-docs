using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace Epsitec.Designer.Protocol
{
	public sealed class Server : System.IDisposable
	{
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


		private bool isReady;
		private bool isOpen;
		private bool isDisposed;

		private ServiceHost serviceHost;
	}
}
