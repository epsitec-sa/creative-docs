//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Factories;
using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Core.Bricks
{
	/// <summary>
	/// The <c>Bridge</c> class is used to transform <see cref="Brick"/> definitions into
	/// <see cref="Tile"/> instances, in collaboration with <see cref="EntityViewController{T}"/>.
	/// </summary>
	/// <typeparam name="T">The entity type on which the bridge operates.</typeparam>
	public class Bridge<T> : Bridge
		where T : AbstractEntity, new ()
	{
		public Bridge(BridgeContext bridgeContext, EntityViewController<T> controller)
			: base (bridgeContext, controller)
		{
			this.walls = new List<BrickWall<T>> ();
		}


		public EntityViewController<T>			ViewController
		{
			get
			{
				return this.controller as EntityViewController<T>;
			}
		}

		public override bool					ContainsBricks
		{
			get
			{
				return this.walls.Any (x => x.Bricks.Any ());
			}
		}

		
		public BrickWall<T> CreateBrickWall()
		{
			var wall = new BrickWall<T> ();

			wall.BrickAdded += this.HandleBrickWallBrickAdded;
			wall.BrickPropertyAdded += this.HandleBrickWallBrickPropertyAdded;

			this.walls.Add (wall);

			return wall;
		}

		public override void CreateTileDataItems(TileDataItems data)
		{
			this.ViewController.NotifyAboutToCreateUI ();

			foreach (var brick in this.walls.SelectMany (x => x.Bricks))
			{
				this.CreateTileDataItem (data, brick);

				while (this.bridgeContext.HasPendingBridges)
				{
					var bridge = this.bridgeContext.GetNextPendingBridge ();

					bridge.CreateTileDataItems (data);
				}
			}
		}

		private TileDataItem CreateTileDataItem(TileDataItems data, Brick brick)
		{
			var item = new TileDataItem ();
			var root = brick;

			item.DataType = Bridge.Classify (this.controller);

		again:
			if (Brick.ContainsProperty (brick, BrickPropertyKey.OfType))
			{

			}
			else if (Brick.ContainsProperty (brick, BrickPropertyKey.Template))
			{
				//	Don't produce default text properties for bricks which contain AsType
				//	or Template bricks. Instead, specify the default empty text.

				var templateBrick = Brick.GetProperty (brick, BrickPropertyKey.Template).Brick;

				System.Diagnostics.Debug.Assert (templateBrick != null);

				if ((!Brick.ContainsProperty (templateBrick, BrickPropertyKey.Title)) &&
					(!Brick.ContainsProperty (templateBrick, BrickPropertyKey.TitleCompact)))
				{
					Bridge.CreateDefaultTitleProperties (templateBrick);
				}

				if ((!Brick.ContainsProperty (templateBrick, BrickPropertyKey.Text)) &&
					(!Brick.ContainsProperty (templateBrick, BrickPropertyKey.TextCompact)))
				{
					Bridge.CreateDefaultTextProperties (templateBrick);
				}

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

			this.PreProcessAttributes ();

			this.ProcessProperty (brick, BrickPropertyKey.Attribute, x => this.ProcessAttribute (item, x));
			this.ProcessProperty (brick, BrickPropertyKey.Include, x => this.ProcessInclusion (x));

			if ((!item.Title.IsNullOrEmpty ()) &&
				(item.CompactTitle.IsNull ()))
			{
				item.CompactTitle = item.Title;
			}

			this.ProcessProperty (brick, BrickPropertyKey.Title, x => item.TitleAccessor = x);
			this.ProcessProperty (brick, BrickPropertyKey.TitleCompact, x => item.CompactTitleAccessor = x);
			this.ProcessProperty (brick, BrickPropertyKey.Text, x => item.TextAccessor = x);
			this.ProcessProperty (brick, BrickPropertyKey.TextCompact, x => item.CompactTextAccessor = x);

			Brick ofTypeBrick = Brick.GetProperty (brick, BrickPropertyKey.OfType).Brick;

			if (ofTypeBrick != null)
			{
				brick = ofTypeBrick;
				goto again;
			}
			else
			{
				var templateBrick = Brick.GetProperty (brick, BrickPropertyKey.Template).Brick;

				if (templateBrick != null)
				{
					data.Add (item);
					this.ProcessTemplate (data, item, root, templateBrick);
				}
				else if (Brick.ContainsProperty (brick, BrickPropertyKey.Input))
				{
					item.EntityMarshaler = this.ViewController.CreateEntityMarshaler ();
					
					var processor = new InputProcessor<T> (this, data, item, brick);

					processor.ProcessInputs ();
				}
				else
				{
					item.EntityMarshaler = this.ViewController.CreateEntityMarshaler ();

					System.Type fieldType = brick.GetFieldType ();

					if (fieldType == typeof (T))
					{
						//	Type already ok.

						if (Brick.ContainsProperty (brick, BrickPropertyKey.Include))
						{
							//	If this is a brick which defines only a single 'Include' property,
							//	then we don't want to add it to the tile data items; it would just
							//	produce an empty, useless piece of information.

							if (Brick.GetAllProperties (brick).All (x => x.IsDefaultProperty || x.Key == BrickPropertyKey.Include))
							{
								return null;
							}
						}
					}
					else
					{
						item.SetEntityConverter<T> (brick.GetResolver (fieldType));

						if (this.autoCreateNullEntity)
						{
							item.SetEntityAutoCreator (this.controller.BusinessContext, fieldType, brick.CreateResolverSetter (fieldType));
						}
					}
					
					data.Add (item);
				}
			}

			return item;
		}

		private void PreProcessAttributes()
		{
			this.autoCreateNullEntity = false;
		}


		private void ProcessAttribute(TileDataItem item, BrickMode value)
		{
			switch (value)
			{
				case BrickMode.AutoGroup:
					item.AutoGroup = true;
					break;

				case BrickMode.AutoCreateNullEntity:
					this.autoCreateNullEntity = true;
					break;

				case BrickMode.DefaultToCreationOrSummarySubView:
					item.DefaultMode = ViewControllerMode.CreationOrSummary;
					break;

				case BrickMode.DefaultToCreationOrEditionSubView:
					item.DefaultMode = ViewControllerMode.CreationOrEdition;
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

				case BrickMode.FullWidthPanel:
					item.FullWidthPanel = true;
					break;

				case BrickMode.HideRemoveButton:
					item.HideRemoveButton = true;
					break;

				default:
					if (value.IsSpecialController ())
					{
						item.ControllerSubTypeId = value.GetControllerSubTypeId ();
					}
					break;
			}
		}

		private void ProcessInclusion(Expression expression)
		{
			var lambda = expression as LambdaExpression;
			var func   = lambda.Compile ();
			var entity = func.DynamicInvoke (this.ViewController.Entity) as AbstractEntity;
			var name   = this.controller.Name + EntityInfo.GetFieldCaption (lambda).Name;

			//	Create the controller for the included sub-view, which will represent the entity
			//	pointed to by the expression :

			var sub = EntityViewControllerFactory.Create (name, entity, ViewControllerMode.Edition, this.controller.Orchestrator, this.controller);
			var all = CoreViewController.GetAllControllers (this.controller.Orchestrator.DataViewController).OfType<EntityViewController> ().ToArray ();
			var subType = sub.GetType ();

			if (all.Any (x => x.GetType () == subType && x.GetEntity () == entity))
			{
				//	If there is already a controller for the exact same entity, we won't include
				//	the sub-view controller and we won't generate any UI for it, as it would result
				//	in the creation of a duplicate.
			}
			else
			{
				this.ViewController.AddUIController (sub);
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
			object arg2 = this.controller.BusinessContext;

			return System.Activator.CreateInstance (constructedCollectionTemplateType, arg1, arg2) as CollectionTemplate;
		}

		private CollectionAccessor DynamicCreateCollectionAccessor(Brick root, System.Type templateFieldType, CollectionTemplate collectionTemplate)
		{
			var accessorFactoryType = typeof (DynamicAccessorFactory<,,>);
			var accessorFactoryTypeArg1 = typeof (T);
			var accessorFactoryTypeArg2 = root.GetFieldType ();
			var accessorFactoryTypeArg3 = templateFieldType;

			var genericAccessorFactoryType = accessorFactoryType.MakeGenericType (accessorFactoryTypeArg1, accessorFactoryTypeArg2, accessorFactoryTypeArg3);

			var entityGetter   = this.ViewController.EntityGetter;
			var entityResolver = root.GetResolver (templateFieldType);

#if false
			var constructor = genericAccessorFactoryType.GetConstructors ()[0];
			var args        = new object[] { specificEntityGetter, specificResolver, collectionTemplate };
			
			var accessorFactory = constructor.Invoke (args) as DynamicAccessorFactory;
#else
			var accessorFactory = System.Activator.CreateInstance (genericAccessorFactoryType,
				/**/											   entityGetter, entityResolver, collectionTemplate) as DynamicAccessorFactory;
#endif
			return accessorFactory.CollectionAccessor;
		}

		private void ProcessProperty(Brick brick, BrickPropertyKey key, System.Action<BrickMode> setter)
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

		private void ProcessProperty(Brick brick, BrickPropertyKey key, System.Action<Accessor<FormattedText>> setter)
		{
			var formatter = this.ToAccessor (brick, Brick.GetProperty (brick, key));

			if (formatter != null)
			{
				setter (formatter);
			}
		}

		private Accessor<FormattedText> ToAccessor(Brick brick, BrickProperty property)
		{
			var getter    = this.ViewController.EntityGetter;
			var resolver  = brick.GetResolver (null);
			var formatterExpression = (property.ExpressionValue as LambdaExpression);

			if (formatterExpression == null)
			{
				return null;
			}

			var formatterFunc = formatterExpression.Compile ();
			
			if (resolver == null)
			{
				System.Func<FormattedText> composite =
					delegate
					{
						var expression = property.ExpressionValue as LambdaExpression;
						var source = getter ();
						var target = source as AbstractEntity;

						if (target.IsNull ())
						{
							return FormattedText.Empty;
						}
						else
						{
							return (FormattedText) formatterFunc.DynamicInvoke (target);
						}
					};

				return new Accessor<FormattedText> (composite);
			}
			else
			{
				System.Func<FormattedText> composite =
					delegate
					{
						var expression = property.ExpressionValue as LambdaExpression;
						var source = getter ();
						var target = resolver.DynamicInvoke (source) as AbstractEntity;

						if (target.IsNull ())
						{
							return FormattedText.Empty;
						}
						else
						{
							return (FormattedText) formatterFunc.DynamicInvoke (target);
						}
					};

				return new Accessor<FormattedText> (composite);
			}
		}


		private readonly List<BrickWall<T>> walls;

		private bool autoCreateNullEntity;
	}
}
