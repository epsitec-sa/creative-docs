Ext.define('Epsitec.cresus.webcore.EntityPanel', {
  extend: 'Epsitec.cresus.webcore.ColumnPanel',
  alternateClassName: ['Epsitec.EntityPanel'],

  /* Config */

  border: false,
  margin: 5,

  /* Properties */

  entityId: null,
  viewMode: '1',
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
