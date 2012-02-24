using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using System;

using System.Collections;
using System.Collections.Generic;

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

			var fieldType = lambda.ReturnType;

			bool nullable = fieldType.IsNullable ();

			if (nullable)
			{
				fieldType = fieldType.GetNullableTypeUnderlyingType ();
			}

			var factoryType = (nullable ? typeof (NullableMarshallerFactory<>) : typeof (NonNullableMarshallerFactory<>)).MakeGenericType (fieldType);

			this.id = id;
			this.fieldType = fieldType;
			this.isEntityType = fieldType.IsEntity ();
			this.isCollectionType = fieldType.IsGenericIListOfEntities ();
			this.getterFunc = getterLambda == null ? null : getterLambda.Compile ();
			this.setterFunc = setterLambda == null ? null : setterLambda.Compile ();
			this.marshalerFactory = Activator.CreateInstance (factoryType, lambda, this.getterFunc, this.setterFunc) as AbstractMarshallerFactory;
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


		private readonly int id;
		private readonly Type fieldType;
		private readonly Delegate getterFunc;
		private readonly Delegate setterFunc;
		private readonly AbstractMarshallerFactory marshalerFactory;
		private readonly bool isEntityType;
		private readonly bool isCollectionType;


	}


}
