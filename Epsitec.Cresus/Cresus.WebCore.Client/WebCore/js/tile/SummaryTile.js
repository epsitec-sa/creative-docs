// This class represents tiles that are the summary of a single entity. It is
// mainly composed of a summary text, and if its entity does not exist (like
// the comment for a person for instance), a click on it will instantiate the
// comment.

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

    /* Configuration */

    //overCls: 'tile-over',

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

    /* Methods */

    createSummaryTools: function(options)  {
      var tools = Ext.Array.clone(options.tools || []);

      if (options.isRoot) {
        tools.push({
          type: 'refresh',
          tooltip: Epsitec.Texts.getRefreshTip(),
          handler: function() { this.column.refresh(); },
          scope: this
        });

        tools.push({
          type: 'pin',
          tooltip: Epsitec.Texts.getPinToBagTip(),
          handler: function(e, t) { 
            var app = Epsitec.Cresus.Core.getApplication();
            /*var summaryLines = this.body.dom.textContent.replace(/\r\n/g, '\n').split('\n');

            var cleanSummaryLines = new Array();
            for(var i = 0; i<summaryLines.length; i++){
                if (summaryLines[i]){
                  cleanSummaryLines.push(summaryLines[i]);
              }
            }*/
            //var summary = this.initialConfig.html.split(/<br\s*[\/]?>/gi)[0];
            var entity = {
                  summary: this.initialConfig.html,
                  entityType: this.title,
                  id: this.entityId
                };
        
            app.addEntityToBag(entity,this.bodyClickHandler);
          },
          scope: this
        });

      }

      return tools;
    },

    openNextTile: function(callback) {
      this.column.selectTile(this);
      this.column.addEntityColumnWithCallback(
          this.subViewMode, this.subViewId, this.entityId, callback
      );
    },

    addEntityColumn: function(entityId, refresh) {
      this.column.selectTile(this);
      if(Ext.isDefined(this.subViewId)) {
        this.column.addEntityColumn(
          this.subViewMode, this.subViewId, entityId, refresh
        );
      }
      
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

    // Overrides the method defined in Tile.
    getState: function() {
      return {
        type: 'summaryTile',
        entityId: this.entityId
      };
    },

    // Overrides the method defined in Tile.
    setState: function(state) {
      this.select(true);
    },

    // Overrides the method defined in Tile.
    isStateApplicable: function(state) {
      return state.type === 'summaryTile' && state.entityId === this.entityId;
    }
  });
});
