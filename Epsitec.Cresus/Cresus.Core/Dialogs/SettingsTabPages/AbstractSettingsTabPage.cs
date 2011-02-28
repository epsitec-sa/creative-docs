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


		public event EventHandler AcceptStateChanging;

		protected void OnAcceptStateChanging()
		{
			if (this.AcceptStateChanging != null)
			{
				this.AcceptStateChanging (this);
			}
		}


		private readonly ISettingsDialog				container;

		private string									errorMessage;
	}
}
