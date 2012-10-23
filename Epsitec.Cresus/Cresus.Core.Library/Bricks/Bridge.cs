//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.DataLayer.Context;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Bricks.Helpers;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;


namespace Epsitec.Cresus.Core.Bricks
{
	/// <summary>
	/// The <c>Bridge</c> class is used to transform <see cref="Brick"/> definitions into
	/// <see cref="Tile"/> instances.
	/// </summary>
	public abstract partial class Bridge
	{
		protected Bridge(BridgeContext bridgeContext, EntityViewController controller)
		{
			this.bridgeContext = bridgeContext;
			this.controller = controller;
		}

		
		//public EntityViewController Controller
		//{
		//    get
		//    {
		//        return this.controller;
		//    }
		//}

		public abstract bool ContainsBricks
		{
			get;
		}

		public EntityViewController GetGenericViewController()
		{
			return this.controller;
		}

		public static TileDataType Classify(EntityViewController controller)
		{
			if (controller is Epsitec.Cresus.Core.Controllers.EditionControllers.IEditionViewController)
			{
				return TileDataType.EditableItem;
			}
			else if (controller is Epsitec.Cresus.Core.Controllers.CreationControllers.ICreationViewController)
			{
				return TileDataType.EditableItem;
			}
			else if (controller is Epsitec.Cresus.Core.Controllers.SummaryControllers.ISummaryViewController)
			{
				return TileDataType.SimpleItem;
			}
			else
			{
				return TileDataType.Undefined;
			}
		}

		public abstract void CreateTileDataItems(TileDataItems data);

		protected static void CreateDefaultProperties(Brick brick, System.Type type)
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

			BrickProperty nameProperty = new BrickProperty (BrickPropertyKey.Name, typeName).MarkAsDefaultProperty ();
			BrickProperty iconProperty = new BrickProperty (BrickPropertyKey.Icon, typeIcon).MarkAsDefaultProperty ();

			Brick.AddProperty (brick, nameProperty);
			Brick.AddProperty (brick, iconProperty);

			Bridge.CreateDefaultLabelProperty (brick, labels, 0, BrickPropertyKey.Title);
			Bridge.CreateDefaultLabelProperty (brick, labels, 1, BrickPropertyKey.TitleCompact);
		}

		protected static void CreateDefaultTextProperties(Brick brick)
		{
			if (!Brick.ContainsProperty (brick, BrickPropertyKey.Text))
			{
				Expression<System.Func<AbstractEntity, FormattedText>> expression = x => x.GetSummary ();
				Brick.AddProperty (brick, new BrickProperty (BrickPropertyKey.Text, expression).MarkAsDefaultProperty ());
			}

			if (!Brick.ContainsProperty (brick, BrickPropertyKey.TextCompact))
			{
				Expression<System.Func<AbstractEntity, FormattedText>> expression = x => x.GetCompactSummary ();
				Brick.AddProperty (brick, new BrickProperty (BrickPropertyKey.TextCompact, expression).MarkAsDefaultProperty ());
			}
		}

		protected static void CreateDefaultTitleProperties(Brick brick)
		{
			if (!Brick.ContainsProperty (brick, BrickPropertyKey.Title))
			{
				Expression<System.Func<AbstractEntity, FormattedText>> expression = x => x.GetTitle ();
				Brick.AddProperty (brick, new BrickProperty (BrickPropertyKey.Title, expression).MarkAsDefaultProperty ());
			}
		}

		protected static void CreateDefaultLabelProperty(Brick brick, IList<string> labels, int i, BrickPropertyKey key)
		{
			if (i < labels.Count)
			{
				BrickProperty property = new BrickProperty (key, labels[i]).MarkAsDefaultProperty ();
				Brick.AddProperty (brick, property);
			}
		}

		protected void HandleBrickWallBrickAdded(object sender, BrickAddedEventArgs e)
		{
			var brick = e.Brick;
			var type  = e.FieldType;

			Bridge.CreateDefaultProperties (brick, type);
		}

		protected void HandleBrickWallBrickPropertyAdded(object sender, BrickPropertyAddedEventArgs e)
		{
			var brick    = e.Brick;
			var property = e.Property;

			switch (property.Key)
			{
				case BrickPropertyKey.OfType:
					Bridge.PostProcessPropertyOfType (brick, property);
					break;

				case BrickPropertyKey.Attribute:
					Bridge.PostProcessPropertyAttribute (brick, property);
					break;
			}
		}

		private static void PostProcessPropertyOfType(Brick brick, BrickProperty property)
		{
			var type = property.Brick.GetFieldType ();
			Bridge.CreateDefaultProperties (brick, type);
		}

		private static void PostProcessPropertyAttribute(Brick brick, BrickProperty property)
		{
			var attributeValue = property.AttributeValue;

			if ((attributeValue != null) &&
				(attributeValue.ContainsValue<BrickMode> ()))
			{
				var brickMode = attributeValue.GetValue<BrickMode> ();

				if (brickMode.IsSpecialController ())
				{
					var nameProperty = Brick.GetProperty (brick, BrickPropertyKey.Name);
					var nameSuffix   = brickMode.ToString ();

					Brick.AddProperty (brick, new BrickProperty (BrickPropertyKey.Name, string.Concat (nameProperty.StringValue, ".", nameSuffix)));
				}
			}
		}

		protected void ProcessProperty(Brick brick, BrickPropertyKey key, System.Action<bool> setter)
		{
			if (Brick.ContainsProperty (brick, key))
			{
				setter (true);
			}
		}

		protected void ProcessProperty(Brick brick, BrickPropertyKey key, System.Action<string> setter)
		{
			var value = Brick.GetProperty (brick, key).StringValue;

			if (value != null)
			{
				setter (value);
			}
		}

		protected void ProcessProperty(Brick brick, BrickPropertyKey key, System.Action<Expression> setter)
		{
			var value = Brick.GetProperty (brick, key).ExpressionValue;

			if (value != null)
			{
				setter (value);
			}
		}

		protected void ProcessTemplateProperty(Brick brick, BrickPropertyKey key, System.Action<object> setter)
		{
			var expression = Brick.GetProperty (brick, key).ExpressionValue;

			if (expression == null)
			{
				return;
			}

			var expressionType = expression.GetType ();
			var function = expressionType.InvokeMember ("Compile", BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod, null, expression, null);

			if (function != null)
			{
				setter (function);
			}
		}

		protected readonly BridgeContext		bridgeContext;
		protected readonly EntityViewController	controller;
	}
}