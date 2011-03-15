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
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Core.Controllers.DataAccessors
{
	/// <summary>
	/// The <c>Bridge</c> class is used to transform <see cref="Brick"/> definitions into
	/// <see cref="Tile"/> instances, in collaboration with <see cref="EntityViewController{T}"/>.
	/// </summary>
	/// <typeparam name="T">The entity type on which the bridge operates.</typeparam>
	public class Bridge<T> : Bridge
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
				Bridge.CreateDefaultTextProperties (brick);
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
				var processor = new InputProcessor (this.controller, data, item, brick);
				
				processor.ProcessInputs ();
			}
			else
			{
				item.EntityMarshaler = this.controller.CreateEntityMarshaler ();

				if (brick.GetFieldType () == typeof (T))
				{
					//	Type already ok.
				}
				else
				{
					item.SetEntityConverter<T> (brick.GetResolver (brick.GetFieldType ()));
				}
				data.Add (item);
			}

			return item;
		}


		#region InputProcessor Class

		class InputProcessor
		{
			public InputProcessor(EntityViewController<T> controller, TileDataItems data, TileDataItem item, Brick root)
			{
				this.controller = controller;
				this.business = this.controller.BusinessContext;
				this.data  = data;
				this.item  = item;
				this.root  = root;
				this.actions = new List<System.Action<FrameBox, UIBuilder>> ();
				this.inputProperties = Brick.GetProperties (this.root, BrickPropertyKey.Input);
			}
			
			public void ProcessInputs()
			{
				foreach (var property in this.inputProperties)
				{
					switch (property.Key)
					{
						case BrickPropertyKey.Input:
							this.CreateActionsForInput (property.Brick, this.inputProperties);
							break;
					}
				}

				this.RecordActions ();
			}

			private void CreateActionsForInput(Brick brick, BrickPropertyCollection inputProperties)
			{
				var fieldProperties = Brick.GetProperties (brick, BrickPropertyKey.Field, BrickPropertyKey.HorizontalGroup);

				foreach (var property in fieldProperties)
				{
					switch (property.Key)
					{
						case BrickPropertyKey.Field:
							this.CreateActionForInputField (property.ExpressionValue, fieldProperties);
							break;

						case BrickPropertyKey.HorizontalGroup:
							this.CreateActionsForHorizontalGroup (property);
							break;
					}
				}

				if ((inputProperties != null) &&
					(inputProperties.PeekAfter (BrickPropertyKey.Separator).HasValue))
				{
					this.CreateActionForSeparator ();
				}
			}

			private void CreateActionsForHorizontalGroup(BrickProperty property)
			{
				int index = this.actions.Count ();

				var title = Brick.GetProperty (property.Brick, BrickPropertyKey.Title).StringValue;

				this.CreateActionsForInput (property.Brick, null);

				var actions = new List<System.Action<FrameBox, UIBuilder>> ();

				while (index < this.actions.Count)
				{
					actions.Add (this.actions[index]);
					this.actions.RemoveAt (index);
				}

				if (actions.Count == 0)
				{
					return;
				}

				System.Action<FrameBox, UIBuilder> groupAction =
					(tile, builder) =>
					{
						var group = builder.CreateGroup (tile as EditionTile, title);
						group.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;
						actions.ForEach (x => x (group, builder));
					};

				this.actions.Add (groupAction);
			}

			private void CreateActionForInputField(Expression expression, BrickPropertyCollection fieldProperties)
			{
				LambdaExpression lambda = expression as LambdaExpression;

				if (lambda == null)
				{
					throw new System.ArgumentException (string.Format ("Expression {0} for input must be a lambda", expression.ToString ()));
				}

				var fieldType  = lambda.ReturnType;
				var entityType = typeof (AbstractEntity);

				int    width = InputProcessor.GetInputWidth (fieldProperties);
				string title = InputProcessor.GetInputTitle (fieldProperties);

				if ((fieldType.IsClass) &&
					(entityType.IsAssignableFrom (fieldType)))
				{
					//	The field is an entity : use an AutoCompleteTextField for it.

					var factory = DynamicFactories.AutoCompleteTextFieldDynamicFactory.Create<T> (business, lambda, this.controller.EntityGetter, title);
					this.actions.Add ((tile, builder) => factory.CreateUI (tile, builder));

					return;
				}

				if ((fieldType == typeof (string)) ||
					(fieldType == typeof (Date)))
				{
					var factory = DynamicFactories.TextFieldDynamicFactory.Create<T> (business, lambda, this.controller.EntityGetter, title, width);
					this.actions.Add ((tile, builder) => factory.CreateUI (tile, builder));

					return;
				}

				if ((fieldType.IsGenericType) &&
					(fieldType.GetGenericTypeDefinition () == typeof (System.Collections.Generic.IList<>)))
				{
					var itemType = fieldType.GetGenericArguments ()[0];

					if ((itemType.IsClass) &&
						(entityType.IsAssignableFrom (itemType)))
					{
						var factory = DynamicFactories.ItemPickerDynamicFactory.Create<T> (business, lambda, itemType, this.controller.EntityGetter, title);
						this.actions.Add ((tile, builder) => factory.CreateUI (tile, builder));
					}
				}
			}
			
			private void CreateActionForSeparator()
			{
				this.actions.Add ((tile, builder) => builder.CreateMargin (tile as EditionTile, horizontalSeparator: true));
			}

			private void RecordActions()
			{
				if ((this.actions != null) &&
					(this.actions.Count > 0))
				{
					if (this.item == null)
					{
						this.item = new TileDataItem ();
					}

					if (this.actions.Count == 1)
					{
						var singleAction = this.actions[0];
						this.item.CreateEditionUI = (tile, builder) => singleAction (tile, builder);
					}
					else
					{
						var multiActions = this.actions.ToArray ();
						
						this.item.CreateEditionUI = (tile, builder) =>
							{
								foreach (var action in multiActions)
								{
									var subTile = builder.CreateEditionTile (tile);
									action (subTile, builder);
								}
							};
					}
				}

				if (this.item != null)
				{
					this.data.Add (this.item);
					this.actions.Clear ();
					this.item = null;
				}
			}

			private static int GetInputWidth(BrickPropertyCollection properties)
			{
				var widthProperty = properties.PeekAfter (BrickPropertyKey.Width);

				if (widthProperty.HasValue)
				{
					return widthProperty.Value.IntValue.GetValueOrDefault (0);
				}
				else
				{
					return 0;
				}
			}

			private static string GetInputTitle(BrickPropertyCollection properties)
			{
				var titleProperty = properties.PeekBefore (BrickPropertyKey.Title);

				if (titleProperty.HasValue)
				{
					return titleProperty.Value.StringValue;
				}
				else
				{
					return null;
				}
			}

			private readonly EntityViewController<T> controller;
			private readonly BusinessContext business;
			private readonly TileDataItems data;
			private readonly Brick root;
			private readonly List<System.Action<FrameBox, UIBuilder>> actions;
			private readonly BrickPropertyCollection inputProperties;
			
			private TileDataItem item;
		}

		#endregion

		
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
		
		private void ProcessProperty(Brick brick, BrickPropertyKey key, System.Action<Accessor<FormattedText>> setter)
		{
			var formatter = this.ToAccessor (Brick.GetProperty (brick, key));

			if (formatter != null)
			{
				setter (formatter);
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