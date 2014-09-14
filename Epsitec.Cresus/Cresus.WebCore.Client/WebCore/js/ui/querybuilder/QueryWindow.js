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
    queryTree: null,
    builder: null,
    queryNameField: null,

    /* Constructor */

    constructor: function(columnDefinitions) {
      var tabManager, store, config;

      this.builder = Ext.create('Epsitec.QueryBuilderPanel', columnDefinitions);
      store = Ext.create('Ext.data.JsonStore', {
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

      this.queryTree = Ext.create('Ext.tree.Panel', {
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
        dockedItems: [{
          xtype: 'toolbar',
          dock: 'top',
          items: this.createQuerySavingTools()
        }],
        items: [{
          region: 'west',
          title: 'Mes requêtes',
          width: 200,
          split: true,
          collapsible: true,
          collapsed: false,
          floatable: false,
          items: this.queryTree
        }, this.builder]
      };

      this.builder.init();

      this.callParent([config]);
    },

    saveQuery : function ()
    {

    },

    removeQuery : function ()
    {

    },

    createQuerySavingTools : function ()
    {
      var buttons = [];

      buttons.push({
        xtype: 'label',
        text: 'Nom de la requête :'
      });

      this.queryNameField = Ext.create('Ext.form.field.Text',{
        width: 80,
        emptyText: '',
        name: 'queryName'
      });

      buttons.push(this.queryNameField);

      buttons.push({
        xtype: 'label',
        text: 'Publique ?'
      });

      buttons.push(Ext.create('Ext.Button', {
        text: 'Enregistrer',
        iconCls: 'icon-add',
        listeners: {
          click: this.saveQuery,
          scope: this
        }
      }));

      buttons.push(Ext.create('Ext.Button', {
        text: 'Supprimer',
        iconCls: 'icon-remove',
        listeners: {
          click: this.removeQuery,
          scope: this
        }
      }));

      return buttons;
    }
  });
});
