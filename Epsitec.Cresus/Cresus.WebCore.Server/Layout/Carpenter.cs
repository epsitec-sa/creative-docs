using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Bricks;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Controllers;

using Epsitec.Cresus.WebCore.Server.Core;
using Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor;
using Epsitec.Cresus.WebCore.Server.NancyModules;

using System;

using System.Collections.Generic;

using System.Linq;
using System.Linq.Expressions;


namespace Epsitec.Cresus.WebCore.Server.Layout
{


	/// <summary>
	/// The goal of the Carpenter class is to transform brick walls in tiles. It takes as input the
	/// a BrickWall which is a definition for the tiles. With the brick wall and the root entity
	/// that is associated with it, it builds tiles objects that must be displayed.
	/// </summary>
	internal static class Carpenter
	{


		public static IEnumerable<AbstractTile> BuildTiles(BusinessContext businessContext, Caches caches, BrickWall brickWall, ViewControllerMode viewMode, AbstractEntity entity)
		{
			bool isFirst = true;

			foreach (var brick in brickWall.Bricks)
			{
				foreach (var tile in Carpenter.BuildTiles (businessContext, caches, brick, viewMode, entity, isFirst))
				{
					yield return tile;
				}

				isFirst = false;
			}
		}


		private static IEnumerable<AbstractTile> BuildTiles(BusinessContext businessContext, Caches caches, Brick brick, ViewControllerMode viewMode, AbstractEntity entity, bool isFirst)
		{
			switch (viewMode)
			{
				case ViewControllerMode.Creation:
					throw new NotImplementedException ();

				case ViewControllerMode.Edition:
					return Carpenter.BuildEditionTiles (businessContext, caches, brick, entity);

				case ViewControllerMode.None:
					throw new NotImplementedException ();

				case ViewControllerMode.Summary:
					return Carpenter.BuildSummaryTiles (businessContext, caches, brick, entity, isFirst);

				default:
					throw new NotImplementedException ();
			}
		}


		private static IEnumerable<AbstractTile> BuildSummaryTiles(BusinessContext businessContext, Caches caches, Brick brick, AbstractEntity entity, bool isFirst)
		{
			var summaryBrick = Carpenter.GetSummaryBrick (brick);
			var templateBrick = Carpenter.GetOptionalTemplateBrick (summaryBrick);

			if (templateBrick == null)
			{
				return new List<AbstractTile> ()
				{
					Carpenter.BuildSummaryTile (businessContext, caches, summaryBrick, entity, isFirst)
				};
			}
			else
			{
				return Carpenter.BuildCollectionSummaryTiles (businessContext, caches, templateBrick, entity);
			}
		}


		private static Brick GetSummaryBrick(Brick brick)
		{
			Brick result = null;

			var currentBrick = brick;

			while (currentBrick != null)
			{
				result = currentBrick;

				currentBrick = Brick.GetProperty (currentBrick, BrickPropertyKey.OfType).Brick;
			}

			return result;
		}


		private static Brick GetOptionalTemplateBrick(Brick brick)
		{
		    var brickProperty = Carpenter.GetOptionalBrickProperty (brick, BrickPropertyKey.Template);

			return brickProperty.HasValue
				? brickProperty.Value.Brick
				: null;
		}


		private static AbstractTile BuildSummaryTile(BusinessContext businessContext, Caches caches, Brick brick, AbstractEntity entity, bool isFirst)
		{
			var entityGetter = Carpenter.GetEntityGetter (brick);
			var tileEntity = entityGetter (entity);

			var entityId = Tools.GetEntityId (businessContext, tileEntity);

			var iconUri = Carpenter.GetMandatoryString (brick, BrickPropertyKey.Icon);
			var iconClass = IconManager.GetCssClassName (iconUri, IconSize.Sixteen, brick.GetBrickType ());

			var titleGetter = Carpenter.GetMandatoryTextGetter (brick, BrickPropertyKey.Title);
			var title = titleGetter (tileEntity).ToString ();

			var textGetter = Carpenter.GetMandatoryTextGetter (brick, BrickPropertyKey.Text);
			var text = textGetter (tileEntity).ToString ();

			var subViewMode = Tools.ViewModeToString (ViewControllerMode.Edition);
			string autoCreatorId = null;
			string subViewId = null;

			foreach (var brickMode in Carpenter.GetBrickModes (brick))
			{
				switch (brickMode)
				{
					case BrickMode.DefaultToSummarySubView:
						subViewMode = Tools.ViewModeToString (ViewControllerMode.Summary);
						break;

					case BrickMode.AutoCreateNullEntity:
						autoCreatorId = caches.AutoCreatorCache.Get (brick.GetLambda ()).Id;
						break;

					default:
						if (brickMode.IsSpecialController ())
						{
							subViewId = Tools.ViewIdToString (brickMode.GetControllerSubTypeId ());
						}
						break;
				}
			}

			return new SummaryTile ()
			{
				EntityId = entityId,
				IsRoot = isFirst,
				SubViewMode = subViewMode,
				SubViewId = subViewId,
				AutoCreatorId = autoCreatorId,
				IconClass = iconClass,
				Title = title,
				Text = text
			};
		}


