Ext.define('Epsitec.cresus.webcore.LeftEntityList', {
  extend: 'Epsitec.cresus.webcore.EntityList',
  alternateClassName: ['Epsitec.LeftEntityList'],

  /* Config */

  multiSelect: false,

  /* Additional methods */

  onSelectionChange: function(entityItems) {
    this.columnManager.removeAllColumns();
    if (entityItems.length === 1) {
      this.columnManager.addEntityColumn('1', 'null', entityItems[0].id);
    }
  }
});
