//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Business.UserManagement;
using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Library;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Features
{
	public class FeatureManager : CoreAppComponent
	{
		public FeatureManager(CoreApp app)
			: base (app)
		{
			this.data        = this.Host.GetComponent<CoreData> ();
			this.userManager = this.data.GetComponent<UserManager> ();
		}

		/// <summary>
		/// Gets the associated business context.
		/// </summary>
		/// <value>The business context.</value>
		public IBusinessContext					BusinessContext
		{
			get
			{
				if (this.businessContext == null)
				{
					this.businessContext = Resolvers.InterfaceImplementationResolver<IBusinessContext>.CreateInstance (this.Host);
					this.businessContext.GlobalLock = GlobalLocks.FeatureManagement;
				}

				return this.businessContext;
			}
		}

		public ProductCustomizationEntity		Customizations
		{
			get
			{
				this.SetupProductCustomizationEntity ();

				return this.activeCustomizations;
			}
		}

		/// <summary>
		/// Saves changes done to the features and customizations.
		/// </summary>
		public void SaveChangesAndDisposeBusinessContext()
		{
			if (this.businessContext != null)
			{
				this.businessContext.SaveChanges ();
				this.DisposeBusinessContext ();
			}
		}

		/// <summary>
		/// Discards all changes done to the features and customizations.
		/// </summary>
		public void DiscardChangesAndDisposeBusinessContext()
		{
			if (this.businessContext != null)
			{
				this.businessContext.Discard ();
				this.DisposeBusinessContext ();
			}
		}

		private void DisposeBusinessContext()
		{
			this.businessContext.Dispose ();
			
			this.businessContext     = null;
			this.activeCustomizations = null;
		}
		
		private void SetupProductCustomizationEntity()
		{
			if (this.activeCustomizations.IsNotNull ())
			{
				return;
			}

			var context = this.BusinessContext;
			var entity  = context.GetAllEntities<ProductCustomizationEntity> ().FirstOrDefault ();

			if (entity == null)
			{
				entity = context.CreateEntity<ProductCustomizationEntity> ();
			}

			this.activeCustomizations = entity;
		}

		#region Factory Class

		private sealed class Factory : Epsitec.Cresus.Core.Factories.DefaultCoreAppComponentFactory<FeatureManager>
		{
			public override bool CanCreate(CoreApp host)
			{
				return host.ContainsComponent<CoreData> ();
			}
		}

		#endregion


		private readonly CoreData				data;
		private readonly UserManager			userManager;

		private IBusinessContext				businessContext;
		private ProductCustomizationEntity		activeCustomizations;
	}
}
