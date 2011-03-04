//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Dialogs.SettingsTabPages
{
	/// <summary>
	/// Onglet de SettingsDialog.
	/// </summary>
	public abstract class AbstractSettingsTabPage
	{
		public AbstractSettingsTabPage(ISettingsDialog container)
		{
			this.container = container;
		}


		public ISettingsDialog Container
		{
			get
			{
				return this.container;
			}
		}

		public abstract void AcceptChanges();

		public abstract void RejectChanges();

		public virtual void CreateUI(Widget parent)
		{
		}


		public string ErrorMessage
		{
			get
			{
				return this.errorMessage;
			}
			set
			{
				if (this.errorMessage != value)
				{
					this.errorMessage = value;
					this.OnAcceptStateChanging ();
				}
			}
		}


		protected void OnAcceptStateChanging()
		{
			var handler = this.AcceptStateChanging;

			if (handler != null)
			{
				handler (this);
			}
		}

		
		public event EventHandler						AcceptStateChanging;
		
		private readonly ISettingsDialog				container;
		private string									errorMessage;
	}
}
