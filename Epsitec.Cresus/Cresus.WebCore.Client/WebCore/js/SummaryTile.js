Ext.define('Epsitec.cresus.webcore.SummaryTile', {
  extend: 'Epsitec.cresus.webcore.Tile',
  alternateClassName: ['Epsitec.SummaryTile'],
  alias: 'widget.summarytile',

  /* Config */

  margin: '0 0 5 0',
  bodyCls: 'summary',
  overCls: 'over-summary',

  /* Properties */

  isRoot: false,
  subViewMode: '2',
  subViewId: 'null',
  autoCreatorId: null,

  /* Constructor */

  constructor: function(options)  {
    this.addRefreshButton(options);
    this.callParent(arguments);
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

  addRefreshButton: function(options)  {
    if (options.isRoot) {
      options.tools = options.tools || [];
      options.tools.push({
        type: 'refresh',
        tooltip: 'Refresh entity',
        handler: function() { this.column.refresh(); },
        scope: this
      });
    }
  },

  addEntityColumn: function(entityId, refresh) {
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

    if (!success) {
      Epsitec.ErrorHandler.handleError(response);
      return;
    }

    json = Epsitec.Tools.decodeJson(response.responseText);
    if (json === null) {
      return;
    }

    newEntityId = json.content;
    refresh = this.entityId !== newEntityId;
    this.addEntityColumn(newEntityId, refresh);
  }
});
