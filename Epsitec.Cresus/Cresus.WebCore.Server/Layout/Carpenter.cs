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


		public static IEnumerable<AbstractTileData> BuildTileData(ViewControllerMode viewMode, BrickWall brickWall, Caches caches)
		{
			bool isFirst = true;

			foreach (var brick in brickWall.Bricks)
			{
				yield return Carpenter.BuildTileData (viewMode, brick, caches, isFirst);

				isFirst = false;
			}
		}


		private static AbstractTileData BuildTileData(ViewControllerMode viewMode, Brick brick, Caches caches, bool isFirst)
		{
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

			summaryTileData.Icon = Carpenter.GetMandatoryString (brick, BrickPropertyKey.Icon);
			summaryTileData.EntityType = brick.GetBrickType ();

			summaryTileData.TitleGetter = Carpenter.GetMandatoryTextGetter (brick, BrickPropertyKey.Title);
			summaryTileData.TextGetter = Carpenter.GetMandatoryTextGetter (brick, BrickPropertyKey.Text);

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
				?? Carpenter.GetTextGetterFromExpression<FormattedText> (brick, property.Value);
		}


		private static Func<AbstractEntity, FormattedText> GetMandatoryTextGetter(Brick brick, BrickPropertyKey key)
		{
			var property = Carpenter.GetMandatoryBrickProperty (brick, key);

			var textGetter = Carpenter.GetTextGetterFromString (property)
						  ?? Carpenter.GetTextGetterFromExpression<FormattedText> (brick, property);

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


		private static Func<AbstractEntity, T> GetTextGetterFromExpression<T>(Brick brick, BrickProperty property)
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
				Icon = Carpenter.GetMandatoryString (templateBrick, BrickPropertyKey.Icon),
				PropertyAccessor = propertyAccessor,
				TitleGetter = Carpenter.GetMandatoryTextGetter (templateBrick, BrickPropertyKey.Title),
				TextGetter = Carpenter.GetMandatoryTextGetter (templateBrick, BrickPropertyKey.Text),
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

			editionTileData.Icon = Carpenter.GetMandatoryString (brick, BrickPropertyKey.Icon);
			editionTileData.EntityType = brick.GetBrickType ();
			editionTileData.TitleGetter = Carpenter.GetMandatoryTextGetter (brick, BrickPropertyKey.Title);

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
				TitleGetter = Carpenter.GetOptionalTextGetter (brick, BrickPropertyKey.Title),
			};

			var horizontalBricks = Carpenter.BuildHorizontalFieldData (caches, brick);

			horizontalGroupData.Fields.AddRange (horizontalBricks);

			return horizontalGroupData;
		}


		private static AbstractEditionTilePartData BuildFieldData(Caches caches, BrickPropertyCollection brickProperties, BrickProperty fieldProperty)
		{
			return Carpenter.BuildFieldData (caches, brickProperties, fieldProperty, true);
		}


		private static AbstractFieldData BuildFieldData(Caches caches, BrickPropertyCollection brickProperties, BrickProperty fieldProperty, bool includeTitle)
		{
			var expression = fieldProperty.ExpressionValue;
			var lambda = (LambdaExpression) expression;

			var propertyAccessorCache = caches.PropertyAccessorCache;
			var	propertyAccessor = propertyAccessorCache.Get (lambda);

			return Carpenter.BuildFieldData (propertyAccessor, brickProperties, includeTitle);
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


		private static FormattedText GetFieldDataTitle(AbstractPropertyAccessor propertyAccessor)
		{
			var caption = EntityInfo.GetFieldCaption (propertyAccessor.Property.CaptionId);

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
					EntityGetter = Carpenter.GetTextGetterFromExpression<AbstractEntity> (brick, includeProperty)
				};
			}
		}


		private static AbstractFieldData BuildFieldData(AbstractPropertyAccessor propertyAccessor, BrickPropertyCollection brickProperties, bool includeTitle)
		{
			switch (propertyAccessor.PropertyAccessorType)
			{
				case PropertyAccessorType.Boolean:
					return Carpenter.GetBooleanFieldData (propertyAccessor, brickProperties, includeTitle);

				case PropertyAccessorType.Date:
					return Carpenter.GetDateFieldData (propertyAccessor, brickProperties, includeTitle);

				case PropertyAccessorType.Decimal:
					return Carpenter.GetDecimalFieldData (propertyAccessor, brickProperties, includeTitle);

				case PropertyAccessorType.EntityCollection:
					return Carpenter.GetEntityCollectionFieldData (propertyAccessor, brickProperties, includeTitle);

				case PropertyAccessorType.EntityReference:
					return Carpenter.GetEntityReferenceFieldData (propertyAccessor, brickProperties, includeTitle);

				case PropertyAccessorType.Enumeration:
					return Carpenter.GetEnumerationFieldData (propertyAccessor, brickProperties, includeTitle);

				case PropertyAccessorType.Integer:
					return Carpenter.GetIntegerFieldData (propertyAccessor, brickProperties, includeTitle);

				case PropertyAccessorType.Text:
					return Carpenter.GetTextFieldData (propertyAccessor, brickProperties, includeTitle);

				default:
					throw new NotImplementedException ();
			}
		}


		private static BooleanFieldData GetBooleanFieldData(AbstractPropertyAccessor propertyAccessor, BrickPropertyCollection brickProperties, bool includeTitle)
		{
			var fieldData = Carpenter.GetBasicFieldData<BooleanFieldData> (propertyAccessor, brickProperties, includeTitle);

			var castedAccessor = (BooleanPropertyAccessor) propertyAccessor;
			fieldData.ValueGetter = e => (bool?) castedAccessor.GetValue (e);

			return fieldData;
		}


		private static DateFieldData GetDateFieldData(AbstractPropertyAccessor propertyAccessor, BrickPropertyCollection brickProperties, bool includeTitle)
		{
			var fieldData = Carpenter.GetBasicFieldData<DateFieldData> (propertyAccessor, brickProperties, includeTitle);

			var castedAccessor = (DatePropertyAccessor) propertyAccessor;
			fieldData.ValueGetter = e => (string) castedAccessor.GetValue (e);

			return fieldData;
		}


		private static DecimalFieldData GetDecimalFieldData(AbstractPropertyAccessor propertyAccessor, BrickPropertyCollection brickProperties, bool includeTitle)
		{
			var fieldData = Carpenter.GetBasicFieldData<DecimalFieldData> (propertyAccessor, brickProperties, includeTitle);

			var castedAccessor = (DecimalPropertyAccessor) propertyAccessor;
			fieldData.ValueGetter = e => (decimal?) castedAccessor.GetValue (e);

			return fieldData;
		}


		private static EntityCollectionFieldData GetEntityCollectionFieldData(AbstractPropertyAccessor propertyAccessor, BrickPropertyCollection brickProperties, bool includeTitle)
		{
			var fieldData = Carpenter.GetBasicFieldData<EntityCollectionFieldData> (propertyAccessor, brickProperties, includeTitle);

			var castedAccessor = (EntityCollectionPropertyAccessor) propertyAccessor;
			fieldData.ValueGetter = e => castedAccessor.GetEntityCollection (e);
			fieldData.CollectionType = castedAccessor.CollectionType;

			return fieldData;
		}


		private static EntityReferenceFieldData GetEntityReferenceFieldData(AbstractPropertyAccessor propertyAccessor, BrickPropertyCollection brickProperties, bool includeTitle)
		{
			var fieldData = Carpenter.GetBasicFieldData<EntityReferenceFieldData> (propertyAccessor, brickProperties, includeTitle);

			var castedAccessor = (EntityReferencePropertyAccessor) propertyAccessor;
			fieldData.ValueGetter = e => castedAccessor.GetEntity (e);
			fieldData.ReferenceType = castedAccessor.Type;

			return fieldData;
		}


		private static EnumerationFieldData GetEnumerationFieldData(AbstractPropertyAccessor propertyAccessor, BrickPropertyCollection brickProperties, bool includeTitle)
		{
			var fieldData = Carpenter.GetBasicFieldData<EnumerationFieldData> (propertyAccessor, brickProperties, includeTitle);

			var castedAccessor = (EnumerationPropertyAccessor) propertyAccessor;
			fieldData.ValueGetter = e => (string) castedAccessor.GetValue (e);
			fieldData.EnumerationType = castedAccessor.Type;

			return fieldData;
		}


		private static IntegerFieldData GetIntegerFieldData(AbstractPropertyAccessor propertyAccessor, BrickPropertyCollection brickProperties, bool includeTitle)
		{
			var fieldData = Carpenter.GetBasicFieldData<IntegerFieldData> (propertyAccessor, brickProperties, includeTitle);

			var castedAccessor = (IntegerPropertyAccessor) propertyAccessor;
			fieldData.ValueGetter = e => (long?) castedAccessor.GetValue (e);

			return fieldData;
		}


		private static TextFieldData GetTextFieldData(AbstractPropertyAccessor propertyAccessor, BrickPropertyCollection brickProperties, bool includeTitle)
		{
			var fieldData = Carpenter.GetBasicFieldData<TextFieldData> (propertyAccessor, brickProperties, includeTitle);

			var castedAccessor = (TextPropertyAccessor) propertyAccessor;
			fieldData.ValueGetter = e => (string) castedAccessor.GetValue (e);
			fieldData.IsMultiline = StringType.IsMultilineText (castedAccessor.Property.Type);
			fieldData.IsPassword = Carpenter.IsFieldDataPassword (brickProperties);

			return fieldData;
		}


		private static T GetBasicFieldData<T>(AbstractPropertyAccessor propertyAccessor, BrickPropertyCollection brickProperties, bool includeTitle)
			where T : AbstractFieldData, new ()
		{
			var fieldData = new T ();

			fieldData.Id = propertyAccessor.Id;
			fieldData.IsReadOnly = Carpenter.IsFieldDataReadOnly (brickProperties);
			fieldData.AllowBlank = propertyAccessor.Property.IsNullable;
			fieldData.Title = includeTitle
				? Carpenter.GetFieldDataTitle (brickProperties) ?? Carpenter.GetFieldDataTitle (propertyAccessor)
				: FormattedText.Empty;
			
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
