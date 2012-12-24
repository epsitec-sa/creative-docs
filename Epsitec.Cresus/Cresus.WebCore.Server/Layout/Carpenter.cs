using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Bricks;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;
using Epsitec.Cresus.Core.Controllers.SetControllers;

using Epsitec.Cresus.WebCore.Server.Core;
using Epsitec.Cresus.WebCore.Server.Core.Databases;
using Epsitec.Cresus.WebCore.Server.Core.IO;
using Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor;
using Epsitec.Cresus.WebCore.Server.NancyModules;

using System;

using System.Collections.Generic;

using System.Linq;
using System.Linq.Expressions;


namespace Epsitec.Cresus.WebCore.Server.Layout
{


	internal sealed class Carpenter
	{


		private Carpenter(BusinessContext businessContext, Caches caches, DatabaseManager databaseManager, AbstractEntity entity)
		{
			this.businessContext = businessContext;
			this.caches = caches;
			this.databaseManager = databaseManager;
			this.entity = entity;
		}


		public static EntityColumn BuildEntityColumn(BusinessContext businessContext, Caches caches, DatabaseManager databaseManager, AbstractEntity entity, ViewControllerMode viewMode, int? viewId)
		{
			var carpenter = new Carpenter (businessContext, caches, databaseManager, entity);

			return carpenter.BuildEntityColumn (viewMode, viewId);
		}


		public static IEnumerable<AbstractTile> BuildTiles(BusinessContext businessContext, Caches caches, DatabaseManager databaseManager, AbstractEntity entity, ViewControllerMode viewMode, int? viewId)
		{
			var carpenter = new Carpenter (businessContext, caches, databaseManager, entity);

			return carpenter.BuildTiles (viewMode, viewId);
		}


		private	EntityColumn BuildEntityColumn(ViewControllerMode viewMode, int? viewId)
		{
			switch (viewMode)
			{
				case ViewControllerMode.Action:
				case ViewControllerMode.Edition:
				case ViewControllerMode.Summary:
					return this.BuildTileColumn (viewMode, viewId);

				case ViewControllerMode.Set:
					return this.BuildSetColumn (viewMode, viewId);

				default:
					throw new NotImplementedException ();
			}
		}


		private SetColumn BuildSetColumn(ViewControllerMode viewMode, int? viewId)
		{
			using (var controller = Mason.BuildController (this.businessContext, this.entity, viewMode, viewId))
			{
				var setController = (ISetViewController) controller;

				var iconUri = setController.GetIcon ();
				var entityType = this.entity.GetType ();
				var displayDataSetId = setController.GetDisplayDataSetId ();
				var pickDataSetId = setController.GetPickDataSetId ();

				return new SetColumn ()
				{
					EntityId = this.GetEntityId (this.entity),
					ViewMode = DataIO.ViewModeToString (viewMode),
					ViewId = DataIO.ViewIdToString (viewId),
					Icon = Carpenter.GetIconClass (iconUri, entityType),
					Title = setController.GetTitle ().ToString (),
					DisplayDatabase = this.databaseManager.GetDatabase (displayDataSetId),
					PickDatabase = this.databaseManager.GetDatabase (pickDataSetId),
				};
			}
		}


		private TileColumn BuildTileColumn(ViewControllerMode viewMode, int? viewId)
		{
			return new TileColumn ()
			{
				EntityId = this.GetEntityId (this.entity),
				ViewMode = DataIO.ViewModeToString (viewMode),
				ViewId = DataIO.ViewIdToString (viewId),
				Tiles = this.BuildTiles (viewMode, viewId).ToList (),
			};
		}


		private IEnumerable<AbstractTile> BuildTiles(ViewControllerMode viewMode, int? viewId)
		{
			bool isFirst = true;

			foreach (var brick in Mason.BuildBrickWall (this.businessContext, entity, viewMode, viewId).Bricks)
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
				case ViewControllerMode.Action:
					return new List<ActionTile> ()
					{
						this.BuildActionTile (brick),
					};

				case ViewControllerMode.Edition:
					return this.BuildEditionTiles (brick);

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
				Text = Carpenter.GetText (tileEntity, brick, BrickPropertyKey.Text),
				Actions = this.BuildActionItems (tileEntity, brick).ToList (),
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

			var autoGroup = Carpenter.GetAutoGroup (brickModes);
			
			return autoGroup
				? this.BuildGroupedSummaryTile (brick, tileEntities, subViewMode, subViewId, hideAddButton, hideRemoveButton, iconClass, propertyAccessorId)
				: this.BuildCollectionSummaryTiles (brick, tileEntities, subViewMode, subViewId, hideAddButton, hideRemoveButton, iconClass, propertyAccessorId);
		}


