Ext.require([
  'Epsitec.cresus.webcore.tile.EntityTile',
  'Epsitec.cresus.webcore.tools.Texts',
  'Epsitec.cresus.webcore.tools.Tools',
  'Epsitec.cresus.webcore.tools.ViewMode'
],
function() {
  Ext.define('Epsitec.cresus.webcore.tile.SummaryTile', {
    extend: 'Epsitec.cresus.webcore.tile.EntityTile',
    alternateClassName: ['Epsitec.SummaryTile'],
    alias: 'widget.epsitec.summarytile',

    /* Config */

    overCls: 'tile-over',

    /* Properties */

    subViewMode: Epsitec.ViewMode.edition,
    subViewId: 'null',
    autoCreatorId: null,

    /* Constructor */

    constructor: function(options) {
      var newOptions = {
        tools: this.createSummaryTools(options)
      };
      Ext.applyIf(newOptions, options);

      this.callParent([newOptions]);
      return this;
    },

    /* Listeners */

    listeners: {
      render: function() {
        this.body.on('click', this.bodyClickHandler, this);
      }
    },

    bodyClickHandler: function() {
      if (this.autoCreatorId !== null) {
        this.autoCreateNullEntity();
      }
      else {
        this.addEntityColumn(this.entityId, false);
      }
    },

    /* Additional methods */

    createSummaryTools: function(options)  {
      var tools = Ext.Array.clone(options.tools || []);

      if (options.isRoot) {
        tools.push({
          type: 'refresh',
          tooltip: Epsitec.Texts.getRefreshTip(),
          handler: function() { this.column.refresh(); },
          scope: this
        });
      }

      return tools;
    },

    addEntityColumn: function(entityId, refresh) {
      this.column.selectTile(this);
      this.column.addEntityColumn(
          this.subViewMode, this.subViewId, entityId, refresh
      );
    },

    autoCreateNullEntity: function()  {
      this.setLoading();

      Ext.Ajax.request({
        url: 'proxy/entity/autoCreate',
        method: 'POST',
        params: {
          entityId: this.column.entityId,
          autoCreatorId: this.autoCreatorId
        },
        callback: this.autoCreateNullEntityCallback,
        scope: this
      });
    },

    autoCreateNullEntityCallback: function(options, success, response) {
      var json, newEntityId, refresh;

      this.setLoading(false);

      json = Epsitec.Tools.processResponse(success, response);
      if (json === null) {
        return;
      }

      newEntityId = json.content.entityId;
      refresh = this.entityId !== newEntityId;
      this.entityId = newEntityId;
      this.addEntityColumn(this.entityId, refresh);
    },

    getState: function() {
      return {
        type: 'summaryTile',
        entityId: this.entityId
      };
    },

    setState: function(state) {
      this.select(true);
    },

    isStateApplicable: function(state) {
      return state.type === 'summaryTile' && state.entityId === this.entityId;
    }
  });
});