		private static Func<AbstractEntity, AbstractEntity> GetEntityGetter(Brick brick)
		{
			var resolver = brick.GetResolver (null);

			if (resolver == null)
			{
				return x => x;
			}
			else
			{
				return x => (AbstractEntity) resolver.DynamicInvoke (x);
			}
		}


		private static IEnumerable<BrickMode> GetBrickModes(Brick brick)
		{
			return from attribute in Brick.GetProperties (brick, BrickPropertyKey.Attribute)
				   let value = attribute.AttributeValue
				   where value != null
				   where value.ContainsValue<BrickMode> ()
				   select value.GetValue<BrickMode> ();
		}


		private static IEnumerable<AbstractTile> BuildCollectionSummaryTiles(BusinessContext businessContext, Caches caches, Brick brick, AbstractEntity entity)
		{
			var lambda = brick.GetLambda ();
			var propertyAccessorCache = caches.PropertyAccessorCache;
			var propertyAccessor = (EntityCollectionPropertyAccessor) propertyAccessorCache.Get (lambda);

			var tileEntitiesGetter = Carpenter.GetEntitiesGetter (brick, propertyAccessor);
			var tileEntities = tileEntitiesGetter (entity);

			var brickType = brick.GetBrickType ();

			var icon = Carpenter.GetMandatoryString (brick, BrickPropertyKey.Icon);
			var iconClass = IconManager.GetCssClassName (icon, IconSize.Sixteen, brickType);

			var propertyAccessorId = propertyAccessor.Id;

			var subViewMode = Tools.ViewModeToString (ViewControllerMode.Edition);
			string subViewId = null;
			var hideAddButton = false;
			var hideRemoveButton = false;
			string autoCreatorId = null;
			bool isRoot = false;

			foreach (var brickMode in Carpenter.GetBrickModes (brick))
			{
				switch (brickMode)
				{
					case BrickMode.DefaultToSummarySubView:
						subViewMode = Tools.ViewModeToString (ViewControllerMode.Summary);
						break;

					case BrickMode.HideAddButton:
						hideAddButton = true;
						break;

					case BrickMode.HideRemoveButton:
						hideRemoveButton = true;
						break;

					default:
						if (brickMode.IsSpecialController ())
						{
							subViewId = InvariantConverter.ToString (brickMode.GetControllerSubTypeId ());
						}
						break;
				}
			}

			if (tileEntities.Count == 0)
			{
				yield return new EmptySummaryTile ()
				{
					EntityId = null,
					IsRoot = isRoot,
					SubViewMode = subViewMode,
					SubViewId = subViewId,
					AutoCreatorId = autoCreatorId,
					IconClass = iconClass,
					Title = null,
					Text = null,
					PropertyAccessorId = propertyAccessorId,
					HideAddButton = hideAddButton,
					HideRemoveButton = hideRemoveButton,
				};
			}
			else
			{
				foreach (var tileEntity in tileEntities)
				{
					var titleGetter = Carpenter.GetMandatoryTextGetter (brick, BrickPropertyKey.Title);
					var title = titleGetter (tileEntity).ToString ();

					var textGetter = Carpenter.GetMandatoryTextGetter (brick, BrickPropertyKey.Text);
					var text = textGetter (tileEntity).ToString ();

					yield return new CollectionSummaryTile ()
					{
						EntityId = Tools.GetEntityId (businessContext, tileEntity),
						IsRoot = isRoot,
						SubViewMode = subViewMode,
						SubViewId = subViewId,
						AutoCreatorId = autoCreatorId,
						IconClass = iconClass,
						Title = title,
						Text = text,
						PropertyAccessorId = propertyAccessorId,
						HideAddButton = hideAddButton,
						HideRemoveButton = hideRemoveButton,
					};
				}
			}
		}


