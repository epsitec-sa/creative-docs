using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Bricks;
using Epsitec.Cresus.Core.Controllers;

using Epsitec.Cresus.WebCore.Server.UserInterface.TileData;

using System;

using System.Collections.Generic;

using System.Linq;
using System.Linq.Expressions;


namespace Epsitec.Cresus.WebCore.Server.UserInterface
{


	/// <summary>
	/// The goal of the Carpenter class is to transform brick walls in tiles. It takes as input the
	/// a BrickWall which is a definition for the tiles. With the brick wall and the root entity
	/// that is associated with it, it builds AbstractData objects which contain the data of the
	/// tiles that must be displayed.
	/// </summary>
	internal static class Carpenter
	{


		// NOTE The OfType property has not been tested properly. Do not expect it to work. See the
		// two bug reports below for more informations.

		
		// TODO There is a bug in the way the brick tree is processed. When looking for a property
		// of a brick, the carpenter only looks in the property list of the brick. It should also
		// look in the property lists of its parents, from the closest to the farthest until it
		// finds the requested property.
		// Fixing this bug might require some changes in the Brick implementation as not all of them
		// have a reference on their parent, and some have a reference on their parent but typed
		// with a different type. Too bad we can't override a property with a property of a derived
		// type in C#.
		// However, this is a problem only if there is an OfType brick somewhere in the tree. As we
		// don't use them in Aider, this bug is not a priority.


		// TODO I believe that there is a second bug regarding the OfType properties. With such a
		// property, the function used to resolve the entities targeted by the collection should be
		// modified by appending a call to the LINQ function IEnumerable<T>.OfType<T>() in order to
		// filter it.


		public static IEnumerable<ITileData> BuildTileData(BrickWall brickWall)
		{
			foreach (var brick in brickWall.Bricks)
			{
				yield return Carpenter.BuildTileData (brick);
			}
		}


		private static ITileData BuildTileData(Brick brick)
		{
			var mode = Carpenter.GetTileMode (brick);

			switch (mode)
			{
				case ViewControllerMode.Creation:
					throw new NotImplementedException ();

				case ViewControllerMode.Edition:
					return Carpenter.BuildEditionTileDataItem (brick);

				case ViewControllerMode.None:
					throw new NotImplementedException ();

				case ViewControllerMode.Summary:
					return Carpenter.BuildSummaryTileDataItem (brick);

				default:
					throw new NotImplementedException ();
			}
		}


		private static ViewControllerMode GetTileMode(Brick brick)
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


		private static SummaryTileData BuildSummaryTileDataItem(Brick brick)
		{
			var summaryTileData = Carpenter.CreateSummaryTileData ();

			var summaryBrick = Carpenter.GetSummaryBrick (brick);

			Carpenter.PopulateSummaryTileData (summaryBrick, summaryTileData);

			return summaryTileData;
		}


