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
      columnDefinitions: columnDefinitions,
      multiSelect: options.multiSelect,
      onSelectionChange: options.onSelectionChange
    });
  },

  getEntityList: function() {
    return this.entityList;
  }
});
