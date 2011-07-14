//	Copyright © 2008-2011, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Factories;
using Epsitec.Cresus.Core.Library;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Types;

namespace Epsitec.Cresus.Core
{
	public sealed class CoreSession : CoreApp
	{
		public static BrickWall GetBrickWall(AbstractEntity entity, ViewControllerMode mode)
		{
			var controller = EntityViewControllerFactory.Create ("js", entity, mode, null, resolutionMode: Resolvers.ResolutionMode.InspectOnly);
			var brickWall  = controller.CreateBrickWallForInspection ();

			brickWall.BrickAdded += HandleBrickWallBrickAdded;
			brickWall.BrickPropertyAdded += HandleBrickWallBrickPropertyAdded;

			controller.BuildBricksForInspection (brickWall);

			return brickWall;
		}

		public override string ApplicationIdentifier
		{
			get
			{
				return "CoreSession";
			}
		}

		public override string ShortWindowTitle
		{
			get
			{
				throw new System.NotImplementedException ();
			}
		}

		protected static void HandleBrickWallBrickAdded(object sender, BrickAddedEventArgs e)
		{
			var brick = e.Brick;
			var type  = e.FieldType;

			CreateDefaultProperties (brick, type);
		}

		protected static void HandleBrickWallBrickPropertyAdded(object sender, BrickPropertyAddedEventArgs e)
		{
			var brick    = e.Brick;
			var property = e.Property;

			if (property.Key == BrickPropertyKey.AsType)
			{
				var type = property.Brick.GetFieldType ();
				CreateDefaultProperties (brick, type);
			}
		}



		private static void CreateDefaultProperties(Brick brick, System.Type type)
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

			CreateLabelProperty (brick, labels, 0, BrickPropertyKey.Title);
			CreateLabelProperty (brick, labels, 1, BrickPropertyKey.TitleCompact);
		}

		private static void CreateLabelProperty(Brick brick, IList<string> labels, int i, BrickPropertyKey key)
		{
			if (i < labels.Count)
			{
				BrickProperty property = new BrickProperty (key, labels[i]);
				Brick.AddProperty (brick, property);
			}
		}
	}
}
