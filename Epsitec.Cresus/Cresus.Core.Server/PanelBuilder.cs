using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;
using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Bricks;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Types.Converters.Marshalers;

namespace Epsitec.Cresus.Core.Server
{
	/// <summary>
	/// Allow to create an ExtJS 4 panel by inferring the layout using
	/// AbstractEntities 
	/// </summary>
	class PanelBuilder
	{

		/// <summary>
		/// Create use a builder to create a panel
		/// </summary>
		/// <param name="entity">Entity to use to create the panelBuilder</param>
		/// <param name="mode">Controller mode</param>
		/// <returns></returns>
		public static Dictionary<string, object> BuildController(AbstractEntity entity, ViewControllerMode mode, CoreSession coreSession)
		{
			var builder = new PanelBuilder (entity, mode, coreSession);
			return builder.Run ();
		}

		private PanelBuilder(AbstractEntity entity, ViewControllerMode mode, CoreSession coreSession)
		{
			this.rootEntity = entity;
			this.controllerMode = mode;
			this.coreSession = coreSession;
		}

		/// <summary>
		/// Creates a controller according to an entity and a ViewMode
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="mode"></param>
		/// <returns>Name of the generated panel</returns>
		private Dictionary<string, object> Run()
		{
			var customerSummaryWall = CoreSession.GetBrickWall (this.rootEntity, this.controllerMode);

			// Open the main panel
			var dic = new Dictionary<string, object> ();

			//dic["title"] = this.rootEntity.GetType ().ToString ();
			dic["parentEntity"] = GetEntityKey (this.rootEntity);

			var items = new List<Dictionary<string, object>> ();
			dic["items"] = items;

			foreach (var brick in customerSummaryWall.Bricks)
			{
				var panels = GetPanels (brick);
				items.AddRange (panels);
			}

			// The very first item is the root
			items.First ()["isRoot"] = true;

			return dic;
		}

		private List<Dictionary<string, object>> GetPanels(Brick brick)
		{
			var item = new WebDataItem ();
			Brick processedBrick = BrickProcessor.ProcessBrick (brick, item);

			var panels = CreatePanelContent (processedBrick, item);
			return panels;
		}

		/// <summary>
		/// Create a panel according to a brick
		/// </summary>
		/// <param name="brick">Brick to use</param>
		/// <returns></returns>
		private List<Dictionary<string, object>> CreatePanelContent(Brick brick, WebDataItem item)
		{
			var list = new List<Dictionary<string, object>> ();

			var brickType = brick.GetFieldType ();
			var resolver = brick.GetResolver (brickType);

			if (item.DataType == TileDataType.CollectionItem)
			{
				var obj = resolver.DynamicInvoke (this.rootEntity);

				var col = (obj as IEnumerable).Cast<AbstractEntity> ().Where (c => c.GetType () == brickType);

				if ((col.Any ()))
				{
					col.ForEach (e => list.AddRange (CreatePanelsForEntity (brick, item, e)));
				}
				else
				{
					// This collection is empty, but we want to show its panel
					// so the user will be able to add one.
					list.Add (CreateEmptyPanel (brick, item));
				}
			}
			else
			{
				AbstractEntity entity;
				if (resolver != null)
				{
					entity = resolver.DynamicInvoke (this.rootEntity) as AbstractEntity;
				}
				else
				{
					entity = this.rootEntity;
				}

				list.AddRange (CreatePanelsForEntity (brick, item, entity));
			}

			return list;
		}

		private List<Dictionary<string, object>> CreatePanelsForEntity(Brick brick, WebDataItem item, AbstractEntity entity)
		{

			var list = new List<Dictionary<string, object>> ();
			var parent = GetBasicPanelForm (item);
			list.Add (parent);

			var entityKey = GetEntityKey (entity);
			parent["entityId"] = entityKey;

			if (item.DefaultMode == ViewControllerMode.Summary)
			{
				parent["clickToEdit"] = false;
			}

			AddControllerSpecificData (parent, item, entity);

			var inputs = HandleInputs (brick);
			if (inputs != null && inputs.Any ())
			{
				parent["items"] = inputs;
			}

			var children = CreateChildren (brick);
			if (children != null && children.Any ())
			{
				list.AddRange (children);
			}

			return list;
		}

