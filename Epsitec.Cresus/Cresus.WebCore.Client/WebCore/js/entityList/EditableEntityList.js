Ext.require([
  'Epsitec.cresus.webcore.entityList.EntityList'
],
function() {
  Ext.define('Epsitec.cresus.webcore.entityList.EditableEntityList', {
    extend: 'Epsitec.cresus.webcore.entityList.EntityList',
    alternateClassName: ['Epsitec.EditableEntityList'],

    /* Properties */

    creationViewId: null,

    /* Constructor */

    constructor: function(options) {
      var newOptions = {
        toolbarButtons: this.createEditionButtons(options)
      };
      Ext.applyIf(newOptions, options);

      this.callParent([newOptions]);
      return this;
    },

    /* Additional methods */

    createEditionButtons: function(options) {
      var buttons = [];

      if (options.enableCreate) {
        buttons.push(Ext.create('Ext.Button', {
          text: options.addLabel,
          iconCls: 'icon-add',
          listeners: {
            click: this.onAddHandler,
            scope: this
          }
        }));
      }

      if (options.enableDelete) {
        buttons.push(Ext.create('Ext.Button', {
          text: options.removeLabel,
          iconCls: 'icon-remove',
          listeners: {
            click: this.onRemoveHandler,
            scope: this
          }
        }));
      }

      return buttons;
    },

    onAddHandler: function() {
      this.handleAdd();
    },

    onRemoveHandler: function() {
      var entityItems;

      entityItems = this.getSelectedItems();
      if (entityItems.length === 0) {
        return;
      }

      this.handleRemove(entityItems);
    }
  });
});
