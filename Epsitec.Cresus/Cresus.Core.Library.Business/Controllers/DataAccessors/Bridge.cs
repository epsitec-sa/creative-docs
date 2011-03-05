//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.DataLayer.Context;

namespace Epsitec.Cresus.Core.Controllers.DataAccessors
{
	public class Bridge<T>
		where T : AbstractEntity, new ()
	{
		public Bridge(EntityViewController<T> controller)
		{
			this.controller = controller;
		}

		public BrickWall<T> CreateBrickWall()
		{
			var wall = new BrickWall<T> ();

			wall.BrickAdded += this.HandleBrickWallBrickAdded;
			wall.BrickPropertyAdded += this.HandleBrickWallBrickPropertyAdded;

			return wall;
		}
		
		public TileDataItem CreateTileDataItem(TileDataItems data, Brick brick)
		{
			var item = new TileDataItem ();
			var root = brick;


		again:
			if (Brick.ContainsProperty (brick, BrickPropertyKey.AsType))
			{

			}
			else if (Brick.ContainsProperty (brick, BrickPropertyKey.Template))
			{
				//	Don't produce default text properties for bricks which contain AsType
				//	or Template bricks.

				if (!Brick.ContainsProperty (brick, BrickPropertyKey.Text))
				{
					Brick.AddProperty (brick, new BrickProperty (BrickPropertyKey.Text, CollectionTemplate.DefaultEmptyText));
				}
			}
			else
			{
				Bridge<T>.CreateDefaultTextProperties (brick);
			}

			this.ProcessProperty (brick, BrickPropertyKey.Name, x => item.Name = x);
			this.ProcessProperty (brick, BrickPropertyKey.Icon, x => item.IconUri = x);
			
			this.ProcessProperty (brick, BrickPropertyKey.Title, x => item.Title = x);
			this.ProcessProperty (brick, BrickPropertyKey.TitleCompact, x => item.CompactTitle = x);
			this.ProcessProperty (brick, BrickPropertyKey.Text, x => item.Text = x);
			this.ProcessProperty (brick, BrickPropertyKey.TextCompact, x => item.CompactText = x);

			this.ProcessProperty (brick, BrickPropertyKey.AutoGroup, x => item.AutoGroup = x);

			if ((!item.Title.IsNullOrEmpty) &&
				(item.CompactTitle.IsNull))
			{
				item.CompactTitle = item.Title;
			}

			this.ProcessProperty (brick, BrickPropertyKey.Title, x => item.TitleAccessor = x);
			this.ProcessProperty (brick, BrickPropertyKey.TitleCompact, x => item.CompactTitleAccessor = x);
			this.ProcessProperty (brick, BrickPropertyKey.Text, x => item.TextAccessor = x);
			this.ProcessProperty (brick, BrickPropertyKey.TextCompact, x => item.CompactTextAccessor = x);

			Brick asTypeBrick = Brick.GetProperty (brick, BrickPropertyKey.AsType).Brick;

			if (asTypeBrick != null)
			{
				brick = asTypeBrick;
				goto again;
			}

			Brick templateBrick = Brick.GetProperty (brick, BrickPropertyKey.Template).Brick;

			if (templateBrick != null)
			{
				data.Add (item);
				this.ProcessTemplate (data, item, root, templateBrick);
			}
			else if (Brick.ContainsProperty (brick, BrickPropertyKey.Input))
			{
				this.ProcessInputs (data, item, root, brick);
				data.Add (item);
			}
			else
			{
				item.EntityMarshaler = this.controller.CreateEntityMarshaler ();
				data.Add (item);
			}

			return item;
		}

		private void ProcessInputs(TileDataItems data, TileDataItem item, Brick root, Brick brick)
		{
			var properties = Brick.GetProperties (brick, BrickPropertyKey.Input, BrickPropertyKey.Separator);
			var actions    = new List<System.Action<EditionTile, UIBuilder>> ();

			foreach (var property in properties)
			{
				switch (property.Key)
				{
					case BrickPropertyKey.Input:
						actions.AddRange (this.CreateActionsForInput (data, item, root, property.Brick).Where (x => x != null));
						break;
					case BrickPropertyKey.Separator:
						actions.Add (this.CreateActionForProcessSeparator (data, item, root));
						break;
				}
			}

			item.CreateEditionUI =
				(tile, builder)
				=>
				{
					actions.ForEach (x => x (tile, builder));
				};
		}

		private System.Action<EditionTile, UIBuilder> CreateActionForProcessSeparator(TileDataItems data, TileDataItem item, Brick root)
		{
			return (tile, builder) => builder.CreateMargin (tile, horizontalSeparator: true);
		}

		private IEnumerable<System.Action<EditionTile, UIBuilder>> CreateActionsForInput(TileDataItems data, TileDataItem item, Brick root, Brick brick)
		{
			var properties = Brick.GetProperties (brick, BrickPropertyKey.Field);
			
			foreach (var property in properties)
			{
				switch (property.Key)
				{
					case BrickPropertyKey.Field:
						yield return this.CreateActionForInputField (data, item, root, brick, property.ExpressionValue, properties);
						break;
				}
			}
		}

		private System.Action<EditionTile, UIBuilder> CreateActionForInputField(TileDataItems data, TileDataItem item, Brick root, Brick brick, Expression expression, BrickPropertyCollection properties)
		{
			var controller = this.controller;
			var business   = this.controller.BusinessContext;

			LambdaExpression lambda = expression as LambdaExpression;

			if (lambda == null)
			{
				throw new System.ArgumentException (string.Format ("Expression {0} for input must be a lambda", expression.ToString ()));
			}

			var fieldType  = lambda.ReturnType;
			var entityType = typeof (AbstractEntity);

			int width = Bridge<T>.GetWidth (properties);

			if ((fieldType.IsClass) &&
				(entityType.IsAssignableFrom (fieldType)))
			{
				//	The field is an entity : use an AutoCompleteTextField for it.

				var factory = DynamicFactories.AutoCompleteTextFieldDynamicFactory.Create<T> (lambda, business, this.controller.EntityGetter);
				return (tile, builder) => factory.CreateUI (tile, builder);
			}

			if ((fieldType == typeof (string)) ||
				(fieldType == typeof (Date)))
			{
				var factory = DynamicFactories.TextFieldDynamicFactory.Create<T> (lambda, business, this.controller.EntityGetter, width);
				return (tile, builder) => factory.CreateUI (tile, builder);
			}

			return null;
		}

		private static int GetWidth(BrickPropertyCollection properties)
		{
			var widthProperty = properties.Peek (BrickPropertyKey.Width);

			if (widthProperty.HasValue)
			{
				return widthProperty.Value.IntValue.GetValueOrDefault (0);
			}
			else
			{
				return 0;
			}
		}

		private static object DynamicCreateSelectionController(System.Type type, BusinessContext context)
		{
			var controllerType = typeof (SelectionController<>).MakeGenericType (type);
			return System.Activator.CreateInstance (controllerType, context);
		}

		private void HandleBrickWallBrickAdded(object sender, BrickAddedEventArgs e)
		{
			var brick = e.Brick;
			var type  = e.FieldType;
			
			Bridge<T>.CreateDefaultProperties (brick, type);
		}

		private void HandleBrickWallBrickPropertyAdded(object sender, BrickPropertyAddedEventArgs e)
		{
			var brick    = e.Brick;
			var property = e.Property;

			if (property.Key == BrickPropertyKey.AsType)
			{
				var type = property.Brick.GetFieldType ();
				Bridge<T>.CreateDefaultProperties (brick, type);
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

			Bridge<T>.CreateLabelProperty (brick, labels, 0, BrickPropertyKey.Title);
			Bridge<T>.CreateLabelProperty (brick, labels, 1, BrickPropertyKey.TitleCompact);
		}

		private static void CreateDefaultTextProperties(Brick brick)
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
		
		private static void CreateLabelProperty(Brick brick, IList<string> labels, int i, BrickPropertyKey key)
		{
			if (i < labels.Count)
			{
				BrickProperty property = new BrickProperty (key, labels[i]);
				Brick.AddProperty (brick, property);
			}
		}

		private void ProcessTemplate(TileDataItems data, TileDataItem item, Brick root, Brick templateBrick)
		{
			var templateName      = Brick.GetProperty (templateBrick, BrickPropertyKey.Name).StringValue ?? item.Name;
			var templateFieldType = templateBrick.GetFieldType ();

			CollectionTemplate collectionTemplate = this.DynamicCreateCollectionTemplate (templateName, templateFieldType);

			this.ProcessTemplateProperty (templateBrick, BrickPropertyKey.Title,        x => collectionTemplate.GenericDefine (CollectionTemplateProperty.Title, x));
			this.ProcessTemplateProperty (templateBrick, BrickPropertyKey.TitleCompact, x => collectionTemplate.GenericDefine (CollectionTemplateProperty.CompactTitle, x));
			this.ProcessTemplateProperty (templateBrick, BrickPropertyKey.Text,         x => collectionTemplate.GenericDefine (CollectionTemplateProperty.Text, x));
			this.ProcessTemplateProperty (templateBrick, BrickPropertyKey.TextCompact,  x => collectionTemplate.GenericDefine (CollectionTemplateProperty.CompactText, x));

			CollectionAccessor accessor = this.DynamicCreateCollectionAccessor (root, templateFieldType, collectionTemplate);
			
			data.Add (accessor);
		}

		private CollectionTemplate DynamicCreateCollectionTemplate(string name, System.Type templateFieldType)
		{
			var genericCollectionTemplateType     = typeof (CollectionTemplate<>);
			var genericCollectionTemplateTypeArg  = templateFieldType;
			var constructedCollectionTemplateType = genericCollectionTemplateType.MakeGenericType (genericCollectionTemplateTypeArg);

			object arg1 = name;
			object arg2 = this.controller;
			object arg3 = this.controller.BusinessContext.DataContext;

			return System.Activator.CreateInstance (constructedCollectionTemplateType, arg1, arg2, arg3) as CollectionTemplate;
		}

		private CollectionAccessor DynamicCreateCollectionAccessor(Brick root, System.Type templateFieldType, CollectionTemplate collectionTemplate)
		{
			var accessorFactoryType = typeof (DynamicAccessorFactory<,,>);
			var accessorFactoryTypeArg1 = typeof (T);
			var accessorFactoryTypeArg2 = root.GetFieldType ();
			var accessorFactoryTypeArg3 = templateFieldType;
			
			var genericAccessorFactoryType = accessorFactoryType.MakeGenericType (accessorFactoryTypeArg1, accessorFactoryTypeArg2, accessorFactoryTypeArg3);
			
			var accessorFactory = System.Activator.CreateInstance (genericAccessorFactoryType,
				/**/											   this.controller, root.GetResolver (templateFieldType), collectionTemplate) as DynamicAccessorFactory;

			return accessorFactory.CollectionAccessor;
		}
		
		private void ProcessProperty(Brick brick, BrickPropertyKey key, System.Action<bool> setter)
		{
			if (Brick.ContainsProperty (brick, key))
			{
				setter (true);
			}
		}

		private void ProcessProperty(Brick brick, BrickPropertyKey key, System.Action<string> setter)
		{
			var value = Brick.GetProperty (brick, key).StringValue;

			if (value != null)
			{
				setter (value);
			}
		}

		private void ProcessProperty(Brick brick, BrickPropertyKey key, System.Action<Accessor<FormattedText>> setter)
		{
			var formatter = this.ToAccessor (Brick.GetProperty (brick, key));

			if (formatter != null)
			{
				setter (formatter);
			}
		}

		private void ProcessTemplateProperty(Brick brick, BrickPropertyKey key, System.Action<object> setter)
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

		private Accessor<FormattedText> ToAccessor(BrickProperty property)
		{
			System.Func<T, FormattedText> formatter = property.GetFormatter<T> ();
			
			if (formatter == null)
			{
				return null;
			}
			else
			{
				return this.controller.CreateAccessor (formatter);
			}
		}


		private readonly EntityViewController<T> controller;
	}


}