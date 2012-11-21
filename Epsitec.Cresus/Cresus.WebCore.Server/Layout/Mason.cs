using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Factories;
using Epsitec.Cresus.Core.Resolvers;

using System;

using System.Collections.Generic;

using System.Linq.Expressions;


namespace Epsitec.Cresus.WebCore.Server.Layout
{


	/// <summary>
	/// The Mason is the dedicated factory of BrickWalls for the PanelBuilder class. Its single job
	/// is to creates instances of the BrickWall class based on a given entity, the controller view
	/// mode and the specific controller id.
	/// </summary>
	internal static class Mason
	{


		public static BrickWall BuildBrickWall(AbstractEntity entity, ViewControllerMode viewMode, int? viewId)
		{
			using (var controller = Mason.BuildController (entity, viewMode, viewId))
			{
				var brickWall = controller.CreateBrickWallForInspection ();

				brickWall.BrickAdded += Mason.HandleBrickAdded;
				brickWall.BrickPropertyAdded += Mason.HandleBrickPropertyAdded;

				controller.BuildBricksForInspection (brickWall);

				Mason.SetupMissingValues (brickWall);

				return brickWall;
			}
		}


		public static EntityViewController BuildController(AbstractEntity entity, ViewControllerMode viewMode, int? viewId)
		{
			var name = "js";
			var resolutionMode = ResolutionMode.InspectOnly;

			return EntityViewControllerFactory.Create (name, entity, viewMode, null, null, viewId, null, resolutionMode);
		}


		private static void HandleBrickAdded(object sender, BrickAddedEventArgs e)
		{
			var brick = e.Brick;
			var type = e.FieldType;

			Mason.CreateDefaultProperties (brick, type);
		}


		private static void HandleBrickPropertyAdded(object sender, BrickPropertyAddedEventArgs e)
		{
			if (e.Property.Key == BrickPropertyKey.OfType)
			{
				var brick = e.Property.Brick;
				var type = e.Property.Brick.GetBrickType ();
				
				Mason.CreateDefaultProperties (brick, type);
			}
		}


		private static void CreateDefaultProperties(Brick brick, Type type)
		{
			var typeInfo = EntityInfo.GetStructuredType (type);

			if (typeInfo == null || typeInfo.Caption == null)
			{
				return;
			}

			var typeName = typeInfo.Caption.Name;
			var typeIcon = typeInfo.Caption.Icon ?? "Data." + typeName;
			var labels = typeInfo.Caption.Labels;

			BrickProperty nameProperty = new BrickProperty (BrickPropertyKey.Name, typeName);
			BrickProperty iconProperty = new BrickProperty (BrickPropertyKey.Icon, typeIcon);

			Brick.AddProperty (brick, nameProperty);
			Brick.AddProperty (brick, iconProperty);

			Mason.CreateLabelProperty (brick, labels, 0, BrickPropertyKey.Title);
			Mason.CreateLabelProperty (brick, labels, 1, BrickPropertyKey.TitleCompact);
		}


		private static void CreateLabelProperty(Brick brick, IList<string> labels, int i, BrickPropertyKey key)
		{
			if (i < labels.Count)
			{
				BrickProperty property = new BrickProperty (key, labels[i]);
				
				Brick.AddProperty (brick, property);
			}
		}


		private static void SetupMissingValues(BrickWall brickWall)
		{
			foreach (var brick in brickWall.Bricks)
			{
				Mason.SetupMissingValues (brick);
			}
		}


		private static void SetupMissingValues(Brick brick)
		{
			var currentBrick = brick;

			while (currentBrick != null)
			{
				if (!Brick.ContainsProperty (currentBrick, BrickPropertyKey.OfType))
				{
					Mason.SetupBrickDefaultValues (currentBrick);
				}

				currentBrick = Brick.GetProperty (currentBrick, BrickPropertyKey.OfType).Brick;
			}
		}


		private static void SetupBrickDefaultValues(Brick brick)
		{
			if (Brick.ContainsProperty (brick, BrickPropertyKey.Template))
			{
				var templateBrick = Brick.GetProperty (brick, BrickPropertyKey.Template).Brick;
				var icon = Brick.GetProperty (brick, BrickPropertyKey.Icon).StringValue;

				Mason.AddPropertyIfNotDefined (templateBrick, BrickPropertyKey.Title, x => x.GetTitle ());
				Mason.AddPropertyIfNotDefined (templateBrick, BrickPropertyKey.Text, x => x.GetSummary ());
				Mason.AddPropertyIfNotDefined(templateBrick, BrickPropertyKey.Icon, icon);

				Mason.AddPropertyIfNotDefined (brick, BrickPropertyKey.Text, CollectionTemplate.DefaultEmptyText);
			}
			else
			{
				Mason.AddPropertyIfNotDefined (brick, BrickPropertyKey.Text, x => x.GetSummary ());
			}
		}


		private static void AddPropertyIfNotDefined(Brick brick, BrickPropertyKey key, string text)
		{
			if (!Brick.ContainsProperty (brick, key))
			{
				Brick.AddProperty (brick, new BrickProperty (key, text));
			}
		}


		private static void AddPropertyIfNotDefined(Brick brick, BrickPropertyKey key, Expression<Func<AbstractEntity, FormattedText>> expression)
		{
			if (!Brick.ContainsProperty (brick, key))
			{
				Brick.AddProperty (brick, new BrickProperty (key, expression));
			}
		}


		private static void AddPropertyIfNotDefined(Brick brick, BrickPropertyKey key, FormattedText text)
		{
			if (!Brick.ContainsProperty (brick, key))
			{
				Brick.AddProperty (brick, new BrickProperty (key, text));
			}
		}


	}


}

