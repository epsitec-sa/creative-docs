using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer.Context;

using Epsitec.Cresus.WebCore.Server.CoreServer;

using System;

using System.Collections;
using System.Collections.Generic;

using System.Diagnostics;

using System.Globalization;

using System.Linq;
using System.Linq.Expressions;
using Epsitec.Cresus.WebCore.Server.UserInterface.Tile;
using Epsitec.Cresus.WebCore.Server.UserInterface.TileData;


namespace Epsitec.Cresus.WebCore.Server.UserInterface
{
	
	
	/// <summary>
	/// Allow to create an ExtJS 4 panel by inferring the layout using
	/// AbstractEntities 
	/// </summary>
	internal sealed class PanelBuilder
	{


		private PanelBuilder(AbstractEntity entity, ViewControllerMode mode, int? controllerSubTypeId, CoreSession coreSession)
		{
			this.rootEntity = entity;
			this.controllerMode = mode;
			this.controllerSubTypeId = controllerSubTypeId;
			this.coreSession = coreSession;
		}


		private DataContext DataContext
		{
			get
			{
				return this.BusinessContext.DataContext;
			}
		}


		private BusinessContext BusinessContext
		{
			get
			{
				return this.coreSession.GetBusinessContext ();
			}
		}


		/// <summary>
		/// Creates a controller according to an entity and a ViewMode
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="mode"></param>
		/// <returns>Name of the generated panel</returns>
		private Dictionary<string, object> Run()
		{
			var panel = new Dictionary<string, object> ();

			panel["parentEntity"] = this.GetEntityId (this.rootEntity);
			panel["controllerMode"] = Tools.ViewControllerModeToString (this.controllerMode);
			panel["controllerSubTypeId"] = Tools.ControllerSubTypeIdToString (this.controllerSubTypeId);
			panel["items"] = this.GetPanels (this.rootEntity, this.controllerMode, this.controllerSubTypeId);

			return panel;
		}


		private List<Dictionary<string, object>> GetPanels(AbstractEntity entity, ViewControllerMode controllerMode, int? controllerSubTypeId)
		{
			var brickWall = this.GetBrickWall (entity, controllerMode, controllerSubTypeId);

			// NOTE Switch this bool if you want to use the new or the old way to generate the panels.
			// The old way is still here only for testing purposes and will be removed soon.
			bool useNewWay = false;

			List<Dictionary<string, object>> panels;

			if (useNewWay)
			{
				var tileData = this.GetTileData (brickWall);
				var tiles = this.GetTiles (tileData, entity);
				panels = tiles.Select (t => t.ToDictionary ()).ToList ();
			}
			else
			{
				panels = brickWall.Bricks.SelectMany (b => this.GetPanels (b)).ToList ();
			}

			panels[0]["isRoot"] = true;

			return panels;
		}


		private BrickWall GetBrickWall(AbstractEntity entity, ViewControllerMode controllerMode, int? controllerSubTypeId)
		{
			return Mason.BuildBrickWall (entity, controllerMode, controllerSubTypeId);
		}


		private IEnumerable<ITileData> GetTileData(BrickWall brickWall)
		{
			return Carpenter.BuildTileData (brickWall);
		}


		private IEnumerable<AbstractTile> GetTiles(IEnumerable<ITileData> tileData, AbstractEntity entity)
		{
			return tileData.SelectMany
			(
				td => td.ToTiles
				(
					entity, 
					e => this.GetEntityId (e), 
					u => this.GetIconClass (u), 
					l => this.GetLambdaId (l),
					t => this.GetTypeName (t), 
					e => this.BuildEditionTiles (e), 
					t => this.GetEntities (t), 
					l => this.GetPanelFieldAccessor (l)
				)
			);
		}


		private string GetEntityId(AbstractEntity entity)
		{
			string entityId = null;
			
			if (entity != null)
			{
				var entityKey = this.DataContext.GetNormalizedEntityKey (entity);

				if (entityKey.HasValue)
				{
					entityId = entityKey.Value.ToString ();
				}
			}

			return entityId;
		}


		private string GetIconClass(string uri)
		{
			return IconManager.GetCSSClassName (uri, IconSize.Sixteen);
		}


		private string GetLambdaId(LambdaExpression lambda)
		{
			int lambdaId = this.coreSession.PanelFieldAccessorCache.Get (lambda).Id;

			return InvariantConverter.ToString (lambdaId);
		}


		private string GetTypeName(Type type)
		{
			return type.AssemblyQualifiedName;
		}


		private IEnumerable<AbstractTile> BuildEditionTiles(AbstractEntity entity)
		{
			var brickWall = this.GetBrickWall (entity, ViewControllerMode.Edition, null);
			var tileData = this.GetTileData (brickWall);

			return this.GetTiles (tileData, entity);
		}


		private IEnumerable<AbstractEntity> GetEntities(Type entityType)
		{
			return this.BusinessContext.Data.GetAllEntities (entityType, DataExtractionMode.Sorted, this.DataContext);
		}


