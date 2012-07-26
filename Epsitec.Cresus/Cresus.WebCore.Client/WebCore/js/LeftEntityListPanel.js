Ext.define('Epsitec.cresus.webcore.LeftEntityListPanel', {
  extend: 'Epsitec.cresus.webcore.EntityListPanel',
  alternateClassName: ['Epsitec.LeftEntityListPanel'],

  /* Config */

  multiSelect: false,

  /* Additional methods */

  // Overrides Epsitec.EntityListPanel.onSelectionChange.
  onSelectionChange: function(entityIds) {
    this.columnManager.removeAllColumns();
    if (entityIds.length === 1) {
      this.columnManager.addEntityColumn('1', 'null', entityIds[0]);
    }
  }
});
