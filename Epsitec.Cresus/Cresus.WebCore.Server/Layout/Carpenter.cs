using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Bricks;
using Epsitec.Cresus.Core.Controllers;

using Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor;
using Epsitec.Cresus.WebCore.Server.Core.PropertyAutoCreator;

using Epsitec.Cresus.WebCore.Server.Layout.TileData;

using System;

using System.Collections.Generic;

using System.Linq;
using System.Linq.Expressions;


namespace Epsitec.Cresus.WebCore.Server.Layout
{


	/// <summary>
	/// The goal of the Carpenter class is to transform brick walls in tiles. It takes as input the
	/// a BrickWall which is a definition for the tiles. With the brick wall and the root entity
	/// that is associated with it, it builds AbstractData objects which contain the data of the
	/// tiles that must be displayed.
	/// </summary>
	internal static class Carpenter
	{


		public static IEnumerable<ITileData> BuildTileData(BrickWall brickWall, PropertyAccessorCache propertyAccessorCache, AutoCreatorCache autoCreatorCache)
		{
			bool isFirst = true;

			foreach (var brick in brickWall.Bricks)
			{
				yield return Carpenter.BuildTileData (brick, propertyAccessorCache, autoCreatorCache, isFirst);

				isFirst = false;
			}
		}


		private static ITileData BuildTileData(Brick brick, PropertyAccessorCache propertyAccessorCache, AutoCreatorCache autoCreatorCache, bool isFirst)
		{
			var viewMode = Carpenter.GetTileViewMode (brick);

			switch (viewMode)
			{
				case ViewControllerMode.Creation:
					throw new NotImplementedException ();

				case ViewControllerMode.Edition:
					return Carpenter.BuildEditionTileDataItem (brick, propertyAccessorCache);

				case ViewControllerMode.None:
					throw new NotImplementedException ();

				case ViewControllerMode.Summary:
					return Carpenter.BuildSummaryTileDataItem (brick, propertyAccessorCache, autoCreatorCache, isFirst);

				default:
					throw new NotImplementedException ();
			}
		}


		private static ViewControllerMode GetTileViewMode(Brick brick)
		{
			var edition = Brick.ContainsProperty (brick, BrickPropertyKey.Include)
					   || Brick.ContainsProperty (brick, BrickPropertyKey.Input);

			if (edition)
			{
				return ViewControllerMode.Edition;
			}
			else
			{
				return ViewControllerMode.Summary;
			}
		}


		private static SummaryTileData BuildSummaryTileDataItem(Brick brick, PropertyAccessorCache propertyAccessorCache, AutoCreatorCache autoCreatorCache, bool isFirst)
		{
			var summaryTileData = Carpenter.CreateSummaryTileData ();

			var summaryBrick = Carpenter.GetSummaryBrick (brick);

			Carpenter.PopulateSummaryTileData (summaryBrick, summaryTileData, propertyAccessorCache, autoCreatorCache);

			summaryTileData.IsRoot = isFirst;

			return summaryTileData;
		}


