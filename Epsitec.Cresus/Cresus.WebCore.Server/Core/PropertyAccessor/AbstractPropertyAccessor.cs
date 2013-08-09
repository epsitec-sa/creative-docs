using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Types;

using System;

using System.Linq.Expressions;


namespace Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor
{


	/// <summary>
	/// This class is the base class of all property accessors. The property accessors are used to
	/// read, write, and check values of entity properties. Each subclass  manages different kind
	/// of properties (value, reference and collection properties).
	/// </summary>
	/// <remarks>
	/// A property accessor is fully defined by the lambda expression that is used to create it. It
	/// also contains an id that can be given to the javascript client and the used to resolve the
	/// property accessor back.
	/// The GetValue(...), SetValue(...) and CheckValue(...) methods are thread safe.
	/// </remarks>
	internal abstract class AbstractPropertyAccessor
	{


		public AbstractPropertyAccessor(LambdaExpression lambda, FieldType fieldType, string id)
		{
			this.id = id;
			this.lambda = lambda;
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


		public LambdaExpression Lambda
		{
			get
			{
				return this.lambda;
			}
		}


		public Type Type
		{
			get
			{
				return this.lambda.ReturnType;
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


		public abstract IValidationResult CheckValue(object value);


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
				case FieldType.Time:
					return new ValuePropertyAccessor (lambda, fieldType, id);

				default:
					throw new NotImplementedException ();
			}
		}


		private readonly string id;


		private readonly LambdaExpression lambda;


		private readonly FieldType fieldType;


		private readonly StructuredTypeField property;


		private readonly Delegate getter;


		private readonly Delegate setter;


	}


}
