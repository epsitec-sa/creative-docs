using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Types.Converters.Marshalers;

using System;

using System.Collections;
using System.Collections.Generic;

using System.Linq;
using System.Linq.Expressions;

using System.Reflection;


namespace Epsitec.Cresus.WebCore.Server.UserInterface
{


	internal class PanelFieldAccessor
	{


		public PanelFieldAccessor(LambdaExpression lambda, int id)
		{
			var getterLambda = lambda;
			var setterLambda = ExpressionAnalyzer.CreateSetter (getterLambda);

			var lambdaMember = (MemberExpression) lambda.Body;
			var propertyInfo = lambdaMember.Member as PropertyInfo;
			var fieldType    = lambda.ReturnType;
			var sourceType   = lambda.Parameters[0].Type;

			bool nullable    = fieldType.IsNullable ();

			if (nullable)
			{
				fieldType = fieldType.GetNullableTypeUnderlyingType ();
			}

			var factoryType = (nullable ? typeof (NullableMarshallerFactory<,>) : typeof (NonNullableMarshallerFactory<,>)).MakeGenericType (sourceType, fieldType);

			this.id               = id;
			this.fieldType        = fieldType;
			this.isEntityType     = fieldType.IsEntity ();
			this.isCollectionType = fieldType.IsGenericIListOfEntities ();
			this.lambda           = lambda;
			this.getterFunc       = getterLambda == null ? null : getterLambda.Compile ();
			this.setterFunc       = setterLambda == null ? null : setterLambda.Compile ();
			this.marshalerFactory = Activator.CreateInstance (factoryType, this) as MarshallerFactory;
		}


		public int Id
		{
			get
			{
				return this.id;
			}
		}


		public bool IsReadOnly
		{
			get
			{
				return this.setterFunc == null;
			}
		}


		public bool IsEntityType
		{
			get
			{
				return this.isEntityType;
			}
		}


		public bool IsCollectionType
		{
			get
			{
				return this.isCollectionType;
			}
		}


		public Type CollectionItemType
		{
			get
			{
				if (this.IsCollectionType)
				{
					return this.fieldType.GetGenericArguments ()[0];
				}
				else
				{
					return null;
				}
			}
		}


		public IList GetCollection(AbstractEntity entity)
		{
			if (this.IsCollectionType)
			{
				return this.getterFunc.DynamicInvoke (entity) as IList;
			}
			else
			{
				return null;
			}
		}


		public void SetCollection(AbstractEntity entity, IEnumerable<AbstractEntity> collection)
		{
			var source = this.GetCollection (entity);
			
			using (source.SuspendNotifications ())
			{
				source.Clear ();
				source.AddRange (collection);
			}
		}


		public string GetStringValue(AbstractEntity entity)
		{
			return this.marshalerFactory.CreateMarshaler (entity).GetStringValue ();
		}
		

		public void SetStringValue(AbstractEntity entity, string value)
		{
			this.marshalerFactory.CreateMarshaler (entity).SetStringValue (value);
		}


		public void SetEntityValue(AbstractEntity entity, AbstractEntity value)
		{
			if (!this.IsReadOnly)
			{
				this.setterFunc.DynamicInvoke (entity, value);
			}
		}


		private abstract class MarshallerFactory
		{


			public MarshallerFactory(PanelFieldAccessor accessor)
			{
				this.accessor = accessor;
			}


			public abstract Marshaler CreateMarshaler(AbstractEntity entity);


			protected readonly PanelFieldAccessor accessor;


		}


		private sealed class NullableMarshallerFactory<TSource, TField> : MarshallerFactory
			where TField : struct
			where TSource : AbstractEntity
		{


			public NullableMarshallerFactory(PanelFieldAccessor accessor)
				: base (accessor)
			{
			}


			public override Marshaler CreateMarshaler(AbstractEntity entity)
			{
				TSource source = entity as TSource;

				var getter = this.CreateGetter (source);
				var setter = this.CreateSetter (source);
				var lambda = this.accessor.lambda;

				return new NullableMarshaler<TField> (getter, setter, lambda);
			}


			private Func<TField?> CreateGetter(TSource source)
			{
				return () => (TField?) this.accessor.getterFunc.DynamicInvoke (source);
			}


			private Action<TField?> CreateSetter(TSource source)
			{
				return x => this.accessor.setterFunc.DynamicInvoke (source, x);
			}


		}


		private sealed class NonNullableMarshallerFactory<TSource, TField> : MarshallerFactory
			where TSource : AbstractEntity
		{


			public NonNullableMarshallerFactory(PanelFieldAccessor accessor)
				: base (accessor)
			{
			}


			public override Marshaler CreateMarshaler(AbstractEntity entity)
			{
				TSource source = entity as TSource;

				var getter = this.CreateGetter (source);
				var setter = this.CreateSetter (source);
				var lambda = this.accessor.lambda;

				return new NonNullableMarshaler<TField> (getter, setter, lambda);
			}


			private Func<TField> CreateGetter(TSource source)
			{
				return () => (TField) this.accessor.getterFunc.DynamicInvoke (source);
			}

			
			private Action<TField> CreateSetter(TSource source)
			{
				return x => this.accessor.setterFunc.DynamicInvoke (source, x);
			}


		}


		private readonly int id;
		private readonly LambdaExpression lambda;
		private readonly Type fieldType;
		private readonly Delegate getterFunc;
		private readonly Delegate setterFunc;
		private readonly MarshallerFactory marshalerFactory;
		private readonly bool isEntityType;
		private readonly bool isCollectionType;


	}


}
