//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Types.Converters.Marshalers;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Core.Server
{
	public class PanelFieldAccessor
	{
		public PanelFieldAccessor(LambdaExpression lambda, int id)
		{
			var getterLambda = lambda;
			var setterLambda = ExpressionAnalyzer.CreateSetter (getterLambda);

			var lambdaMember = (MemberExpression) lambda.Body;
			var propertyInfo = lambdaMember.Member as System.Reflection.PropertyInfo;
			var typeField    = EntityInfo.GetStructuredTypeField (propertyInfo);
			var fieldType    = lambda.ReturnType;
			var sourceType   = lambda.Parameters[0].Type;

			bool nullable    = fieldType.IsNullable ();

			if (nullable)
			{
				fieldType = fieldType.GetNullableTypeUnderlyingType ();
			}

			var factoryType = (nullable ? typeof (NullableFactory<,>) : typeof (NonNullableFactory<,>)).MakeGenericType (sourceType, fieldType);

			this.id               = id;
			this.lambda           = lambda;
			this.fieldType        = fieldType;
			this.getterFunc       = getterLambda == null ? null : getterLambda.Compile ();
			this.setterFunc       = setterLambda == null ? null : setterLambda.Compile ();
			this.marshalerFactory = System.Activator.CreateInstance (factoryType, this) as DynamicFactory;
			this.isEntityType     = fieldType.IsEntity ();
			this.isCollectionType = fieldType.IsGenericIListOfEntities ();
		}


		public int Id
		{
			get
			{
				return this.id;
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

		public System.Type CollectionItemType
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

		public System.Collections.IList GetCollection(AbstractEntity entity)
		{
			if (this.IsCollectionType)
			{
				return this.getterFunc.DynamicInvoke (entity) as System.Collections.IList;
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

		public bool CanWrite
		{
			get
			{
				return this.setterFunc != null;
			}
		}
		
		public void SetStringValue(AbstractEntity entity, string value)
		{
			var marshaler = this.marshalerFactory.CreateMarshaler (entity);
			marshaler.SetStringValue (value);
		}

		public void SetEntityValue(AbstractEntity entity, AbstractEntity value)
		{
			if (this.setterFunc != null)
			{
				this.setterFunc.DynamicInvoke (entity, value);
			}
		}

		public string GetStringValue(AbstractEntity entity)
		{
			var marshaler = this.marshalerFactory.CreateMarshaler (entity);
			return marshaler.GetStringValue ();
		}


		public static string GetLambdaFootprint(LambdaExpression lambda)
		{
			return string.Concat (lambda.ToString (), "/",
								  lambda.ReturnType.FullName, "/",
								  lambda.Parameters[0].Type.FullName);
		}

		private abstract class DynamicFactory
		{
			public DynamicFactory(PanelFieldAccessor accessor)
			{
				this.accessor = accessor;
			}

			public abstract Marshaler CreateMarshaler(AbstractEntity entity);

			protected readonly PanelFieldAccessor accessor;
		}

		private sealed class NullableFactory<TSource, TField> : DynamicFactory
			where TField : struct
			where TSource : AbstractEntity
		{
			public NullableFactory(PanelFieldAccessor accessor)
				: base (accessor)
			{
			}

			private System.Func<TField?> CreateGetter(TSource source)
			{
				return () => (TField?) this.accessor.getterFunc.DynamicInvoke (source);
			}

			private System.Action<TField?> CreateSetter(TSource source)
			{
				return x => this.accessor.setterFunc.DynamicInvoke (source, x);
			}

			public override Marshaler CreateMarshaler(AbstractEntity entity)
			{
				TSource source = entity as TSource;
				return new NullableMarshaler<TField> (this.CreateGetter (source), this.CreateSetter (source), this.accessor.lambda);
			}
		}

		private sealed class NonNullableFactory<TSource, TField> : DynamicFactory
			where TSource : AbstractEntity
		{
			public NonNullableFactory(PanelFieldAccessor accessor)
				: base (accessor)
			{
			}

			private System.Func<TField> CreateGetter(TSource source)
			{
				return () => (TField) this.accessor.getterFunc.DynamicInvoke (source);
			}

			private System.Action<TField> CreateSetter(TSource source)
			{
				return x => this.accessor.setterFunc.DynamicInvoke (source, x);
			}

			public override Marshaler CreateMarshaler(AbstractEntity entity)
			{
				TSource source = entity as TSource;
				return new NonNullableMarshaler<TField> (this.CreateGetter (source), this.CreateSetter (source), this.accessor.lambda);
			}
		}

		private readonly int					id;
		private readonly LambdaExpression		lambda;
		private readonly System.Type			fieldType;
		private readonly System.Delegate		getterFunc;
		private readonly System.Delegate		setterFunc;
		private readonly DynamicFactory			marshalerFactory;
		private readonly bool					isEntityType;
		private readonly bool					isCollectionType;
	}
}