		private Dictionary<string, object> GetBasicPanelForm(WebDataItem item)
		{
			var panel = new Dictionary<string, object> ();

			var controllerName = GetControllerName (this.controllerMode);
			panel["xtype"] = controllerName;

			string title = item.Title.ToSimpleText ();
			panel["title"] = title;

			var icon = IconsBuilder.GetCSSClassName (item.IconUri, IconSize.Sixteen);
			if (icon != null)
			{
				panel["iconCls"] = icon;
			}

			return panel;
		}

		private Dictionary<string, object> CreateEmptyPanel(Brick brick, WebDataItem item)
		{
			var panel = GetBasicPanelForm (item);

			panel["xtype"] = "emptysummary";

			return panel;
		}

		private void AddControllerSpecificData(Dictionary<string, object> parent, WebDataItem item, AbstractEntity entity)
		{
			switch (this.controllerMode)
			{
				case ViewControllerMode.Summary:
					parent["html"] = entity.GetSummary ().ToString ();
					parent["hideRemoveButton"] = item.HideRemoveButton;
					parent["hideAddButton"] = item.HideAddButton;
					break;

				case ViewControllerMode.Edition:
					break;

				case ViewControllerMode.Creation:
					break;

				case ViewControllerMode.None:
					break;

				default:
					throw new System.NotImplementedException ("Make sure this switch has all possible branches");
			}
		}

		private List<Dictionary<string, object>> HandleInputs(Brick brick)
		{
			if (!Brick.ContainsProperty (brick, BrickPropertyKey.Input))
			{
				return null;
			}

			var list = new List<Dictionary<string, object>> ();

			var inputs = Brick.GetProperties (brick, BrickPropertyKey.Input);

			foreach (var property in inputs)
			{
				list.AddRange (this.CreateInput (property.Brick, inputs));
			}

			return list;
		}

		private List<Dictionary<string, object>> CreateInput(Brick brick, BrickPropertyCollection inputProperties)
		{
			var list = new List<Dictionary<string, object>> ();

			var fieldProperties = Brick.GetProperties (brick, BrickPropertyKey.Field, BrickPropertyKey.HorizontalGroup);

			foreach (var property in fieldProperties)
			{
				switch (property.Key)
				{
					case BrickPropertyKey.Field:
						list.AddRange (this.CreateInputField (property.ExpressionValue, fieldProperties));
						break;

					case BrickPropertyKey.HorizontalGroup:
						list.Add (this.CreateHorizontalGroup (property));
						break;
				}
			}

			if (inputProperties != null)
			{
				if (inputProperties.PeekAfter (BrickPropertyKey.Separator, -1).HasValue)
				{
					list.Add (PanelBuilder.GetSeparator ());
				}

				// /!\ Caution! Hot code! /!\
				if (inputProperties.PeekBefore (BrickPropertyKey.GlobalWarning, -1).HasValue)
				{
					list.Add (PanelBuilder.GetGlobalWarning ());
				}
			}

			return list;
		}

