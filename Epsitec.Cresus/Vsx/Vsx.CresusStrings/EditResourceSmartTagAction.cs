using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using Epsitec.Cresus.ResourceManagement;
using Epsitec.Designer.Protocol;
using Epsitec.Tools;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;

namespace Epsitec.Cresus.Strings
{
	internal class EditResourceSmartTagAction : ISmartTagAction
	{
		public EditResourceSmartTagAction(CresusDesigner cresusDesigner, IReadOnlyDictionary<CultureInfo, ResourceItem> multiCultureResourceItem, string displayText)
		{
			this.cresusDesigner = cresusDesigner;
			this.multiCultureResourceItem = multiCultureResourceItem;
			this.displayText = displayText;
		}


		#region ISmartTagAction Members

		public ReadOnlyCollection<SmartTagActionSet> ActionSets
		{
			get
			{
				return null;
			}
		}

		public string DisplayText
		{
			get
			{
				return this.displayText;
			}
		}

		public ImageSource Icon
		{
			get
			{
				return null;
			}
		}

		public bool IsEnabled
		{
			get
			{
				return true;
			}
		}

		public void Invoke()
		{
			this.InvokeAsync ().ConfigureAwait(false);
		}

		#endregion

		private async Task InvokeAsync()
		{
			await this.cresusDesigner.NavigateToStringAsync (this.ResourceItem.Druid.ToString ());
		}

		//private Task CreateInvokeTask(CancellationToken cancellationToken)
		//{
		//	return Task.Run (() =>
		//	{
		//		var binding = new NetNamedPipeBinding (NetNamedPipeSecurityMode.None);
		//		cancellationToken.ThrowIfCancellationRequested ();
		//		var address = new EndpointAddress (Addresses.DesignerAddress);
		//		cancellationToken.ThrowIfCancellationRequested ();
		//		using (var factory = new ChannelFactory<INavigator> (binding, address))
		//		{
		//			cancellationToken.ThrowIfCancellationRequested ();
		//			INavigator proxy = factory.CreateChannel ();
		//			cancellationToken.ThrowIfCancellationRequested ();
		//			using (IClientChannel channel = proxy as IClientChannel)
		//			{
		//				cancellationToken.ThrowIfCancellationRequested ();
		//				proxy.NavigateToString (this.ResourceItem.Druid.ToString ());
		//			}
		//		}
		//	}, cancellationToken);
		//}

		private ResourceItem ResourceItem
		{
			get
			{
				return this.multiCultureResourceItem.First ().Value;
			}
		}

		private readonly CresusDesigner cresusDesigner;
		private readonly string displayText;
		private readonly IReadOnlyDictionary<CultureInfo, ResourceItem> multiCultureResourceItem;
	}
}
