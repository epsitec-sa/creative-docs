Ext.require([
  'Epsitec.cresus.webcore.Callback',
  'Epsitec.cresus.webcore.EntityFieldListItem',
  'Epsitec.cresus.webcore.EntityPicker',
  'Epsitec.cresus.webcore.Texts'
],
function() {
  Ext.define('Epsitec.cresus.webcore.EntityFieldList', {
    extend: 'Ext.grid.Panel',
    alternateClassName: ['Epsitec.EntityFieldList'],

    /* Config */

    border: true,
    selModel: {
      selType: 'rowmodel',
      mode: 'MULTI'
    },
    hideHeaders: true,
    columns: [{
      flex: 1,
      dataIndex: 'displayed'
    }],

    /* Properties */

    databaseName: null,

    /* Constructor */

    constructor: function(values, readOnly, options) {
      var newOptions = {
        store: this.createStore(values),
        viewConfig: {
          emptyText: '&nbsp' + Epsitec.Texts.getEmptyItemText(),
          deferEmptyText: false
        }
      };

      if (!readOnly) {
        newOptions.tbar = this.createTBar();
        newOptions.viewConfig.plugins = {
          ptype: 'gridviewdragdrop'
        };
      }

      Ext.merge(newOptions, options);

      this.callParent([newOptions]);
      return this;
    },

    /* Additional methods */

    createTBar: function() {
      var buttonAdd, buttonRemove;

      buttonAdd = Ext.create('Ext.Button', {
        text: Epsitec.Texts.getAddLabel(),
        iconCls: 'icon-add',
        listeners: {
          click: this.onAddClick,
          scope: this
        }
      });

      buttonRemove = Ext.create('Ext.Button', {
        text: Epsitec.Texts.getRemoveLabel(),
        iconCls: 'icon-remove',
        listeners: {
          click: this.onRemoveClick,
          scope: this
        }
      });

      return [buttonAdd, buttonRemove];
    },

    createStore: function(values) {
      return Ext.create('Ext.data.JsonStore', {
        model: 'Epsitec.cresus.webcore.EntityFieldListItem',
        proxy: {
          type: 'memory',
          reader: {
            type: 'json',
            root: 'values'
          }
        },
        data: {
          values: values
        }
      });
    },

    onAddClick: function() {
      var callback = Epsitec.Callback.create(this.entityPickerCallback, this);
      Epsitec.EntityPicker.show(this.databaseName, true, callback);
    },

    entityPickerCallback: function(items) {
      var records = items.map(function(item) {
        return Ext.create('Epsitec.EntityFieldListItem', {
          submitted: item.id,
          displayed: item.summary
        });
      });
      this.store.add(records);
    },

    onRemoveClick: function() {
      var records = this.getSelectionModel().getSelection();
      this.store.remove(records);
    },

    getItems: function() {
      var records = this.store.getRange();
      return records.map(function(r) { return r.toItem(); });
    },

    resetContent: function() {
      this.store.rejectChanges();
    }
  });
});