		private IEnumerable<AbstractTile> BuildGroupedSummaryTile(Brick brick, IEnumerable<AbstractEntity> tileEntities, string subViewMode, string subViewId, bool hideAddButton, bool hideRemoveButton, string iconClass, string propertyAccessorId)
		{
			yield return new GroupedSummaryTile
			{
				IsRoot = false,
				SubViewMode = subViewMode,
				SubViewId = subViewId,
				IconClass = iconClass,
				Title = Carpenter.GetOptionalText (brick, BrickPropertyKey.Title),
				PropertyAccessorId = propertyAccessorId,
				HideAddButton = hideAddButton,
				HideRemoveButton = hideRemoveButton,
				Items = this.BuildGroupdSummaryTileItems (brick, tileEntities).ToList (),
			};
		}


		private IEnumerable<GroupedSummaryTileItem> BuildGroupdSummaryTileItems(Brick brick, IEnumerable<AbstractEntity> tileEntities)
		{
			foreach (var tileEntity in tileEntities)
			{
				var entityId = this.GetEntityId (tileEntity);
				var text = Carpenter.GetText (tileEntity, brick, BrickPropertyKey.Text);
				
				yield return new GroupedSummaryTileItem ()
				{
					EntityId = entityId,
					Text = text,
				};
			}
		}


		private IEnumerable<AbstractTile> BuildCollectionSummaryTiles(Brick brick, IEnumerable<AbstractEntity> tileEntities, string subViewMode, string subViewId, bool hideAddButton, bool hideRemoveButton, string iconClass, string propertyAccessorId)
		{
			var actions = new List<ActionItem> ();
			
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
					Actions = actions,
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
					Title = Carpenter.GetOptionalText (brick, BrickPropertyKey.Title),
					PropertyAccessorId = propertyAccessorId,
					HideAddButton = hideAddButton,
					HideRemoveButton = hideRemoveButton,
					Actions = actions,
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
			ViewControllerMode viewMode;

			if (brickModes.Contains (BrickMode.DefaultToSummarySubView))
			{
				viewMode = ViewControllerMode.Summary;
			}
			else if (brickModes.Contains (BrickMode.DefaultToSetSubView))
			{
				viewMode = ViewControllerMode.Set;
			}
			else
			{
				viewMode = ViewControllerMode.Edition;
			}

			return DataIO.ViewModeToString (viewMode);
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
				? DataIO.ViewIdToString (mode.GetControllerSubTypeId ())
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


		private static bool GetAutoGroup(ISet<BrickMode> brickModes)
		{
			return brickModes.Contains (BrickMode.AutoGroup);
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
					Bricks = bricks,
					Actions = this.BuildActionItems (tileEntity, brick).ToList (),
				};
			}

