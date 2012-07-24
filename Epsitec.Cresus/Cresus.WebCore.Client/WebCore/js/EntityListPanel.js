Ext.define('Epsitec.cresus.webcore.EntityListPanel', {
  extend: 'Epsitec.cresus.webcore.ColumnPanel',
  alternateClassName: ['Epsitec.EntityListPanel'],

  /* Config */

  layout: 'fit',

  /* Properties */
  entityList: null,

  /* Constructor */

  constructor: function(options) {
    this.callParent(arguments);

    var databaseName = options.databaseName;

    this.entityList = Ext.create('Epsitec.EntityList', this, databaseName);

    this.add(this.entityList);

    return this;
  },

  onCreate: function(entityId) { },

  onDelete: function(entityIds) { },

  onRefresh: function() { },

  onSelectionChange: function(entityIds) {
    this.columnManager.clearColumns();

    if (entityIds.length === 1) {
      this.columnManager.addEntityColumn('summary', 'null', entityIds[0]);
    }
  }

});
