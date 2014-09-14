Ext.require([
],
function() {
  Ext.define('Epsitec.cresus.webcore.ui.querybuilder.QueryBuilderPanel', {
    extend: 'Ext.Panel',
    alternateClassName: ['Epsitec.QueryBuilderPanel'],

    /* Properties */

    columnDefinitions: null,
    /* Constructor */

    constructor: function(columnDefinitions) {
      this.columnDefinitions = columnDefinitions;
      var config = {
        region: 'center',
        title: 'Editeur',
        layout: {
          type: 'vbox'
        }
      };

      this.callParent([config]);
    },

    /* Methods */

    init: function() {
      var firstElement = Ext.create(
          'Epsitec.QueryElement', this, this.columnDefinitions, false);

      this.insert(firstElement);
      this.doLayout();
    },

    onAddElement: function(event, toolEl, panel) {
      var nextElement = Ext.create(
          'Epsitec.QueryElement', this, this.columnDefinitions, true);

      this.insert(nextElement);
      this.doLayout();
    }
  });
});
