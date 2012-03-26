//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.BigList.Processors
{
	public sealed class KeyDownProcessor : EventProcessor
	{
		private KeyDownProcessor(IEventProcessorHost host)
		{
			this.host = host;
			this.policy = this.host.GetPolicy<KeyDownProcessorPolicy> ();
		}

		public static bool Attach(IEventProcessorHost host, Message message, Point pos)
		{
			if (host.EventProcessors.OfType<KeyDownProcessor> ().Any ())
			{
				return false;
			}

			var proc = new KeyDownProcessor (host);

			proc.host.Register (proc);
			proc.Process (message, pos);

			return true;
		}


		protected override bool Process(Message message, Point pos)
		{
			return false;
		}


		private readonly IEventProcessorHost	host;
		private readonly KeyDownProcessorPolicy policy;
	}
}
