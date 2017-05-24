API
===

- Join to the server
  - [C] `{"type":"join","nick":"Anon"}`
  - [S] `{"type":"join","status":0}`
- Get list of players
  - [C] `{"type":"get_players"}`
  - [S] `{"type":"players","players":[{"nick":"Player1","id":"GUID","status":1},{"nick":"Player2","id":"GUID","status":1}]}`
- Invite player
  - [C] `{"type":"send_invite","player_id":"GUID"}`
  - [S] `{"type":"send_invite","player_id":"GUID","status":0}`
- Answer the invitation
  - [C] `{"type":"answer_invite","player_id":"GUID","answer":0}`
  - [S] `{"type":"answer_invite","player_id":"GUID","status":0}`
