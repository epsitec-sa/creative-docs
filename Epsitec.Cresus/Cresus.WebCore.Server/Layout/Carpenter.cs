using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Bricks;
using Epsitec.Cresus.Core.Controllers;

using Epsitec.Cresus.WebCore.Server.Core;
using Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor;
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


		public static IEnumerable<ITileData> BuildTileData(BrickWall brickWall, Caches caches)
		{
			bool isFirst = true;

			foreach (var brick in brickWall.Bricks)
			{
				yield return Carpenter.BuildTileData (brick, caches, isFirst);

				isFirst = false;
			}
		}


		private static ITileData BuildTileData(Brick brick, Caches caches, bool isFirst)
		{
			var viewMode = Carpenter.GetTileViewMode (brick);

			switch (viewMode)
			{
				case ViewControllerMode.Creation:
					throw new NotImplementedException ();

				case ViewControllerMode.Edition:
					return Carpenter.BuildEditionTileDataItem (brick, caches);

				case ViewControllerMode.None:
					throw new NotImplementedException ();

				case ViewControllerMode.Summary:
					return Carpenter.BuildSummaryTileDataItem (brick, caches, isFirst);

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


		private static SummaryTileData BuildSummaryTileDataItem(Brick brick, Caches caches, bool isFirst)
		{
			var summaryTileData = Carpenter.CreateSummaryTileData ();

			var summaryBrick = Carpenter.GetSummaryBrick (brick);

			Carpenter.PopulateSummaryTileData (summaryBrick, summaryTileData, caches);

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


		private static void PopulateSummaryTileData(Brick brick, SummaryTileData summaryTileData, Caches caches)
		{
			summaryTileData.EntityGetter = Carpenter.GetEntityGetter (brick);

			summaryTileData.Icon = Carpenter.GetMandatoryValue (brick, BrickPropertyKey.Icon);
			summaryTileData.EntityType = brick.GetBrickType ();

			summaryTileData.TitleGetter = Carpenter.GetMandatoryGetter (brick, BrickPropertyKey.Title);
			summaryTileData.TextGetter = Carpenter.GetMandatoryGetter (brick, BrickPropertyKey.Text);

			Carpenter.PopulateSummaryTileDataWithAttributes (brick, summaryTileData, caches);

			summaryTileData.Template = Carpenter.GetOptionalTemplate (brick, caches);
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


		private static void PopulateSummaryTileDataWithAttributes(Brick brick, SummaryTileData summaryTileData, Caches caches)
		{
			foreach (var brickMode in Carpenter.GetBrickModes (brick))
			{
				Carpenter.PupulateSummaryTileDataWithAttribute (brick, brickMode, summaryTileData, caches);
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


		private static void PupulateSummaryTileDataWithAttribute(Brick brick, BrickMode brickMode, SummaryTileData summaryTileData, Caches caches)
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
					summaryTileData.AutoCreator = caches.AutoCreatorCache.Get (brick.GetLambda ());
					break;

				default:
					if (brickMode.IsSpecialController ())
					{
						summaryTileData.SubViewId = brickMode.GetControllerSubTypeId ();
					}
					break;
			}
		}


		private static CollectionTileData GetOptionalTemplate(Brick brick, Caches caches)
		{
			var templateBrickProperty = Carpenter.GetOptionalBrickProperty (brick, BrickPropertyKey.Template);

			if (templateBrickProperty.HasValue)
			{
				var templateBrick = templateBrickProperty.Value.Brick;

				return Carpenter.GetTemplate (brick, templateBrick, caches);
			}
			else
			{
				return null;
			}
		}


		private static CollectionTileData GetTemplate(Brick brick, Brick templateBrick, Caches caches)
		{
			var lambda = brick.GetLambda ();
			var propertyAccessorCache = caches.PropertyAccessorCache;
			var propertyAccessor = (EntityCollectionPropertyAccessor) propertyAccessorCache.Get (lambda);

			return new CollectionTileData ()
			{
				EntityType = templateBrick.GetBrickType (),
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
			var templateType = brick.GetBrickType ();
			
			if (collectionType != templateType)
			{
				return e => rawEntitiesGetter (e).Where (t => templateType.IsAssignableFrom (t.GetType ()));
			}
			else
			{
				return rawEntitiesGetter;
			}
		}


		private static EditionTileData BuildEditionTileDataItem(Brick brick, Caches caches)
		{
			var editionTileData = new EditionTileData ();

			editionTileData.Icon = Carpenter.GetMandatoryValue (brick, BrickPropertyKey.Icon);
			editionTileData.EntityType = brick.GetBrickType ();
			editionTileData.TitleGetter = Carpenter.GetMandatoryGetter (brick, BrickPropertyKey.Title);

			editionTileData.Bricks.AddRange (Carpenter.BuildEditionData (brick, caches));
			editionTileData.Includes.AddRange (Carpenter.BuildIncludeData (brick));

			return editionTileData;
		}


		private static IEnumerable<AbstractEditionTilePartData> BuildEditionData(Brick brick, Caches caches)
		{
			var editionData = new List<AbstractEditionTilePartData> ();

			foreach (var property in Brick.GetAllProperties (brick))
			{
				switch (property.Key)
				{
					case BrickPropertyKey.Input:
						editionData.AddRange (Carpenter.BuildInputData (property.Brick, caches));
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


		private static IEnumerable<AbstractEditionTilePartData> BuildInputData(Brick brick, Caches caches)
		{
			var brickProperties = Brick.GetProperties (brick, BrickPropertyKey.Field, BrickPropertyKey.HorizontalGroup);

			foreach (var brickProperty in brickProperties)
			{
				switch (brickProperty.Key)
				{
					case BrickPropertyKey.HorizontalGroup:
						yield return Carpenter.BuildHorizontalGroupData (caches, brickProperty.Brick);
						break;

					case BrickPropertyKey.Field:
						yield return Carpenter.BuildFieldData (caches, brickProperties, brickProperty);
						break;
				}
			}
		}


		private static AbstractEditionTilePartData BuildHorizontalGroupData(Caches caches, Brick brick)
		{
			var horizontalGroupData = new HorizontalGroupData ()
			{
				Title = Carpenter.GetHorizontalGroupTitle (brick),
			};

			var horizontalBricks = Carpenter.BuildHorizontalFieldData (caches, brick);

			horizontalGroupData.Fields.AddRange (horizontalBricks);

			return horizontalGroupData;
		}


		private static AbstractEditionTilePartData BuildFieldData(Caches caches, BrickPropertyCollection brickProperties, BrickProperty fieldProperty)
		{
			return Carpenter.BuildFieldData (caches, brickProperties, fieldProperty, true);
		}


		private static AbstractFieldData BuildFieldData(Caches caches, BrickPropertyCollection brickProperties, BrickProperty fieldProperty, bool inculdeTitle)
		{
			var expression = fieldProperty.ExpressionValue;
			var lambda = (LambdaExpression) expression;

			var title = inculdeTitle
				? Carpenter.GetFieldDataTitle (brickProperties) ?? Carpenter.GetFieldDataTitle (expression)
				: FormattedText.Empty;

			var propertyAccessorCache = caches.PropertyAccessorCache;
			var	propertyAccessor = propertyAccessorCache.Get (lambda);
			var isReadOnly = Carpenter.IsFieldDataReadOnly (brickProperties);

			var fieldData = Carpenter.BuildFieldData (propertyAccessor, title, isReadOnly);

			if (propertyAccessor.PropertyAccessorType == PropertyAccessorType.Text)
			{
				var isPassword = Carpenter.IsFieldDataPassword (brickProperties);
				var textFieldData = (TextFieldData) fieldData;

				textFieldData.IsPassword = isPassword;
			}

			return fieldData;
		}


		private static bool IsFieldDataReadOnly(BrickPropertyCollection brickProperties)
		{
			return brickProperties.PeekAfter (BrickPropertyKey.ReadOnly, -1).HasValue;
		}


		private static bool IsFieldDataPassword(BrickPropertyCollection brickProperties)
		{
			return brickProperties.PeekAfter (BrickPropertyKey.Password, -1).HasValue;
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


		private static IEnumerable<AbstractFieldData> BuildHorizontalFieldData(Caches caches, Brick brick)
		{
			var brickProperties = Brick.GetProperties (brick, BrickPropertyKey.Field);
			
			return brickProperties.Select
			(
				b => Carpenter.BuildFieldData (caches, brickProperties, b, false)
			);
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
			var fieldData = Carpenter.GetFieldData (propertyAccessor.PropertyAccessorType);

			fieldData.Title = title;
			fieldData.IsReadOnly = isReadOnly;
			fieldData.PropertyAccessor = propertyAccessor;

			return fieldData;
		}


		private static AbstractFieldData GetFieldData(PropertyAccessorType type)
		{
			switch (type)
			{
				case PropertyAccessorType.Boolean:
					return new BooleanFieldData ();

				case PropertyAccessorType.Date:
					return new DateFieldData ();

				case PropertyAccessorType.Decimal:
					return new DecimalFieldData ();

				case PropertyAccessorType.EntityCollection:
					return new EntityCollectionFieldData ();

				case PropertyAccessorType.EntityReference:
					return new EntityReferenceFieldData ();

				case PropertyAccessorType.Enumeration:
					return new EnumerationFieldData ();

				case PropertyAccessorType.Integer:
					return new IntegerFieldData ();

				case PropertyAccessorType.Text:
					return new TextFieldData ();

				default:
					throw new NotImplementedException ();
			}
		}


	}


}
