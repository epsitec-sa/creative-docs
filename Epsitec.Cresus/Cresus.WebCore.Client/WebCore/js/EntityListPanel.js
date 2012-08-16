Ext.define('Epsitec.cresus.webcore.EntityListPanel', {
  extend: 'Ext.panel.Panel',
  alternateClassName: ['Epsitec.EntityListPanel'],

  /* Config */

  layout: 'fit',

  /* Properties */

  entityList: null,

  /* Constructor */

  constructor: function(options) {
    this.callParent([options.container]);
    this.setupEntityList(options.list);
    return this;
  },

  /* Additional methods */

  setupEntityList: function(listOptions) {
    this.setLoading(true);
    Ext.Ajax.request({
      url: 'proxy/database/columns/' + listOptions.databaseName,
      callback: function(requestOptions, success, response) {
        this.setupEntityListCallback(listOptions, success, response);
      },
      scope: this
    });
  },

  setupEntityListCallback: function(options, success, response) {
    var json, columnDefinitions;

    this.setLoading(false);

    json = Epsitec.Tools.processResponse(success, response);
    if (json === null) {
      return;
    }

    columnDefinitions = json.content.columns;
    this.entityList = this.createEntityList(options, columnDefinitions);
    this.add(this.entityList);
  },

  createEntityList: function(options, columnDefinitions) {
    var type = options.editable ?
        'Epsitec.EditableEntityList' : 'Epsitec.EntityList';

    return Ext.create(type, {
      databaseName: options.databaseName,
      fields: this.createFields(columnDefinitions),
      columns: this.createColumns(columnDefinitions),
      multiSelect: options.multiSelect,
      onSelectionChange: options.onSelectionChange
    });
  },

  createFields: function(columnDefinitions) {
    var basicFields = this.createBasicFields(),
        dynamicFields = this.createDynamicFields(columnDefinitions);

    return basicFields.concat(dynamicFields);
  },

  createBasicFields: function() {
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
  },

  createDynamicFields: function(columnDefinitions) {
    return columnDefinitions.map(function(c) {
      return {
        name: c.name,
        type: c.type
      };
    });
  },

  createColumns: function(columnDefinitions) {
    var basicColumns = this.createBasicColumns(columnDefinitions),
        dynamicColumns = this.createDynamicColumns(columnDefinitions);

    return basicColumns.concat(dynamicColumns);
  },

  createBasicColumns: function(columnDefinitions) {
    var basicColumns = [
      {
        xtype: 'rownumberer',
        width: 35
      }
    ];

    if (Epsitec.Tools.isArrayEmpty(columnDefinitions)) {
      basicColumns.push({
        text: 'Summary',
        flex: 1,
        dataIndex: 'summary'
      });
    }

    return basicColumns;
  },

  createDynamicColumns: function(columnDefinitions) {
    return columnDefinitions.map(function(c) {
      return {
        text: c.title,
        flex: 1,
        dataIndex: c.name
      };
    });
  },

  getEntityList: function() {
    return this.entityList;
  }
});
