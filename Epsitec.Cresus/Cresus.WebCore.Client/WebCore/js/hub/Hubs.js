Ext.require([
],
function() {
  Ext.define('Epsitec.cresus.webcore.hub.Hubs', {
    alternateClassName: ['Epsitec.Hubs'],

    hub: null,

    constructor: function(ToastrFunc, username) {
      var me = this;
      $.getScript('signalr/hubs', function() {

        $.connection.hub.logging = false;
        if(epsitecConfig.featureChat)
        {
          $.chat({
            user: {
               Id: 'tempid',
               Name: username,
               ProfilePictureUrl: ''
            },
            // text displayed when the other user is typing
            typingText: ' écrit...',
            // the title for the user's list window
            titleText: 'Messagerie instantanée',
            // text displayed when there's no other users in the room
            emptyRoomText: "Aucun autre utilisateur connecté",
            // the adapter you are using
            adapter: new SignalRAdapter()
          });
        }
        // Start the connection
        var toastrInstance = new ToastrFunc();
        $.connection.hub.start(function() {
          toastrInstance.init(username, me);
          me.initHub(username);
          $.connection.chatHub.server.updateUserInfo(username,"");
        });
      });
    },

    initHub: function() {
      this.hub = $.connection.notificationHub;
      this.hub.server.setupUserConnection();      
    }
  });
});
