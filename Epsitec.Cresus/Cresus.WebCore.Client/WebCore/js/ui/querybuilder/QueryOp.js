Ext.require([
],
function() {
  Ext.define('Epsitec.cresus.webcore.ui.querybuilder.QueryOp', {
    extend: 'Ext.Panel',
    alternateClassName: ['Epsitec.QueryOp'],

    /* Properties */

    operatorComboStore: null,
    operatorCombo: null,
    components: null,

    /* Constructor */

    constructor: function(builder) {
      this.components = [];

      var config = {
        layout: 'fit',
        closable: true,
        title: 'QueryOp',
        items: this.components
      };
      this.initOperatorDataStore();
      this.callParent([config]);
    }
  });
});
