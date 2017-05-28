API
===

### Join to the server
  - [C] `{"type":"join","nick":"Anon"}`
  - [S] `{"type":"join","status":0}`
    - 0 - Success
    - 1 - NickInvalid
    - 2 - NickOccupied
    - 3 - AlreadyJoined

### Get list of players
  - [C] `{"type":"get_players"}`
  - [S] `{"type":"players","players":[{"nick":"Player1","id":"GUID","status":1},{"nick":"Player2","id":"GUID","status":1}]}`
    - 0 - Connected
    - 1 - Joined
    - 2 - OnGame
    - 3 - Left
    - 4 - Disconnected

### Invite player
  - [C] `{"type":"send_invite","player_id":"GUID"}`
  - [S] `{"type":"send_invite","player_id":"GUID","status":0}`
    - 0 - Success
    - 1 - SelfInvite
    - 2 - PlayerNotExist
    - 3 - AlreadyInvited

### Invite notification
  - [S] `{"type":"invite","player_id":"GUID"}`

### Answer the player's invitation
  - [C] `{"type":"answer_invite","player_id":"GUID","answer":0}`
    - 0 - Accept
    - 1 - Reject
  - [S] `{"type":"answer_invite","player_id":"GUID","status":0}`
    - 0 - Success
    - 1 - InvalidPlayer
    - 2 - NotInvited

### Game created notification
  - [S] `{"type":"game","white_player_id":"GUID","black_player_id":"GUID"}`

### Send move
  - [C] `{"type":"move","game_id":"GUID","move":"???"}`
  - [S] `{"type":"move","game_id":"GUID","status":0}`
    - 0 - Success
    - 1 - GameNotExist
    - 2 - NotPlayersTurn
    - 3 - InvalidMove

### End turn
  - [C] `{"type":"end_turn","game_id":"GUID"}`
  - [S] `{"type":"end_turn","game_id":"GUID","status":0}`
    - 0 - Success
    - 1 - GameNotExist
    - 2 - NotPlayersTurn

### Move from opponent notification
   - [S] `{"type":"move_done","player_id":"GUID","move":"???"}`

### Player change (status changed or new player joined)
  - [S] `{"type":"player","player":{"nick":"Player1","id":"GUID","status":1}}`
    - 0 - Connected
    - 1 - Joined
    - 2 - OnGame
    - 3 - Left
    - 4 - Disconnected

### Ping server
  - [C] `{"type":"ping"}`
  - [S] `{"type":"pong"}`