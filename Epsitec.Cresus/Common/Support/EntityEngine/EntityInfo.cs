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

		/// <summary>
		/// Walks through the entity graph, starting from the specified root, following the
		/// edges defined by the collection of properties.
		/// </summary>
		/// <param name="rootEntity">The example.</param>
		/// <param name="fieldPath">The field path.</param>
		/// <param name="nullNodeAction">What to do when a <c>null</c> node is reached.</param>
		/// <returns>
		/// The leaf entity or <c>null</c>.
		/// </returns>
		public static AbstractEntity WalkEntityGraph(AbstractEntity rootEntity, IEnumerable<PropertyInfo> fieldPath, NullNodeAction nullNodeAction)
		{
			var node = rootEntity;

			foreach (var fieldPropertyInfo in fieldPath)
			{
				if (fieldPropertyInfo.CanRead)
				{
					var child = fieldPropertyInfo.GetValue (node, EntityInfo.emptyIndexArray) as AbstractEntity;

					if (child == null)
					{
						switch (nullNodeAction)
						{
							case NullNodeAction.ReturnNull:
								return null;

							case NullNodeAction.CreateMissing:
								break;

							default:
								throw new System.NotImplementedException (string.Format ("{0} not implemented", nullNodeAction.GetQualifiedName ()));
						}

						child = System.Activator.CreateInstance (fieldPropertyInfo.PropertyType) as AbstractEntity;
						fieldPropertyInfo.SetValue (node, child, EntityInfo.emptyIndexArray);
					}

					node = child;
				}
			}

			return node;
		}


		private static readonly object[]		emptyIndexArray = new object[0];
	}
}