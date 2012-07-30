Ext.define('Epsitec.cresus.webcore.Menu', {
  extend: 'Ext.Toolbar',
  alternateClassName: ['Epsitec.Menu'],

  /* Properties */

  application: null,

  /* Constructor */

  constructor: function() {
    this.callParent(arguments);
    this.add(this.createDatabasesGroup(), '->', this.createToolsGroup());
    return this;
  },

  /* Additional methods */

  createDatabasesGroup: function() {
    var group = Ext.create('Ext.container.ButtonGroup', {
      title: 'Databases',
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

    if (!success) {
      Epsitec.ErrorHandler.handleError(response);
      return;
    }

    json = Epsitec.Tools.decodeJson(response.responseText);
    if (json === null) {
      return;
    }

    databases = json.content;

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

  createToolsGroup: function() {
    var aboutButton, logoutButton;

    aboutButton = this.createButton({
      text: 'About',
      handler: this.aboutButtonHandler,
      iconCls: 'epsitec-cresus-core-images-base-softwareuserrole-icon32'
    });

    logoutButton = this.createButton({
      text: 'Logout',
      handler: this.logoutButtonHandler,
      iconCls: 'epsitec-cresus-core-images-usermanager-icon32'
    });

    return Ext.create('Ext.container.ButtonGroup', {
      title: 'Options',
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
      callback: function() { window.location.reload(); }
    });
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