		private List<Dictionary<string, object>> CreateInputField(Expression expression, BrickPropertyCollection fieldProperties)
		{
			var list = new List<Dictionary<string, object>> ();
			var entityDictionnary = new Dictionary<string, object> ();
			var lambdaDictionnary = new Dictionary<string, object> ();
			list.Add (entityDictionnary);
			list.Add (lambdaDictionnary);

			LambdaExpression lambda = expression as LambdaExpression;

			if (lambda == null)
			{
				throw new System.ArgumentException (string.Format ("Expression {0} for input must be a lambda", expression.ToString ()));
			}

			var func   = lambda.Compile ();
			var obj = func.DynamicInvoke (this.rootEntity);
			var entity = obj as AbstractEntity;

			var fieldType  = lambda.ReturnType;
			var fieldMode  = PanelBuilder.GetFieldEditionSettings (lambda);

			//int    width  = InputProcessor.GetInputWidth (fieldProperties);
			//int    height = InputProcessor.GetInputHeight (fieldProperties);
			string title  = PanelBuilder.GetInputTitle (fieldProperties);

			var caption   = EntityInfo.GetFieldCaption (lambda);
			title     = title ?? PanelBuilder.GetInputTitle (caption);

			//System.Collections.IEnumerable collection = InputProcessor.GetInputCollection (fieldProperties);
			//int? specialController = InputProcessor.GetSpecialController (fieldProperties);


			string fieldName = fieldMode.FieldId.ToString ().Trim ('[', ']');

			lambdaDictionnary["xtype"] = "hiddenfield";
			lambdaDictionnary["name"] = "lambda_" + fieldName;
			var accessor = this.coreSession.GetPanelFieldAccessor (lambda);
			lambdaDictionnary["value"] = accessor == null ? "-1" : accessor.Id.ToString ();

			entityDictionnary["xtype"] = "textfield";
			entityDictionnary["fieldLabel"] = title;
			entityDictionnary["name"] = fieldName;

			var businessContext = this.coreSession.GetBusinessContext ();

			if (accessor.IsEntityType)
			{
				var items = businessContext.Data.GetAllEntities (fieldType, DataExtractionMode.Sorted, this.DataContext);
				var data = from item in items
						   select new object[]
						   {
							   this.GetEntityKey (item),
							   item.GetCompactSummary ().ToString ()
						   };

				entityDictionnary["value"] = this.GetEntityKey (entity);
				entityDictionnary["xtype"] = "epsitec.entitycombo";
				entityDictionnary["store"] = data;

				return list;
			}

			if (fieldType == typeof (string) ||
			    fieldType == typeof (FormattedText) ||
			    fieldType == typeof (long) ||
			    fieldType == typeof (long?) ||
			    fieldType == typeof (decimal) ||
			    fieldType == typeof (decimal?) ||
			    fieldType == typeof (int) ||
			    fieldType == typeof (int?) ||
			    fieldType == typeof (bool) ||
			    fieldType == typeof (bool?))
			{
				entityDictionnary["value"] = accessor.GetStringValue (this.rootEntity);
				return list;
			}

			if (fieldType == typeof (System.DateTime) ||
			    fieldType == typeof (System.DateTime?))
			{
				//	TODO: handle date & time
			}

			if (fieldType == typeof (Date) ||
			    fieldType == typeof (Date?))
			{
				entityDictionnary["xtype"]  = "datefield";
				entityDictionnary["format"] = "d.m.Y";
				entityDictionnary["value"]  = accessor.GetStringValue (this.rootEntity);

				return list;
			}

			if (accessor.IsCollectionType)
			{
				var items = businessContext.Data.GetAllEntities (accessor.CollectionItemType, DataExtractionMode.Sorted, businessContext.DataContext);
				var found = accessor.GetCollection (this.rootEntity).Cast<AbstractEntity> ();

				//	TODO: create a collection of checkboxes to pick one or more items from the list...

				return list;
			}

			var underlyingType = fieldType.GetNullableTypeUnderlyingType ();

			if ((fieldType.IsEnum) ||
			        ((underlyingType != null) && (underlyingType.IsEnum)))
			{
				//	The field is an enumeration : use an AutoCompleteTextField for it.

				//var factory = DynamicFactories.EnumAutoCompleteTextFieldDynamicFactory.Create<T> (business, lambda, this.controller.EntityGetter, title, width);
				//this.actions.Add (new UIAction ((tile, builder) => factory.CreateUI (tile, builder))
				//{
				//    FieldInfo = fieldMode
				//});

				//return;

				entityDictionnary["xtype"] = "epsitec.enumcombo";
				entityDictionnary["value"] = obj.ToString ();
				entityDictionnary["storeUrl"] = fieldType.AssemblyQualifiedName;

				return list;
			}

			//System.Diagnostics.Debug.WriteLine (
			//    string.Format ("*** Field {0} of type {1} : no automatic binding implemented in Bridge<{2}>",
			//        lambda.ToString (), fieldType.FullName, typeof (T).Name));

			return list;
		}

