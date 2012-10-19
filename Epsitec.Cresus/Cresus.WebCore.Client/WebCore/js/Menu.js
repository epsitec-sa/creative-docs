Ext.require([
  'Epsitec.cresus.webcore.ScopeSelector',
  'Epsitec.cresus.webcore.Texts',
  'Epsitec.cresus.webcore.Tools'
],
function() {
  Ext.define('Epsitec.cresus.webcore.Menu', {
    extend: 'Ext.Toolbar',
    alternateClassName: ['Epsitec.Menu'],

    /* Config */

    layout: {
      xtype: 'hbox',
      align: 'stretch'
    },

    /* Properties */

    application: null,

    /* Constructor */

    constructor: function() {
      this.callParent(arguments);
      this.add(
          this.createDatabasesGroup(),
          '->',
          this.createScopeSelector(),
          this.createToolsGroup()
      );
      return this;
    },

    /* Additional methods */

    createDatabasesGroup: function() {
      var group = Ext.create('Ext.container.ButtonGroup', {
        title: Epsitec.Texts.getDatabasesTitle(),
        headerPosition: 'bottom'
      });

      Ext.Ajax.request({
        url: 'proxy/database/list',
        callback: function(options, success, response) {
          this.createDatabasesGroupCallback(success, response, group);
        },
        scope: this
      });

      return group;
    },

    createDatabasesGroupCallback: function(success, response, group) {
      var json, databases, i;

      json = Epsitec.Tools.processResponse(success, response);
      if (json === null) {
        return;
      }

      databases = json.content.databases;

      for (i = 0; i < databases.length; i += 1) {
        this.createDatabaseButton(group, databases[i]);
      }
    },

    createDatabaseButton: function(group, database) {
      var databaseButton = this.createButton({
        text: database.title,
        handler: function() { this.databaseClickCallback(database); },
        iconCls: database.cssClass
      });
      group.add(databaseButton);
    },

    databaseClickCallback: function(database) {
      this.application.tabManager.showEntityTab(database);
    },

    createScopeSelector: function() {
      var scopeSelector = Ext.create('Epsitec.cresus.webcore.ScopeSelector', {
        margin: '5 5 5 5'
      });

      return Ext.create('Ext.container.ButtonGroup', {
        title: 'Scopes',
        headerPosition: 'bottom',
        items: [scopeSelector]
      });
    },

    createToolsGroup: function() {
      var aboutButton, logoutButton;

      aboutButton = this.createButton({
        text: Epsitec.Texts.getAboutLabel(),
        handler: this.aboutButtonHandler,
        iconCls: 'epsitec-cresus-core-images-base-softwareuserrole-icon32'
      });

      logoutButton = this.createButton({
        text: Epsitec.Texts.getLogoutLabel(),
        handler: this.logoutButtonHandler,
        iconCls: 'epsitec-cresus-core-images-usermanager-icon32'
      });

      return Ext.create('Ext.container.ButtonGroup', {
        title: Epsitec.Texts.getToolsTitle(),
        headerPosition: 'bottom',
        items: [aboutButton, logoutButton]
      });
    },

    aboutButtonHandler: function() {
      this.application.tabManager.showPageTab('About box', 'proxy/page/about');
    },

    logoutButtonHandler: function() {
      Ext.Ajax.request({
        url: 'proxy/log/out',
        method: 'POST',
        callback: this.logoutCallback,
        scope: this
      });
    },

    logoutCallback: function(options, success, response) {
      var json;

      json = Epsitec.Tools.processResponse(success, response);
      if (json === null) {
        return;
      }

      window.location.reload();
    },

    createButton: function(options) {
      return Ext.create('Ext.Action', {
        text: options.text,
        handler: options.handler,
        scale: 'large',
        scope: this,
        iconAlign: 'top',
        iconCls: options.iconCls
      });
    }
  });
});
