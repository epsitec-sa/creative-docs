Ext.define('Epsitec.cresus.webcore.LeftEntityList', {
  extend: 'Epsitec.cresus.webcore.EntityList',
  alternateClassName: ['Epsitec.LeftEntityList'],

  /* Config */

  multiSelect: false,

  /* Additional methods */

  onSelectionChange: function(entityIds) {
    this.columnManager.removeAllColumns();
    if (entityIds.length === 1) {
      this.columnManager.addEntityColumn('1', 'null', entityIds[0]);
    }
  }
});
