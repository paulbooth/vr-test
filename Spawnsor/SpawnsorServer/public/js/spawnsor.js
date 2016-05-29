
$(function onload() {
var RECOGNIZE_THRESHOLD = 0.25;
var NUM_POINTS_TO_ZOMBIE = 90;

var socket;

var canvas, c;
var isDown, points, strokeId, recognizer, lastResult, color;


function rand(low, high) {
  return Math.floor((high - low + 1) * Math.random()) + low;
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
    console.log(lastResult);
    console.log(getPointsSaveString());
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
  var numZombies = getNumberOfZombiesToSpawn();
  if (numZombies > 0) {
    socket.emit('zombie', {'number': numZombies});
  }
  points.length = 0;
  strokeId = 0;
  lastResult = null;
  c.clearRect(0,0,canvas.width,canvas.height);
  color = [rand(0,200), rand(0,200), rand(0,200)];
  updateShapeButton();
  updateClearText();
}

function updateClearText() {
  var text = 'Clear';
  var numZombies = getNumberOfZombiesToSpawn();
  if (numZombies > 0) {
    text += ' (' + numZombies + ')';
  }
  $('#clear').text(text);
}

function getNumberOfZombiesToSpawn() {
  return 0; //strokeId + Math.floor(points.length / NUM_POINTS_TO_ZOMBIE);
}

function toXYPoint(point) {
  return {
    X: point.X / canvas.height,
    Y: point.Y / canvas.height
  };
}

function toXYPoints(points) {
  return points.map(toXYPoint);
}

function spawnJunk() {
  socket.emit('spawn custom', {
    'color': getNormalizedColor(),
    'points': toXYPoints(points),
    'corners': toXYPoints(getMeshCorners())
  });
  clear();
}

function vector(x, y){
  return new Point(x, y, 0)
}

function delta(a, b){
  return vector(a.X - b.X, a.Y - b.Y)
}

function angle(d){
  return Math.atan((1.0*d.Y)/d.X)
}

function angle_between(a, b){
  return Math.acos((a.X*b.X + a.Y*b.Y)/(len(a)*len(b)))
}

function len(v){
  return Math.sqrt(v.X*v.X + v.Y*v.Y)
}

function unit(c){
  var l=len(c)
  return vector(c.X/len(c), c.Y/len(c))
}

function scale(c, f){
  return vector(c.X*f, c.Y*f)
}

function add(a, b){
  return vector(a.X+b.X, a.Y+b.Y)
}

function rotate(v, a){
  return vector(  v.X*Math.cos(a) - v.Y*Math.sin(a),
          v.X*Math.sin(a) + v.Y*Math.cos(a))
}

function average(l){
  var X=0
  var Y=0
  for (var i=0; i<l.length; i++){X+=l[i].X; Y+=l[i].Y}
  return vector(X/l.length, Y/l.length)
}

function getMeshCorners() {
  var corners = [points[0]];
  var n = 0
  var t = 0
  var lastCorner = points[0];
  for (var i=1; i<points.length-2; i++){

    var pt=points[i+1]
    var d=delta(lastCorner, points[i-1])

    if (len(d)>10 && n>1){
      ac=delta(points[i-1], pt)
      if (Math.abs(angle_between(ac, d)) > Math.PI/8 || n<100){
        pt.indeX=i
        corners.push(pt)
        lastCorner=pt
        n=0
        t=0
      }
    }
    n++
  }

  corners.push(points[points.length-1])
  corners.push(points[0])

  // var oldFill = c.fillStyle;
  // var oldStroke = c.strokeStyle;

  // c.strokeStyle = 'rgb(0,0,0)';
  // c.fillStyle = 'rgba(0, 255, 255, 0.3)';
  // c.beginPath();
  // c.moveTo(corners[0].X, corners[0].Y);
  // for (var i = 1; i < corners.length; i++){
  //   c.lineTo(corners[i].X, corners[i].Y);
  // }
  // c.stroke();
  // c.fill();

  // c.fillStyle='rgba(255, 0, 0, 0.5)'
  // for (var i=0; i<corners.length; i++){
  //   c.beginPath();
  //   c.arc(corners[i].X, corners[i].Y, 4, 0, 2*Math.PI, false);
  //   c.fill();
  // }
  // c.fillStyle = oldFill;

  // console.log(corners);
  return corners;
}

function getNormalizedColor() {
  return color.map(function(val) { return val / 255; });
}

function spawnDrawnObject() {
  if (lastResult) {
    var maxX = -Infinity, maxY = -Infinity, minX = Infinity, minY = Infinity;
    points.forEach(function(point) {
      if (point.X > maxX) {
        maxX = point.X;
      }
      if (point.X < minX) {
        minX = point.X;
      }
      if (point.Y > maxY) {
        maxY = point.Y;
      }
      if (point.Y < minY) {
        minY = point.Y;
      }
    });
    socket.emit('spawn', {
      'name': lastResult.Name,
      'color': getNormalizedColor(),
      // size is calibrated to give a max 2 times usual height, square scaled
      'sizeX': (maxX - minX)/canvas.height * 2,
      'sizeY': (maxY - minY)/canvas.height * 2
    });
  }
  clear();
}


function onTouchStart(e) {
  e.preventDefault();
  var touch = e.touches[0];
  var mouseEvent = new MouseEvent("mousedown", {
    clientX: touch.clientX,
    clientY: touch.clientY
  });
  canvas.dispatchEvent(mouseEvent);
}

function onTouchMove(e) {
  e.preventDefault();
  var touch = e.touches[0];
  var mouseEvent = new MouseEvent("mousemove", {
    clientX: touch.clientX,
    clientY: touch.clientY
  });
  canvas.dispatchEvent(mouseEvent);
}

function onTouchEnd(e) {
  e.preventDefault();
  var mouseEvent = new MouseEvent("mouseup", {});
  canvas.dispatchEvent(mouseEvent);
}

function preventDocumentMobileScroll() {
  document.body.addEventListener("touchstart", function (e) {
    if (e.target == canvas) {
      e.preventDefault();
    }
  }, false);
  document.body.addEventListener("touchend", function (e) {
    if (e.target == canvas) {
      e.preventDefault();
    }
  }, false);
  document.body.addEventListener("touchmove", function (e) {
    if (e.target == canvas) {
      e.preventDefault();
    }
  }, false);
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

  canvas.addEventListener("touchstart", onTouchStart, false);
  canvas.addEventListener("touchmove", onTouchMove, false);
  canvas.addEventListener("touchend", onTouchEnd, false);

  preventDocumentMobileScroll();

  $('#clear').click(clear);
  $('#shape').click(spawnDrawnObject);
  $('#junk').click(spawnJunk);

  socket = io();
})();

});
