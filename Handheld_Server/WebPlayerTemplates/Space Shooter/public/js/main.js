/* global Connection */
/// <reference path="../../typings/jquery/jquery.d.ts"/>
/* global nipplejs */
/*
 Copyright (C) 2015 Electronic Arts Inc.  All rights reserved.

 This software is solely licensed pursuant to the Hackathon License Agreement,
 Available at:  [URL to Hackathon License Agreement].
 All other use is strictly prohibited.
 */
// Utils
var u = {};
u.distance = function (p1, p2) {
    var dx = p2.x - p1.x;
    var dy = p2.y - p1.y;

    return Math.sqrt((dx * dx) + (dy * dy));
};

u.angle = function(p1, p2) {
    var dx = p2.x - p1.x;
    var dy = p2.y - p1.y;

    return u.degrees(Math.atan2(dy, dx));
};

u.findCoord = function(p, d, a) {
    var b = {x: 0, y: 0};
    a = u.radians(a);
    b.x = p.x - d * Math.cos(a);
    b.y = p.y - d * Math.sin(a);
    return b;
};

u.radians = function(a) {
    return a * (Math.PI / 180);
};

u.degrees = function(a) {
    return a * (180 / Math.PI);
};

var playersId;
var messageTemplate;


$(document).ready(function () {


	var conn = new Connection();

	$("#sign-in").on("click", function() {
		console.log("signing in and connecting");

		$("#name-input").hide();
		$("#sign-in").hide();

		var playersName = document.getElementById("name-input").value;
		console.log("got value: ", playersName);

		playersId = makeid();
		messageTemplate = {"type": null, "playerId": playersId, "playerName":playersName};

		messageTemplate["type"] = "connect";

		conn.sendMessage(messageTemplate);
	});		
	
	

	



	var health = 100;

	var myJoystick = document.getElementById("joystick-knob");

	//var hammertime = new Hammer(myJoystick);
	var manager = new Hammer.Manager(myJoystick);
	// hammertime.on("pan", function(ev) {
	// 	console.log("panned");
	// });

	var defaultPosition = 55;
	var maxDelta = 80;

	var pan1 = new Hammer.Pan({
		event: "pan1",
		pointers: 1,
		direction: Hammer.DIRECTION_ALL
	});

	var rotateA = new Hammer.Rotate({
		event:'rotate',
		pointers: 2
	});

	rotateA.recognizeWith(pan1);
	pan1.requireFailure(rotateA);

	manager.add([rotateA, pan1]);

	manager.currentAngle = 0;

	manager.on("pan1start pan2start", function(ev) {
		moveStart(ev);
	});

	manager.on("pan1move pan2move", function(ev) {
		moveUpdate(ev);

	    messageTemplate["type"] = "movement";

	    var newX = parseInt(ev.target.offsetLeft)- 50;
	    messageTemplate.direction = newX;


	    messageTemplate.force = "millIOns";
	    conn.sendMessage(messageTemplate);

	});

	manager.on("pan1end pan2end", function(ev) {
		manager.rotationLast = ev.rotation;
		moveStart(ev);
		console.log("pan end");
		resetJoystick(ev);

	});

	manager.on("rotatemove",function(ev){
		console.log("rotatemove-ing");

	    var isCW = ev.rotation > manager.rotationLast;

	    var delta = Math.abs(ev.rotation-manager.rotationLast);

	    // depending on the order of touches
	    // ev.rotation jumps from ~-50 to ~300
	    if (delta>100) delta=5; // handle ev.rotation jump

	    if (!isCW) {
	      manager.currentAngle-=delta;
	    } else {
	      manager.currentAngle+=delta;
	    }

	    performRotation(ev.target,manager.currentAngle,ev);

	    manager.rotationLast = ev.rotation;
	    messageTemplate["type"] = "movement";
	    messageTemplate.angle = ev.rotation;
	    messageTemplate.force = "millIOns";
	    conn.sendMessage(messageTemplate);	
	});

	  manager.on("rotateend",function(ev){
	  	console.log("rotateend-ing");
	  });







	console.log("Document Loaded");

	// INIT..


	// Movement Joystick
	
	var movement	= new VirtualJoystick({
		container	: document.body,
		strokeStyle	: 'cyan',
		limitStickTravel: true,
		stickRadius	: 50	
	});
	movement.addEventListener('touchStartValidation', function(event){
		var touch	= event.changedTouches[0];
		if( touch.pageX >= window.innerWidth/2 )	return false;
		return true
	});
	
	var prevAngle;
	var prevDistance;
	setInterval(function() {
		var angle = u.angle({x:0,y:0}, {x:movement.deltaX(), y:movement.deltaY()});
		var distance = u.distance({x:0,y:0}, {x:movement.deltaX(), y:movement.deltaY()});
		
		if (prevAngle !== angle && prevDistance != distance) {
			prevAngle = angle !== 0 ? angle : prevAngle;
			prevDistance = distance;

			//messageTemplate["type"] = "movement";
			//messageTemplate.angle = -prevAngle;
			//messageTemplate.force = distance / 50;
			//conn.sendMessage(messageTemplate);			
		}

	}, 1000/20);

	// Shooting Joystick

	var shooting	= new VirtualJoystick({
		container	: document.body,
		strokeStyle	: 'orange',
		limitStickTravel: true,
		stickRadius	: 0		
	});
	shooting.addEventListener('touchStartValidation', function(event){
		var touch	= event.changedTouches[0];
		if( touch.pageX < window.innerWidth/2 )	return false;
		return true
	});
	shooting.addEventListener('touchStart', function(){
		messageTemplate["type"] = "fire";
		conn.sendMessage(messageTemplate);
	})
	
	//jump button 

	$("#jump-button").on("click", function() {
		console.log("jumping");

		messageTemplate["type"] = "jump";
		messageTemplate.force = "trillions";
		messageTemplate.jump = "yes";

		conn.sendMessage(messageTemplate);	
	});		

	$("#attack-button").on("click", function() {
		console.log("attacking");
		messageTemplate["type"] = "attack";
		messageTemplate.force = "a billion";
		conn.sendMessage(messageTemplate);	
	});		



	//i dont think this is ever called anymore
	$("#spawn").on('touchup', function() {
		$("#spawn").hide();
		$("#health").show();		
		conn.sendMessage({"type": "spawn"});
	});


	// Process incoming game messages
	$(document).on("game_message", function (e, message) {
		console.log("Received Message: " + JSON.stringify(message));
		var payload = message.payload;
		switch (payload.type) {
			case "health":
				var vibrateTime = (health - payload.health) * 25;
				navigator.vibrate(100);
				health = payload.health;
				if (health <= 0) {
					$("#spawn").show();
					$("#health").hide();
				}
				$('#health').text(Math.round(health));
			case "disabled":
				if (payload.playerId != playersId) {
					console.log("this player is disabled");
					document.body.style.background = "black";
				}
				break;
			case "enabled":
					console.log("this player is re-enabled");
					document.body.style.background = "white";
			
				break;
			case "died":
				if (payload.playerId == playersId) {
					console.log("got that player died");
					$('#health').text("YOU DIED");
					document.body.style.background = "red";
				}
				break;

			case "score":
				if (payload.playerId == playersId) {
					$('#score').text("My Score: " + payload.points);
				}
				break;

			default:
				break;
		}
	});

	 function moveStart(ev) {
	 	console.log("move start");
    manager.lastX = ev.center.x;
    manager.lastY = ev.center.y;
    if (manager.currentTween) manager.currentTween.kill();
  }

  function moveUpdate(ev) {
  	console.log("move updating");
    var deltaX = manager.lastX-ev.center.x;
    var deltaY = manager.lastY-ev.center.y;

   var newX = parseInt(ev.target.offsetLeft)-deltaX;
   var newY = parseInt(ev.target.offsetTop)-deltaY;

      $(ev.target).css({
        "left":newX,
        "top":newY
      })

    manager.lastX = ev.center.x;
    manager.lastY = ev.center.y;
    console.log("new x: ", newX, "newY: ", newY);
  }

  function resetJoystick(ev) {
  	console.log("resettting joystick");


      $(ev.target).css({
        "left":50,
        "top":50
      })
  }


  function performRotation(target,angle,ev) {

    this.options = {

      rotationCenterX: 50,
      rotationCenterY: 50
    };

    this.element = $(target);

    moveUpdate(ev);

    this.element.css('transform-origin', this.options.rotationCenterX + '% ' + this.options.rotationCenterY + '%');
    this.element.css('-ms-transform-origin', this.options.rotationCenterX + '% ' + this.options.rotationCenterY + '%'); /* IE 9 */
    this.element.css(
      '-webkit-transform-origin',
      this.options.rotationCenterX + '% ' + this.options.rotationCenterY + '%'); /* Chrome, Safari, Opera */

    this.element.css('transform','rotate(' + angle + 'deg)');
    this.element.css('-moz-transform','rotate(' + angle + 'deg)');
    this.element.css('-webkit-transform','rotate(' + angle + 'deg)');
    this.element.css('-o-transform','rotate(' + angle + 'deg)');
  }


  function impartMomentum(ev) {
    var v = ev.velocity;
    var vX = Math.abs(ev.velocityX)<Math.abs(ev.overallVelocityX) ? ev.overallVelocityX : ev.velocityX;
    var vY = Math.abs(ev.velocityY)<Math.abs(ev.overallVelocityY) ? ev.overallVelocityY : ev.velocityY;
    var decel = 1; // distance per second squared
    var time = Math.abs(v) / decel;

    var coefficient = 300;
    // d = v^2 / 2a

    // get time travelled
    // get destination x
    var dX = Math.pow(vX,2) / (2*decel) * coefficient;
    dX = (ev.velocityX>0) ? dX : -dX;
    // get destination y
    var dY = Math.pow(vY,2) / (2*decel) * coefficient;
    dY = (ev.velocityY>0) ? dY : -dY;

    var finalX = ev.target.offsetLeft+dX, finalY = ev.target.offsetTop+dY;

    // get animation handler to cancel on new event start
    manager.currentTween = TweenLite.to(ev.target, time, {left:finalX+"px",top:finalY+"px",
      onUpdate: function(){
        // prevent object from leaving bounds
        if ((ev.target.offsetLeft<=0) || (ev.target.offsetTop<=0)) {
           manager.currentTween.kill();
        }
      }
    });
  }


  function findPos(obj) {
    var curleft = 0, curtop = 0;
    if (obj.offsetParent) {
        do {
            curleft += obj.offsetLeft;
            curtop += obj.offsetTop;
        } while (obj = obj.offsetParent);
        return { x: curleft, y: curtop };
    }
    return undefined;
}

function rgbToHex(r, g, b) {
    if (r > 255 || g > 255 || b > 255)
        throw "Invalid color component";
    return ((r << 16) | (g << 8) | b).toString(16);
}

function makeid()
{
    var text = "";
    var possible = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    for( var i=0; i < 20; i++ )
        text += possible.charAt(Math.floor(Math.random() * possible.length));

    return text;
}
	
});

