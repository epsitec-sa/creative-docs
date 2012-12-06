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
    cls: 'menu',

    /* Properties */

    application: null,

    /* Constructor */

    constructor: function() {
      this.callParent(arguments);
      this.add(
          this.createMenuGroup(),
          '->',
          this.createScopeSelector(),
          this.createToolsGroup()
      );
      return this;
    },

    /* Additional methods */

    createMenuGroup: function() {
      var group = Ext.create('Ext.container.ButtonGroup', {
        title: Epsitec.Texts.getDatabasesTitle(),
        headerPosition: 'bottom'
      });

      Ext.Ajax.request({
        url: 'proxy/database/list',
        callback: function(options, success, response) {
          this.createMenuGroupCallback(success, response, group);
        },
        scope: this
      });

      return group;
    },

    createMenuGroupCallback: function(success, response, group) {
      var json, menuItems, i, menuItem;

      json = Epsitec.Tools.processResponse(success, response);
      if (json === null) {
        return;
      }

      menuItems = json.content.menu;

      for (i = 0; i < menuItems.length; i += 1) {
        menuItem = this.createMenuItem(menuItems[i], true);
        group.add(menuItem);
      }
    },

    createMenuItem: function(menuItem, topLevel) {
      switch (menuItem.type) {
        case 'database':
          return this.createDatabaseButton(menuItem, topLevel);

        case 'subMenu':
          return this.createSubMenu(menuItem, topLevel);

        default:
          throw 'invalid menu item type: ' + menuItem.type;
      }
    },

    createDatabaseButton: function(menuItem, topLevel) {
      return this.createButton({
        text: menuItem.title,
        handler: function() { this.databaseClickCallback(menuItem); },
        iconCls: this.getItemIconClass(menuItem, topLevel)
      });
    },

    databaseClickCallback: function(database) {
      this.application.tabManager.showEntityTab(database);
    },

    createSubMenu: function(menuItem, topLevel) {
      var items, item, i;

      items = [];

      for (i = 0; i < menuItem.items.length; i += 1) {
        item = this.createMenuItem(menuItem.items[i], false);
        items.push(item);
      }

      return {
        text: menuItem.title,
        iconCls: this.getItemIconClass(menuItem, topLevel),
        scale: 'large',
        iconAlign: 'top',
        menu: {
          xtype: 'menu',
          plain: true,
          items: items
        }
      };
    },

    getItemIconClass: function(menuItem, topLevel) {
      return topLevel ? menuItem.iconLarge : menuItem.iconSmall;
    },

    createScopeSelector: function() {
      var scopeSelector = Ext.create('Epsitec.cresus.webcore.ScopeSelector', {
        margin: '5 5 5 5'
      });

      return Ext.create('Ext.container.ButtonGroup', {
        title: Epsitec.Texts.getScopeTitle(),
        headerPosition: 'bottom',
        items: [scopeSelector]
      });
    },

    createToolsGroup: function() {
      var aboutButton, logoutButton;

      aboutButton = this.createButton({
        text: Epsitec.Texts.getAboutLabel(),
        handler: this.aboutButtonHandler,
        iconCls: 'epsitec-cresus-core-images-data-feedback-icon32'
      });

      logoutButton = this.createButton({
        text: Epsitec.Texts.getLogoutLabel(),
        handler: this.logoutButtonHandler,
        iconCls: 'epsitec-cresus-core-images-action-logout-icon32'
      });

      return Ext.create('Ext.container.ButtonGroup', {
        title: Epsitec.Texts.getToolsTitle(),
        headerPosition: 'bottom',
        items: [aboutButton, logoutButton]
      });
    },

    aboutButtonHandler: function() {
      var title, url;

      title = Epsitec.Texts.getAboutLabel();
      url = 'proxy/page/about';

      this.application.tabManager.showPageTab(title, url);
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
      var newOptions = {
        scale: 'large',
        scope: this,
        iconAlign: 'top'
      };
      Ext.applyIf(newOptions, options);

      return Ext.create('Ext.Action', newOptions);
    }
  });
});
