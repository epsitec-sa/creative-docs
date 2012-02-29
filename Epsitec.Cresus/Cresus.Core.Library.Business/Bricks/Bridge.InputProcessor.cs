//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;


namespace Epsitec.Cresus.Core.Bricks
{
	public partial class Bridge
	{
		protected class InputProcessor<T>
			where T : AbstractEntity, new ()
		{
			public InputProcessor(Bridge<T> bridge, TileDataItems data, TileDataItem item, Brick root)
			{
				this.bridge          = bridge;
				this.bridgeContext   = bridge.bridgeContext;
				this.controller      = bridge.ViewController;
				this.business        = bridge.controller.BusinessContext;
				this.data            = data;
				this.item            = item;
				this.root            = root;
				this.actions         = new List<UIAction> ();
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

				if (inputProperties != null)
				{
					if (inputProperties.PeekAfter (BrickPropertyKey.Separator, -1).HasValue)
					{
						this.CreateActionForSeparator ();
					}

					if (inputProperties.PeekBefore (BrickPropertyKey.GlobalWarning, -1).HasValue)
					{
						this.CreateActionForGlobalWarning ();
					}
				}
			}

			private void CreateActionsForHorizontalGroup(BrickProperty property)
			{
				int index = this.actions.Count ();

				var title = Brick.GetProperty (property.Brick, BrickPropertyKey.Title).StringValue;

				this.CreateActionsForInput (property.Brick, null);

				var actions = new List<UIAction> ();

				while (index < this.actions.Count)
				{
					actions.Add (this.actions[index]);
					this.actions.RemoveAt (index);
				}

				if (actions.Count == 0)
				{
					return;
				}

				this.actions.Add (new UIGroupAction (actions, title));
			}

			private FieldInfo GetFieldEditionSettings(LambdaExpression lambda)
			{
				FieldInfo info = new FieldInfo (EntityInfo<T>.GetTypeId (), lambda);
				info.Settings = this.bridgeContext.FeatureManager.GetFieldEditionSettings (info.EntityId, info.FieldId);
				return info;
			}

			private void CreateActionForInputField(Expression expression, BrickPropertyCollection fieldProperties)
			{
				LambdaExpression lambda = expression as LambdaExpression;

				if (lambda == null)
				{
					throw new System.ArgumentException (string.Format ("Expression {0} for input must be a lambda", expression.ToString ()));
				}

				var fieldType  = lambda.ReturnType;
				var entityType = typeof (T);
				var fieldMode  = this.GetFieldEditionSettings (lambda);

				int    width  = InputProcessor<T>.GetInputWidth (fieldProperties);
				int    height = InputProcessor<T>.GetInputHeight (fieldProperties);
				bool   readOnly = InputProcessor<T>.GetReadOnly (fieldProperties);
				string title  = InputProcessor<T>.GetInputTitle (fieldProperties);

				System.Collections.IEnumerable collection = InputProcessor<T>.GetInputCollection (fieldProperties);
				int? specialController = InputProcessor<T>.GetSpecialController (fieldProperties);

				if (fieldType.IsEntity ())
				{
					//	The field is an entity : use an AutoCompleteTextField for it.

					var factory = DynamicFactories.EntityAutoCompleteTextFieldDynamicFactory.Create<T> (this.business, lambda, this.controller.EntityGetter, title, collection, specialController, readOnly);
					this.actions.Add (new UIAction ((tile, builder) => factory.CreateUI (tile, builder))
					{
						FieldInfo = fieldMode
					});

					return;
				}

				if (fieldType == typeof (string) ||
					fieldType == typeof (FormattedText) ||
					fieldType == typeof (System.DateTime) ||
					fieldType == typeof (System.DateTime?) ||
					fieldType == typeof (Date) ||
					fieldType == typeof (Date?) ||
					fieldType == typeof (long) ||
					fieldType == typeof (long?) ||
					fieldType == typeof (decimal) ||
					fieldType == typeof (decimal?) ||
					fieldType == typeof (int) ||
					fieldType == typeof (int?) ||
					fieldType == typeof (bool) ||
					fieldType == typeof (bool?))
				{
					width = InputProcessor<T>.GetDefaultFieldWidth (fieldType, width);

					//	Produce either a text field or a variation of such a widget (pull-down list, etc.)
					//	based on the real type being edited.

					var factory = DynamicFactories.TextFieldDynamicFactory.Create<T> (this.business, lambda, this.controller.EntityGetter, title, width, height, readOnly, collection);
					this.actions.Add (new UIAction ((tile, builder) => factory.CreateUI (tile, builder))
					{
						FieldInfo = fieldMode
					});

					return;
				}

				if (fieldType.IsGenericIListOfEntities ())
				{
					//	Produce an item picker for the list of entities. The field type is a collection
					//	of entities represented as [ Field ]--->>* Entity in the Designer.

					var factory = DynamicFactories.ItemPickerDynamicFactory.Create<T> (this.business, lambda, this.controller.EntityGetter, title, specialController, readOnly);
					this.actions.Add (new UIAction ((tile, builder) => factory.CreateUI (tile, builder))
					{
						FieldInfo = fieldMode
					});

					return;
				}

				var underlyingType = fieldType.GetNullableTypeUnderlyingType ();

				if ((fieldType.IsEnum) ||
					((underlyingType != null) && (underlyingType.IsEnum)))
				{
					//	The field is an enumeration : use an AutoCompleteTextField for it.

					var factory = DynamicFactories.EnumAutoCompleteTextFieldDynamicFactory.Create<T> (this.business, lambda, this.controller.EntityGetter, title, width, readOnly);
					this.actions.Add (new UIAction ((tile, builder) => factory.CreateUI (tile, builder))
					{
						FieldInfo = fieldMode
					});

					return;
				}

				System.Diagnostics.Debug.WriteLine (
					string.Format ("*** Field {0} of type {1} : no automatic binding implemented in Bridge<{2}>",
						lambda.ToString (), fieldType.FullName, typeof (T).Name));
			}

			private static int GetDefaultFieldWidth(System.Type fieldType, int width)
			{
				if (width == 0)  // width not specified by an explicit .Width (n) ?
				{
					if ((fieldType == typeof (System.DateTime)) ||
						(fieldType == typeof (System.DateTime?)))
					{
						return 150;
					}

					if ((fieldType == typeof (Date)) ||
						(fieldType == typeof (Date?)))
					{
						return 100;
					}

					if ((fieldType == typeof (long)) ||
						(fieldType == typeof (long?)))
					{
						return 100;
					}

					if ((fieldType == typeof (decimal)) ||
						(fieldType == typeof (decimal?)))
					{
						return 100;
					}

					if ((fieldType == typeof (int)) ||
						(fieldType == typeof (int?)))
					{
						return 70;
					}
				}

				return width;
			}

			private void CreateActionForSeparator()
			{
				this.actions.Add (new UIAction ((tile, builder) => builder.CreateMargin (tile as EditionTile, horizontalSeparator: true)));
			}

			private void CreateActionForGlobalWarning()
			{
				this.actions.Add (new UIAction ((tile, builder) => builder.CreateWarning (tile as EditionTile)));
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
						this.item.CreateUI = singleAction.Execute;
					}
					else
					{
						var multiActions = new UIMultiAction (this.actions);
						this.item.CreateUI = multiActions.Execute;
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
				var property = properties.PeekAfter (BrickPropertyKey.Width, -1);

				if (property.HasValue)
				{
					return property.Value.IntValue.GetValueOrDefault (0);
				}
				else
				{
					return 0;
				}
			}

			private static int GetInputHeight(BrickPropertyCollection properties)
			{
				var property = properties.PeekAfter (BrickPropertyKey.Height, -1);

				if (property.HasValue)
				{
					return property.Value.IntValue.GetValueOrDefault (0);
				}
				else
				{
					return 0;
				}
			}

			private static bool GetReadOnly(BrickPropertyCollection properties)
			{
				return properties.PeekAfter (BrickPropertyKey.ReadOnly, -1).HasValue;
			}

			private static string GetInputTitle(BrickPropertyCollection properties)
			{
				var property = properties.PeekBefore (BrickPropertyKey.Title, -1);

				if (property.HasValue)
				{
					return property.Value.StringValue;
				}
				else
				{
					return null;
				}
			}

			private static int? GetSpecialController(BrickPropertyCollection properties)
			{
				var property = properties.PeekAfter (BrickPropertyKey.SpecialController, -1);

				if (property.HasValue)
				{
					return property.Value.IntValue;
				}
				else
				{
					return null;
				}
			}

			private static System.Collections.IEnumerable GetInputCollection(BrickPropertyCollection properties)
			{
				var property = properties.PeekAfter (BrickPropertyKey.FromCollection, -1);

				if (property.HasValue)
				{
					return property.Value.CollectionValue;
				}
				else
				{
					return null;
				}
			}

			private readonly Bridge				bridge;
			private readonly BridgeContext		bridgeContext;
			private readonly EntityViewController<T> controller;
			private readonly BusinessContext	business;
			private readonly TileDataItems		data;
			private readonly Brick				root;
			private readonly List<UIAction>		actions;
			private readonly BrickPropertyCollection inputProperties;
			
			private TileDataItem				item;
		}
	}
}