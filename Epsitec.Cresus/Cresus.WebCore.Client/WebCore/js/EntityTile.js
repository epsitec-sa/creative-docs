Ext.require([
  'Epsitec.cresus.webcore.Tile',
  'Epsitec.cresus.webcore.Tools'
],
function() {
  Ext.define('Epsitec.cresus.webcore.EntityTile', {
    extend: 'Epsitec.cresus.webcore.Tile',
    alternateClassName: ['Epsitec.EntityTile'],

    /* Properties */

    entityId: null,
    selectedClass: 'tile-selected',

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
      var menu = this.createActionMenu(actions);
      return {
        type: 'gear',
        handler: function(e, t) { menu.showBy(t); }
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
        handler: function() { this.handleAction(action.viewId); },
        scope: this
      };
    },

    handleAction: function(viewId) {
      var callback = Epsitec.Callback.create(this.handleActionCallback, this);
      this.column.showAction(viewId, this.entityId, callback);
    },

    handleActionCallback: function() {
      this.column.refreshAll();
    },

    select: function(selected) {
      if (this.isSelected() !== selected) {
        this.setSelected(selected);
        if (selected) {
          this.addCls(this.selectedClass);
        }
        else {
          this.removeCls(this.selectedClass);
        }
      }
    }
  });
});
