function SignalRChat() {
  this.hub = $.connection.chatHub;

  $.chat({
            user: {
               Id: 'tempid',
               Name: 'tempuser',
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

  //Initialize
  this.init = function(con,username, client) {
    this.hub = con.chatHub;
    this.hub.server.updateUserInfo(username,'meu@email.com');
  };
}
