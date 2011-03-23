//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Cresus.Bricks;

using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Epsitec.Cresus.Core.Controllers.DataAccessors
{
	/// <summary>
	/// The <c>Bridge</c> class is used to transform <see cref="Brick"/> definitions into
	/// <see cref="Tile"/> instances.
	/// </summary>
	public abstract class Bridge
	{
		protected Bridge(EntityViewController controller)
		{
			this.controller = controller;
		}

		public EntityViewController Controller
		{
			get
			{
				return this.controller;
			}
		}

		public abstract bool ContainsBricks
		{
			get;
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

			BrickProperty nameProperty = new BrickProperty (BrickPropertyKey.Name, typeName);
			BrickProperty iconProperty = new BrickProperty (BrickPropertyKey.Icon, typeIcon);

			Brick.AddProperty (brick, nameProperty);
			Brick.AddProperty (brick, iconProperty);

			Bridge.CreateLabelProperty (brick, labels, 0, BrickPropertyKey.Title);
			Bridge.CreateLabelProperty (brick, labels, 1, BrickPropertyKey.TitleCompact);
		}

		protected static void CreateDefaultTextProperties(Brick brick)
		{
			if (!Brick.ContainsProperty (brick, BrickPropertyKey.Text))
			{
				Expression<System.Func<AbstractEntity, FormattedText>> expression = x => x.GetSummary ();
				Brick.AddProperty (brick, new BrickProperty (BrickPropertyKey.Text, expression));
			}

			if (!Brick.ContainsProperty (brick, BrickPropertyKey.TextCompact))
			{
				Expression<System.Func<AbstractEntity, FormattedText>> expression = x => x.GetCompactSummary ();
				Brick.AddProperty (brick, new BrickProperty (BrickPropertyKey.TextCompact, expression));
			}
		}

		protected static void CreateLabelProperty(Brick brick, IList<string> labels, int i, BrickPropertyKey key)
		{
			if (i < labels.Count)
			{
				BrickProperty property = new BrickProperty (key, labels[i]);
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

			if (property.Key == BrickPropertyKey.AsType)
			{
				var type = property.Brick.GetFieldType ();
				Bridge.CreateDefaultProperties (brick, type);
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


		private readonly EntityViewController controller;
	}
}
