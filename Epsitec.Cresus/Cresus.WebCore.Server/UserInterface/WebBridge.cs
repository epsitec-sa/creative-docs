using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Bricks;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Factories;
using Epsitec.Cresus.Core.Resolvers;

using System;

using System.Collections.Generic;

using System.Diagnostics;

using System.Linq;
using System.Linq.Expressions;



namespace Epsitec.Cresus.WebCore.Server.UserInterface
{
	
	
	internal static class WebBridge
	{


		// Similar to Bridge<T>.CreateTileDataItem(...)
		public static Brick ProcessBrick(Brick brick, WebTileDataItem item)
		{
			Brick oldBrick;

			do
			{
				if (Brick.ContainsProperty (brick, BrickPropertyKey.OfType))
				{

				}
				else if (Brick.ContainsProperty (brick, BrickPropertyKey.Template))
				{
					//	Don't produce default text properties for bricks which contain AsType
					//	or Template bricks. Instead, specify the default empty text.

					var templateBrick = Brick.GetProperty (brick, BrickPropertyKey.Template).Brick;

					Debug.Assert (templateBrick != null);

					if ((!Brick.ContainsProperty (templateBrick, BrickPropertyKey.Title)) &&
						(!Brick.ContainsProperty (templateBrick, BrickPropertyKey.TitleCompact)))
					{
						WebBridge.CreateDefaultTitleProperties (templateBrick);
					}

					if ((!Brick.ContainsProperty (templateBrick, BrickPropertyKey.Text)) &&
						(!Brick.ContainsProperty (templateBrick, BrickPropertyKey.TextCompact)))
					{
						WebBridge.CreateDefaultTextProperties (templateBrick);
					}

					if (!Brick.ContainsProperty (brick, BrickPropertyKey.Text))
					{
						Brick.AddProperty (brick, new BrickProperty (BrickPropertyKey.Text, CollectionTemplate.DefaultEmptyText));
					}
				}
				else
				{
					WebBridge.CreateDefaultTextProperties (brick);
				}

				WebBridge.ProcessProperty (brick, BrickPropertyKey.Name, x => item.Name = x);
				WebBridge.ProcessProperty (brick, BrickPropertyKey.Icon, x => item.IconUri = x);

				WebBridge.ProcessProperty (brick, BrickPropertyKey.Title, x => item.Title = x);
				WebBridge.ProcessProperty (brick, BrickPropertyKey.TitleCompact, x => item.CompactTitle = x);
				WebBridge.ProcessProperty (brick, BrickPropertyKey.Text, x => item.Text = x);
				WebBridge.ProcessProperty (brick, BrickPropertyKey.TextCompact, x => item.CompactText = x);

				WebBridge.ProcessProperty (brick, BrickPropertyKey.Attribute, x => WebBridge.ProcessAttribute (item, x));

				if ((!item.Title.IsNullOrEmpty) && 
					(item.CompactTitle.IsNull))
				{
					item.CompactTitle = item.Title;
				}

				// I've no idea where this comes from. It might be something totally outdated, but
				// I'm not sure.
				if (Brick.ContainsProperty (brick, BrickPropertyKey.CollectionAnnotation))
				{
					item.DataType = TileDataType.CollectionItem;
				}

				oldBrick = brick;
				brick = Brick.GetProperty (brick, BrickPropertyKey.OfType).Brick;
			} while (brick != null);

			return oldBrick;
		}


		public static BrickWall GetBrickWall(AbstractEntity entity, ViewControllerMode mode)
		{
			using (var controller = EntityViewControllerFactory.Create ("js", entity, mode, null, null, resolutionMode: ResolutionMode.InspectOnly))
			{
				var brickWall = controller.CreateBrickWallForInspection ();
				
				brickWall.BrickAdded += WebBridge.HandleBrickWallBrickAdded;
				brickWall.BrickPropertyAdded += WebBridge.HandleBrickWallBrickPropertyAdded;
				
				controller.BuildBricksForInspection (brickWall);
				
				return brickWall;
			}
		}


