using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.WebCore.Server.Layout.TileData;

using System;

using System.Linq.Expressions;


namespace Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor
{


	internal abstract class AbstractPropertyAccessor
	{


		public AbstractPropertyAccessor(LambdaExpression lambda, string id)
		{
			this.id = id;
			this.type = lambda.ReturnType;
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


		public abstract PropertyAccessorType PropertyAccessorType
		{
			get;
		}


		public StructuredTypeField Property
		{
			get
			{
				return this.property;
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


		public static AbstractPropertyAccessor Create(LambdaExpression lambda, string id)
		{
			var type = lambda.ReturnType;

			if (AbstractPropertyAccessor.IsEntityCollectionType (type))
			{
				return new EntityCollectionPropertyAccessor (lambda, id);
			}
			else if (AbstractPropertyAccessor.IsEntityReferenceType (type))
			{
				return new EntityReferencePropertyAccessor (lambda, id);
			}
			else if (AbstractPropertyAccessor.IsEnumerationType (type))
			{
				return new EnumerationPropertyAccessor (lambda, id);
			}
			else if (AbstractPropertyAccessor.IsDateType (type))
			{
				return new DatePropertyAccessor (lambda, id);
			}
			else if (AbstractPropertyAccessor.IsBooleanType (type))
			{
				return new BooleanPropertyAccessor (lambda, id);
			}
			else if (AbstractPropertyAccessor.IsIntegerType (type))
			{
				return new IntegerPropertyAccessor (lambda, id);
			}
			else if (AbstractPropertyAccessor.IsDecimalType (type))
			{
				return new DecimalPropertyAccessor (lambda, id);
			}
			else if (AbstractPropertyAccessor.IsTextType (type))
			{
				return new TextPropertyAccessor (lambda, id);
			}
			else
			{
				throw new NotSupportedException ();
			}
		}
		

		private static bool IsEntityCollectionType(Type type)
		{
			return type.IsGenericIListOfEntities ();
		}


		private static bool IsEntityReferenceType(Type type)
		{
			return type.IsEntity ();
		}


		private static bool IsDateType(Type type)
		{
			return type == typeof (Date)
		        || type == typeof (Date?);
		}


		private static bool IsBooleanType(Type type)
		{
			return type == typeof (bool)
		        || type == typeof (bool?);
		}


		private static bool IsEnumerationType(Type type)
		{
			var underlyingType = type.GetNullableTypeUnderlyingType ();

			return type.IsEnum || (underlyingType != null && underlyingType.IsEnum);
		}


		private static bool IsIntegerType(Type type)
		{
			return type == typeof (short)
		        || type == typeof (short?)
		        || type == typeof (int)
		        || type == typeof (int?)
		        || type == typeof (long)
		        || type == typeof (long?);
		}


		private static bool IsDecimalType(Type type)
		{
			return type == typeof (decimal)
		        || type == typeof (decimal?);
		}


		private static bool IsTextType(Type type)
		{
			return type == typeof (string)
		        || type == typeof (FormattedText)
		        || type == typeof (FormattedText?);
		}



		private readonly string id;


		private readonly Type type;


		private readonly StructuredTypeField property;


		private readonly Delegate getter;


		private readonly Delegate setter;

	
	}


}
