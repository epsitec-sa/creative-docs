Ext.define('Epsitec.Cresus.Core.Static.EntityPanel', {
  extend: 'Epsitec.Cresus.Core.Static.ColumnPanel',

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
