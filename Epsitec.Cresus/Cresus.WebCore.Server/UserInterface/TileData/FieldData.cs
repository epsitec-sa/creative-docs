using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.WebCore.Server.UserInterface.PanelFieldAccessor;
using Epsitec.Cresus.WebCore.Server.UserInterface.Tile;

using System;

using System.Collections.Generic;

using System.Linq;
using System.Linq.Expressions;


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


		public override AbstractEditionTilePart ToAbstractEditionTilePart(AbstractEntity entity, Func<AbstractEntity, string> entityIdGetter, Func<Type, IEnumerable<AbstractEntity>> entitiesGetter, Func<LambdaExpression, AbstractPanelFieldAccessor> panelFieldAccessorGetter)
		{
			return this.ToAbstractField (entity, entityIdGetter, entitiesGetter, panelFieldAccessorGetter);
		}


		public AbstractField ToAbstractField(AbstractEntity entity, Func<AbstractEntity, string> entityIdGetter, Func<Type, IEnumerable<AbstractEntity>> entitiesGetter, Func<LambdaExpression, AbstractPanelFieldAccessor> panelFieldAccessorGetter)
		{
			var panelFieldAccessor = panelFieldAccessorGetter (this.Lambda);

			var entityListPanelFieldAccessor = panelFieldAccessor as EntityListPanelFieldAccessor;
			var entityPanelFieldAccessor = panelFieldAccessor as EntityPanelFieldAccessor;
			var stringPanelFieldAccessor = panelFieldAccessor as StringPanelFieldAccessor;

			if (entityPanelFieldAccessor != null)
			{
				return this.GetEntityField (entity, entityPanelFieldAccessor, entityIdGetter, entitiesGetter);
			}
			else if (entityListPanelFieldAccessor != null)
			{
				return this.GetCollectionField (entity, entitiesGetter, entityIdGetter, entityListPanelFieldAccessor);
			}
			else if (stringPanelFieldAccessor != null)
			{
				if (this.IsTypeSuitableForEnumField (panelFieldAccessor.Type))
				{
					return this.GetEnumField (entity, stringPanelFieldAccessor);
				}
				else if (this.IsTypeSuitableForDateField (panelFieldAccessor.Type))
				{
					return this.GetDateField (entity, stringPanelFieldAccessor);
				}
				else if (this.IsTypeSuitableForTextField (panelFieldAccessor.Type))
				{
					return this.GetTextField (entity, stringPanelFieldAccessor);
				}
			}
			
			throw new NotSupportedException ("Field type is not supported.");
		}


		private AbstractField GetEntityField(AbstractEntity entity, EntityPanelFieldAccessor panelFieldAccessor, Func<AbstractEntity, string> entityIdGetter, Func<Type, IEnumerable<AbstractEntity>> entitiesGetter)
		{
			var target = panelFieldAccessor.GetEntity (entity);

			var entityField = new EntityField ()
			{
				PanelFieldAccessorId = panelFieldAccessor.Id,
				Title = this.Title.ToString (),
				IsReadOnly = this.IsReadOnly,
				Value = entityIdGetter (target),
			};

			var possibleValues = entitiesGetter (panelFieldAccessor.Type)
				.Select (e => Tuple.Create (entityIdGetter (e), e.GetCompactSummary ().ToString ()));

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


		private TextField GetTextField(AbstractEntity entity, StringPanelFieldAccessor panelFieldAccessor)
		{
			return new TextField ()
			{
				PanelFieldAccessorId = panelFieldAccessor.Id,
				Title = this.Title.ToString (),
				IsReadOnly = this.IsReadOnly,
				Value = panelFieldAccessor.GetString (entity),
			};
		}


		private bool IsTypeSuitableForDateField(Type type)
		{
			return type == typeof (Date)
				|| type == typeof (Date?);
		}


		private DateField GetDateField(AbstractEntity entity, StringPanelFieldAccessor panelFieldAccessor)
		{
			return new DateField ()
			{
				PanelFieldAccessorId = panelFieldAccessor.Id,
				Title = this.Title.ToString (),
				IsReadOnly = this.IsReadOnly,
				Value = panelFieldAccessor.GetString (entity),
			};
		}


		private AbstractField GetCollectionField(AbstractEntity entity, Func<Type, IEnumerable<AbstractEntity>> entitiesGetter, Func<AbstractEntity, string> entityIdGetter, EntityListPanelFieldAccessor panelFieldAccessor)
		{
			var collectionField = new CollectionField ()
			{
				PanelFieldAccessorId = panelFieldAccessor.Id,
				Title = this.Title.ToString (),
				IsReadOnly = this.IsReadOnly,
			};

			var possibleValues = entitiesGetter (panelFieldAccessor.CollectionType);
			var checkedValues = panelFieldAccessor.GetCollection (entity).Cast<AbstractEntity> ();

			var checkBoxFields = possibleValues.Select ((v, i) => new CheckBoxField ()
			{
				Checked = checkedValues.Contains (v),
				InputValue = entityIdGetter (v),
				Label = v.GetSummary ().ToString (),
				Index = i
			});

			collectionField.CheckBoxFields.AddRange (checkBoxFields);

			return collectionField;
		}


		private bool IsTypeSuitableForEnumField(Type type)
		{
			var underlyingType = type.GetNullableTypeUnderlyingType ();

			return type.IsEnum || (underlyingType != null && underlyingType.IsEnum);
		}


		private AbstractField GetEnumField(AbstractEntity entity, StringPanelFieldAccessor panelFieldAccessor)
		{
			return new EnumField ()
			{
				PanelFieldAccessorId = panelFieldAccessor.Id,
				Title = this.Title.ToString (),
				IsReadOnly = this.IsReadOnly,
				Value = panelFieldAccessor.GetString (entity),
				TypeName = panelFieldAccessor.Type.AssemblyQualifiedName,
			};
		}


	}


}

