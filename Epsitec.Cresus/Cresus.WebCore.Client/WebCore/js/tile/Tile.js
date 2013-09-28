// This class is the base class for all tiles. The tiles are the elements that
// compose tile columns.

Ext.require([
  'Epsitec.cresus.webcore.tools.Tools',
  'Epsitec.cresus.webcore.ui.DropZone'
],
function() {
  Ext.define('Epsitec.cresus.webcore.tile.Tile', {
    extend: 'Ext.form.Panel',
    alternateClassName: ['Epsitec.Tile'],

    /* Configuration */

    minHeight: 50,
    minWidth: 200,
    maxWidth: 400,
    border: false,
    style: {
      borderRight: '1px solid #99BCE8',
      borderBottom: '1px solid #99BCE8',
      borderLeft: '1px solid #99BCE8'
    },
    bodyCls: 'tile',

    /* Properties */

    column: null,
    selected: false,
    actionMenu: null,
    maxCharForButtons: null,
    /* Constructor */

    constructor: function(options) {
      var column = options.column;

      var newOptions = {
        tools: this.createEntityTileTools(options),
        dockedItems: this.createEntityTileDockTools(options),   
      };

      if(Ext.isDefined(options.isRoot))
      {
        if(column.columnId != 0 && options.isRoot==true)
        {
          newOptions.closable = true;
        }
      }
      else
      {
        if(column.columnId != 0)
        {
          newOptions.closable = true;
        }
      }
      
      
      Ext.applyIf(newOptions, options);

      this.callParent([newOptions]);

      this.on('close',this.closeTile,this);

      return this;
    },

    /* Methods */
    closeTile: function() {
      var columnManager = this.column.columnManager;
      columnManager.removeColumns(this.column.columnId,columnManager.columns.length-1);
      columnManager.columns[this.column.columnId-1].selectTile(null);
    },

    createEntityTileDockTools: function(options) {
      var actions, toolbars;
      actions = options.actions;
      toolbars = [];
      var tile = this;
      Ext.Array.each(actions, function(a) {   
        if(a.displayMode == "Button")
        {
          
          var button = {};
          button.xtype = 'button';
          button.text = a.title;
          button.width = 400;
          button.cls = 'tile-button';
          button.overCls = 'tile-button-over';
          button.textAlign = 'left';
          
          button.requiresAdditionalEntity = a.requiresAdditionalEntity;
          button.handler = function() { this.handleAction(a.viewId); };
          button.scope = tile;

          var toolbar = Ext.create('Ext.Toolbar', {
            dock: 'bottom',
            width: 400,
            items: [button]
          });

          toolbars.unshift(toolbar);
        }

        if(a.displayMode == "OnDrop")
        {
          if(epsitecConfig.featureEntityBag) {
            var dropZone = Ext.create('Epsitec.DropZone',a.title, a.title,
              function (data) {
                this.handleTemplateAction(a.viewId, data.id);
                Epsitec.Cresus.Core.app.entityBag.removeEntityFromBag(data);
              }, tile);

            dropZone.requiresAdditionalEntity = a.requiresAdditionalEntity;
            Epsitec.Cresus.Core.app.entityBag.registerDropZone(dropZone);
            toolbars.unshift(dropZone);
          }
        }        
      });
      return toolbars;
    },

    createEntityTileTools: function(options)  {
      var tools, actions, needToCreateTool;

      actions = options.actions;
      needToCreateTool  = false;
      Ext.Array.each(actions, function(action) {
        if(action.displayMode=="Menu" || action.displayMode=="Default"){
          needToCreateTool = true;
        }
      });
      if (!Ext.isDefined(actions) || Ext.isEmpty(actions) || !needToCreateTool) {
        return options.tools;
      }

      tools = Ext.Array.clone(options.tools || []);
      tools.unshift(this.createActionTool(actions));
      return tools;
    },

    createActionTool: function(actions) {
      this.actionMenu = this.createActionMenu(actions);
      return {
        type: 'gear',
        handler: function(e, t) { this.actionMenu.showBy(t); },
        scope: this
      };
    },

    createActionMenu: function(actions) {
      return Ext.create('Ext.menu.Menu', {
        floating: true,
        plain: true,
        items: actions.map(this.createActionMenuItem, this)
      });
    },

    createActionMenuItem: function(action) {
      if(action.displayMode=="Menu" || action.displayMode=="Default")
      {
        return {
          text: action.title,
          requiresAdditionalEntity: action.requiresAdditionalEntity,
          handler: function() { this.handleAction(action.viewId); },
          scope: this
        };
      }
      else
        return null;
    },

    handleAction: function(viewId) {
      var callback = Epsitec.Callback.create(this.handleActionCallback, this);
      this.showAction(viewId, callback);
    },

    handleTemplateAction: function(viewId, aEntityId) {
      var callback = Epsitec.Callback.create(this.handleActionCallback, this);
      this.showTemplateAction(viewId, aEntityId, callback);
    },

    getEntityId: function() {
      // This method is supposed to be overriden in derived classes.
    },

    showAction: function(viewId, callback) {
      // This method is supposed to be overriden in derived classes.
    },

    showTemplateAction: function(viewId, aEntityId, callback) {
      // This method is supposed to be overriden in derived classes.
    },

    handleActionCallback: function() {
      this.column.refreshAll();
    },

    // Some actions in the menu requires an additional entity to be enabled. We
    // update their enabled status with this method here.
    updateActionMenuState: function(hasAdditionalEntity) {
      var items, item, i;

      if (this.actionMenu === null) {
        return;
      }

      items = this.actionMenu.items.items;

      for (i = 0; i < items.length; i += 1) {
        item = items[i];
        if (item.requiresAdditionalEntity) {
          if (hasAdditionalEntity) {
            item.enable();
          }
          else {
            item.disable();
          }
        }
      }

      items = this.dockedItems;
      if(!Ext.isDefined(items.items))
      {
        for (i = 0; i < items.length; i += 1) {
          item = items[i];
          if (item.items.items[0].requiresAdditionalEntity) {
            if (hasAdditionalEntity) {
              item.items.items[0].enable();
            }
            else {
              item.items.items[0].disable();
            }
          }
        }
      }
      else
      {
        for (i = 0; i < items.length; i += 1) {
          item = items.items[i];
          if (item.items.items[0].requiresAdditionalEntity) {
            if (hasAdditionalEntity) {
              item.items.items[0].enable();
            }
            else {
              item.items.items[0].disable();
            }
          }
        }
      }
      
    },

    // A tile might be selected by the user when he clicks on it. We keep track
    // of the selection status with these two methods.
    setSelected: function(selected) {
      this.selected = selected;
    },

    isSelected: function() {
      return this.selected;
    },

    // The getState, setState and isStateApplicable methods are used when the
    // columns are refreshed. As their instances are replaced by new ones, the
    // instances of tiles are also replaced by new ones and we must have a way
    // to restore their state.
    getState: function() {
      // This method is supposed to be overriden in derived classes.
    },

    setState: function(state) {
      // This method is supposed to be overriden in derived classes.
    },

    isStateApplicable: function(state) {
      // This method is supposed to be overriden in derived classes.
    }
  });
});
