Ext.define('Epsitec.cresus.webcore.EntityListPanel', {
  extend: 'Epsitec.cresus.webcore.ColumnPanel',

  /* Config */

  layout: 'fit',

  /* Constructor */

  constructor: function(options) {
    this.callParent(arguments);

    var databaseName = options.databaseName;
    var columnManager = options.columnManager;

    var entityList = Ext.create(
        'Epsitec.cresus.webcore.EntityList', databaseName, columnManager
        );

    this.add(entityList);

    return this;
  }
});
