﻿Ext.require([
  'Epsitec.cresus.webcore.querybuilder.QueryElement',
  'Epsitec.cresus.webcore.querybuilder.QueryOp',
  'Epsitec.cresus.webcore.querybuilder.QueryBuilderPanel'
],
function() {
  Ext.define('Epsitec.cresus.webcore.ui.querybuilder.QueryWindow', {
    extend: 'Ext.Window',
    alternateClassName: ['Epsitec.QueryWindow'],

    finalQuery: null,
    myQueryTree: null,
    builder: null,

    constructor: function(columnDefinitions) {

      this.builder = Ext.create('Epsitec.QueryBuilderPanel', columnDefinitions);
      var store = Ext.create('Ext.data.TreeStore', {
        root: {
          expanded: true,
          children: [
            {
              text: 'Privée', expanded: true, children: [
                { text: '1', leaf: true },
                { text: '2', leaf: true }
              ]
            },
            {
              text: 'Publique', expanded: true, children: [
                { text: 'Canton de VD', leaf: true },
                { text: 'Lausannois', leaf: true }
              ]
            }
          ]
        }
      });

      this.myQueryTree = Ext.create('Ext.tree.Panel', {
        store: store,
        border: false,
        rootVisible: false
      });

      var config = {
        title: 'Editeur de requêtes',
        width: 800,
        height: 600,
        header: 'false',
        constrain: true,
        renderTo: Ext.get(Epsitec.Cresus.Core.getApplication().tabManager.getLayout().getActiveItem().el),
        layout: {
          type: 'border',
          padding: 5
        },
        closable: true,
        closeAction: 'hide',
        items: [{
          region: 'west',
          title: 'Mes requêtes',
          width: 200,
          split: true,
          collapsible: true,
          collapsed: false,
          floatable: false,
          items: this.myQueryTree
        }, this.builder]
      };

      this.builder.init();

      this.callParent([config]);
    }
  });
});