		private static Func<AbstractEntity, IList<AbstractEntity>> GetEntitiesGetter(Brick brick, EntityCollectionPropertyAccessor propertyAccessor)
		{
			Func<AbstractEntity, IList<AbstractEntity>> rawEntitiesGetter = e =>
			{
				return propertyAccessor.GetEntityCollection (e);
			};

			var collectionType = propertyAccessor.CollectionType;
			var templateType = brick.GetBrickType ();

			if (collectionType != templateType)
			{
				return e => rawEntitiesGetter (e)
					.Where (t => templateType.IsAssignableFrom (t.GetType ()))
					.Cast<AbstractEntity> ()
					.ToList ();
			}
			else
			{
				return rawEntitiesGetter;
			}
		}


		private static IEnumerable<AbstractTile> BuildEditionTiles(BusinessContext businessContext, Caches caches, Brick brick, AbstractEntity entity)
		{
			var bricks = Carpenter.BuildEditionTileParts (businessContext, caches, brick, entity).ToList ();

			if (bricks.Count > 0)
			{
				var entityId = Tools.GetEntityId (businessContext, entity);

				var iconUri = Carpenter.GetMandatoryString (brick, BrickPropertyKey.Icon);
				var iconClass = IconManager.GetCssClassName (iconUri, IconSize.Sixteen, entity.GetType ());

				var titleGetter = Carpenter.GetMandatoryTextGetter (brick, BrickPropertyKey.Title);
				var title = titleGetter (entity).ToString ();

				yield return new EditionTile ()
				{
					EntityId = entityId,
					IconClass = iconClass,
					Title = title,
					Bricks = bricks
				};
			}

			foreach (var includeProperty in Brick.GetProperties (brick, BrickPropertyKey.Include))
			{
				var includedEntityGetter = Carpenter.GetGetterFromExpression<AbstractEntity> (includeProperty);
				var includedEntity = includedEntityGetter (entity);

				var includedTiles = LayoutBuilder.GetTiles (businessContext, caches, includedEntity, ViewControllerMode.Edition, null);

				foreach (var includedTile in includedTiles)
				{
					yield return includedTile;
				}
			}
		}


		private static IEnumerable<AbstractEditionTilePart> BuildEditionTileParts(BusinessContext businessContext, Caches caches, Brick brick, AbstractEntity entity)
		{
			foreach (var property in Brick.GetAllProperties (brick))
			{
				switch (property.Key)
				{
					case BrickPropertyKey.Input:
						foreach (var inputPart in Carpenter.BuildInputParts (businessContext, caches, property.Brick, entity))
						{
							yield return inputPart;
						}
						break;

					case BrickPropertyKey.Separator:
						yield return new Separator ();
						break;

					case BrickPropertyKey.GlobalWarning:
						yield return new GlobalWarning ();
						break;

					default:
						// Nothing to do here. We simply ignore the property.
						break;
				}
			}
		}


		private static IEnumerable<AbstractEditionTilePart> BuildInputParts(BusinessContext businessContext, Caches caches, Brick brick, AbstractEntity entity)
		{
			var brickProperties = Brick.GetProperties (brick, BrickPropertyKey.Field, BrickPropertyKey.HorizontalGroup);

			foreach (var brickProperty in brickProperties)
			{
				switch (brickProperty.Key)
				{
					case BrickPropertyKey.HorizontalGroup:
						yield return Carpenter.BuildHorizontalGroup (businessContext, brickProperty.Brick, caches, entity);
						break;

					case BrickPropertyKey.Field:
						yield return Carpenter.BuildField (businessContext, caches, entity, brickProperties, brickProperty, true);
						break;
				}
			}
		}


