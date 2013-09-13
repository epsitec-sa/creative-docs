// This class is the base class for all tiles. The tiles are the elements that
// compose tile columns.

Ext.require([
  'Epsitec.cresus.webcore.tools.Tools'
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
    maxCharForButtons: 20,
    /* Constructor */

    constructor: function(options) {
      var newOptions = {
        tools: this.createEntityTileTools(options),
        dockedItems: this.createEntityTileDockTools(options),   
      };
      Ext.applyIf(newOptions, options);

      this.callParent([newOptions]);
      return this;
    },

    /* Methods */
    createEntityTileDockTools: function(options) {
      var actions, toolsbars;
      actions = options.actions;
      toolsbars = [];
      var tile = this;
      Ext.Array.each(actions, function(a) {   
        if(a.displayMode == "Button")
        {
          var text;
          var textLength = a.title.length;
          var isLarge = false;
          if(textLength>tile.maxCharForButtons)
          {
              isLarge = true;
              var end = a.title.substring(tile.maxCharForButtons,textLength);
              var sepPos = end.indexOf(' ') + tile.maxCharForButtons;
              var start = a.title.substring(0,sepPos);
              end = a.title.substring(sepPos,textLength);
              text = start + '<br>' + end;
          }
          else
          {
              text = a.title;
          }
          var button = {};
          button.xtype = 'button';
          button.text = text;
          button.width = 350;
          button.textAlign = 'left';
          if(isLarge)
          {
            button.scale = 'large';
          }
          button.requiresAdditionalEntity = a.requiresAdditionalEntity;
          button.handler = function() { this.handleAction(a.viewId); };
          button.scope = tile;

          var toolbar = Ext.create('Ext.Toolbar', {
            dock: 'bottom',
            width: 400,
            items: button
          });
          toolsbars.unshift(toolbar);
        }

        if(a.displayMode == "OnDrop")
        {
          var toolbar = Ext.create('Ext.Toolbar', {
            dock: 'bottom',
            width: 400,
            height: 40,
            items: tile.createDropZone(a.title)
          });

          toolsbars.unshift(toolbar);  
        }        
      });
      return toolsbars;        
    },

    //TODO: REFACTOR AS TILE DROPZONE
    createDropZone: function (title) {

      var dropZoneStore = Ext.create('Ext.data.Store', {
        model: 'Bag',
        data: [{
          id: 1,
          summary: "---",
          entityType: "---",
          data: "---"
        }]
      });
      var dropZone = Ext.create('Ext.view.View', {
          dock: 'bottom',
          cls: 'entity-view',
          tpl: '<tpl for=".">' +
                  '<div class="entitybag-target">' + title + '</div>' +
               '</tpl>',
          itemSelector: 'div.entitybag-source',
          overItemCls: 'entitybag-over',
          selectedItemClass: 'entitybag-selected',
          singleSelect: true,
          store: dropZoneStore,
          listeners: {
              render: Epsitec.Cresus.Core.app.entityBag.initializeEntityDropZone
          }
      });

      return dropZone;
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

    showAction: function(viewId, callback) {
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
