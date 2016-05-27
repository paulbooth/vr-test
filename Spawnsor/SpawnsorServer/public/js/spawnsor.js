
$(function onload() {
var RECOGNIZE_THRESHOLD = 0.25;

var canvas, c;
var isDown, points, strokeId, recognizer;


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

  var clr = "rgb(" + rand(0,200) + "," + rand(0,200) + "," + rand(0,200) + ")";
  c.strokeStyle = clr;
  c.fillStyle = clr;
  c.fillRect(point.X - 4, point.Y - 4, 9, 9);
}

function onMouseMove(e) {
  if (isDown) {
    var point = getPoint(e, strokeId);
    points.push(point);
    drawConnectedPoint(points.length - 2, points.length - 1);
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
    var result = recognizer.Recognize(points);
    console.log(result);
    console.log(getPointsSaveString());
    if (result.Score > RECOGNIZE_THRESHOLD) {
      $('#shape').text(result.Name + ": (" + result.Score.toFixed(3) + ")");
    }
  }
}

function clear() {
  $('#shape').text('');
  points.length = 0;
  strokeId = 0;
  c.clearRect(0,0,canvas.width,canvas.height);
}

(function init() {
  canvas = document.getElementById('canvas');
  canvas.width = window.innerWidth;
  canvas.height = window.innerHeight;
  c = canvas.getContext('2d');
  isDown = false;
  points = new Array();
  strokeId = 0;
  recognizer = new PDollarRecognizer();

  $(canvas).mousedown(onMouseDown);
  $(canvas).mousemove(onMouseMove);
  $(canvas).mouseup(onMouseUp);

  $('#clear').click(clear);
})();

});
