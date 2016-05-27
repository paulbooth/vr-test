
$(function onload() {
var RECOGNIZE_THRESHOLD = 0.25;
var NUM_POINTS_TO_ZOMBIE = 90;

var socket;

var canvas, c;
var isDown, points, strokeId, recognizer, lastResult, color;


function rand(low, high) {
  return Math.floor((high - low + 1) * Math.random()) + low;
}

function getPos(e){
  var offset=$(canvas).offset()
  return {
    x:e.pageX-offset.left,
    y:e.pageY-offset.top,
  }
}

function getPoint(e, strokeId) {
  var offset=$(canvas).offset()
  return new Point(e.pageX-offset.left,
                   e.pageY-offset.top,
                   strokeId);
}

function onMouseDown(e) {
  isDown = true;
  var point = getPoint(e, ++strokeId);
  points.push(point);

  color = [rand(0,200), rand(0,200), rand(0,200)];
  var clr = "rgb(" + color[0] + "," + color[1] + "," + color[2] + ")";
  c.strokeStyle = clr;
  c.fillStyle = clr;
  c.fillRect(point.X - 4, point.Y - 4, 9, 9);
  updateClearText();
}

function onMouseMove(e) {
  if (isDown) {
    var point = getPoint(e, strokeId);
    points.push(point);
    drawConnectedPoint(points.length - 2, points.length - 1);
    if (points.length % NUM_POINTS_TO_ZOMBIE == 0) {
      updateClearText();
    }
  }
}

function drawConnectedPoint(from, to) {
  c.beginPath();
  c.moveTo(points[from].X, points[from].Y);
  c.lineTo(points[to].X, points[to].Y);
  c.closePath();
  c.stroke();
}

function getPointsSaveString() {
  var s = 'this.PointClouds.push(new PointCloud(name, new Array(';
  points.forEach(function(p, i) {
    s += '\n\tnew Point(' + p.X + ', ' + p.Y +', ' + p.ID + ')';
    if (i < points.length - 1) {
      s += ',';
    }
  });
  s += '\n)));'
  return s;
}

function onMouseUp(e) {
  if (isDown) {
    isDown = false;
    lastResult = recognizer.Recognize(points);
    // console.log(lastResult);
    // console.log(getPointsSaveString());
    updateShapeButton();
  }
}

function updateShapeButton() {
  if (lastResult && lastResult.Score > RECOGNIZE_THRESHOLD) {
    $('#shape').show().text('Send ' + lastResult.Name + ': (' + lastResult.Score.toFixed(3) + ')');
  } else {
    $('#shape').text('').hide();
  }
}

function clear() {
  $('#shape').text('');
  points.length = 0;
  if (strokeId > 0) {
    socket.emit('zombie', {'number': getNumberOfZombiesToSpawn()});
  }
  strokeId = 0;
  lastResult = null;
  c.clearRect(0,0,canvas.width,canvas.height);
  updateShapeButton();
  updateClearText();
}

function updateClearText() {
  var text = 'Clear';
  if (strokeId > 0) {
    text += ' (' + getNumberOfZombiesToSpawn() + ')';
  }
  $('#clear').text(text);
}

function getNumberOfZombiesToSpawn() {
  return strokeId + Math.floor(points.length / NUM_POINTS_TO_ZOMBIE);
}

function spawnDrawnObject() {
  if (lastResult) {
    socket.emit('spawn', {'name': lastResult.Name});
  }
  clear();
}

(function init() {
  canvas = document.getElementById('canvas');
  canvas.width = window.innerWidth;
  canvas.height = window.innerHeight;
  c = canvas.getContext('2d');
  isDown = false;
  points = new Array();
  recognizer = new PDollarRecognizer();

  clear();

  $(canvas).mousedown(onMouseDown);
  $(canvas).mousemove(onMouseMove);
  $(canvas).mouseup(onMouseUp);

  $('#clear').click(clear);
  $('#shape').click(spawnDrawnObject);

  socket = io();
})();

});
