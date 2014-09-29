
Ext.require(
  [],
  function() {
    Ext.define('Epsitec.cresus.webcore.ui.querybuilder.QueryWindow', {
      extend: 'Ext.Window',
      alternateClassName: ['Epsitec.QueryWindow'],

      /* Properties */

      /* Constructor */

      constructor: function(options) {
        var tabManager;
        tabManager = Epsitec.Cresus.Core.getApplication().tabManager;



        newOptions = {
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
          items: [ {
            xtype: "component",
            autoEl: {
                tag: "iframe",
                frameborder: 0,
                src: "proxy/page/query.html#" + tabManager.currentTab
            }
          } ]
        };

        Ext.applyIf(newOptions, options);
        this.callParent([newOptions]);
      }
    });
  });
