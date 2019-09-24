//	Copyright © 2012-2019, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Factories;
using Epsitec.Cresus.Core.Resolvers;

using Epsitec.Cresus.WebCore.Server.Core.IO;

using System.Linq.Expressions;

namespace Epsitec.Cresus.WebCore.Server.Layout
{
	/// <summary>
	/// This class is responsible for finding the appropriate ViewController instances, and using
	/// them to build their brick wall.
	/// </summary>
	/// <remarks>
	/// The implementation of this class has been inspired by the Bridge class. The Bridge class
	/// has been used as a rough specification for the Mason, in how the default values and the
	/// handling of new bricks and brick properties should be handled. It is highly probable that
	/// despite my best efforts, they are sublty different and might have diverged over time.
	/// </remarks>
	internal static class Mason
	{
		public static BrickWall BuildBrickWall(BusinessContext businessContext, AbstractEntity entity, AbstractEntity additionalEntity, ViewControllerMode viewMode, ViewId viewId)
		{
			using (var controller = Mason.BuildController (businessContext, entity, additionalEntity, viewMode, viewId))
			{
				if (controller == null)
				{
					return null;
				}

				var brickWall = controller.BuildBrickWall ();

				brickWall.BrickAdded += Mason.HandleBrickAdded;
				brickWall.BrickPropertyAdded += Mason.HandleBrickPropertyAdded;

				controller.BuildBricks (brickWall);

				Mason.SetupInheritedValues (brickWall);

				return brickWall;
			}
		}

		public static EntityViewController BuildController(BusinessContext businessContext, AbstractEntity entity, AbstractEntity additionalEntity, ViewControllerMode viewMode, ViewId viewId)
		{
			var name = "js";
			var resolutionMode = ResolutionMode.NullOnError;

			return EntityViewControllerFactory.Create (name, entity, viewMode, null, null, viewId, null, resolutionMode, businessContext, additionalEntity);
		}

		public static T BuildController<T>(BusinessContext businessContext, AbstractEntity entity, AbstractEntity additionalEntity, ViewControllerMode viewMode, ViewId viewId)
			where T : class
		{
			//	Here I would simply be able to cast the controller, but it is not possible because
			//	I might want to cast it to an interface. The compiler won't let me cast it to an
			//	interface that is not in the controller type hierarchy. So I simulate the cast with
			//	the as operator and I throw an exception if the result is null.

			var controller = Mason.BuildController (businessContext, entity, additionalEntity, viewMode, viewId) as T;

			if (controller == null)
			{
				throw new System.Exception ("Controller is not of expected type.");
			}

			return controller;
		}


		private static void HandleBrickAdded(object sender, BrickAddedEventArgs e)
		{
			Mason.CreateDefaultProperties (e.Brick);
		}

		private static void HandleBrickPropertyAdded(object sender, BrickPropertyAddedEventArgs e)
		{
			var brickProperty = e.Property;
			var childBrick = brickProperty.Brick;
			var parentBrick = e.Brick;

			switch (brickProperty.Key)
			{
				case BrickPropertyKey.OfType:
					Mason.CreateDefaultProperties (childBrick);
					break;

				case BrickPropertyKey.Template:
					Mason.CopyProperties
					(
						parentBrick, childBrick, BrickPropertyKey.Icon, BrickPropertyKey.Title,
						BrickPropertyKey.Text
					);
					break;

				case BrickPropertyKey.DefineAction:
					Mason.CopyProperties (parentBrick, childBrick, BrickPropertyKey.Icon);
					break;
			}
		}


		private static void CreateDefaultProperties(Brick brick)
		{
			var type = brick.GetBrickType ();
			var structuredType = EntityInfo.GetStructuredType (type);

			if (structuredType != null || structuredType.Caption != null)
			{
				var caption = structuredType.Caption;
				var icon = caption.Icon ?? "Data." + caption.Name;

				Mason.CreateStringProperty (brick, BrickPropertyKey.Icon, icon);
				Mason.CreateLabelProperty (brick, BrickPropertyKey.Title, structuredType.Caption);
			}

			Mason.CreateExpressionProperty (brick, BrickPropertyKey.Text, x => x.GetSummary ());
		}

		private static void CreateStringProperty(Brick brick, BrickPropertyKey key, string value)
		{
			Brick.AddProperty (brick, new BrickProperty (key, value));
		}

		private static void CreateLabelProperty(Brick brick, BrickPropertyKey key, Caption caption)
		{
			var labels = caption.Labels;

			if (labels.Count > 0)
			{
				Brick.AddProperty (brick, new BrickProperty (key, labels[0]));
			}
		}

		private static void CreateExpressionProperty(Brick brick, BrickPropertyKey key, Expression<System.Func<AbstractEntity, FormattedText>> expression)
		{
			Brick.AddProperty (brick, new BrickProperty (key, expression));
		}


		private static void CopyProperties(Brick parent, Brick child, params BrickPropertyKey[] brickPropertyKeys)
		{
			foreach (var brickProperty in Brick.GetProperties (parent))
			{
				if (System.Array.IndexOf (brickPropertyKeys, brickProperty.Key) >= 0)
				{
					Brick.AddProperty (child, brickProperty);
				}
			}
		}

		private static void SetupInheritedValues(BrickWall brickWall)
		{
			foreach (var brick in brickWall.Bricks)
			{
				Mason.SetupInheritedValues (brick);
			}
		}

		private static void SetupInheritedValues(Brick brick)
		{
			var currentBrick = brick;

			while (Brick.ContainsProperty (currentBrick, BrickPropertyKey.OfType))
			{
				currentBrick = Brick.GetProperty (currentBrick, BrickPropertyKey.OfType).Brick;
			}

			if (Brick.ContainsProperty (brick, BrickPropertyKey.Template))
			{
				var templateBrick = Brick.GetProperty (brick, BrickPropertyKey.Template).Brick;

				Mason.CopyProperties (currentBrick, templateBrick, BrickPropertyKey.Attribute);
				Mason.CopyProperties (currentBrick, templateBrick, BrickPropertyKey.EnableActionMenu, BrickPropertyKey.EnableActionButton, BrickPropertyKey.EnableActionOnDrop);
			}
		}
	}
}
