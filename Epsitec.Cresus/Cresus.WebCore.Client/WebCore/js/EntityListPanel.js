Ext.define('Epsitec.cresus.webcore.EntityListPanel', {
  extend: 'Epsitec.cresus.webcore.ColumnPanel',
  alternateClassName: ['Epsitec.EntityListPanel'],

  /* Config */

  layout: 'fit',

  /* Constructor */

  constructor: function(options) {
    this.callParent(arguments);

    var databaseName = options.databaseName;
    var columnManager = options.columnManager;

    this.add(Ext.create('Epsitec.EntityList', databaseName, columnManager));

    return this;
  }
});
