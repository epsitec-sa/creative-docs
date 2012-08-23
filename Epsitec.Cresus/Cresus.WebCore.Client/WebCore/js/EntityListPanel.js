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
      url: 'proxy/database/definition/' + listOptions.databaseName,
      callback: function(requestOptions, success, response) {
        this.setupEntityListCallback(listOptions, success, response);
      },
      scope: this
    });
  },

  setupEntityListCallback: function(options, success, response) {
    var json, columnDefinitions, sorterDefinitions, type;

    this.setLoading(false);

    json = Epsitec.Tools.processResponse(success, response);
    if (json === null) {
      return;
    }

    columnDefinitions = json.content.columns;
    sorterDefinitions = json.content.sorters;

    type = options.editable ?
        'Epsitec.EditableEntityList' : 'Epsitec.EntityList';

    this.entityList = Ext.create(type, {
      databaseName: options.databaseName,
      columnDefinitions: columnDefinitions,
      sorterDefinitions: sorterDefinitions,
      multiSelect: options.multiSelect,
      onSelectionChange: options.onSelectionChange
    });

    this.add(this.entityList);
  },

  getEntityList: function() {
    return this.entityList;
  }
});
