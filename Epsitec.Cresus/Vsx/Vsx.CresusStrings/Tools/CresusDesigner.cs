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
			lock (this.syncRoot)
			{
				this.cts.Cancel ();
				this.DisposeTasks ();
			}
		}

		#endregion


		private async Task<INavigator> NavigatorAsync()
		{
			var factory = await this.EnsureChannelFactoryTask ();
			return await this.EnsureNavigatorTask (factory);
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
			lock (this.syncRoot)
			{
				Interlocked.Exchange (ref this.navigatorTask, null).DisposeResult ().ForgetSafely ();
				Interlocked.Exchange (ref this.channelFactoryTask, null).DisposeResult ().ForgetSafely ();
			}
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

		private CancellationTokenSource EnsureCts()
		{
			lock (this.syncRoot)
			{
				if (this.cts.IsCancellationRequested)
				{
					Interlocked.Exchange (ref this.cts, new CancellationTokenSource ());
				}
				return this.cts;
			}
		}

		private Task<ChannelFactory<INavigator>> EnsureChannelFactoryTask()
		{
			lock (this.syncRoot)
			{
				if (this.channelFactoryTask == null || this.channelFactoryTask.Status == TaskStatus.Canceled || this.channelFactoryTask.Status == TaskStatus.Faulted)
				{
					var cancellationToken = this.EnsureCts ().Token;
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
		}

		private Task<INavigator> EnsureNavigatorTask(ChannelFactory<INavigator> factory)
		{
			lock (this.syncRoot)
			{
				if (this.navigatorTask == null || this.navigatorTask.Status == TaskStatus.Canceled || this.navigatorTask.Status == TaskStatus.Faulted)
				{
					var cancellationToken = this.EnsureCts ().Token;
					this.navigatorTask = Task.Run (() =>
					{
						cancellationToken.ThrowIfCancellationRequested ();
						return factory.CreateChannel ();
					}, cancellationToken);
				}
				return this.navigatorTask;
			}
		}


		private readonly object syncRoot = new object ();

		private CancellationTokenSource cts;
		private Task<ChannelFactory<INavigator>> channelFactoryTask;
		private Task<INavigator> navigatorTask;

	}
}
