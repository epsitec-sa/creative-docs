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

    /* Constructor */

    constructor: function(options) {
      var newOptions = {
        tools: this.createEntityTileTools(options)
      };
      Ext.applyIf(newOptions, options);

      this.callParent([newOptions]);
      return this;
    },

    /* Methods */

    createEntityTileTools: function(options)  {
      var tools, actions;

      actions = options.actions;

      if (!Ext.isDefined(actions) || Ext.isEmpty(actions)) {
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
