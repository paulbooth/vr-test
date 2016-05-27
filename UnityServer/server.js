var app = require('express')();
var http = require('http').Server(app);
var io = require('socket.io')(http);

app.get('/', function(req, res){
  res.sendfile('index.html');
});

io.on('connection', function(socket){
  console.log('connected');
  socket.on('cube', function(msg){
    console.log('message: ' + msg);
    io.emit('cube', {'size': msg});
  });

  socket.on('move', function(msg){
    console.log('move', msg);
  })
});

http.listen(3000, function(){
  console.log('listening on *:3000');
});
