using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Factories;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Server.UserInterface;

using System;

using System.Collections.Generic;

using System.Linq;
using System.Linq.Expressions;


namespace Epsitec.Cresus.Core.Server.CoreServer
{


	public class CoreSession : CoreApp
	{


		public CoreSession(string id)
		{
			this.id = id;
			this.coreData = this.GetComponent<CoreData> ();
			
			this.panelFieldAccessors = new Dictionary<string, PanelFieldAccessor> ();
			this.panelFieldAccessorsById = new Dictionary<int, PanelFieldAccessor> ();

			Library.UI.Services.SetApplication (this);
		}


		public string Id
		{
			get
			{
				return this.id;
			}
		}


		public CoreData CoreData
		{
			get
			{
				return this.coreData;
			}
		}
		

		public override string ApplicationIdentifier
		{
			get
			{
				return "CoreSession";
			}
		}


		public override string ShortWindowTitle
		{
			get
			{
				return "CoreSession";
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


		public PanelFieldAccessor GetPanelFieldAccessor(LambdaExpression lambda)
		{
			PanelFieldAccessor accessor;
			
			string key = PanelFieldAccessor.GetLambdaFootprint (lambda);

			if (this.panelFieldAccessors.TryGetValue (key, out accessor))
			{
				return accessor;
			}
			else
			{
				int id = this.panelFieldAccessors.Count;

				accessor = CoreSession.CreatePanelFieldAccessor (lambda, id);

				this.panelFieldAccessors[key] = accessor;
				this.panelFieldAccessorsById[id] = accessor;
				
				return accessor;
			}
		}


		public PanelFieldAccessor GetPanelFieldAccessor(int id)
		{
			PanelFieldAccessor accessor;

			if (this.panelFieldAccessorsById.TryGetValue (id, out accessor))
			{
				return accessor;
			}
			else
			{
				return null;
			}
		}


		private static PanelFieldAccessor CreatePanelFieldAccessor(LambdaExpression lambda, int id)
		{
			try
			{
				return new PanelFieldAccessor (lambda, id);
			}
			catch
			{
				return null;
			}
		}


		protected override void Dispose(bool disposing)
		{
			this.DisposeBusinessContext ();
				
			base.Dispose (disposing);
		}


		private readonly string id;


		private readonly CoreData coreData;


		private BusinessContext businessContext;


		private readonly Dictionary<string, PanelFieldAccessor> panelFieldAccessors;
		private readonly Dictionary<int, PanelFieldAccessor> panelFieldAccessorsById;


	}


}
