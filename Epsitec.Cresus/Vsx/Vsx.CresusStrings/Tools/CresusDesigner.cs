using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Epsitec.Designer.Protocol;

namespace Epsitec.Tools
{
	[Export (typeof (CresusDesigner))]
	public class CresusDesigner : IDisposable
	{
		public CresusDesigner()
		{
			this.cts = new CancellationTokenSource ();
			this.NavigatorAsync ().ConfigureAwait(false);
		}

		public async Task NavigateToDruidAsync(string bundleName, string druid)
		{
			if (bundleName == "Captions")
			{
				await this.NavigateToCaptionAsync (druid);
			}
			else
			{
				await this.NavigateToStringAsync (druid);
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
			this.cts.Cancel ();
			this.DisposeTasks ();
		}

		#endregion


		private async Task<INavigator> NavigatorAsync()
		{
			var factory = await this.EnsureChannelFactoryTask (this.cts.Token).ConfigureAwait (false);
			return await this.EnsureNavigatorTask (factory, this.cts.Token);
		}

		private async Task NavigateToStringAsync(string id)
		{
			var navigator = await this.NavigatorAsync ();
			await Task.Run (() => this.NavigateToString (navigator, id), this.cts.Token);
		}

		private async Task NavigateToCaptionAsync(string id)
		{
			var navigator = await this.NavigatorAsync ();
			await Task.Run (() => this.NavigateToCaption (navigator, id), this.cts.Token);
		}

		private void DisposeTasks()
		{
			Interlocked.Exchange (ref this.navigatorTask, null).DisposeResult ().ForgetSafely ();
			Interlocked.Exchange (ref this.channelFactoryTask, null).DisposeResult ().ForgetSafely ();
		}

		private void NavigateToString(INavigator navigator, string druid)
		{
			try
			{
				navigator.NavigateToString (druid);
			}
			catch (CommunicationException)
			{
				this.DisposeTasks ();
			}
		}

		private void NavigateToCaption(INavigator navigator, string druid)
		{
			try
			{
				navigator.NavigateToCaption (druid);
			}
			catch (CommunicationException)
			{
				this.DisposeTasks ();
			}
		}

		private Task<ChannelFactory<INavigator>> EnsureChannelFactoryTask(CancellationToken cancellationToken)
		{
			if (this.channelFactoryTask == null || this.channelFactoryTask.Status == TaskStatus.Canceled || this.channelFactoryTask.Status == TaskStatus.Faulted)
			{
				this.channelFactoryTask = Task.Run (() =>
				{
					var binding = new NetNamedPipeBinding (NetNamedPipeSecurityMode.None);
					cancellationToken.ThrowIfCancellationRequested ();
					var address = new EndpointAddress (Addresses.DesignerAddress);
					cancellationToken.ThrowIfCancellationRequested ();
					return new ChannelFactory<INavigator> (binding, address);
				}, cancellationToken);
			}
			return this.channelFactoryTask;
		}

		private Task<INavigator> EnsureNavigatorTask(ChannelFactory<INavigator> factory, CancellationToken cancellationToken)
		{
			if (this.navigatorTask == null || this.navigatorTask.Status == TaskStatus.Canceled || this.navigatorTask.Status == TaskStatus.Faulted)
			{
				this.navigatorTask = Task.Run (() =>
				{
					cancellationToken.ThrowIfCancellationRequested ();
					return factory.CreateChannel();
				}, cancellationToken);
			}
			return this.navigatorTask;
		}

		private CancellationTokenSource cts;
		private Task<ChannelFactory<INavigator>> channelFactoryTask;
		private Task<INavigator> navigatorTask;

	}
}
