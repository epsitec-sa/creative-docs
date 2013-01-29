Ext.require([
  'Epsitec.cresus.webcore.tools.Tools'
],
function() {
  Ext.define('Epsitec.cresus.webcore.tile.Tile', {
    extend: 'Ext.form.Panel',
    alternateClassName: ['Epsitec.Tile'],

    /* Config */

    minHeight: 50,
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

    /* Constructor */

    constructor: function(options) {
      var newOptions = {
        tools: this.createEntityTileTools(options)
      };
      Ext.applyIf(newOptions, options);

      this.callParent([newOptions]);
      return this;
    },

    /* Additional methods */

    createEntityTileTools: function(options)  {
      var tools, actions, noActions;

      actions = options.actions;
      noActions = Epsitec.Tools.isUndefined(actions) ||
          Epsitec.Tools.isArrayEmpty(actions);

      if (noActions) {
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
      return {
        text: action.title,
        requiresAdditionalEntity: action.requiresAdditionalEntity,
        handler: function() { this.handleAction(action.viewId); },
        scope: this
      };
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
    },

    setSelected: function(selected) {
      this.selected = selected;
    },

    isSelected: function() {
      return this.selected;
    }
  });
});
