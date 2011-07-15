//	Copyright © 2011, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Factories;
using Epsitec.Cresus.Core.Library;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Server
{
	public sealed class CoreSession : CoreApp
	{
		public CoreSession(string id)
		{
			this.id = id;
			this.creationDateTime = System.DateTime.UtcNow;
			this.coreData = this.GetComponent<CoreData> ();
		}


		public string							Id
		{
			get
			{
				return this.id;
			}
		}

		public bool								IsDisposed
		{
			get
			{
				return this.isDisposed;
			}
		}

		public CoreData							CoreData
		{
			get
			{
				return this.coreData;
			}
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


		public static BrickWall GetBrickWall(AbstractEntity entity, ViewControllerMode mode)
		{
			var controller = EntityViewControllerFactory.Create ("js", entity, mode, null, resolutionMode: Resolvers.ResolutionMode.InspectOnly);
			var brickWall  = controller.CreateBrickWallForInspection ();

			brickWall.BrickAdded += HandleBrickWallBrickAdded;
			brickWall.BrickPropertyAdded += HandleBrickWallBrickPropertyAdded;

			controller.BuildBricksForInspection (brickWall);

			return brickWall;
		}


		public BusinessContext GetBusinessContext()
		{
			if (this.businessContext == null)
			{
				this.businessContext = new BusinessContext (this.coreData);
			}

			return this.businessContext;
		}

		public void DisposeBusinessContext()
		{
			if (this.businessContext != null)
			{
				this.businessContext.Dispose ();
				this.businessContext = null;
			}
		}

		
		protected override void Dispose(bool disposing)
		{
			if (this.isDisposed == false)
			{
				this.DisposeBusinessContext ();
				this.isDisposed = true;
				base.Dispose (disposing);
			}
		}

		
		private static void HandleBrickWallBrickAdded(object sender, BrickAddedEventArgs e)
		{
			var brick = e.Brick;
			var type  = e.FieldType;

			CreateDefaultProperties (brick, type);
		}

		private static void HandleBrickWallBrickPropertyAdded(object sender, BrickPropertyAddedEventArgs e)
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


		private readonly string id;
		private readonly System.DateTime creationDateTime;

		private readonly CoreData coreData;
		
		private bool isDisposed;
		private BusinessContext businessContext;
	}
}
