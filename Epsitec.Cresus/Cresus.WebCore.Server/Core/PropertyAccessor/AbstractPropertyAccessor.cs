using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using System;

using System.Linq.Expressions;


namespace Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor
{


	internal abstract class AbstractPropertyAccessor
	{


		public AbstractPropertyAccessor(LambdaExpression lambda, FieldType fieldType, string id)
		{
			this.id = id;
			this.type = lambda.ReturnType;
			this.fieldType = fieldType;
			this.property = EntityInfo.GetStructuredTypeField (lambda);

			this.getter = lambda.Compile ();

			var setterLambda = ExpressionAnalyzer.CreateSetter (lambda);

			if (setterLambda != null)
			{
				this.setter = setterLambda.Compile ();
			}
		}


		public string Id
		{
			get
			{
				return this.id;
			}
		}


		public Type Type
		{
			get
			{
				return this.type;
			}
		}


		public FieldType FieldType
		{
			get
			{
				return this.fieldType;
			}
		}


		public StructuredTypeField Property
		{
			get
			{
				return this.property;
			}
		}


		public virtual object GetValue(AbstractEntity entity)
		{
			return this.getter.DynamicInvoke (entity);
		}


		public virtual void SetValue(AbstractEntity entity, object value)
		{
			this.setter.DynamicInvoke (entity, value);
		}


		public abstract bool CheckValue(object value);


		public static AbstractPropertyAccessor Create(LambdaExpression lambda, string id)
		{
			var fieldType = FieldTypeSelector.GetFieldType (lambda.ReturnType);

			switch (fieldType)
			{
				case FieldType.EntityCollection:
					return new EntityCollectionPropertyAccessor (lambda, id);

				case FieldType.EntityReference:
					return new EntityReferencePropertyAccessor (lambda, id);

				case FieldType.Boolean:
				case FieldType.Date:
				case FieldType.Decimal:
				case FieldType.Enumeration:
				case FieldType.Integer:
				case FieldType.Text:
					return new ValuePropertyAccessor (lambda, fieldType, id);

				default:
					throw new NotImplementedException ();
			}
		}


		private readonly string id;


		private readonly Type type;


		private readonly FieldType fieldType;


		private readonly StructuredTypeField property;


		private readonly Delegate getter;


		private readonly Delegate setter;

	
	}


}