		private Dictionary<string, object> CreateHorizontalGroup(BrickProperty property)
		{

			var dic = new Dictionary<string, object> ();

			dic["xtype"] = "fieldset";
			dic["layout"] = "column";

			var title = Brick.GetProperty (property.Brick, BrickPropertyKey.Title).StringValue;
			dic["title"] = title;

			new List<Dictionary<string, object>> ();
			var list = this.CreateInput (property.Brick, null);

			foreach (var l in list)
			{
				l["columnWidth"] = 1.0 / list.Count (input => (string) input["xtype"] != "hiddenfield");
				l["margin"] = "0 5 0 0";
				l.Remove ("fieldLabel");
			}

			dic["items"] = list;

			//int index = this.actions.Count ();

			//var title = Brick.GetProperty (property.Brick, BrickPropertyKey.Title).StringValue;

			//this.CreateActionsForInput (property.Brick, null);

			//var actions = new List<UIAction> ();

			//while (index < this.actions.Count)
			//{
			//    actions.Add (this.actions[index]);
			//    this.actions.RemoveAt (index);
			//}

			//if (actions.Count == 0)
			//{
			//    return;
			//}

			//this.actions.Add (new UIGroupAction (actions, title));

			return dic;
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

		public static string GetInputTitle(Caption caption)
		{
			if (caption == null)
			{
				return null;
			}

			if (caption.HasLabels)
			{
				return caption.DefaultLabel;
			}

			return caption.Description ?? caption.Name;
		}

		private static FieldInfo GetFieldEditionSettings(LambdaExpression lambda)
		{
			//FieldInfo info = new FieldInfo (EntityInfo<T>.GetTypeId (), lambda);
			//info.Settings = this.bridge.bridgeContext.FeatureManager.GetFieldEditionSettings (info.EntityId, info.FieldId);
			//return info;

			FieldInfo info = new FieldInfo (EntityInfo<AbstractContactEntity>.GetTypeId (), lambda);
			return info;
		}

		private static Dictionary<string, object> GetSeparator()
		{
			var dic = new Dictionary<string, object> ();
			var autoEl = new Dictionary<string, string> ();

			dic["xtype"] = "box";
			dic["margin"] = "10 0";
			dic["autoEl"] = autoEl;
			autoEl["tag"] = "hr";

			return dic;
		}

		private static Dictionary<string, object> GetGlobalWarning()
		{
			var dic = new Dictionary<string, object> ();

			dic["xtype"] = "displayfield";
			dic["value"] = "<i><b>ATTENTION:</b> Les modifications effectuées ici seront répercutées dans tous les enregistrements.</i>";
			dic["cls"] = "global-warning";

			return dic;
		}

		/// <summary>
		/// Create "leaves" for a panel. 
		/// Uses the "Include" property from the Brick
		/// </summary>
		/// <param name="brick"></param>
		/// <returns></returns>
		private List<Dictionary<string, object>> CreateChildren(Brick brick)
		{
			if (!Brick.ContainsProperty (brick, BrickPropertyKey.Include))
			{
				return null;
			}

			var list = new List<Dictionary<string, object>> ();

			var includes = Brick.GetProperties (brick, BrickPropertyKey.Include);

			foreach (var include in includes)
			{
				var lambda = include.ExpressionValue as LambdaExpression;
				var func   = lambda.Compile ();
				var child = func.DynamicInvoke (this.rootEntity) as AbstractEntity;

				if (child.IsNull ())
				{
					continue;
				}

				// Recursively build the panels
				var childPanel = PanelBuilder.BuildController (child, this.controllerMode, this.coreSession);
				list.AddRange (childPanel["items"] as List<Dictionary<string, object>>);
			}

			return list;

		}

		private string GetControllerName(ViewControllerMode mode)
		{
			return this.controllerMode.ToString ().ToLower ();
		}

		private string GetEntityKey(AbstractEntity entity)
		{
			if (entity == null)
			{
				return null;
			}

			var key = this.DataContext.GetNormalizedEntityKey (entity);

			if (!key.HasValue)
			{
				return null;
			}

			return key.Value.ToString ();
		}

		private DataContext DataContext
		{
			get
			{
				return this.coreSession.GetBusinessContext ().DataContext;
			}
		}

		private readonly AbstractEntity rootEntity;
		private readonly ViewControllerMode controllerMode;
		private readonly CoreSession coreSession;


		/*
		 * 
		 * Code:
		 * + : Complete
		 * - : Does not apply here
		 * ~ : To check
		 *   : TODO
		 * 
		 * */

		/*
		public enum BrickPropertyKey
		{
			+Name,
			+Icon,
			+Title,
			+TitleCompact,
			+Text,
			+TextCompact,

			~Attribute,

			~Template,
			-OfType,

			Input,
			Field,
			-Width,
			-Height,
			+Separator,
			+HorizontalGroup,
			FromCollection,
			SpecialController,
			+GlobalWarning,

			CollectionAnnotation,
			+Include,
		}
		 */

		/*
		public enum BrickMode
		{
			-AutoGroup,

			+DefaultToSummarySubview,

			+HideAddButton,
			+HideRemoveButton,

			-SpecialController0,
			-SpecialController1,
			-SpecialController2,
			-SpecialController3,

			FullHeightStretch,
		}*/
	}
}
