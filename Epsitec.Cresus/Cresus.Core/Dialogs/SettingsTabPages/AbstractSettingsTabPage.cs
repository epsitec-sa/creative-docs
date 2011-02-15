//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Debug;
using Epsitec.Common.Dialogs;
using Epsitec.Common.Drawing;
using Epsitec.Common.IO;
using Epsitec.Common.Printing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Dialogs.SettingsTabPages
{
	/// <summary>
	/// Onglet de SettingsDialog.
	/// </summary>
	public abstract class AbstractSettingsTabPage
	{
		public AbstractSettingsTabPage(CoreApplication application)
		{
			this.application = application;
		}


		public virtual void AcceptChangings()
		{
		}

		public virtual void RejectChangings()
		{
		}

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


		protected readonly CoreApplication				application;
		private string									errorMessage;
	}
}
