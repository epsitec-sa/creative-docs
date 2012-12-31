Ext.require([
  'Epsitec.cresus.webcore.plugins.AiderGroupReader',
  'Epsitec.cresus.webcore.tools.EntityPicker'
],
function() {
  Ext.define('Epsitec.cresus.webcore.plugins.AiderGroupPicker', {
    extend: 'Epsitec.cresus.webcore.tools.EntityPicker',
    alternateClassName: ['Epsitec.AiderGroupPicker'],

    /* Properties */

    treePanel: null,
    selectedGroupId: null,

    /* Constructor */

    constructor: function(options) {
      var newOptions;

      this.treePanel = this.createTreePanel(options);

      newOptions = {
        items: [this.treePanel],
        listeners: {
          resize: this.handleShow,
          scope: this
        }
      };
      Ext.applyIf(newOptions, options);

      this.callParent([newOptions]);
      return this;
    },

    /* Additional methods */

    createTreePanel: function(options) {
      return Ext.create('Ext.tree.Panel', {
        rootVisible: false,
        hideHeaders: true,
        lines: false,
        rowLines: true,
        useArrows: true,
        viewConfig: {
          stripeRows: true
        },
        store: this.createTreeStore(options),
        columns: [{
          xtype: 'treecolumn',
          dataIndex: 'summary',
          flex: 1
        }]
      });
    },

    createTreeStore: function(options) {
      return Ext.create('Ext.data.TreeStore', {
        model: 'Epsitec.cresus.webcore.field.EntityFieldListItem',
        proxy: {
          type: 'ajax',
          url: options.url,
          reader: Ext.create('Epsitec.AiderGroupReader')
        },
        nodeParam: 'group',
        defaultRootProperty: 'groups',
        root: options.groups,
        listeners: {
          beforeappend: this.handleTreeStoreAppend,
          scope: this
        }
      });
    },

    handleTreeStoreAppend: function(parentNode, childNode) {
      var data, iconCls;

      data = childNode.raw;
      if (Ext.isDefined(data.group)) {
        childNode.set('id', data.group.id);
        childNode.set('summary', data.group.summary);
      }

      if (Ext.isDefined(data.groups)) {
        childNode.set('expanded', true);
      }

      if (Ext.isDefined(data.hasSubgroups)) {
        childNode.set('leaf', !data.hasSubgroups);
      }

      iconCls = 'epsitec-aider-images-data-aidergroup-people-icon16';
      childNode.set('iconCls', iconCls);
    },

    handleShow: function() {
      this.selectItem(this.selectedGroupId);
    },

    selectItem: function(id) {
      var record = this.treePanel.getStore().getNodeById(id);
      this.treePanel.getSelectionModel().select(record);
    },

    getSelectedItems: function() {
      var records = this.treePanel.getSelectionModel().getSelection();
      return records.map(function(r) { return r.toItem(); });
    }
  });
});
