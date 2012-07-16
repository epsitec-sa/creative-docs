using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using System;

using System.Linq.Expressions;


namespace Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor
{


	internal abstract class AbstractPropertyAccessor
	{


		public AbstractPropertyAccessor(LambdaExpression lambda, int id)
		{
			this.id = id;
			this.type = lambda.ReturnType;
			this.fieldType = FieldTypeSelector.GetFieldType (this.type);
			this.property = EntityInfo.GetStructuredTypeField (lambda);

			this.getter = lambda.Compile ();

			var setterLambda = ExpressionAnalyzer.CreateSetter (lambda);

			if (setterLambda != null)
			{
				this.setter = setterLambda.Compile ();
			}
		}


		public int Id
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


		public StructuredTypeField Property
		{
			get
			{
				return this.property;
			}
		}


		public FieldType FieldType
		{
			get
			{
				return this.fieldType;
			}
		}


		protected Delegate Getter
		{
			get
			{
				return this.getter;
			}
		}


		protected Delegate Setter
		{
			get
			{
				return this.setter;
			}
		}


		public abstract void SetValue(AbstractEntity entity, object value);


		public abstract bool CheckValue(AbstractEntity entity, object value);


		public static AbstractPropertyAccessor Create(LambdaExpression lambda, int id)
		{
			var type = lambda.ReturnType;

			if (type.IsEntity ())
			{
				return new EntityReferencePropertyAccessor (lambda, id);
			}
			else if (type.IsGenericIListOfEntities ())
			{
				return new EntityCollectionPropertyAccessor (lambda, id);
			}
			else
			{
				return new TextPropertyAccessor (lambda, id);
			}
		}


		private readonly int id;


		private readonly Type type;


		private readonly StructuredTypeField property;


		private readonly FieldType fieldType;


		private readonly Delegate getter;


		private readonly Delegate setter;

	
	}


}
