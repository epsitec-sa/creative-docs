using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

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
	internal sealed class Carpenter
	{


		private Carpenter(BusinessContext businessContext, Caches caches, AbstractEntity entity)
		{
			this.businessContext = businessContext;
			this.caches = caches;
			this.entity = entity;
		}


		public static EntityColumn BuildEntityColumn(BusinessContext businessContext, Caches caches, AbstractEntity entity, ViewControllerMode viewMode, int? viewId)
		{
			var carpenter = new Carpenter (businessContext, caches, entity);

			return carpenter.BuildEntityColumn (viewMode, viewId);
		}


		public static IEnumerable<AbstractTile> BuildTiles(BusinessContext businessContext, Caches caches, AbstractEntity entity, ViewControllerMode viewMode, int? viewId)
		{
			var carpenter = new Carpenter (businessContext, caches, entity);

			return carpenter.BuildTiles (viewMode, viewId);
		}


		private	EntityColumn BuildEntityColumn(ViewControllerMode viewMode, int? viewId)
		{
			return new EntityColumn ()
			{
				EntityId = this.GetEntityId (this.entity),
				ViewMode = Carpenter.GetViewMode (viewMode),
				ViewId = Carpenter.GetViewId (viewId),
				Tiles = this.BuildTiles (viewMode, viewId).ToList (),
			};
		}


		private IEnumerable<AbstractTile> BuildTiles(ViewControllerMode viewMode, int? viewId)
		{
			bool isFirst = true;

			foreach (var brick in Mason.BuildBrickWall (entity, viewMode, viewId).Bricks)
			{
				foreach (var tile in this.BuildTiles (brick, viewMode, isFirst))
				{
					yield return tile;
				}

				isFirst = false;
			}
		}


		private IEnumerable<AbstractTile> BuildTiles(Brick brick, ViewControllerMode viewMode, bool isFirst)
		{
			switch (viewMode)
			{
				case ViewControllerMode.Creation:
					throw new NotImplementedException ();

				case ViewControllerMode.Edition:
					return this.BuildEditionTiles (brick);

				case ViewControllerMode.None:
					throw new NotImplementedException ();

				case ViewControllerMode.Summary:
					return this.BuildSummaryTiles (brick, isFirst);

				default:
					throw new NotImplementedException ();
			}
		}


		private IEnumerable<AbstractTile> BuildSummaryTiles(Brick brick, bool isFirst)
		{
			var summaryBrick = Carpenter.GetSummaryBrick (brick);
			var templateBrick = Carpenter.GetOptionalTemplateBrick (summaryBrick);

			if (templateBrick == null)
			{
				return new List<AbstractTile> ()
				{
					this.BuildSummaryTile (summaryBrick, isFirst)
				};
			}
			else
			{
				return this.BuildCollectionSummaryTiles (templateBrick);
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


		private AbstractTile BuildSummaryTile(Brick brick, bool isFirst)
		{
			var tileEntity = this.ResolveTileEntity (brick);
			var brickModes = Carpenter.GetBrickModes (brick).ToSet ();
			
			return new SummaryTile ()
			{
				EntityId = this.GetEntityId (tileEntity),
				IsRoot = isFirst,
				SubViewMode = Carpenter.GetSubViewMode (brickModes),
				SubViewId = Carpenter.GetSubViewId (brickModes),
				AutoCreatorId = this.GetAutoCreatorId (brick, brickModes),
				IconClass = Carpenter.GetIconClass (brick),
				Title = Carpenter.GetText (tileEntity, brick, BrickPropertyKey.Title),
				Text = Carpenter.GetText (tileEntity, brick, BrickPropertyKey.Text)
			};
		}


		private IEnumerable<AbstractTile> BuildCollectionSummaryTiles(Brick brick)
		{
			var tileEntities = this.ResolveTileEntities (brick);
			var brickModes = Carpenter.GetBrickModes (brick).ToSet ();

			var subViewMode = Carpenter.GetSubViewMode (brickModes);
			var subViewId = Carpenter.GetSubViewId (brickModes);
			var hideAddButton = Carpenter.GetHideAddButton (brickModes);
			var hideRemoveButton = Carpenter.GetHideRemoveButton (brickModes);
			var iconClass = Carpenter.GetIconClass (brick);
			var propertyAccessorId = this.caches.PropertyAccessorCache.Get (brick.GetLambda ()).Id;

			bool empty = true;

			foreach (var tileEntity in tileEntities)
			{
				empty = false;

				yield return new CollectionSummaryTile ()
				{
					EntityId = this.GetEntityId (tileEntity),
					IsRoot = false,
					SubViewMode = subViewMode,
					SubViewId = subViewId,
					AutoCreatorId = null,
					IconClass = iconClass,
					Title = Carpenter.GetText (tileEntity, brick, BrickPropertyKey.Title),
					Text = Carpenter.GetText (tileEntity, brick, BrickPropertyKey.Text),
					PropertyAccessorId = propertyAccessorId,
					HideAddButton = hideAddButton,
					HideRemoveButton = hideRemoveButton,
				};
			}

			if (empty)
			{
				yield return new EmptySummaryTile ()
				{
					EntityId = null,
					IsRoot = false,
					SubViewMode = subViewMode,
					SubViewId = subViewId,
					AutoCreatorId = null,
					IconClass = iconClass,
					Title = null,
					Text = null,
					PropertyAccessorId = propertyAccessorId,
					HideAddButton = hideAddButton,
					HideRemoveButton = hideRemoveButton,
				};
			}
		}


		private AbstractEntity ResolveTileEntity(Brick brick)
		{
			var resolver = brick.GetResolver (null);

			return resolver == null
				? this.entity
				: (AbstractEntity) resolver.DynamicInvoke (this.entity);
		}


		private IEnumerable<AbstractEntity> ResolveTileEntities(Brick brick)
		{
			var brickType = brick.GetBrickType ();
			var resolver = brick.GetResolver (null);

			var entities = (IEnumerable<AbstractEntity>) resolver.DynamicInvoke (this.entity);

			return from entity in entities
				   let entityType = entity.GetType ()
				   where entityType == brickType || brickType.IsAssignableFrom (entityType)
				   select entity;
		}


		private static IEnumerable<BrickMode> GetBrickModes(Brick brick)
		{
			return from attribute in Brick.GetProperties (brick, BrickPropertyKey.Attribute)
				   let value = attribute.AttributeValue
				   where value != null
				   where value.ContainsValue<BrickMode> ()
				   select value.GetValue<BrickMode> ();
		}


		private static string GetSubViewMode(ISet<BrickMode> brickModes)
		{
			var viewMode = brickModes.Contains (BrickMode.DefaultToSummarySubView)
				? ViewControllerMode.Summary
				: ViewControllerMode.Edition;

			return Carpenter.GetViewMode (viewMode);
		}


		private string GetAutoCreatorId(Brick brick, ISet<BrickMode> brickModes)
		{
			return brickModes.Contains (BrickMode.AutoCreateNullEntity)
				? this.caches.AutoCreatorCache.Get (brick.GetLambda ()).Id
				: null;
		}


		private static string GetSubViewId(ISet<BrickMode> brickModes)
		{
			var mode = brickModes.FirstOrDefault (m => m.IsSpecialController ());

			return mode != default (BrickMode)
				? Carpenter.GetViewId (mode.GetControllerSubTypeId ())
				: null;
		}


		private static bool GetHideAddButton(ISet<BrickMode> brickModes)
		{
			return brickModes.Contains (BrickMode.HideAddButton);
		}


		private static bool GetHideRemoveButton(ISet<BrickMode> brickModes)
		{
			return brickModes.Contains (BrickMode.HideRemoveButton);
		}


		private IEnumerable<AbstractTile> BuildEditionTiles(Brick brick)
		{
			var tileEntity = this.entity;

			var bricks = this.BuildEditionTileParts (brick, tileEntity).ToList ();

			if (bricks.Count > 0)
			{
				yield return new EditionTile ()
				{
					EntityId = this.GetEntityId (tileEntity),
					IconClass = Carpenter.GetIconClass (brick),
					Title = Carpenter.GetText (tileEntity, brick, BrickPropertyKey.Title),
					Bricks = bricks
				};
			}

			foreach (var includeProperty in Brick.GetProperties (brick, BrickPropertyKey.Include))
			{
				var includedEntity = Carpenter.GetIncludedEntity (tileEntity, includeProperty);
				var includedTiles = Carpenter.BuildTiles (this.businessContext, this.caches, includedEntity, ViewControllerMode.Edition, null);

				foreach (var includedTile in includedTiles)
				{
					yield return includedTile;
				}
			}
		}


		private static AbstractEntity GetIncludedEntity(AbstractEntity entity, BrickProperty property)
		{
			var expressionValue = (LambdaExpression) property.ExpressionValue;
			
			return (AbstractEntity) expressionValue.Compile ().DynamicInvoke (entity);
		}


		private IEnumerable<AbstractEditionTilePart> BuildEditionTileParts(Brick brick, AbstractEntity entity)
		{
			foreach (var property in Brick.GetAllProperties (brick))
			{
				switch (property.Key)
				{
					case BrickPropertyKey.Input:
						foreach (var inputPart in this.BuildInputParts (property.Brick, entity))
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
				}
			}
		}


		private IEnumerable<AbstractEditionTilePart> BuildInputParts(Brick brick, AbstractEntity entity)
		{
			var brickProperties = Brick.GetProperties (brick, BrickPropertyKey.Field, BrickPropertyKey.HorizontalGroup);

			foreach (var brickProperty in brickProperties)
			{
				switch (brickProperty.Key)
				{
					case BrickPropertyKey.HorizontalGroup:
						yield return this.BuildHorizontalGroup (brickProperty.Brick, entity);
						break;

					case BrickPropertyKey.Field:
						yield return this.BuildField (entity, brickProperties, brickProperty, true);
						break;
				}
			}
		}


		private AbstractEditionTilePart BuildHorizontalGroup(Brick brick, AbstractEntity entity)
		{
			return new HorizontalGroup ()
			{
				Title = Carpenter.GetOptionalText (entity, brick, BrickPropertyKey.Title),
				Fields = this.BuildHorizontalFields (brick, entity).ToList ()
			};
		}


		private IEnumerable<AbstractField> BuildHorizontalFields(Brick brick, AbstractEntity entity)
		{
			var brickProperties = Brick.GetProperties (brick, BrickPropertyKey.Field);

			foreach (var brickProperty in brickProperties)
			{
				yield return this.BuildField (entity, brickProperties, brickProperty, false);
			}
		}


		private AbstractField BuildField(AbstractEntity entity, BrickPropertyCollection brickProperties, BrickProperty fieldProperty, bool includeTitle)
		{
			var propertyAccessor = this.GetPropertyAccessor (fieldProperty);

			switch (propertyAccessor.PropertyAccessorType)
			{
				case PropertyAccessorType.Boolean:
					return Carpenter.BuildBooleanField (entity, propertyAccessor, brickProperties, includeTitle);

				case PropertyAccessorType.Date:
					return Carpenter.BuildDateField (entity, propertyAccessor, brickProperties, includeTitle);

				case PropertyAccessorType.Decimal:
					return Carpenter.BuildDecimalField (entity, propertyAccessor, brickProperties, includeTitle);

				case PropertyAccessorType.EntityCollection:
					return this.BuildEntityCollectionField (entity, propertyAccessor, brickProperties, includeTitle);

				case PropertyAccessorType.EntityReference:
					return this.BuildEntityReferenceField (entity, propertyAccessor, brickProperties, includeTitle);

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


		private AbstractPropertyAccessor GetPropertyAccessor(BrickProperty fieldProperty)
		{
			var lambda = (LambdaExpression) fieldProperty.ExpressionValue;

			return  this.caches.PropertyAccessorCache.Get (lambda);
		}


		private static BooleanField BuildBooleanField(AbstractEntity entity, AbstractPropertyAccessor propertyAccessor, BrickPropertyCollection brickProperties, bool includeTitle)
		{
			var field = Carpenter.BuildField<BooleanField> (propertyAccessor, brickProperties, includeTitle);

			var castedAccessor = (BooleanPropertyAccessor) propertyAccessor;
			field.Value = (bool?) castedAccessor.GetValue (entity);

			return field;
		}


		private static DateField BuildDateField(AbstractEntity entity, AbstractPropertyAccessor propertyAccessor, BrickPropertyCollection brickProperties, bool includeTitle)
		{
			var field = Carpenter.BuildField<DateField> (propertyAccessor, brickProperties, includeTitle);

			var castedAccessor = (DatePropertyAccessor) propertyAccessor;
			field.Value = (string) castedAccessor.GetValue (entity);

			return field;
		}


		private static DecimalField BuildDecimalField(AbstractEntity entity, AbstractPropertyAccessor propertyAccessor, BrickPropertyCollection brickProperties, bool includeTitle)
		{
			var field = Carpenter.BuildField<DecimalField> (propertyAccessor, brickProperties, includeTitle);

			var castedAccessor = (DecimalPropertyAccessor) propertyAccessor;
			field.Value = (decimal?) castedAccessor.GetValue (entity);

			return field;
		}


		private EntityCollectionField BuildEntityCollectionField(AbstractEntity entity, AbstractPropertyAccessor propertyAccessor, BrickPropertyCollection brickProperties, bool includeTitle)
		{
			var field = Carpenter.BuildField<EntityCollectionField> (propertyAccessor, brickProperties, includeTitle);

			var castedAccessor = (EntityCollectionPropertyAccessor) propertyAccessor;
			field.Values = castedAccessor
				.GetEntityCollection (entity)
				.Select (e => this.BuildEntityValue (e))
				.ToList ();
			field.TypeName = Tools.TypeToString (castedAccessor.CollectionType);

			return field;
		}


		private EntityReferenceField BuildEntityReferenceField(AbstractEntity entity, AbstractPropertyAccessor propertyAccessor, BrickPropertyCollection brickProperties, bool includeTitle)
		{
			var field = Carpenter.BuildField<EntityReferenceField> (propertyAccessor, brickProperties, includeTitle);

			var castedAccessor = (EntityReferencePropertyAccessor) propertyAccessor;
			field.Value = this.BuildEntityValue (castedAccessor.GetEntity (entity));
			field.TypeName = Tools.TypeToString (castedAccessor.Type);

			return field;
		}


		private static EnumerationField BuildEnumerationField(AbstractEntity entity, AbstractPropertyAccessor propertyAccessor, BrickPropertyCollection brickProperties, bool includeTitle)
		{
			var field = Carpenter.BuildField<EnumerationField> (propertyAccessor, brickProperties, includeTitle);

			var castedAccessor = (EnumerationPropertyAccessor) propertyAccessor;
			field.Value = (string) castedAccessor.GetValue (entity);
			field.TypeName = Tools.TypeToString (castedAccessor.Type);

			return field;
		}


		private static IntegerField BuildIntegerField(AbstractEntity entity, AbstractPropertyAccessor propertyAccessor, BrickPropertyCollection brickProperties, bool includeTitle)
		{
			var field = Carpenter.BuildField<IntegerField> (propertyAccessor, brickProperties, includeTitle);

			var castedAccessor = (IntegerPropertyAccessor) propertyAccessor;
			field.Value = (long?) castedAccessor.GetValue (entity);

			return field;
		}


		private static AbstractField BuildTextField(AbstractEntity entity, AbstractPropertyAccessor propertyAccessor, BrickPropertyCollection brickProperties, bool includeTitle)
		{
			var castedAccessor = (TextPropertyAccessor) propertyAccessor;
			var value = (string) castedAccessor.GetValue (entity);

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
			return new T ()
			{
				Id = propertyAccessor.Id,
				IsReadOnly = Carpenter.IsReadOnly (brickProperties),
				AllowBlank = propertyAccessor.Property.IsNullable,
				Title = includeTitle
					? Carpenter.GetFieldTitle (brickProperties) ?? Carpenter.GetFieldTitle (propertyAccessor)
					: null,
			};
		}


		private static bool IsReadOnly(BrickPropertyCollection brickProperties)
		{
			return brickProperties.PeekAfter (BrickPropertyKey.ReadOnly, -1).HasValue;
		}


		private static string GetFieldTitle(BrickPropertyCollection brickProperties)
		{
			var titleProperty = brickProperties.PeekBefore (BrickPropertyKey.Title, -1);

			return titleProperty.HasValue
				? titleProperty.Value.StringValue
				: null;
		}


		private static string GetFieldTitle(AbstractPropertyAccessor propertyAccessor)
		{
			var caption = EntityInfo.GetFieldCaption (propertyAccessor.Property.CaptionId);

			if (caption != null)
			{
				return caption.HasLabels
					? caption.DefaultLabel
					: caption.Description ?? caption.Name;
			}

			return null;
		}


		public EntityValue BuildEntityValue(AbstractEntity entity)
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
				Submitted = this.GetEntityId (entity),
			};
		}


		private static string GetOptionalText(AbstractEntity entity, Brick brick, BrickPropertyKey key)
		{
			var property = Carpenter.GetOptionalBrickProperty (brick, key);

			if (!property.HasValue)
			{
				return null;
			}

			var stringValue = property.Value.StringValue;

			if (stringValue != null)
			{
				return stringValue;
			}

			var expressionValue = property.Value.ExpressionValue as LambdaExpression;

			if (expressionValue != null)
			{
				var objectValue = expressionValue.Compile ().DynamicInvoke (entity);

				if (objectValue == null)
				{
					return null;
				}
				else if (objectValue is string)
				{
					return (string) objectValue;
				}
				else if (objectValue is FormattedText)
				{
					return ((FormattedText) objectValue).ToString ();
				}
				else
				{
					throw new NotSupportedException ();
				}
			}

			return null;
		}


		private static string GetText(AbstractEntity entity, Brick brick, BrickPropertyKey key)
		{
			var text = Carpenter.GetOptionalText (entity, brick, key);

			if (text == null)
			{
				throw new InvalidOperationException ("Text should have a value");
			}

			return text;
		}


		private static BrickProperty? GetOptionalBrickProperty(Brick brick, BrickPropertyKey key)
		{
			if (!Brick.ContainsProperty (brick, key))
			{
				return null;
			}
			
			return Brick.GetProperty (brick, key);
		}


		private static BrickProperty GetBrickProperty(Brick brick, BrickPropertyKey key)
		{
			var brickProperty = Carpenter.GetOptionalBrickProperty (brick, key);

			if (!brickProperty.HasValue)
			{
				throw new NotSupportedException ("Brick property is missing.");
			}

			return brickProperty.Value;
		}


		private string GetEntityId(AbstractEntity entity)
		{
			return Tools.GetEntityId (this.businessContext, entity);
		}


		private static string GetIconClass(Brick brick)
		{
			var iconUri = Carpenter.GetBrickProperty (brick, BrickPropertyKey.Icon).StringValue;
			
			return IconManager.GetCssClassName (iconUri, IconSize.Sixteen, brick.GetBrickType ());
		}


		private static string GetViewMode(ViewControllerMode viewMode)
		{
			return Tools.ViewModeToString (viewMode);
		}


		private static string GetViewId(int? viewId)
		{
			return Tools.ViewIdToString (viewId);
		}


		private readonly BusinessContext businessContext;


		private readonly Caches caches;


		private readonly AbstractEntity entity;


	}


}
