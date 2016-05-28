var express = require('express');
var app = express();
var http = require('http').Server(app);
var io = require('socket.io')(http);

app.set('view engine', 'pug');
app.set('views', __dirname + '/views')
app.use(express.static('public'));

app.get('/', function(req, res){
  // res.sendFile('index.html');
  res.render('index');
});

// io stuff

io.on('connection', function(socket){
  console.log('connected');
  socket.on('zombie', function(msg){
    console.log('ZOMBIE!', msg);
    io.emit('zombie', msg);
  });

  socket.on('spawn', function(msg){
    console.log('spawn object!', msg);
    io.emit('spawn', msg);
  });
  socket.on('spawn custom', function(msg){
    console.log('CUSTOM SPAWN', msg);
    io.emit('spawn custom', msg);
  });
});


// start 'er up

http.listen(3000, function(){
  console.log('listening on *:3000');
});
