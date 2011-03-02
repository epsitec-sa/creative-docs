//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Bricks;

using System.Collections.Generic;
using System.Reflection;
using System.Linq;

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
			this.ProcessProperty (brick, BrickPropertyKey.Name, x => item.Name = x);
			this.ProcessProperty (brick, BrickPropertyKey.Icon, x => item.IconUri = x);
			
			this.ProcessProperty (brick, BrickPropertyKey.Title, x => item.Title = x);
			this.ProcessProperty (brick, BrickPropertyKey.TitleCompact, x => item.CompactTitle = x);
			this.ProcessProperty (brick, BrickPropertyKey.Text, x => item.Text = x);
			this.ProcessProperty (brick, BrickPropertyKey.TextCompact, x => item.CompactText = x);

			this.ProcessProperty (brick, BrickPropertyKey.Title, x => item.TitleAccessor = x);
			this.ProcessProperty (brick, BrickPropertyKey.TitleCompact, x => item.CompactTitleAccessor = x);
			this.ProcessProperty (brick, BrickPropertyKey.Text, x => item.TextAccessor = x);
			this.ProcessProperty (brick, BrickPropertyKey.TextCompact, x => item.CompactTextAccessor = x);

			Brick parentAsTypeBrick = Brick.GetProperty (brick, BrickPropertyKey.AsType).Brick;

			if (parentAsTypeBrick != null)
			{
				brick = parentAsTypeBrick;
				goto again;
			}

			Brick templateBrick = Brick.GetProperty (brick, BrickPropertyKey.Template).Brick;

			if (templateBrick != null)
			{
				data.Add (item);
				this.ProcessTemplate (data, item, root, templateBrick);
			}
			else
			{
				item.EntityMarshaler = this.controller.CreateEntityMarshaler ();
				data.Add (item);
			}

			return item;
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
			var genericCollectionTemplateType     = typeof (CollectionTemplate<>);
			var genericCollectionTemplateTypeArg  = templateBrick.GetFieldType ();
			var constructedCollectionTemplateType = genericCollectionTemplateType.MakeGenericType (genericCollectionTemplateTypeArg);

			object arg1 = Brick.GetProperty (templateBrick, BrickPropertyKey.Name).StringValue ?? item.Name;
			object arg2 = this.controller;
			object arg3 = this.controller.BusinessContext.DataContext;
			
			var collectionTemplate = System.Activator.CreateInstance (constructedCollectionTemplateType, arg1, arg2, arg3) as CollectionTemplate;

			this.ProcessTemplateProperty (templateBrick, BrickPropertyKey.Title, x => collectionTemplate.GenericDefine (CollectionTemplateProperty.Title, x));
			this.ProcessTemplateProperty (templateBrick, BrickPropertyKey.TitleCompact, x => collectionTemplate.GenericDefine (CollectionTemplateProperty.CompactTitle, x));
			this.ProcessTemplateProperty (templateBrick, BrickPropertyKey.Text, x => collectionTemplate.GenericDefine (CollectionTemplateProperty.Text, x));
			this.ProcessTemplateProperty (templateBrick, BrickPropertyKey.TextCompact, x => collectionTemplate.GenericDefine (CollectionTemplateProperty.CompactText, x));

			var genericCollectionAccessorFactoryType = typeof (Zzz<,,>);
			var genericCollectionAccessorFactoryTypeArg1 = typeof (T);
			var genericCollectionAccessorFactoryTypeArg2 = root.GetFieldType ();
			var genericCollectionAccessorFactoryTypeArg3 = genericCollectionTemplateTypeArg;
			var constructedCollectionAccessorFactoryType = genericCollectionAccessorFactoryType.MakeGenericType (genericCollectionAccessorFactoryTypeArg1, genericCollectionAccessorFactoryTypeArg2, genericCollectionAccessorFactoryTypeArg3);
			var accessorFactory = System.Activator.CreateInstance (constructedCollectionAccessorFactoryType) as Zzz;
			var accessorFactoryCreateArgs = new object[]
			{
				this.controller, root.GetResolver (genericCollectionTemplateTypeArg), collectionTemplate
			};


			constructedCollectionAccessorFactoryType.InvokeMember ("Create", BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod, null, accessorFactory, accessorFactoryCreateArgs);
			

			var accessor = accessorFactory.CollectionAccessor;

			data.Add (accessor);
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
	
	internal abstract class Zzz
	{
		protected Zzz()
		{
		}

		public CollectionAccessor CollectionAccessor
		{
			get;
			set;
		}
	}
		
	internal class Zzz<T1, T2, T3> : Zzz
		where T1 : AbstractEntity, new ()
		where T2 : AbstractEntity, new ()
		where T3 : T2, new ()
	{
		public Zzz()
		{
		}
		
		public void Create(object arg1, object arg2, object arg3)
		{
			EntityViewController<T1> controller = (EntityViewController<T1>) arg1;
			System.Func<T1, IList<T2>> collectionResolver = (System.Func<T1, IList<T2>>) arg2;
			CollectionTemplate<T3> template = (CollectionTemplate<T3>) arg3;

			this.CollectionAccessor = CollectionAccessor.Create (controller.EntityGetter, collectionResolver, template);
		}
	}

}
