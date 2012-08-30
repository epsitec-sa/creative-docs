//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Business.UserManagement;
using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Extensions;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Library.Settings;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Core.Features
{
	public class FeatureManager : CoreAppComponent, System.IDisposable
	{
		public FeatureManager(CoreApp app)
			: base (app)
		{
			this.data            = this.Host.GetComponent<CoreData> ();
			this.userManager     = this.data.GetComponent<UserManager> ();
			this.productFeatures = new List<ProductFeatureEntity> ();
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
					this.businessContext = InterfaceImplementationResolver<IBusinessContext>.CreateInstance (this.data);
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

		public ProductSettingsEntity			ProductSettings
		{
			get
			{
				this.SetupProductSettingsEntity ();
				return this.activeProductSettings;
			}
		}


		public void EnableGodMode()
		{
			this.overrideCommandEnable = true;
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

		public TileFieldEditionSettings GetFieldEditionSettings(Druid entityId, Druid fieldId, UserSummary user = null)
		{
			user = this.GetUserSummary (user);

			var entitySettings = this.GetEntityMergedSettings (entityId, user);

			if (entitySettings.IsEmpty)
			{
				return new TileFieldEditionSettings (TileVisibilityMode.Visible, TileEditionMode.ReadWrite);
			}
			else
			{
				var fieldSettings = entitySettings[fieldId];
				return new TileFieldEditionSettings (fieldSettings.FieldVisibilityMode.Simplify (), fieldSettings.FieldEditionMode.Simplify ());
			}
		}

		public bool IsCommandEnabled(Druid commandId, UserSummary user = null)
		{
			if (this.overrideCommandEnable)
			{
				return true;
			}

			user = this.GetUserSummary (user);

			var customCommandSettings  = this.Customizations.Settings.UserCommandSetSettings;
			var featureCommandSettings = this.ProductSettings.LicensedFeatures.Select (x => x.EnabledSettings.UserCommandSetSettings);

			var customDisabledCommands  = customCommandSettings.GetDisabledCommands (commandId);
			var featureDisabledCommands = featureCommandSettings.SelectMany (x => x.GetDisabledCommands (commandId));
			var featureEnabledCommands  = featureCommandSettings.SelectMany (x => x.GetEnabledCommands (commandId));

			bool anyDisabled = customDisabledCommands.Concat (featureDisabledCommands).Where (x => user.Matches (x)).Any ();

			if (anyDisabled)
			{
				return false;
			}

			bool anyEnabled = featureEnabledCommands.Where (x => user.Matches (x)).Any ();

			return anyEnabled;
		}

		#region IDisposable Members

		public void Dispose()
		{
			this.DisposeBusinessContext ();
		}

		#endregion

		
		private TileEntityMergedSettings GetEntityMergedSettings(Druid entity, UserSummary user)
		{
			string entityId = entity.ToString ();

			var customEditionSettings     = this.GetAllSoftwareUISettings ().SelectMany (x => x.EntityEditionSettings).Where (x => x.EntityId == entityId);
			var tileEntityEditionSettings = customEditionSettings.Select (x => x.DisplaySettings);
			var fieldEditionSettings      = tileEntityEditionSettings.SelectMany (x => x.GetAllFieldSettings (s => user.Matches (s)));

			var result = new TileEntityMergedSettings (entity);

			foreach (var tuple in fieldEditionSettings)
			{
				result.Accumulate (tuple.Item1, tuple.Item2);
			}

			return result;
		}

		private UserSummary GetUserSummary(UserSummary user)
		{
			return user ?? this.userManager.GetUserSummary ();
		}
		
		private static Druid GetEntityId<T>()
			where T : AbstractEntity, new ()
		{
			return EntityInfo<T>.GetTypeId ();
		}
		
		private IEnumerable<SoftwareUISettingsEntity> GetAllSoftwareUISettings()
		{
			yield return this.Customizations.Settings;

			foreach (var feature in this.ProductSettings.LicensedFeatures)
			{
				yield return feature.EnabledSettings;
			}

		}
		
		private void DisposeBusinessContext()
		{
			if (this.businessContext != null)
			{
				this.businessContext.Dispose ();
				this.businessContext = null;
			}
			
			this.activeCustomizations = null;
		}

		private void SetupProductFreatureList()
		{
			//	TODO: ...
		}
		
		private void SetupProductCustomizationEntity()
		{
			if (this.activeCustomizations.IsNotNull ())
			{
				return;
			}

			var now = System.DateTime.UtcNow;

			var context = this.BusinessContext;
			var entity  = context.GetAllEntities<ProductCustomizationEntity> ().Where (x => now.InRange (x)).FirstOrDefault ();

			if (entity == null)
			{
				entity = context.CreateEntity<ProductCustomizationEntity> ();
				
				context.SaveChanges ();
				context.ReleaseLock ();
			}

			this.activeCustomizations = entity;
		}

		private void SetupProductSettingsEntity()
		{
			if (this.activeProductSettings.IsNotNull ())
			{
				return;
			}

			var now = System.DateTime.UtcNow;

			var context = this.BusinessContext;
			var entity  = context.GetAllEntities<ProductSettingsEntity> ().Where (x => now.InRange (x)).FirstOrDefault ();

			this.ValidateProductSettings (entity);

			if (entity == null)
			{
				//	TODO: protest if there is no valid product settings ! for now, we create an empty product settings entity

				entity = context.CreateEntity<ProductSettingsEntity> ();
				
				context.SaveChanges ();
				context.ReleaseLock ();
			}

			this.activeProductSettings = entity;
		}

		private bool ValidateProductSettings(ProductSettingsEntity entity)
		{
			//	TODO: check validity of entity using the hash
			return true;
		}

		private bool ValidateProductFeature(ProductFeatureEntity entity)
		{
			//	TODO: check validity of entity using the hash
			return true;
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

		private bool							overrideCommandEnable;

		private IBusinessContext				businessContext;
		private ProductCustomizationEntity		activeCustomizations;
		private ProductSettingsEntity			activeProductSettings;

		private List<ProductFeatureEntity>		productFeatures;
	}
}