		private static AbstractEditionTilePart BuildHorizontalGroup(BusinessContext businessContext, Brick brick, Caches caches, AbstractEntity entity)
		{
			var horizontalGroup = new HorizontalGroup ();

			var titleGetter = Carpenter.GetOptionalTextGetter (brick, BrickPropertyKey.Title);
			horizontalGroup.Title = titleGetter (entity).ToString ();

			horizontalGroup.Fields = Carpenter.BuildHorizontalFields (businessContext, caches, brick, entity).ToList ();

			return horizontalGroup;
		}


		private static IEnumerable<AbstractField> BuildHorizontalFields(BusinessContext businessContext, Caches caches, Brick brick, AbstractEntity entity)
		{
			var brickProperties = Brick.GetProperties (brick, BrickPropertyKey.Field);

			foreach (var brickProperty in brickProperties)
			{
				yield return Carpenter.BuildField (businessContext, caches, entity, brickProperties, brickProperty, false);
			}
		}


		private static AbstractField BuildField(BusinessContext businessContext, Caches caches, AbstractEntity entity, BrickPropertyCollection brickProperties, BrickProperty fieldProperty, bool includeTitle)
		{
			var propertyAccessor = Carpenter.GetPropertyAccessor (caches, fieldProperty);

			switch (propertyAccessor.PropertyAccessorType)
			{
				case PropertyAccessorType.Boolean:
					return Carpenter.BuildBooleanField (entity, propertyAccessor, brickProperties, includeTitle);

				case PropertyAccessorType.Date:
					return Carpenter.BuildDateField (entity, propertyAccessor, brickProperties, includeTitle);

				case PropertyAccessorType.Decimal:
					return Carpenter.BuildDecimalField (entity, propertyAccessor, brickProperties, includeTitle);

				case PropertyAccessorType.EntityCollection:
					return Carpenter.BuildEntityCollectionField (businessContext, entity, propertyAccessor, brickProperties, includeTitle);

				case PropertyAccessorType.EntityReference:
					return Carpenter.BuildEntityReferenceField (businessContext, entity, propertyAccessor, brickProperties, includeTitle);

				case PropertyAccessorType.Enumeration:
					return Carpenter.BuildEnumerationField (entity, propertyAccessor, brickProperties, includeTitle);

				case PropertyAccessorType.Integer:
					return Carpenter.BuildIntegerField (entity, propertyAccessor, brickProperties, includeTitle);

				case PropertyAccessorType.Text:
					return Carpenter.BuildTextField (entity, propertyAccessor, brickProperties, includeTitle);

				default:
					throw new NotImplementedException ();
			}
		}


		private static AbstractPropertyAccessor GetPropertyAccessor(Caches caches, BrickProperty fieldProperty)
		{
			var lambda = (LambdaExpression) fieldProperty.ExpressionValue;

			return  caches.PropertyAccessorCache.Get (lambda);
		}


		private static BooleanField BuildBooleanField(AbstractEntity entity, AbstractPropertyAccessor propertyAccessor, BrickPropertyCollection brickProperties, bool includeTitle)
		{
			var field = Carpenter.BuildField<BooleanField> (propertyAccessor, brickProperties, includeTitle);

			var castedAccessor = (BooleanPropertyAccessor) propertyAccessor;
			Func<AbstractEntity, bool?> valueGetter = e => (bool?) castedAccessor.GetValue (e);
			field.Value = valueGetter (entity);

			return field;
		}


		private static DateField BuildDateField(AbstractEntity entity, AbstractPropertyAccessor propertyAccessor, BrickPropertyCollection brickProperties, bool includeTitle)
		{
			var field = Carpenter.BuildField<DateField> (propertyAccessor, brickProperties, includeTitle);

			var castedAccessor = (DatePropertyAccessor) propertyAccessor;
			Func<AbstractEntity, string> valueGetter = e => (string) castedAccessor.GetValue (e);
			field.Value = valueGetter (entity);

			return field;
		}


		private static DecimalField BuildDecimalField(AbstractEntity entity, AbstractPropertyAccessor propertyAccessor, BrickPropertyCollection brickProperties, bool includeTitle)
		{
			var field = Carpenter.BuildField<DecimalField> (propertyAccessor, brickProperties, includeTitle);

			var castedAccessor = (DecimalPropertyAccessor) propertyAccessor;
			Func<AbstractEntity, decimal?> valueGetter = e => (decimal?) castedAccessor.GetValue (e);
			field.Value = valueGetter (entity);

			return field;
		}


