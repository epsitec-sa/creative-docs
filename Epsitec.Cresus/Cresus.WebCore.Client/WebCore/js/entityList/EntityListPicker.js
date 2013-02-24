Ext.require([
  'Epsitec.cresus.webcore.entityList.EntityListPanel',
  'Epsitec.cresus.webcore.tools.EntityPicker'
],
function () {
  Ext.define('Epsitec.cresus.webcore.entityList.EntityListPicker', {
    extend: 'Epsitec.cresus.webcore.tools.EntityPicker',
    alternateClassName: ['Epsitec.EntityListPicker'],

    /* Properties */

    entityListPanel: null,

    /* Constructor */

    constructor: function (options) {
      var newOptions, list, callback;

      callback = Epsitec.Callback.create(this.handleEntityListSelectionChange, this);

      //  I've no clue why I need to assign the callback manually to both lists; I've
      //  observed that if I insert the callback before calling Ext.apply above, the
      //  property 'onselectionChange' will be copied as 'null'. [PA: 2013-02-24]

      list = {};
      Ext.apply(list, options.list);
      list.onSelectionChange = callback;

      this.entityListPanel = this.createEntityListPanel(list);

      newOptions = {
        items: [this.entityListPanel]
      };
      Ext.applyIf(newOptions, options);

      this.callParent([newOptions]);
      this.disableOkButton();

      return this;
    },

    /* Additional methods */

    createEntityListPanel: function (options) {
      return Ext.create('Epsitec.EntityListPanel', {
        container: {},
        list: options
      });
    },

    handleEntityListSelectionChange: function (entityItems) {
      if (entityItems.length == 0) {
        this.disableOkButton();
      } else {
        this.enableOkButton();
      }
    },

    getSelectedItems: function () {
      return this.entityListPanel.getEntityList().getSelectedItems();
    },

    statics: {
      showDatabase: function (databaseName, multiSelect, callback) {
        this.show(callback, {
          entityListTypeName: 'Epsitec.DatabaseEntityList',
          databaseName: databaseName,
          multiSelect: multiSelect,
          onSelectionChange: null
        });
      },

      showSet: function (viewId, entityId, databaseDefinition, callback) {
        this.show(callback, {
          entityListTypeName: 'Epsitec.SetEntityList',
          viewId: viewId,
          entityId: entityId,
          columnDefinitions: databaseDefinition.columns,
          sorterDefinitions: databaseDefinition.sorters,
          multiSelect: true,
          onSelectionChange: null
        });
      },

      show: function (callback, listOptions) {
        var entityListPicker = Ext.create('Epsitec.EntityListPicker', {
          list: listOptions,
          callback: callback
        });
        entityListPicker.show();
      }
    }
  });
});
