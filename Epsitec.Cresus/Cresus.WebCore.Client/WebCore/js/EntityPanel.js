Ext.define('Epsitec.cresus.webcore.EntityPanel', {
  extend: 'Epsitec.cresus.webcore.ColumnPanel',

  /* Config */

  border: false,
  margin: 5,

  /* Properties */

  entityId: null,
  viewMode: 'summary',
  viewId: 'null',

  /* Constructor */

  constructor: function(options) {
    Ext.Array.forEach(options.items,
        function(item) {
          item.entityPanel = this;
        },
        this
    );

    this.callParent(arguments);
  }
});