		private static EntityCollectionField BuildEntityCollectionField(BusinessContext businessContext, AbstractEntity entity, AbstractPropertyAccessor propertyAccessor, BrickPropertyCollection brickProperties, bool includeTitle)
		{
			var field = Carpenter.BuildField<EntityCollectionField> (propertyAccessor, brickProperties, includeTitle);

			var castedAccessor = (EntityCollectionPropertyAccessor) propertyAccessor;
			Func<AbstractEntity, IList<AbstractEntity>> valueGetter = e => castedAccessor.GetEntityCollection (e);
			field.Values = valueGetter (entity)
				.Select (e => Carpenter.BuildEntityValue (businessContext, e))
				.ToList ();

			field.TypeName = Tools.TypeToString (castedAccessor.CollectionType);

			return field;
		}


		private static EntityReferenceField BuildEntityReferenceField(BusinessContext businessContext, AbstractEntity entity, AbstractPropertyAccessor propertyAccessor, BrickPropertyCollection brickProperties, bool includeTitle)
		{
			var field = Carpenter.BuildField<EntityReferenceField> (propertyAccessor, brickProperties, includeTitle);

			var castedAccessor = (EntityReferencePropertyAccessor) propertyAccessor;
			Func<AbstractEntity, AbstractEntity> valueGetter = e => castedAccessor.GetEntity (e);
			field.Value = Carpenter.BuildEntityValue (businessContext, valueGetter (entity));
			field.TypeName = Tools.TypeToString (castedAccessor.Type);

			return field;
		}


		private static EnumerationField BuildEnumerationField(AbstractEntity entity, AbstractPropertyAccessor propertyAccessor, BrickPropertyCollection brickProperties, bool includeTitle)
		{
			var field = Carpenter.BuildField<EnumerationField> (propertyAccessor, brickProperties, includeTitle);

			var castedAccessor = (EnumerationPropertyAccessor) propertyAccessor;
			Func<AbstractEntity, string> valueGetter = e => (string) castedAccessor.GetValue (e);
			field.Value = valueGetter (entity);
			field.TypeName = Tools.TypeToString (castedAccessor.Type);

			return field;
		}


		private static IntegerField BuildIntegerField(AbstractEntity entity, AbstractPropertyAccessor propertyAccessor, BrickPropertyCollection brickProperties, bool includeTitle)
		{
			var field = Carpenter.BuildField<IntegerField> (propertyAccessor, brickProperties, includeTitle);

			var castedAccessor = (IntegerPropertyAccessor) propertyAccessor;
			Func<AbstractEntity, long?> valueGetter = e => (long?) castedAccessor.GetValue (e);
			field.Value = valueGetter (entity);

			return field;
		}


		private static AbstractField BuildTextField(AbstractEntity entity, AbstractPropertyAccessor propertyAccessor, BrickPropertyCollection brickProperties, bool includeTitle)
		{
			var castedAccessor = (TextPropertyAccessor) propertyAccessor;
			Func<AbstractEntity, string> valueGetter = e => (string) castedAccessor.GetValue (e);
			var value = valueGetter (entity);

			if (StringType.IsMultilineText (castedAccessor.Property.Type))
			{
				var field = Carpenter.BuildField<TextAreaField> (propertyAccessor, brickProperties, includeTitle);
				field.Value = value;

				return field;
			}
			else
			{
				var field = Carpenter.BuildField<TextField> (propertyAccessor, brickProperties, includeTitle);

				field.IsPassword = Carpenter.IsPassword (brickProperties);
				field.Value = value;

				return field;
			}
		}


		private static bool IsPassword(BrickPropertyCollection brickProperties)
		{
			return brickProperties.PeekAfter (BrickPropertyKey.Password, -1).HasValue;
		}


		private static T BuildField<T>(AbstractPropertyAccessor propertyAccessor, BrickPropertyCollection brickProperties, bool includeTitle)
			where T : AbstractField, new ()
		{
			var title = includeTitle
				? Carpenter.GetFieldTitle (brickProperties, propertyAccessor).ToString ()
				: "";
			
			return new T ()
			{
				Id = propertyAccessor.Id,
				IsReadOnly = Carpenter.IsReadOnly (brickProperties),
				AllowBlank = propertyAccessor.Property.IsNullable,
				Title = title
			};
		}