			foreach (var includeProperty in Brick.GetProperties (brick, BrickPropertyKey.Include))
			{
				var includedEntity = Carpenter.GetIncludedEntity (tileEntity, includeProperty);
				var includedTiles = Carpenter.BuildTiles (this.businessContext, this.caches, this.databaseManager, includedEntity, ViewControllerMode.Edition, null);

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

			switch (propertyAccessor.FieldType)
			{
				case FieldType.Boolean:
					return Carpenter.BuildBooleanField (entity, propertyAccessor, brickProperties, includeTitle);

				case FieldType.Date:
					return Carpenter.BuildDateField (entity, propertyAccessor, brickProperties, includeTitle);

				case FieldType.Decimal:
					return Carpenter.BuildDecimalField (entity, propertyAccessor, brickProperties, includeTitle);

				case FieldType.EntityCollection:
					return this.BuildEntityCollectionField (entity, propertyAccessor, brickProperties, includeTitle);

				case FieldType.EntityReference:
					return this.BuildEntityReferenceField (entity, propertyAccessor, brickProperties, includeTitle);

				case FieldType.Enumeration:
					return this.BuildEnumerationField (entity, propertyAccessor, brickProperties, includeTitle);

				case FieldType.Integer:
					return Carpenter.BuildIntegerField (entity, propertyAccessor, brickProperties, includeTitle);

				case FieldType.Text:
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
			var field = Carpenter.BuildField<BooleanField> (propertyAccessor, brickProperties, includeTitle, null);

			var entityValue = propertyAccessor.GetValue (entity);
			field.Value = ValueConverter.ConvertEntityToFieldForBool (entityValue);

			return field;
		}


		private static DateField BuildDateField(AbstractEntity entity, AbstractPropertyAccessor propertyAccessor, BrickPropertyCollection brickProperties, bool includeTitle)
		{
			var field = Carpenter.BuildField<DateField> (propertyAccessor, brickProperties, includeTitle, null);

			var entityValue = propertyAccessor.GetValue (entity);
			field.Value = ValueConverter.ConvertEntityToFieldForDate (entityValue);

			return field;
		}


		private static DecimalField BuildDecimalField(AbstractEntity entity, AbstractPropertyAccessor propertyAccessor, BrickPropertyCollection brickProperties, bool includeTitle)
		{
			var field = Carpenter.BuildField<DecimalField> (propertyAccessor, brickProperties, includeTitle, null);

			var entityValue = propertyAccessor.GetValue (entity);
			field.Value = ValueConverter.ConvertEntityToFieldForDecimal (entityValue);

			return field;
		}


		private EntityCollectionField BuildEntityCollectionField(AbstractEntity entity, AbstractPropertyAccessor propertyAccessor, BrickPropertyCollection brickProperties, bool includeTitle)
		{
			var field = Carpenter.BuildField<EntityCollectionField> (propertyAccessor, brickProperties, includeTitle, null);

			var fieldValue = (IEnumerable<AbstractEntity>) propertyAccessor.GetValue (entity);
			field.Values = fieldValue.Select (e => this.BuildEntityValue (e)).ToList ();

			var castedAccessor = (EntityCollectionPropertyAccessor) propertyAccessor;
			field.DatabaseName = this.GetDatabaseName (castedAccessor.CollectionType);

			return field;
		}


		private EntityReferenceField BuildEntityReferenceField(AbstractEntity entity, AbstractPropertyAccessor propertyAccessor, BrickPropertyCollection brickProperties, bool includeTitle)
		{
			var field = Carpenter.BuildField<EntityReferenceField> (propertyAccessor, brickProperties, includeTitle, null);

			var fieldValue = (AbstractEntity) propertyAccessor.GetValue (entity);
			field.Value = this.BuildEntityValue (fieldValue);

			field.DatabaseName = this.GetDatabaseName (propertyAccessor.Type);

			return field;
		}


		private EnumerationField BuildEnumerationField(AbstractEntity entity, AbstractPropertyAccessor propertyAccessor, BrickPropertyCollection brickProperties, bool includeTitle)
		{
			var field = Carpenter.BuildField<EnumerationField> (propertyAccessor, brickProperties, includeTitle, null);

			var entityValue = propertyAccessor.GetValue (entity);
			field.Value = ValueConverter.ConvertEntityToFieldForEnumeration (entityValue);
			field.TypeName = this.caches.TypeCache.GetId (propertyAccessor.Type);

			return field;
		}


		private static IntegerField BuildIntegerField(AbstractEntity entity, AbstractPropertyAccessor propertyAccessor, BrickPropertyCollection brickProperties, bool includeTitle)
		{
			var field = Carpenter.BuildField<IntegerField> (propertyAccessor, brickProperties, includeTitle, null);

			var entityValue = propertyAccessor.GetValue (entity);
			field.Value = ValueConverter.ConvertEntityToFieldForInteger (entityValue);

			return field;
		}


		private static AbstractField BuildTextField(AbstractEntity entity, AbstractPropertyAccessor propertyAccessor, BrickPropertyCollection brickProperties, bool includeTitle)
		{
			var entityValue = propertyAccessor.GetValue (entity);
			var fieldValue = ValueConverter.ConvertEntityToFieldForText (entityValue);

			if (StringType.IsMultilineText (propertyAccessor.Property.Type))
			{
				var field = Carpenter.BuildField<TextAreaField> (propertyAccessor, brickProperties, includeTitle, true);
				field.Value = fieldValue;

				return field;
			}
			else
			{
				var field = Carpenter.BuildField<TextField> (propertyAccessor, brickProperties, includeTitle, true);

				field.IsPassword = Carpenter.IsPassword (brickProperties);
				field.Value = fieldValue;

				return field;
			}
		}


		private static bool IsPassword(BrickPropertyCollection brickProperties)
		{
			return brickProperties.PeekAfter (BrickPropertyKey.Password, -1).HasValue;
		}


		private static T BuildField<T>(AbstractPropertyAccessor propertyAccessor, BrickPropertyCollection brickProperties, bool includeTitle, bool? overrideAllowBlank)
			where T : AbstractField, new ()
		{
			return new T ()
			{
				Id = propertyAccessor.Id,
				IsReadOnly = Carpenter.IsReadOnly (brickProperties),
				AllowBlank = overrideAllowBlank ?? propertyAccessor.Property.IsNullable,
				Title = includeTitle
					? Carpenter.GetFieldTitle (brickProperties) ?? Carpenter.GetFieldTitle (propertyAccessor)
					: null,
			};
		}


		private IEnumerable<ActionItem> BuildActionItems(AbstractEntity entity, Brick brick)
		{
			return Brick.GetProperties (brick, BrickPropertyKey.EnableAction)
				.Select (p => this.BuildActionItem (entity, p.IntValue.Value));
		}


		private ActionItem BuildActionItem(AbstractEntity entity, int viewId)
		{
			var viewMode = ViewControllerMode.Action;

			using (var controller = Mason.BuildController<IActionViewController> (this.businessContext, entity, viewMode, viewId))
			{
				return new ActionItem ()
				{
					ViewId = InvariantConverter.ToString (viewId),
					Title = controller.GetTitle ().ToString (),
				};
			}
		}


		private ActionTile BuildActionTile(Brick brick)
		{
			var tileEntity = this.entity;
			var actionBrick = Carpenter.GetBrickProperty (brick, BrickPropertyKey.DefineAction).Brick;

			return new ActionTile ()
			{
				EntityId = this.GetEntityId (tileEntity),
				IconClass = Carpenter.GetIconClass(actionBrick),
				Title = Carpenter.GetOptionalText (tileEntity, actionBrick, BrickPropertyKey.Title),
				Text = Carpenter.GetOptionalText (tileEntity, actionBrick, BrickPropertyKey.Text),
				Fields = this.BuildActionFields (tileEntity, actionBrick).ToList (),
			};
		}


		private IEnumerable<AbstractField> BuildActionFields(AbstractEntity entity, Brick brick)
		{
			return Brick.GetProperties (brick, BrickPropertyKey.Field)
				.Select ((b, i) => this.BuildActionField (entity, b.Brick, "id" + i));
		}


		private AbstractField BuildActionField(AbstractEntity entity, Brick brick, string id)
		{
			var actionFieldType = Carpenter.GetBrickProperty (brick, BrickPropertyKey.Type).TypeValue;
			var fieldType = FieldTypeSelector.GetFieldType (actionFieldType);

			switch (fieldType)
			{
				case FieldType.Boolean:
					return Carpenter.BuildBooleanField (entity, brick, actionFieldType, id);

				case FieldType.Date:
					return Carpenter.BuildDateField (entity, brick, actionFieldType, id);

				case FieldType.Decimal:
					return Carpenter.BuildDecimalField (entity, brick, actionFieldType, id);

				case FieldType.EntityCollection:
					return this.BuildEntityCollectionField (entity, brick, actionFieldType, id);

				case FieldType.EntityReference:
					return this.BuildEntityReferenceField (entity, brick, actionFieldType, id);

				case FieldType.Enumeration:
					return this.BuildEnumerationField (entity, brick, actionFieldType, id);

				case FieldType.Integer:
					return Carpenter.BuildIntegerField (entity, brick, actionFieldType, id);

				case FieldType.Text:
					return Carpenter.BuildTextField (entity, brick, id);

				default:
					throw new NotImplementedException ();
			}
		}


		private static BooleanField BuildBooleanField(AbstractEntity entity, Brick brick, Type actionFieldType, string id)
		{
			var allowBlank = actionFieldType.IsNullable ();
			var field = Carpenter.BuildField<BooleanField> (entity, brick, id, allowBlank);

			var entityValue = Carpenter.GetValue (entity, brick);
			field.Value = ValueConverter.ConvertEntityToFieldForBool (entityValue);

			return field;
		}


		private static DateField BuildDateField(AbstractEntity entity, Brick brick, Type actionFieldType, string id)
		{
			var allowBlank = actionFieldType.IsNullable ();
			var field = Carpenter.BuildField<DateField> (entity, brick, id, allowBlank);

			var entityValue = Carpenter.GetValue (entity, brick);
			field.Value = ValueConverter.ConvertEntityToFieldForDate (entityValue);

			return field;
		}


		private static DecimalField BuildDecimalField(AbstractEntity entity, Brick brick, Type actionFieldType, string id)
		{
			var allowBlank = actionFieldType.IsNullable ();
			var field = Carpenter.BuildField<DecimalField> (entity, brick, id, allowBlank);

			var entityValue = Carpenter.GetValue (entity, brick);
			field.Value = ValueConverter.ConvertEntityToFieldForDecimal (entityValue);

			return field;
		}


		private EntityCollectionField BuildEntityCollectionField(AbstractEntity entity, Brick brick, Type actionFieldType, string id)
		{
			var field = Carpenter.BuildField<EntityCollectionField> (entity, brick, id, true);

			var fieldValue = (IEnumerable<AbstractEntity>) Carpenter.GetValue (entity, brick);
			field.Values = fieldValue.Select (v => this.BuildEntityValue (v)).ToList ();

			var entityType = actionFieldType.GetGenericArguments ().Single ();
			field.DatabaseName = this.GetDatabaseName (entityType);

			return field;
		}


		private EntityReferenceField BuildEntityReferenceField(AbstractEntity entity, Brick brick, Type actionFieldType, string id)
		{
			var field = Carpenter.BuildField<EntityReferenceField> (entity, brick, id, true);

			var fieldValue = (AbstractEntity) Carpenter.GetValue (entity, brick);
			field.Value = this.BuildEntityValue (fieldValue);

			field.DatabaseName = this.GetDatabaseName (actionFieldType);

			return field;
		}


		private EnumerationField BuildEnumerationField(AbstractEntity entity, Brick brick, Type actionFieldType, string id)
		{
			var allowBlank = actionFieldType.IsNullable ();
			var field = Carpenter.BuildField<EnumerationField> (entity, brick, id, allowBlank);

			var entityValue = Carpenter.GetValue (entity, brick);
			field.Value = ValueConverter.ConvertEntityToFieldForEnumeration (entityValue);

			field.TypeName = this.caches.TypeCache.GetId (actionFieldType);

			return field;
		}


		private static IntegerField BuildIntegerField(AbstractEntity entity, Brick brick, Type actionFieldType, string id)
		{
			var allowBlank = actionFieldType.IsNullable ();
			var field = Carpenter.BuildField<IntegerField> (entity, brick, id, allowBlank);

			var entityValue = Carpenter.GetValue (entity, brick);
			field.Value = ValueConverter.ConvertEntityToFieldForInteger (entityValue);
			
			return field;
		}


		private static AbstractField BuildTextField(AbstractEntity entity, Brick brick, string id)
		{
			var entityValue = Carpenter.GetValue (entity, brick);
			var fieldValue = ValueConverter.ConvertEntityToFieldForText (entityValue);

			if (Brick.ContainsProperty (brick, BrickPropertyKey.Multiline))
			{
				var field = Carpenter.BuildField<TextAreaField> (entity, brick, id, true);
				field.Value = fieldValue;

				return field;
			}
			else
			{
				var field = Carpenter.BuildField<TextField> (entity, brick, id, true);

				field.IsPassword = Brick.ContainsProperty (brick, BrickPropertyKey.Password);
				field.Value = fieldValue;

				return field;
			}
		}


		private static object GetValue(AbstractEntity entity, Brick brick)
		{
			var property = Brick.GetProperty (brick, BrickPropertyKey.Value);

			var expressionValue = property.ExpressionValue as LambdaExpression;

			if (expressionValue != null)
			{
				return expressionValue.Compile ().DynamicInvoke (entity);
			}
			else
			{
				return property.ObjectValue;
			}
		}


		private static T BuildField<T>(AbstractEntity entity, Brick brick, string id, bool allowBlank)
			where T : AbstractField, new ()
		{
			return new T ()
			{
				Id = id,
				IsReadOnly = false,
				AllowBlank = allowBlank,
				Title = Carpenter.GetText (entity, brick, BrickPropertyKey.Title),
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
			if (entity == null || entity.IsNull ())
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


		private static string GetOptionalText(Brick brick, BrickPropertyKey key)
		{
			string result = null;

			foreach (var property in Brick.GetProperties (brick, key).Reverse ())
			{
				result = property.StringValue;

				if (result != null)
				{
					break;
				}
			}

			return result;
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
			return EntityIO.GetEntityId (this.businessContext, entity);
		}


		private static string GetIconClass(Brick brick)
		{
			var iconUri = Carpenter.GetBrickProperty (brick, BrickPropertyKey.Icon).StringValue;
			var entityType = brick.GetBrickType ();

			return Carpenter.GetIconClass (iconUri, entityType);
		}


		private static string GetIconClass(string iconUri, Type entityType)
		{
			return IconManager.GetCssClassName (iconUri, IconSize.Sixteen, entityType);
		}


		private string GetDatabaseName(Type entityType)
		{
			var commandId = this.databaseManager.GetDatabaseCommandId (entityType);

			return DataIO.DruidToString (commandId);
		}


		private readonly BusinessContext businessContext;


		private readonly Caches caches;


		private readonly DatabaseManager databaseManager;


		private readonly AbstractEntity entity;


	}


}
