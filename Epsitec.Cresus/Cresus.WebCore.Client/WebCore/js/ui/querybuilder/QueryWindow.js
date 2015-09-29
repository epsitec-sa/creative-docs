
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
          maximized: true,
          header: true,
          constrain: true,
          renderTo: Ext.get(tabManager.getLayout().getActiveItem().el),
          layout: {
            type: 'border',
            padding: 5
          },
          closable: true,
          closeAction: 'close',
          listeners: {
            'close': function () {
              Epsitec.Cresus.Core.app.reloadCurrentQueries();
            }
          },
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