		private static bool IsReadOnly(BrickPropertyCollection brickProperties)
		{
			return brickProperties.PeekAfter (BrickPropertyKey.ReadOnly, -1).HasValue;
		}


		private static FormattedText GetFieldTitle(BrickPropertyCollection brickProperties, AbstractPropertyAccessor propertyAccessor)
		{
			return Carpenter.GetFieldTitle (brickProperties)
				?? Carpenter.GetFieldTitle (propertyAccessor);
		}


		private static FormattedText? GetFieldTitle(BrickPropertyCollection brickProperties)
		{
			var titleProperty = brickProperties.PeekBefore (BrickPropertyKey.Title, -1);

			if (titleProperty.HasValue)
			{
				return titleProperty.Value.StringValue;
			}
			else
			{
				return null;
			}
		}


		private static FormattedText GetFieldTitle(AbstractPropertyAccessor propertyAccessor)
		{
			var title = FormattedText.Empty;

			var caption = EntityInfo.GetFieldCaption (propertyAccessor.Property.CaptionId);

			if (caption != null)
			{
				title = caption.HasLabels
					? caption.DefaultLabel
					: caption.Description ?? caption.Name;
			}

			return title;
		}


		public static EntityValue BuildEntityValue(BusinessContext businessContext, AbstractEntity entity)
		{
			if (entity == null)
			{
				return new EntityValue ()
				{
					Displayed = Res.Strings.EmptyValue.ToSimpleText (),
					Submitted = Constants.KeyForNullValue,
				};
			}

			return new EntityValue ()
			{
				Displayed = entity.GetCompactSummary ().ToString (),
				Submitted = Tools.GetEntityId (businessContext, entity),
			};
		}


		private static string GetMandatoryString(Brick brick, BrickPropertyKey key)
		{
			return Carpenter.GetMandatoryBrickProperty (brick, key).StringValue;
		}


		private static Func<AbstractEntity, FormattedText> GetOptionalTextGetter(Brick brick, BrickPropertyKey key)
		{
			var property = Carpenter.GetOptionalBrickProperty (brick, key);

			if (!property.HasValue)
			{
				return e => new FormattedText ();
			}

			return Carpenter.GetTextGetterFromString (property.Value)
		        ?? Carpenter.GetGetterFromExpression<FormattedText> (property.Value);
		}


		private static Func<AbstractEntity, FormattedText> GetMandatoryTextGetter(Brick brick, BrickPropertyKey key)
		{
			var property = Carpenter.GetMandatoryBrickProperty (brick, key);

			var textGetter = Carpenter.GetTextGetterFromString (property)
		                  ?? Carpenter.GetGetterFromExpression<FormattedText> (property);

			if (textGetter == null)
			{
				throw new InvalidOperationException ("Text should have a value");
			}

			return textGetter;
		}


		private static Func<AbstractEntity, FormattedText> GetTextGetterFromString(BrickProperty property)
		{
			Func<AbstractEntity, FormattedText> textGetter = null;

			var stringValue = property.StringValue;

			if (stringValue != null)
			{
				textGetter = _ => (FormattedText) stringValue;
			}

			return textGetter;
		}


		private static Func<AbstractEntity, T> GetGetterFromExpression<T>(BrickProperty property)
		{
			var expressionValue = property.ExpressionValue as LambdaExpression;

			Func<AbstractEntity, T> textGetter = null;

			if (expressionValue != null)
			{
				var expression = expressionValue.Compile ();

				textGetter = x => (T) expression.DynamicInvoke (x);
			}

			return textGetter;
		}


		private static BrickProperty? GetOptionalBrickProperty(Brick brick, BrickPropertyKey key)
		{
			if (Brick.ContainsProperty (brick, key))
			{
				return Brick.GetProperty (brick, key);
			}
			else
			{
				return null;
			}
		}


		private static BrickProperty GetMandatoryBrickProperty(Brick brick, BrickPropertyKey key)
		{
			if (!Brick.ContainsProperty (brick, key))
			{
				throw new InvalidOperationException ("brick property is missing !");
			}

			return Brick.GetProperty (brick, key);
		}


	}


}
