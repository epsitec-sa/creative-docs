//	Copyright © 2011-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Support.EntityEngine;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Bricks
{
	public class SimpleBrick<T> : Brick<T>
		where T : AbstractEntity, new ()
	{
		public SimpleBrick(BrickWall brickWall, Expression resolver)
			: base (brickWall, resolver)
		{
		}


		public SimpleBrick<T> IfCondition(System.Func<bool> predicate)
		{
			return this.IfTrue (predicate ());
		}

		public SimpleBrick<T> IfTrue(bool value)
		{
			if (value == false)
			{
				if (this.ContainsProperties ())
				{
					this.RemoveLastProperty ();
				}
				else
				{
					this.BrickWall.Remove (this);
				}
			}

			return this;
		}

		public SimpleBrick<T> IfFalse(bool value)
		{
			return this.IfTrue (value == false);
		}

		public SimpleBrick<T> Name(string value)
		{
			return Brick.AddProperty (this, new BrickProperty (BrickPropertyKey.Name, value));
		}

		public SimpleBrick<T> Icon(string value)
		{
			return Brick.AddProperty (this, new BrickProperty (BrickPropertyKey.Icon, value));
		}

		public SimpleBrick<T> Title(Mortar<T> value)
		{
			return Brick.AddProperty (this, new BrickProperty (BrickPropertyKey.Title, value));
		}

		public SimpleBrick<T> Title<TResult>(Expression<System.Func<T, TResult>> expression)
		{
			return this.Title (new Mortar<T, TResult> (expression));
		}

		public SimpleBrick<T> TitleCompact(Mortar<T> value)
		{
			return Brick.AddProperty (this, new BrickProperty (BrickPropertyKey.TitleCompact, value));
		}

		public SimpleBrick<T> TitleCompact<TResult>(Expression<System.Func<T, TResult>> expression)
		{
			return this.TitleCompact (new Mortar<T, TResult> (expression));
		}

		public SimpleBrick<T> Text(Mortar<T> value)
		{
			return Brick.AddProperty (this, new BrickProperty (BrickPropertyKey.Text, value));
		}

		public SimpleBrick<T> Text<TResult>(Expression<System.Func<T, TResult>> expression)
		{
			return this.Text (new Mortar<T, TResult> (expression));
		}

		public SimpleBrick<T> TextCompact(Mortar<T> value)
		{
			return Brick.AddProperty (this, new BrickProperty (BrickPropertyKey.TextCompact, value));
		}

		public SimpleBrick<T> TextCompact<TResult>(Expression<System.Func<T, TResult>> expression)
		{
			return this.TextCompact (new Mortar<T, TResult> (expression));
		}

		public SimpleBrick<T> Separator()
		{
			return Brick.AddProperty (this, new BrickProperty (BrickPropertyKey.Separator));
		}

		public SimpleBrick<T> GlobalWarning()
		{
			return Brick.AddProperty (this, new BrickProperty (BrickPropertyKey.GlobalWarning));
		}

		public SimpleBrick<T> Attribute<TAttribute>(TAttribute attributeValue)
		{
			return Brick.AddProperty (this, new BrickProperty (BrickPropertyKey.Attribute, new AttributeValue<TAttribute> (attributeValue)));
		}

		public SimpleBrick<T> AttributeIf<TAttribute>(TAttribute attributeValue, bool condition)
		{
			if (condition)
			{
				this.Attribute (attributeValue);
			}

			return this;
		}

		public SimpleBrick<T> Include<TResult>(Expression<System.Func<T, TResult>> expression)
		{
			return Brick.AddProperty (this, new BrickProperty (BrickPropertyKey.Include, expression));
		}

		public SimpleBrick<T> EnableActionMenu<T1>()
		{
			var ids = typeof (T1).GetCustomAttributes<BrickControllerSubTypeAttribute> (false).Select (x => x.Id);

			if (ids.Any ())
			{
				return Brick.AddProperty (this, new BrickProperty (BrickPropertyKey.EnableActionMenu, ids.Single ()));
			}

			throw new System.ArgumentException ("The type " + typeof (T1).FullName + " does not support ControllerSubTypeAttribute.");
		}

		public SimpleBrick<T> EnableActionButton<T1>()
		{
			var ids = typeof (T1).GetCustomAttributes<BrickControllerSubTypeAttribute> (false).Select (x => x.Id);

			if (ids.Any ())
			{
				return Brick.AddProperty (this, new BrickProperty (BrickPropertyKey.EnableActionButton, ids.Single ()));
			}

			throw new System.ArgumentException ("The type " + typeof (T1).FullName + " does not support ControllerSubTypeAttribute.");
		}

		public SimpleBrick<T> EnableActionOnDrop<T1>()
		{
			var ids = typeof (T1).GetCustomAttributes<BrickControllerSubTypeAttribute> (false).Select (x => x.Id);

			if (ids.Any ())
			{
				return Brick.AddProperty (this, new BrickProperty (BrickPropertyKey.EnableActionOnDrop, ids.Single ()));
			}

			throw new System.ArgumentException ("The type " + typeof (T1).FullName + " does not support ControllerSubTypeAttribute.");
		}

		public SimpleBrick<TOutput> OfType<TOutput>()
			where TOutput : AbstractEntity, new ()
		{
			var child = new SimpleBrick<TOutput> (this.BrickWall, this.Resolver);
			return this.AddChild (child, BrickPropertyKey.OfType);
		}

		public TemplateBrick<T, SimpleBrick<T>> Template()
		{
			var child = new TemplateBrick<T, SimpleBrick<T>> (this);
			return this.AddChild (child, BrickPropertyKey.Template);
		}

		public InputBrick<T, SimpleBrick<T>> Input()
		{
			var child = new InputBrick<T, SimpleBrick<T>> (this);
			return this.AddChild (child, BrickPropertyKey.Input);
		}

		public ActionBrick<T, SimpleBrick<T>> DefineAction()
		{
			var child = new ActionBrick<T, SimpleBrick<T>> (this);
			return this.AddChild (child, BrickPropertyKey.DefineAction);
		}
	}
}