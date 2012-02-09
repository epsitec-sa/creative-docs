//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Marc BETTEX

using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Factories;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Library.UI;

using Epsitec.Cresus.WebCore.Server.UserInterface;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.WebCore.Server.CoreServer
{
	public class CoreSession : CoreApp
	{
		public CoreSession(string id)
		{
			this.id = id;
			this.coreData = this.GetComponent<CoreData> ();
			this.panelFieldAccessorCache = new PanelFieldAccessorCache ();

			Services.SetApplication (this);
		}


		public string							Id
		{
			get
			{
				return this.id;
			}
		}

		public CoreData							CoreData
		{
			get
			{
				return this.coreData;
			}
		}

		public override string					ApplicationIdentifier
		{
			get
			{
				return "CoreSession";
			}
		}

		public override string					ShortWindowTitle
		{
			get
			{
				return "CoreSession";
			}
		}

		internal PanelFieldAccessorCache		PanelFieldAccessorCache
		{
			get
			{
				return this.panelFieldAccessorCache;
			}
		}

		
		public BusinessContext GetBusinessContext()
		{
			if (this.businessContext == null)
			{
				this.businessContext = new BusinessContext (this.coreData);
			}

			return this.businessContext;
		}

		public void DisposeBusinessContext()
		{
			if (this.businessContext != null)
			{
				this.businessContext.Dispose ();
				
				this.businessContext = null;
			}
		}


		protected override void Dispose(bool disposing)
		{
			this.DisposeBusinessContext ();
				
			base.Dispose (disposing);
		}


		private readonly string					id;
		private readonly CoreData				coreData;
		private readonly PanelFieldAccessorCache panelFieldAccessorCache;
		private BusinessContext					businessContext;
	}
}
