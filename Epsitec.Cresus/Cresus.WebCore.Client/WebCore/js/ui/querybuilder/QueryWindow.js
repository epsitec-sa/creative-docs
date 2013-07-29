Ext.require([
  'Epsitec.cresus.webcore.ui.querybuilder.QueryElement',
  'Epsitec.cresus.webcore.ui.querybuilder.QueryOp',
  'Epsitec.cresus.webcore.ui.querybuilder.QueryBuilderPanel'
],
function() {
  Ext.define('Epsitec.cresus.webcore.ui.querybuilder.QueryWindow', {
    extend: 'Ext.Window',
    alternateClassName: ['Epsitec.QueryWindow'],

    /* Properties */

    finalQuery: null,
    myQueryTree: null,
    builder: null,

    /* Constructor */

    constructor: function(columnDefinitions) {
      var tabManager, store, config;

      this.builder = Ext.create('Epsitec.QueryBuilderPanel', columnDefinitions);
      store = Ext.create('Ext.data.TreeStore', {
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

      tabManager = Epsitec.Cresus.Core.getApplication().tabManager;

      config = {
        title: 'Editeur de requêtes',
        width: 800,
        height: 600,
        header: 'false',
        constrain: true,
        renderTo: Ext.get(tabManager.getLayout().getActiveItem().el),
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
