Ext.define('Epsitec.cresus.webcore.EntityListPanel', {
  extend: 'Ext.panel.Panel',
  alternateClassName: ['Epsitec.EntityListPanel'],

  /* Config */

  layout: 'fit',

  /* Properties */

  entityList: null,

  /* Constructor */

  constructor: function(options) {
    this.callParent([options]);
    this.add(options.entityList);
    return this;
  },

  /* Additional methods */

  getEntityList: function() {
    return this.entityList;
  },

  /* Static members */

  statics: {
    create: function(listOptions, containerOptions) {
      var newOptions = {
        entityList: this.createEntityList(listOptions)
      };
      Ext.applyIf(newOptions, containerOptions);

      return Ext.create('Epsitec.EntityListPanel', newOptions);
    },

    createEntityList: function(options) {
      var type = options.editable ?
          'Epsitec.EditableEntityList' : 'Epsitec.EntityList';

      return Ext.create(type, {
        databaseName: options.databaseName,
        fields: this.createFields(),
        columns: this.createColumns(),
        multiSelect: options.multiSelect,
        onSelectionChange: options.onSelectionChange
      });
    },

    createColumns: function() {
      return [
        {
          xtype: 'rownumberer',
          width: 35
        },
        {
          text: 'Summary',
          flex: 1,
          dataIndex: 'summary'
        }
      ];
    },

    createFields: function() {
      return [
        {
          name: 'id',
          type: 'string'
        },
        {
          name: 'summary',
          type: 'string'
        }
      ];
    }
  }
});
