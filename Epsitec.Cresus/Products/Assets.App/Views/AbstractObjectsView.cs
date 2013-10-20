//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.NaiveEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public abstract class AbstractObjectsView
	{
		public AbstractObjectsView(DataAccessor accessor, MainToolbar toolbar)
		{
			this.accessor    = accessor;
			this.mainToolbar = toolbar;
		}


		public virtual Guid ObjectGuid
		{
			get
			{
				return Guid.Empty;
			}
			set
			{
			}
		}

		public virtual Timestamp?				Timestamp
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

	
		public virtual void CreateUI(Widget parent)
		{
		}

		public virtual void Update()
		{
		}


		#region Events handler
		protected void OnViewChanged()
		{
			if (this.ViewChanged != null)
			{
				this.ViewChanged (this);
			}
		}

		public delegate void ViewChangedEventHandler(object sender);
		public event ViewChangedEventHandler ViewChanged;
		#endregion


		protected readonly DataAccessor			accessor;
		protected readonly MainToolbar			mainToolbar;
	}
}
