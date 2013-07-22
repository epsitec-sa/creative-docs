Ext.require([
],
function() {
  Ext.define('Epsitec.cresus.webcore.ui.querybuilder.QueryBuilderPanel', {
    extend: 'Ext.Panel',
    alternateClassName: ['Epsitec.QueryBuilderPanel'],

    columnDefinitions: null,

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

    init: function() {
      var firstElement = Ext.create('Epsitec.QueryElement', this, this.columnDefinitions,false);
      this.insert(firstElement);
      this.doLayout();
    },

    onAddElement: function(event, toolEl, panel) {
      this.insert(Ext.create('Epsitec.QueryElement', this, this.columnDefinitions,true));
      this.doLayout();
    }
  });
});
