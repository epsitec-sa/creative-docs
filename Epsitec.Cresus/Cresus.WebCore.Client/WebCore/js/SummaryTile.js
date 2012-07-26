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

  constructor: function(o)  {
    var options = o || {};
    this.addRefreshButton(options);

    this.callParent([options]);
    return this;
  },

  /* Listeners */

  listeners: {
    render: function() {
      this.body.on('click', this.bodyClicked, this);
    }
  },

  /* Additional methods */

  addRefreshButton: function(options)  {
    options.tools = options.tools || [];

    if (options.isRoot) {
      options.tools.push({
        type: 'refresh',
        tooltip: 'Refresh entity',
        handler: function() { this.entityPanel.refresh(); },
        scope: this
      });
    }
  },

  bodyClicked: function() {
    if (this.autoCreatorId !== null) {
      this.autoCreateNullEntity();
    }
    else {
      this.entityPanel.addEntityColumn(
          this.subViewMode, this.subViewId, this.entityId, false
      );
    }
  },

  autoCreateNullEntity: function()  {
    this.setLoading();

    Ext.Ajax.request({
      url: 'proxy/entity/autoCreate',
      method: 'POST',
      params: {
        entityId: this.entityPanel.entityId,
        autoCreatorId: this.autoCreatorId
      },
      success: function(response, options) {
        this.setLoading(false);

        var json;

        try {
          json = Ext.decode(response.responseText);
        }
        catch (err) {
          options.failure.apply(this, arguments);
          return;
        }

        var newEntityId = json.content;
        var refresh = this.entityId !== newEntityId;

        this.entityPanel.addEntityColumn(
            this.subViewMode, this.subViewId, newEntityId, refresh
        );
      },
      failure: function(response, options) {
        this.setLoading(false);
        Epsitec.ErrorHandler.handleError(response);
      },
      scope: this
    }
    );
  }
});
