//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Widgets.Behaviors
{
	public abstract class SlimFieldBehavior
	{
		protected SlimFieldBehavior(SlimField host)
		{
			this.host = host;
			this.host.UpdatePreferredSize ();
		}



		protected virtual void OnTextEditionStarting(CancelEventArgs e)
		{
			this.TextEditionStarting.Raise (this, e);
		}

		protected virtual void OnTextEditionStarted()
		{
			this.TextEditionStarted.Raise (this);
		}

		protected virtual void OnTextEditionEnded()
		{
			this.TextEditionEnded.Raise (this);
		}


		public event EventHandler<CancelEventArgs> TextEditionStarting;
		public event EventHandler				TextEditionStarted;
		public event EventHandler				TextEditionEnded;
		
		
		protected readonly SlimField			host;
	}
}
