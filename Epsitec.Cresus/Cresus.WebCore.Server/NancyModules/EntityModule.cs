using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.WebCore.Server.Core;
using Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor;

using Epsitec.Cresus.WebCore.Server.NancyHosting;

using Nancy;

using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.NancyModules
{


	/// <summary>
	/// Used to update value of an existing entity
	/// </summary>
	public class EntityModule : AbstractBusinessContextModule
	{


		public EntityModule(CoreServer coreServer)
			: base (coreServer, "/entity")
		{
			Post["/edit/{id}"] = p => this.Execute (b => this.Edit (b, p));
			Post["/autoCreate"] = _ => this.Execute (b => this.AutoCreateNullEntity (b));
		}


		private Response Edit(BusinessContext businessContext, dynamic parameters)
		{
			var propertyAccessorCache = this.CoreServer.PropertyAccessorCache;

			DynamicDictionary form = Request.Form;
			var propertyAccessorsWithValues = EntityModule.GetPropertyAccessorsWithValues (businessContext, propertyAccessorCache, form)
				.ToList ();

			var entity = Tools.ResolveEntity (businessContext, (string) parameters.id);

			var invalidItems = propertyAccessorsWithValues
				.Where (i => !i.Item1.CheckValue (entity, i.Item2))
				.ToList ();

			if (invalidItems.Any ())
			{
				var errors = invalidItems.ToDictionary (
					i => InvariantConverter.ToString (i.Item1.Id),
					i => (object) "Invalid value"
				);

				return CoreResponse.FormFailure (errors);
			}
			else
			{
				using (businessContext.Bind (entity))
				{
					foreach (var propertyAccessorWithValue in propertyAccessorsWithValues)
					{
						var propertyAccessor = propertyAccessorWithValue.Item1;
						var value = propertyAccessorWithValue.Item2;

						propertyAccessor.SetValue (entity, value);
					}

					businessContext.SaveChanges (LockingPolicy.KeepLock);
					return CoreResponse.FormSuccess ();
				}
			}
		}


		private Response AutoCreateNullEntity(BusinessContext businessContext)
		{
			// NOTE Should we add some locking here in order to ensure that we don't have two
			// clients that auto create an entity at the same time ?

			var autoCreatorCache = this.CoreServer.AutoCreatorCache;

			var entity = Tools.ResolveEntity (businessContext, (string) Request.Form.entityId);
			var autoCreatorId = InvariantConverter.ParseInt ((string) Request.Form.autoCreatorId);
			var autoCreator = autoCreatorCache.Get (autoCreatorId);

			var child = autoCreator.Execute (businessContext, entity);

			businessContext.SaveChanges (LockingPolicy.KeepLock);

			var entityId = Tools.GetEntityId (businessContext, child);

			var content = new Dictionary<string, object> ()
			{
				{ "entityId", entityId },
			};

			return CoreResponse.Success (content);
		}


		private static IEnumerable<Tuple<AbstractPropertyAccessor, object>> GetPropertyAccessorsWithValues(BusinessContext businessContext, PropertyAccessorCache propertyAccessorCache, DynamicDictionary form)
		{
			foreach (var rawPropertyAccessorId in form.GetDynamicMemberNames ())
			{
				var propertyAccessorId = InvariantConverter.ParseInt (rawPropertyAccessorId);
				var propertyAccessor = propertyAccessorCache.Get (propertyAccessorId);
				var value = (DynamicDictionaryValue) form[rawPropertyAccessorId];

				var convertedValue = EntityModule.ConvertValue (businessContext, propertyAccessor, value);

				yield return Tuple.Create (propertyAccessor, convertedValue);
			}
		}


		private static object ConvertValue(BusinessContext businessContext, AbstractPropertyAccessor propertyAccessor, DynamicDictionaryValue value)
		{
			switch (propertyAccessor.PropertyAccessorType)
			{
				case PropertyAccessorType.Boolean:
					return EntityModule.ConvertForString (value, v => (bool) v);

				case PropertyAccessorType.Date:
				case PropertyAccessorType.Enumeration:
				case PropertyAccessorType.Text:
					return EntityModule.ConvertForString (value, v => (string) v);

				case PropertyAccessorType.Decimal:
					return EntityModule.ConvertForString (value, v => (decimal) v);

				case PropertyAccessorType.EntityCollection:
					return EntityModule.ConvertForEntityCollection (businessContext, value);

				case PropertyAccessorType.EntityReference:
					return EntityModule.ConvertForEntityReference (businessContext, value);

				case PropertyAccessorType.Integer:
					return EntityModule.ConvertForString (value, v => (long) v);
				
				default:
					throw new NotImplementedException ();
			}
		}


		private static object ConvertForString(DynamicDictionaryValue value, Func<DynamicDictionaryValue, object> converter)
		{
			if (!value.HasValue || string.IsNullOrEmpty ((string) value))
			{
				return null;
			}

			return converter (value);
		}


		private static object ConvertForEntityCollection(BusinessContext businessContext, DynamicDictionaryValue value)
		{
			var rawValue = (string) value.Value;
			var sequence = rawValue.Split (";");

			return sequence
				.Where (id => !string.IsNullOrEmpty (id))
				.Select (id => Tools.ResolveEntity (businessContext, id))
				.ToList ();
		}


		private static object ConvertForEntityReference(BusinessContext businessContext, DynamicDictionaryValue value)
		{
			var entityId = (string) value;

			AbstractEntity entity = null;

			if (!string.IsNullOrEmpty (entityId))
			{
				entity = Tools.ResolveEntity (businessContext, entityId);
			}

			return entity;
		}


	}


}
