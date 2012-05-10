//	Copyright © 2010-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Epsitec.Common.Support.EntityEngine
{
	/// <summary>
	/// The <c>EntityInfo</c> class is used to retrieve runtime information
	/// about an entity, such as its DRUID.
	/// </summary>
	public static class EntityInfo
	{
		public static Druid GetTypeId(System.Type systemType)
		{
			return EntityClassFactory.GetEntityId (systemType);
		}

		public static IEnumerable<Druid> GetAllTypeIds()
		{
			return EntityClassFactory.GetAllEntityIds ();
		}

		public static System.Type GetType(Druid entityId)
		{
			return EntityClassFactory.FindEntityType (entityId);
		}

		public static StructuredType GetStructuredType(Druid entityId)
		{
			if (entityId.IsEmpty)
			{
				return null;
			}
			else
			{
				return SafeResourceResolver.Instance.GetStructuredType (entityId);
			}
		}

		public static StructuredType GetStructuredType(System.Type systemType)
		{
			return EntityInfo.GetStructuredType (EntityClassFactory.GetEntityId (systemType));
		}

		public static INamedType GetFieldType(Expression properyLambdaExpression)
		{
			var field = EntityInfo.GetStructuredTypeField (properyLambdaExpression);

			if (field == null)
			{
				return null;
			}
			else
			{
				return field.Type;
			}
		}

		/// <summary>
		/// Gets the field DRUID based on the expression.
		/// </summary>
		/// <param name="properyLambdaExpression">The lambda expression pointing to the property.</param>
		/// <returns>The DRUID of the field, or <c>Druid.Empty</c> if it cannot be resolved.</returns>
		public static Druid GetFieldDruid(Expression properyLambdaExpression)
		{
			var field = EntityInfo.GetStructuredTypeField (properyLambdaExpression);

			if (field == null)
			{
				return Druid.Empty;
			}
			else
			{
				return field.CaptionId;
			}
		}

		public static string GetEntityName(System.Type systemType)
		{
			if (systemType == null)
			{
				return null;
			}
			else if (systemType.IsEntity ())
			{
				return systemType.Name.StripSuffix ("Entity");
			}
			else
			{
				return null;
			}
		}

		public static StructuredTypeField GetStructuredTypeField(Expression propertyLambdaExpression)
		{
			return EntityInfo.GetStructuredTypeField (ExpressionAnalyzer.GetLambdaPropertyInfo (propertyLambdaExpression));
		}

		public static StructuredTypeField GetStructuredTypeField(PropertyInfo propertyInfo)
		{
			if (propertyInfo == null)
			{
				return null;
			}

			var structuredType = EntityInfo.GetStructuredType (propertyInfo.DeclaringType);
			var fieldAttribute = propertyInfo.GetCustomAttributes (true).OfType<EntityFieldAttribute> ().FirstOrDefault ();

			if ((structuredType == null) ||
				(fieldAttribute == null) ||
				(fieldAttribute.FieldId == null))
			{
				return null;
			}
			else
			{
				return structuredType.GetField (fieldAttribute.FieldId);
			}
		}

		public static Caption GetFieldCaption(Expression propertyLambdaExpression)
		{
			return EntityInfo.GetFieldCaption (ExpressionAnalyzer.GetLambdaPropertyInfo (propertyLambdaExpression));
		}

		public static Caption GetFieldCaption(PropertyInfo propertyInfo)
		{
			if (propertyInfo == null)
			{
				return null;
			}

			var fieldAttribute = propertyInfo.GetCustomAttributes (true).OfType<EntityFieldAttribute> ().FirstOrDefault ();

			if ((fieldAttribute == null) &&
				(propertyInfo.Name.EndsWith ("ForEdition")))
			{
				return EntityInfo.GetFieldCaption (propertyInfo.DeclaringType.GetProperty (propertyInfo.Name.StripSuffix ("ForEdition")));
			}

			if ((fieldAttribute == null) ||
				(fieldAttribute.FieldId == null))
			{
				return null;
			}

			Druid id = Druid.Parse (fieldAttribute.FieldId);

			return SafeResourceResolver.Instance.GetCaption (id);
		}

	}
}
