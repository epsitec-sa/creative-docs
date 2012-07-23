Ext.define('Epsitec.cresus.webcore.Menu', {
  extend: 'Ext.Toolbar',
  alternateClassName: ['Epsitec.Menu'],

  /* Properties */

  application: null,

  /* Constructor */

  constructor: function(options) {
    this.application = options.application;
    this.items = this.items || [];

    this.setupDatabases();
    this.setupTools();

    this.callParent(arguments);
    return this;
  },

  /* Additional methods */

  setupDatabases: function() {
    Ext.Ajax.request({
      url: 'proxy/database/list',
      success: function(response, options) {
        var config;

        try {
          config = Ext.decode(response.responseText);
        }
        catch (err) {
          options.failure.apply(arguments);
          return;
        }

        this.handleMenus(config.content);
      },
      failure: function(response, options) {
        Epsitec.ErrorHandler.handleError(response);
      },
      scope: this
    });
  },

  handleMenus: function(databases) {
    var group = Ext.create('Ext.container.ButtonGroup', {
      title: 'Databases',
      headerPosition: 'bottom'
    });

    Ext.Array.forEach(
        databases,
        function(database) {
          var databaseAction = Ext.create('Ext.Action', {
            text: database.Title,
            handler: function() {
              this.application.tabManager.showEntityTab(database);
            },
            scale: 'large',
            scope: this,
            iconAlign: 'top',
            iconCls: database.CssClass
          });

          group.add(databaseAction);
        },
        this
    );

    this.insert(0, group);
  },

  setupTools: function() {
    var aboutAction = Ext.create('Ext.Action', {
      text: 'About',
      handler: function() {
        var title = 'About box';
        var url = 'proxy/page/about';
        this.application.tabManager.showPageTab(title, url);
      },
      scale: 'large',
      scope: this,
      iconAlign: 'top',
      iconCls: 'epsitec-cresus-core-images-base-softwareuserrole-icon32'
    });

    var logoutAction = Ext.create('Ext.Action', {
      text: 'Logout',
      handler: function() {
        Ext.Ajax.request({
          url: 'proxy/log/out',
          method: 'POST',
          callback: function() {
            window.location.reload();
          }
        });
      },
      scale: 'large',
      iconAlign: 'top',
      iconCls: 'epsitec-cresus-core-images-usermanager-icon32'
    });

    this.items.push(
        '->',
        {
          xtype: 'buttongroup',
          title: 'Options',
          headerPosition: 'bottom',
          items: [
            aboutAction,
            logoutAction
          ]
        }
    );
  }
});