		private static SummaryTileData CreateSummaryTileData()
		{
			return new SummaryTileData ()
			{
				SubViewMode = ViewControllerMode.Edition
			};
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


		private static void PopulateSummaryTileData(Brick brick, SummaryTileData summaryTileData, PropertyAccessorCache propertyAccessorCache, AutoCreatorCache autoCreatorCache)
		{
			summaryTileData.EntityGetter = Carpenter.GetEntityGetter (brick);

			summaryTileData.Icon = Carpenter.GetMandatoryValue (brick, BrickPropertyKey.Icon);
			summaryTileData.EntityType = brick.GetFieldType ();

			summaryTileData.TitleGetter = Carpenter.GetMandatoryGetter (brick, BrickPropertyKey.Title);
			summaryTileData.TextGetter = Carpenter.GetMandatoryGetter (brick, BrickPropertyKey.Text);

			Carpenter.PopulateSummaryTileDataWithAttributes (brick, summaryTileData, autoCreatorCache);

			summaryTileData.Template = Carpenter.GetOptionalTemplate (brick, propertyAccessorCache);
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


		private static string GetMandatoryValue(Brick brick, BrickPropertyKey key)
		{
			return Carpenter.GetMandatoryBrickProperty (brick, key).StringValue;
		}


		private static Func<AbstractEntity, FormattedText> GetMandatoryGetter(Brick brick, BrickPropertyKey key)
		{
			var property = Carpenter.GetMandatoryBrickProperty (brick, key);

			var textGetter = Carpenter.GetBrickValueGetterFromString (property)
						  ?? Carpenter.GetBrickValueGetterFromExpression<FormattedText> (brick, property);

			if (textGetter == null)
			{
				throw new InvalidOperationException ("Text should have a value");
			}

			return textGetter;
		}


		private static Func<AbstractEntity, FormattedText> GetBrickValueGetterFromString(BrickProperty property)
		{
			Func<AbstractEntity, FormattedText> textGetter = null;

			var stringValue = property.StringValue;

			if (stringValue != null)
			{
				textGetter = _ => (FormattedText) stringValue;
			}

			return textGetter;
		}


		private static Func<AbstractEntity, T> GetBrickValueGetterFromExpression<T>(Brick brick, BrickProperty property)
		{
			var expressionValue = property.ExpressionValue as LambdaExpression;

			Func<AbstractEntity, T> textGetter = null;

			if (expressionValue != null)
			{
				var expression = expressionValue.Compile ();

				Func<AbstractEntity, T> rawTextGetter = x => (T) expression.DynamicInvoke (x);

				var resolver = brick.GetResolver (null);

				if (resolver == null)
				{
					textGetter = rawTextGetter;
				}
				else
				{
					textGetter = x => rawTextGetter ((AbstractEntity) resolver.DynamicInvoke (x));
				}
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


		private static void PopulateSummaryTileDataWithAttributes(Brick brick, SummaryTileData summaryTileData, AutoCreatorCache autoCreatorCache)
		{
			foreach (var brickMode in Carpenter.GetBrickModes (brick))
			{
				Carpenter.PupulateSummaryTileDataWithAttribute (brick, brickMode, summaryTileData, autoCreatorCache);
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


		private static void PupulateSummaryTileDataWithAttribute(Brick brick, BrickMode brickMode, SummaryTileData summaryTileData, AutoCreatorCache autoCreatorCache)
		{
			// TODO Implement other brick modes ?

			switch (brickMode)
			{
				case BrickMode.DefaultToSummarySubView:
					summaryTileData.SubViewMode = ViewControllerMode.Summary;
					break;

				case BrickMode.HideAddButton:
					summaryTileData.HideAddButton = true;
					break;

				case BrickMode.HideRemoveButton:
					summaryTileData.HideRemoveButton = true;
					break;

				case BrickMode.AutoCreateNullEntity:
					summaryTileData.AutoCreator = autoCreatorCache.Get (brick.GetLambda ());
					break;

				default:
					if (brickMode.IsSpecialController ())
					{
						summaryTileData.SubViewId = brickMode.GetControllerSubTypeId ();
					}
					break;
			}
		}


		private static CollectionTileData GetOptionalTemplate(Brick brick, PropertyAccessorCache propertyAccessorCache)
		{
			var templateBrickProperty = Carpenter.GetOptionalBrickProperty (brick, BrickPropertyKey.Template);

			if (templateBrickProperty.HasValue)
			{
				var templateBrick = templateBrickProperty.Value.Brick;

				return Carpenter.GetTemplate (brick, templateBrick, propertyAccessorCache);
			}
			else
			{
				return null;
			}
		}


		private static CollectionTileData GetTemplate(Brick brick, Brick templateBrick, PropertyAccessorCache propertyAccessorCache)
		{
			var lambda = brick.GetLambda ();
			var propertyAccessor = (EntityCollectionPropertyAccessor) propertyAccessorCache.Get (lambda);

			return new CollectionTileData ()
			{
				EntityType = templateBrick.GetFieldType (),
				EntitiesGetter = Carpenter.GetEntitiesGetter (brick, propertyAccessor),
				Icon = Carpenter.GetMandatoryValue (templateBrick, BrickPropertyKey.Icon),
				PropertyAccessor = propertyAccessor,
				TitleGetter = Carpenter.GetMandatoryGetter (templateBrick, BrickPropertyKey.Title),
				TextGetter = Carpenter.GetMandatoryGetter (templateBrick, BrickPropertyKey.Text),
			};
		}


		private static Func<AbstractEntity, IEnumerable<AbstractEntity>> GetEntitiesGetter(Brick brick, EntityCollectionPropertyAccessor propertyAccessor)
		{
			Func<AbstractEntity, IEnumerable<AbstractEntity>> rawEntitiesGetter = e =>
			{
				return (IEnumerable<AbstractEntity>) propertyAccessor.GetCollection (e);
			};

			var collectionType = propertyAccessor.CollectionType;
			var templateType = brick.GetFieldType ();
			
			if (collectionType != templateType)
			{
				return e => rawEntitiesGetter (e).Where (t => templateType.IsAssignableFrom (t.GetType ()));
			}
			else
			{
				return rawEntitiesGetter;
			}
		}


		private static EditionTileData BuildEditionTileDataItem(Brick brick, PropertyAccessorCache propertyAccessorCache)
		{
			var editionTileData = new EditionTileData ();

			editionTileData.Icon = Carpenter.GetMandatoryValue (brick, BrickPropertyKey.Icon);
			editionTileData.EntityType = brick.GetFieldType ();
			editionTileData.TitleGetter = Carpenter.GetMandatoryGetter (brick, BrickPropertyKey.Title);

			editionTileData.Items.AddRange (Carpenter.BuildEditionData (brick, propertyAccessorCache));
			editionTileData.Includes.AddRange (Carpenter.BuildIncludeData (brick));

			return editionTileData;
		}


		private static IEnumerable<AbstractEditionTilePartData> BuildEditionData(Brick brick, PropertyAccessorCache propertyAccessorCache)
		{
			var editionData = new List<AbstractEditionTilePartData> ();

			foreach (var property in Brick.GetAllProperties (brick))
			{
				switch (property.Key)
				{
					case BrickPropertyKey.Input:
						editionData.AddRange (Carpenter.BuildInputData (property.Brick, propertyAccessorCache));
						break;

					case BrickPropertyKey.Separator:
						editionData.Add (Carpenter.BuildSeparatorData ());
						break;

					case BrickPropertyKey.GlobalWarning:
						editionData.Add (Carpenter.BuildGlobalWarningData ());
						break;

					default:
						// Nothing to do here. We simply ignore the property.
						break;
				}
			}

			return editionData;
		}


		private static AbstractEditionTilePartData BuildSeparatorData()
		{
			return new SeparatorData ();
		}


		private static AbstractEditionTilePartData BuildGlobalWarningData()
		{
			return new GlobalWarningData ();
		}


		private static IEnumerable<AbstractEditionTilePartData> BuildInputData(Brick brick, PropertyAccessorCache propertyAccessorCache)
		{
			var brickProperties = Brick.GetProperties (brick, BrickPropertyKey.Field, BrickPropertyKey.HorizontalGroup);

			foreach (var brickProperty in brickProperties)
			{
				switch (brickProperty.Key)
				{
					case BrickPropertyKey.HorizontalGroup:
						yield return Carpenter.BuildHorizontalGroupData (brickProperty.Brick, propertyAccessorCache);
						break;

					case BrickPropertyKey.Field:
						yield return Carpenter.BuildFieldData (propertyAccessorCache, brickProperties, brickProperty);
						break;
				}
			}
		}


		private static AbstractEditionTilePartData BuildHorizontalGroupData(Brick brick, PropertyAccessorCache propertyAccessorCache)
		{
			var horizontalGroupData = new HorizontalGroupData ()
			{
				Title = Carpenter.GetHorizontalGroupTitle (brick),
			};

			horizontalGroupData.Fields.AddRange (Carpenter.BuildHorizontalFieldData (brick, propertyAccessorCache));

			return horizontalGroupData;
		}


		private static AbstractEditionTilePartData BuildFieldData(PropertyAccessorCache propertyAccessorCache, BrickPropertyCollection brickProperties, BrickProperty fieldProperty)
		{
			var expression = fieldProperty.ExpressionValue;
			var lambda = (LambdaExpression) expression;
			
			var title = Carpenter.GetFieldDataTitle (brickProperties) ?? Carpenter.GetFieldDataTitle (expression);
			var	propertyAccessor = propertyAccessorCache.Get (lambda);
			var isReadOnly = Carpenter.IsFieldDataReadOnly (brickProperties);

			return Carpenter.BuildFieldData (propertyAccessor, title, isReadOnly);
		}


		private static bool IsFieldDataReadOnly(BrickPropertyCollection brickProperties)
		{
			return brickProperties.PeekAfter (BrickPropertyKey.ReadOnly, -1).HasValue;
		}


		private static FormattedText? GetFieldDataTitle(BrickPropertyCollection brickProperties)
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


		private static FormattedText GetFieldDataTitle(Expression expression)
		{
			var caption = EntityInfo.GetFieldCaption (expression);

			FormattedText title = FormattedText.Empty;

			if (caption != null)
			{
				if (caption.HasLabels)
				{
					title = caption.DefaultLabel;
				}
				else
				{
					title = caption.Description ?? caption.Name;
				}
			}

			return title;
		}


		private static FormattedText GetHorizontalGroupTitle(Brick brick)
		{
			var titleProperty = Carpenter.GetOptionalBrickProperty (brick, BrickPropertyKey.Title);

			if (titleProperty.HasValue)
			{
				return titleProperty.Value.StringValue;
			}
			else
			{
				return FormattedText.Empty;
			}
		}


		private static IEnumerable<AbstractFieldData> BuildHorizontalFieldData(Brick brick, PropertyAccessorCache propertyAccessorCache)
		{
			var brickProperties = Brick.GetProperties (brick, BrickPropertyKey.Field);

			foreach (var brickProperty in brickProperties)
			{
				var lambda = (LambdaExpression) brickProperty.ExpressionValue;

				var title = FormattedText.Empty;
				var propertyAccessor = propertyAccessorCache.Get (lambda);
				var isReadOnly = Carpenter.IsFieldDataReadOnly (brickProperties);

				yield return Carpenter.BuildFieldData (propertyAccessor, title, isReadOnly);
			}
		}


		private static IEnumerable<IncludeData> BuildIncludeData(Brick brick)
		{
			foreach (var includeProperty in Brick.GetProperties (brick, BrickPropertyKey.Include))
			{
				yield return new IncludeData ()
				{
					EntityGetter = Carpenter.GetBrickValueGetterFromExpression<AbstractEntity> (brick, includeProperty)
				};
			}
		}


		private static AbstractFieldData BuildFieldData(AbstractPropertyAccessor propertyAccessor, FormattedText title, bool isReadOnly)
		{
			var fieldData = Carpenter.GetFieldData (propertyAccessor.FieldType);

			fieldData.Title = title;
			fieldData.IsReadOnly = isReadOnly;
			fieldData.PropertyAccessor = propertyAccessor;

			return fieldData;
		}


		private static AbstractFieldData GetFieldData(FieldType fieldType)
		{
			switch (fieldType)
			{
				case FieldType.CheckBox:
					return new CheckboxFieldData ();

				case FieldType.Date:
					return new DateFieldData ();

				case FieldType.Decimal:
					return new DecimalFieldData ();

				case FieldType.EntityCollection:
					return new EntityCollectionFieldData ();

				case FieldType.EntityReference:
					return new EntityReferenceFieldData ();

				case FieldType.Enumeration:
					return new EnumerationFieldData ();

				case FieldType.Integer:
					return new IntegerFieldData ();

				case FieldType.Text:
					return new TextFieldData ();

				default:
					throw new NotImplementedException ();
			}
		}


	}


}
