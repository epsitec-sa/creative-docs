//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using System.Reflection;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;

namespace Epsitec.Common.Support.EntityEngine
{
	internal static class EntityFieldConverter
	{
		public static PropertyInfo ConvertToProperty(EntityField field)
		{
			PropertyInfo match;

			if (EntityFieldConverter.propertyInfoCache.TryGetValue (field, out match))
			{
				return match;
			}

			var entityType = EntityInfo.GetStructuredType (field.EntityId);
			var systemType = entityType.SystemType;
			var fieldId    = field.FieldId;

			var properties = from property in systemType.GetProperties ()
							 where property.GetCustomAttributes<EntityFieldAttribute> (true).Any (a => a.FieldId == fieldId)
							 select property;

			match = properties.FirstOrDefault ();

			EntityFieldConverter.propertyInfoCache[field] = match;

			return match;
		}

		public static EntityField ConvertToEntityInfo(PropertyInfo propertyInfo)
		{
			EntityField match;

			if (EntityFieldConverter.entityFieldCache.TryGetValue (propertyInfo, out match))
			{
				return match;
			}

			match = new EntityField (propertyInfo);

			EntityFieldConverter.entityFieldCache[propertyInfo] = match;
			
			return match;
		}


		private static readonly ConcurrentDictionary<EntityField, PropertyInfo> propertyInfoCache = new ConcurrentDictionary<EntityField, PropertyInfo> ();
		private static readonly ConcurrentDictionary<PropertyInfo, EntityField> entityFieldCache  = new ConcurrentDictionary<PropertyInfo, EntityField> ();
	}
}
