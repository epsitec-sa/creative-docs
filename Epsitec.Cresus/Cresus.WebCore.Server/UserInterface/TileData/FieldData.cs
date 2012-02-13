using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.WebCore.Server.UserInterface.Tile;

using System;

using System.Linq;
using System.Linq.Expressions;
using Epsitec.Cresus.Core.Business;
using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.UserInterface.TileData
{


	internal sealed class FieldData : AbstractEditionTilePartData
	{


		// TODO Add PickFromCollection, ReadOnly, Width, Height


		public FormattedText Title
		{
			get;
			set;
		}


		public LambdaExpression Lambda
		{
			get;
			set;
		}


		public bool IsReadOnly
		{
			get;
			set;
		}


		public override AbstractEditionTilePart ToAbstractEditionTilePart(AbstractEntity entity, Func<AbstractEntity, string> entityIdGetter, Func<Type, IEnumerable<AbstractEntity>> entitiesGetter, Func<LambdaExpression, PanelFieldAccessor> panelFieldAccessorGetter)
		{
			return this.ToAbstractField (entity, entityIdGetter, entitiesGetter, panelFieldAccessorGetter);
		}


		public AbstractField ToAbstractField(AbstractEntity entity, Func<AbstractEntity, string> entityIdGetter, Func<Type, IEnumerable<AbstractEntity>> entitiesGetter, Func<LambdaExpression, PanelFieldAccessor> panelFieldAccessorGetter)
		{
			var fieldTargetGetter = this.Lambda.Compile ();
			var fieldTarget = fieldTargetGetter.DynamicInvoke (entity);

			var fieldType = this.Lambda.ReturnType;
			var panelFieldAccessor = panelFieldAccessorGetter (this.Lambda);
			var panelFieldAccessorId = panelFieldAccessor == null
				? "-1"
				: InvariantConverter.ToString (panelFieldAccessor.Id);

			var fieldName = EntityInfo.GetFieldCaption (this.Lambda).Id.ToString ().Trim ('[', ']');
			var lambdaFieldName = Tools.GetLambdaFieldName (fieldName);

			var title = this.Title.ToString ();

			if (panelFieldAccessor.IsEntityType)
			{
				return this.GetEntityField (entityIdGetter, entitiesGetter, fieldTarget, fieldType, panelFieldAccessorId, fieldName, lambdaFieldName, title);
			}
			else if (this.IsTypeSuitableForTextField (fieldType))
			{
				return this.GetTextField (entity, panelFieldAccessor, panelFieldAccessorId, fieldName, lambdaFieldName, title);
			}
			else if (this.IsTypeSuitableForDateField (fieldType))
			{
				return this.GetDateField (entity, panelFieldAccessor, panelFieldAccessorId, fieldName, lambdaFieldName, title);
			}
			else if (panelFieldAccessor.IsCollectionType)
			{
				return this.GetCollectionField (entity, entitiesGetter, entityIdGetter, panelFieldAccessor, panelFieldAccessorId, fieldName, lambdaFieldName, title);
			}
			else if (this.IsTypeSuitableForEnumField (fieldType))
			{
				return this.GetEnumField (fieldTarget, fieldType, panelFieldAccessorId, fieldName, lambdaFieldName, title);
			}

			throw new NotImplementedException ("Field type is not supported.");
		}


		private AbstractField GetEntityField(Func<AbstractEntity, string> entityIdGetter, Func<Type, IEnumerable<AbstractEntity>> entitiesGetter, object fieldTarget, Type fieldType, string panelFieldAccessorId, string fieldName, string lambdaFieldName, string title)
		{
			var entityField = new EntityField ()
			{
				FieldName = fieldName,
				LambdaFieldName = lambdaFieldName,
				PanelFieldAccessorId = panelFieldAccessorId,
				Title = title,
				Value = entityIdGetter ((AbstractEntity) fieldTarget),

			};

			var possibleValues = entitiesGetter (fieldType)
				.Select (e => Tuple.Create (entityIdGetter (e), e.GetSummary ().ToString ()));

			entityField.PossibleValues.AddRange (possibleValues);

			return entityField;
		}


		private bool IsTypeSuitableForTextField(Type type)
		{
			return type == typeof (string)
				|| type == typeof (FormattedText)
				|| type == typeof (long)
				|| type == typeof (long?)
				|| type == typeof (decimal)
				|| type == typeof (decimal?)
				|| type == typeof (int)
				|| type == typeof (int?)
				|| type == typeof (bool)
				|| type == typeof (bool?);
		}


		private TextField GetTextField(AbstractEntity entity, PanelFieldAccessor panelFieldAccessor, string panelFieldAccessorId, string fieldName, string lambdaFieldName, string title)
		{
			return new TextField ()
			{
				FieldName = fieldName,
				LambdaFieldName = lambdaFieldName,
				PanelFieldAccessorId = panelFieldAccessorId,
				Title = title,
				Value = panelFieldAccessor.GetStringValue (entity),
			};
		}


		private bool IsTypeSuitableForDateField(Type type)
		{
			return type == typeof (Date)
				|| type == typeof (Date?);
		}


		private DateField GetDateField(AbstractEntity entity, PanelFieldAccessor panelFieldAccessor, string panelFieldAccessorId, string fieldName, string lambdaFieldName, string title)
		{
			return new DateField ()
			{
				FieldName = fieldName,
				LambdaFieldName = lambdaFieldName,
				PanelFieldAccessorId = panelFieldAccessorId,
				Title = title,
				Value = panelFieldAccessor.GetStringValue (entity),
			};
		}


		private AbstractField GetCollectionField(AbstractEntity entity, Func<Type, IEnumerable<AbstractEntity>> entitiesGetter, Func<AbstractEntity, string> entityIdGetter, PanelFieldAccessor panelFieldAccessor, string panelFieldAccessorId, string fieldName, string lambdaFieldName, string title)
		{
			var collectionField = new CollectionField ()
			{
				FieldName = fieldName,
				LambdaFieldName = lambdaFieldName,
				PanelFieldAccessorId = panelFieldAccessorId,
				Title = title,
			};

			var possibleValues = entitiesGetter (panelFieldAccessor.CollectionItemType);
			var checkedValues = panelFieldAccessor.GetCollection (entity).Cast<AbstractEntity> ();

			var checkBoxFields = possibleValues.Select ((v, i) => new CheckBoxField ()
			{
				Checked = checkedValues.Contains (v),
				InputValue = entityIdGetter (v),
				Label = v.GetSummary ().ToString (),
				Name = fieldName + "[" + InvariantConverter.ToString (i) + "]",
			});

			collectionField.CheckBoxFields.AddRange (checkBoxFields);

			return collectionField;
		}


		private bool IsTypeSuitableForEnumField(Type type)
		{
			var underlyingType = type.GetNullableTypeUnderlyingType ();

			return type.IsEnum || (underlyingType != null && underlyingType.IsEnum);
		}


		private AbstractField GetEnumField(object fieldTarget, Type fieldType, string panelFieldAccessorId, string fieldName, string lambdaFieldName, string title)
		{
			return new EnumField ()
			{
				FieldName = fieldName,
				LambdaFieldName = lambdaFieldName,
				PanelFieldAccessorId = panelFieldAccessorId,
				Title = title,
				Value = fieldTarget.ToString (),
				TypeName = fieldType.AssemblyQualifiedName,
			};
		}


	}


}