		private PanelFieldAccessor GetPanelFieldAccessor(LambdaExpression lambda)
		{
			return this.coreSession.PanelFieldAccessorCache.Get (lambda);
		}


		public static Dictionary<string, object> BuildController(AbstractEntity entity, ViewControllerMode mode, int? controllerSubTypeId, CoreSession coreSession)
		{
			return new PanelBuilder (entity, mode, controllerSubTypeId, coreSession).Run ();
		}


		private readonly AbstractEntity rootEntity;
		
		
		private readonly ViewControllerMode controllerMode;
		
		
		private readonly int? controllerSubTypeId;
		
		
		private readonly CoreSession coreSession;








		// NOTE All this code is the old way of generating panels and will be deleted soon. Don't
		// rely on it.


		/// <summary>
		/// Get the panel from a brick
		/// </summary>
		/// <returns></returns>
		private List<Dictionary<string, object>> GetPanels(Brick brick)
		{
			var item = new WebTileDataItem ();
			Brick processedBrick = WebBridge.ProcessBrick (brick, item);

			return this.CreatePanelContent (processedBrick, item);
		}


		/// <summary>
		/// Create a panel according to a brick
		/// </summary>
		/// <param name="brick">Brick to use</param>
		/// <returns></returns>
		private List<Dictionary<string, object>> CreatePanelContent(Brick brick, WebTileDataItem item)
		{
			var list = new List<Dictionary<string, object>> ();

			var brickType = brick.GetFieldType ();
			var resolver = brick.GetResolver (brickType);

			if (item.DataType == TileDataType.CollectionItem)
			{
				var obj = resolver.DynamicInvoke (this.rootEntity);

				var col = (obj as IEnumerable).Cast<AbstractEntity> ().Where (c => c.GetType () == brickType).ToList ();

				LambdaExpression lambda = brick.GetLambda ();
				var accessor = this.coreSession.PanelFieldAccessorCache.Get (lambda);

				if (col.Any ())
				{
					foreach (var e in col)
					{
						var panels = this.CreatePanelsForEntity (brick, item, e);
						panels.ForEach (p => p["lambdaId"] = accessor.Id.ToString (CultureInfo.InvariantCulture));
						panels.ForEach (p => p["entityType"] = brickType.AssemblyQualifiedName);
						list.AddRange (panels);
					}
				}
				else
				{
					// This collection is empty, but we want to show its empty panel
					// so the user will be able to add one.
					var panel = this.CreateEmptyPanel (item);
					panel["lambdaId"] = accessor.Id.ToString (CultureInfo.InvariantCulture);
					panel["entityType"] = brickType.AssemblyQualifiedName;
					list.Add (panel);
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

				list.AddRange (this.CreatePanelsForEntity (brick, item, entity));
			}

			return list;
		}


		private List<Dictionary<string, object>> CreatePanelsForEntity(Brick brick, WebTileDataItem item, AbstractEntity entity)
		{
			var list = new List<Dictionary<string, object>> ();
			var parent = this.GetBasicPanelForm (item);
			list.Add (parent);

			var entityKey = this.GetEntityId (entity);
			parent["entityId"] = entityKey;

			parent["subViewControllerMode"] = Tools.ViewControllerModeToString (item.SubViewControllerMode);
			parent["subViewControllerSubTypeId"] = Tools.ControllerSubTypeIdToString (item.SubViewControllerSubTypeId);

			this.AddControllerSpecificData (parent, brick, item, entity);

			var children = this.CreateChildren (brick);
			if (children != null && children.Any ())
			{
				list.AddRange (children);
			}

			return list;
		}


		private Dictionary<string, object> CreateEmptyPanel(WebTileDataItem item)
		{
			var panel = this.GetBasicPanelForm (item);
			panel["xtype"] = "emptysummary";

			return panel;
		}


		private Dictionary<string, object> GetBasicPanelForm(WebTileDataItem item)
		{
			var panel = new Dictionary<string, object> ();

			var controllerName = this.controllerMode.ToString ().ToLower (CultureInfo.InvariantCulture);
			panel["xtype"] = controllerName;

			string title = item.Title.ToSimpleText ();
			panel["title"] = title;

			var icon = IconManager.GetCSSClassName (item.IconUri, IconSize.Sixteen);
			if (icon != null)
			{
				panel["iconCls"] = icon;
			}

			return panel;
		}


		private void AddControllerSpecificData(Dictionary<string, object> parent, Brick brick, WebTileDataItem item, AbstractEntity entity)
		{
			switch (this.controllerMode)
			{
				case ViewControllerMode.Summary:
					PanelBuilder.AddControllerSpecificSummaryData (parent, item, entity);
					break;

				case ViewControllerMode.Edition:
					this.AddControllerSpecificEditionData (parent, brick);
					break;

				case ViewControllerMode.Creation:
					break;

				case ViewControllerMode.None:
					break;

				default:
					throw new NotImplementedException ("Make sure this switch has all possible branches");
			}
		}


		private static void AddControllerSpecificSummaryData(Dictionary<string, object> parent, WebTileDataItem item, AbstractEntity entity)
		{
			parent["html"] = entity.GetSummary ().ToString ();
			parent["hideRemoveButton"] = item.HideRemoveButton;
			parent["hideAddButton"] = item.HideAddButton;
		}


		private void AddControllerSpecificEditionData(Dictionary<string, object> parent, Brick brick)
		{
			var inputs = this.HandleInputs (brick);
			if (inputs != null && inputs.Any ())
			{
				parent["items"] = inputs;
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

				// /!\ Caution! Glaciers are melting! /!\
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
				throw new ArgumentException (string.Format (CultureInfo.InvariantCulture, "Expression {0} for input must be a lambda", expression.ToString ()));
			}

			var func = lambda.Compile ();
			var obj = func.DynamicInvoke (this.rootEntity);
			var entity = obj as AbstractEntity;

			var accessor = this.coreSession.PanelFieldAccessorCache.Get (lambda);
			var fieldType = lambda.ReturnType;

			var caption = EntityInfo.GetFieldCaption (lambda);
			string title = PanelBuilder.GetInputTitle (fieldProperties) ?? PanelBuilder.GetInputTitle (caption);
			string fieldName = caption.Id.ToString ().Trim ('[', ']');

			lambdaDictionnary["xtype"] = "hiddenfield";
			lambdaDictionnary["name"] = Tools.GetLambdaFieldName (fieldName);
			lambdaDictionnary["value"] = accessor == null ? "-1" : accessor.Id.ToString (CultureInfo.InvariantCulture);

			entityDictionnary["xtype"] = "textfield";
			entityDictionnary["fieldLabel"] = title;
			entityDictionnary["name"] = fieldName;

			if (accessor.IsEntityType)
			{
				var data = this.BusinessContext.Data
					.GetAllEntities (fieldType, DataExtractionMode.Sorted, this.DataContext)
					.Select (i => new object[]
		            {
		                this.GetEntityId (i),
		                i.GetCompactSummary ().ToString ()
		            })
					.ToList ();

				entityDictionnary["value"] = this.GetEntityId (entity);
				entityDictionnary["xtype"] = "epsitec.entity";
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

			if (fieldType == typeof (DateTime) ||
		        fieldType == typeof (DateTime?))
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
				var items = this.BusinessContext.Data.GetAllEntities (accessor.CollectionItemType, DataExtractionMode.Sorted, this.DataContext);
				var found = accessor.GetCollection (this.rootEntity).Cast<AbstractEntity> ();

				entityDictionnary["xtype"]  = "epsitec.checkboxes";
				entityDictionnary["defaultType"] = "checkboxfield";
				entityDictionnary["labelAlign"] = "left";
				var checkboxes = new List<object> ();
				entityDictionnary["items"]  = checkboxes;

				int i = 0;
				foreach (var item in items)
				{
					var dic = new Dictionary<string, object> ();
					checkboxes.Add (dic);

					dic["boxLabel"] = item.GetSummary ().ToSimpleText ();
					dic["name"] = string.Format (CultureInfo.InvariantCulture, "{0}[{1}]", entityDictionnary["name"], i++); // Copy the parent's ID
					dic["inputValue"] = this.GetEntityId (item);
					dic["checked"] = found.Contains (item);
					dic["uncheckedValue"] = ""; // We want to return "nothing" when nothing is checked (but we want to return something)
				}

				return list;
			}

			var underlyingType = fieldType.GetNullableTypeUnderlyingType ();

			if ((fieldType.IsEnum) ||
		            ((underlyingType != null) && (underlyingType.IsEnum)))
			{
				entityDictionnary["xtype"] = "epsitec.enum";
				entityDictionnary["value"] = obj.ToString ();
				entityDictionnary["storeClass"] = fieldType.AssemblyQualifiedName;

				return list;
			}

			Debug.WriteLine (
				string.Format (CultureInfo.InvariantCulture, "*** Field {0} of type {1} : no automatic binding implemented in PanelBuilder",
					lambda.ToString (), fieldType.FullName));

			return list;
		}


		private Dictionary<string, object> CreateHorizontalGroup(BrickProperty property)
		{

			var dic = new Dictionary<string, object> ();

			dic["xtype"] = "fieldset";
			dic["layout"] = "column";

			var title = Brick.GetProperty (property.Brick, BrickPropertyKey.Title).StringValue;
			dic["title"] = title;

			var list = this.CreateInput (property.Brick, null);

			foreach (var l in list)
			{
				l["columnWidth"] = 1.0 / list.Count (input => (string) input["xtype"] != "hiddenfield");
				l["margin"] = "0 5 0 0";
				l.Remove ("fieldLabel");
			}

			dic["items"] = list;

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


		private static string GetInputTitle(Caption caption)
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
				var func = lambda.Compile ();
				var child = func.DynamicInvoke (this.rootEntity) as AbstractEntity;

				if (child.IsNull ())
				{
					continue;
				}

				// Recursively build the panels
				var childPanel = PanelBuilder.BuildController (child, this.controllerMode, this.controllerSubTypeId, this.coreSession);
				list.AddRange (childPanel["items"] as List<Dictionary<string, object>>);
			}

			return list;

		}









	}


}