		private static void CreateDefaultProperties(Brick brick, Type type)
		{
			var typeInfo = EntityInfo.GetStructuredType (type) as StructuredType;

			if ((typeInfo == null) ||
				(typeInfo.Caption == null))
			{
				return;
			}

			var typeName = typeInfo.Caption.Name;
			var typeIcon = typeInfo.Caption.Icon ?? "Data." + typeName;
			var labels   = typeInfo.Caption.Labels;

			BrickProperty nameProperty = new BrickProperty (BrickPropertyKey.Name, typeName);
			BrickProperty iconProperty = new BrickProperty (BrickPropertyKey.Icon, typeIcon);

			Brick.AddProperty (brick, nameProperty);
			Brick.AddProperty (brick, iconProperty);

			WebBridge.CreateLabelProperty (brick, labels, 0, BrickPropertyKey.Title);
			WebBridge.CreateLabelProperty (brick, labels, 1, BrickPropertyKey.TitleCompact);
		}


		private static void CreateDefaultTextProperties(Brick brick)
		{
			if (!Brick.ContainsProperty (brick, BrickPropertyKey.Text))
			{
				Expression<Func<AbstractEntity, FormattedText>> expression = x => x.GetSummary ();
				Brick.AddProperty (brick, new BrickProperty (BrickPropertyKey.Text, expression));
			}

			if (!Brick.ContainsProperty (brick, BrickPropertyKey.TextCompact))
			{
				Expression<Func<AbstractEntity, FormattedText>> expression = x => x.GetCompactSummary ();
				Brick.AddProperty (brick, new BrickProperty (BrickPropertyKey.TextCompact, expression));
			}
		}


		private static void CreateDefaultTitleProperties(Brick brick)
		{
			if (Brick.ContainsProperty (brick, BrickPropertyKey.Title))
			{
				return;
			}

			var fieldType = brick.GetFieldType ();
			var methodInfo = fieldType.GetMethod ("GetTitle");

			Expression<Func<AbstractEntity, FormattedText>> expression = x => (FormattedText) methodInfo.Invoke (x, new object[0]);
			Brick.AddProperty (brick, new BrickProperty (BrickPropertyKey.Title, expression));
		}


		private static void CreateLabelProperty(Brick brick, IList<string> labels, int i, BrickPropertyKey key)
		{
			if (i < labels.Count)
			{
				BrickProperty property = new BrickProperty (key, labels[i]);
				Brick.AddProperty (brick, property);
			}
		}


		private static void HandleBrickWallBrickAdded(object sender, BrickAddedEventArgs e)
		{
			var brick = e.Brick;
			var type  = e.FieldType;

			WebBridge.CreateDefaultProperties (brick, type);
		}


		private static void HandleBrickWallBrickPropertyAdded(object sender, BrickPropertyAddedEventArgs e)
		{
			var brick    = e.Brick;
			var property = e.Property;

			if (property.Key == BrickPropertyKey.OfType)
			{
				var type = property.Brick.GetFieldType ();
				WebBridge.CreateDefaultProperties (brick, type);
			}
		}


		private static void ProcessProperty(Brick brick, BrickPropertyKey key, Action<string> setter)
		{
			var value = Brick.GetProperty (brick, key).StringValue;

			if (value != null)
			{
				setter (value);
			}
		}


		private static void ProcessProperty(Brick brick, BrickPropertyKey key, Action<BrickMode> setter)
		{
			foreach (var attributeValue in Brick.GetProperties (brick, key).Select (x => x.AttributeValue))
			{
				if ((attributeValue != null) &&
					(attributeValue.ContainsValue<BrickMode> ()))
				{
					setter (attributeValue.GetValue<BrickMode> ());
				}
			}
		}


		private static void ProcessAttribute(WebTileDataItem item, BrickMode value)
		{
			switch (value)
			{
				case BrickMode.AutoGroup:
					item.AutoGroup = true;
					break;

				case BrickMode.DefaultToSummarySubView:
					item.DefaultMode = ViewControllerMode.Summary;
					break;

				case BrickMode.HideAddButton:
					item.HideAddButton = true;
					break;

				case BrickMode.FullHeightStretch:
					item.FullHeightStretch = true;
					break;

				case BrickMode.HideRemoveButton:
					item.HideRemoveButton = true;
					break;
			}
		}


	}


}