		private static SummaryTileData CreateSummaryTileData()
		{
			return new SummaryTileData ()
			{
				SubViewControllerMode = ViewControllerMode.Edition
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


		private static void PopulateSummaryTileData(Brick brick, SummaryTileData summaryTileData)
		{
			summaryTileData.EntityGetter = Carpenter.GetEntityGetter (brick);
			
			summaryTileData.Icon = Carpenter.GetMandatoryValue (brick, BrickPropertyKey.Icon);
			summaryTileData.EntityType = brick.GetFieldType ();

			summaryTileData.TitleGetter = Carpenter.GetMandatoryGetter (brick, BrickPropertyKey.Title);
			summaryTileData.TextGetter = Carpenter.GetMandatoryGetter (brick, BrickPropertyKey.Text);

			Carpenter.PopulateSummaryTileDataWithAttributes (brick, summaryTileData);

			summaryTileData.Template = Carpenter.GetOptionalTemplate (brick);
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


		private static Func<AbstractEntity, FormattedText> GetOptionalGetter(Brick brick, BrickPropertyKey key)
		{
			var property = Carpenter.GetOptionalBrickProperty (brick, key);

			Func<AbstractEntity, FormattedText> textGetter = null;

			if (property.HasValue)
			{
				textGetter = Carpenter.GetBrickValueGetterFromString (property.Value)
						  ?? Carpenter.GetBrickValueGetterFromExpression<FormattedText> (brick, property.Value);
			}

			return textGetter;
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



		private static void PopulateSummaryTileDataWithAttributes(Brick brick, SummaryTileData summaryTileData)
		{
			foreach (var brickMode in Carpenter.GetBrickModes (brick))
			{
				Carpenter.PupulateSummaryTileDataWithAttribute (brickMode, summaryTileData);
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


		private static void PupulateSummaryTileDataWithAttribute(BrickMode brickMode, SummaryTileData summaryTileData)
		{
			// TODO Implement other brick modes ?

			switch (brickMode)
			{
				case BrickMode.DefaultToSummarySubView:
					summaryTileData.SubViewControllerMode = ViewControllerMode.Summary;
					break;

				case BrickMode.HideAddButton:
					summaryTileData.HideAddButton = true;
					break;

				case BrickMode.HideRemoveButton:
					summaryTileData.HideRemoveButton = true;
					break;

				default:
					if (brickMode.IsSpecialController ())
					{
						summaryTileData.SubViewControllerSubTypeId = brickMode.GetControllerSubTypeId ();
					}
					break;
			}
		}


		private static CollectionTileData GetOptionalTemplate(Brick brick)
		{
			var templateBrickProperty = Carpenter.GetOptionalBrickProperty (brick, BrickPropertyKey.Template);

			if (templateBrickProperty.HasValue)
			{
				var templateBrick = templateBrickProperty.Value.Brick;

				return Carpenter.GetTemplate (brick, templateBrick);
			}
			else
			{
				return null;
			}
		}


		private static CollectionTileData GetTemplate(Brick brick, Brick templateBrick)
		{
			return new CollectionTileData ()
			{
				CollectionGetter = Carpenter.GetCollectionGetter(brick),
				EntityType = templateBrick.GetFieldType (),
				Icon = Carpenter.GetMandatoryValue (templateBrick, BrickPropertyKey.Icon),
				Lambda = brick.GetLambda(),
				TitleGetter = Carpenter.GetMandatoryGetter (templateBrick, BrickPropertyKey.Title),
				TextGetter = Carpenter.GetMandatoryGetter (templateBrick, BrickPropertyKey.Text),
			};
		}


		private static Func<AbstractEntity, IEnumerable<AbstractEntity>> GetCollectionGetter(Brick brick)
		{
			var resolver = brick.GetResolver (null);

			return x => (IEnumerable<AbstractEntity>) resolver.DynamicInvoke (x);
		}


		private static EditionTileData BuildEditionTileDataItem(Brick brick)
		{
			var editionTileData = new EditionTileData ();

			editionTileData.Icon = Carpenter.GetMandatoryValue (brick, BrickPropertyKey.Icon);
			editionTileData.EntityType = brick.GetFieldType ();
			editionTileData.TitleGetter = Carpenter.GetMandatoryGetter (brick, BrickPropertyKey.Title);

			editionTileData.Items.AddRange (Carpenter.BuildEditionData (brick));
			editionTileData.Includes.AddRange (Carpenter.BuildIncludeData (brick));

			return editionTileData;
		}


		private static IEnumerable<AbstractEditionTilePartData> BuildEditionData(Brick brick)
		{
			var editionData = new List<AbstractEditionTilePartData> ();

			foreach (var property in Brick.GetAllProperties (brick))
			{
				switch (property.Key)
				{
					case BrickPropertyKey.Input:
						editionData.AddRange (Carpenter.BuildInputData (property.Brick));
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


		private static IEnumerable<AbstractEditionTilePartData> BuildInputData(Brick brick)
		{
			var brickProperties = Brick.GetProperties (brick, BrickPropertyKey.Field, BrickPropertyKey.HorizontalGroup);

			foreach (var brickProperty in brickProperties)
			{
				switch (brickProperty.Key)
				{
					case BrickPropertyKey.HorizontalGroup:
						yield return Carpenter.BuildHorizontalGroupData (brickProperty.Brick);
						break;

					case BrickPropertyKey.Field:
						yield return Carpenter.BuildFieldData (brickProperties, brickProperty);
						break;
				}
			}
		}


		private static AbstractEditionTilePartData BuildHorizontalGroupData(Brick brick)
		{
			var horizontalGroupData = new HorizontalGroupData ()
			{
				Title = Carpenter.GetHorizontalGroupTitle (brick),
			};

			horizontalGroupData.Fields.AddRange (Carpenter.BuildHorizontalFieldData (brick));

			return horizontalGroupData;
		}


		private static AbstractEditionTilePartData BuildFieldData(BrickPropertyCollection brickProperties, BrickProperty fieldProperty)
		{
			Expression expression = fieldProperty.ExpressionValue;

			return new FieldData ()
			{
				IsReadOnly = Carpenter.IsFieldDataReadOnly (brickProperties),
				Lambda = (LambdaExpression) expression,
				Title = Carpenter.GetFieldDataTitle (brickProperties) ?? Carpenter.GetFieldDataTitle (expression),
			};
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


		private static IEnumerable<FieldData> BuildHorizontalFieldData(Brick brick)
		{
			var brickProperties = Brick.GetProperties (brick, BrickPropertyKey.Field);

			foreach (var brickProperty in brickProperties)
			{
				yield return new FieldData ()
				{
					Title = FormattedText.Empty,
					IsReadOnly = Carpenter.IsFieldDataReadOnly (brickProperties),
					Lambda = (LambdaExpression) brickProperty.ExpressionValue,
				};
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


	}


}
