Ext.require([
],
function () {
  Ext.define('Epsitec.cresus.webcore.ui.querybuilder.QueryOp', {
    extend: 'Ext.Panel',
    alternateClassName: ['Epsitec.QueryOp'],

    operatorComboStore: null,
    operatorCombo: null,
    components: null,

    constructor: function (builder) {
      var me = this;
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
