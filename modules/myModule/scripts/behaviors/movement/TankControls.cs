if (!isObject(TankControlsBehavior))
{
   %template = new BehaviorTemplate(TankControlsBehavior);

   %template.friendlyName = "Tank Controls";
   %template.behaviorType = "Input";
   %template.description  = "Tank style movement control";

   %template.addBehaviorField(forwardKey, "Key to bind to forward movement", keybind, "keyboard up");
   %template.addBehaviorField(backwardKey, "Key to bind to backward movement", keybind, "keyboard down");
   %template.addBehaviorField(leftKey, "Key to bind to left movement", keybind, "keyboard left");
   %template.addBehaviorField(rightKey, "Key to bind to right movement", keybind, "keyboard right");

   %template.addBehaviorField(turnVelocity, "Speed when moving turning", float, 100);
   %template.addBehaviorField(forwardSpeed, "Speed when moving forward", float, 12);
   %template.addBehaviorField(backwardSpeed, "Speed when moving backward", float, 8);
}

function TankControlsBehavior::onBehaviorAdd(%this)
{
   if (!isObject(GlobalActionMap))
      return;

   %id = %this.owner.getId();
   GlobalActionMap.bindCmd(getWord(%this.forwardKey, 0), getWord(%this.forwardKey, 1), %id @ ".moveForward();", %id @ ".stopMovement();");
   GlobalActionMap.bindCmd(getWord(%this.backwardKey, 0), getWord(%this.backwardKey, 1), %id @ ".moveBackward();", %id @ ".stopMovement();");
   GlobalActionMap.bindCmd(getWord(%this.leftKey, 0), getWord(%this.leftKey, 1), %id @ ".turnLeft();", %id @ ".stopTurn();");
   GlobalActionMap.bindCmd(getWord(%this.rightKey, 0), getWord(%this.rightKey, 1), %id @ ".turnRight();", %id @ ".stopTurn();");
   
   %this.idleSound = alxPlay("MyModule:tankIdleSound");
}

function TankControlsBehavior::onBehaviorRemove(%this)
{
    if (!isObject(GlobalActionMap))
       return;

    GlobalActionMap.unbind(getWord(%this.forwardKey, 0), getWord(%this.forwardKey, 1));
    GlobalActionMap.unbind(getWord(%this.backwardKey, 0), getWord(%this.backwardKey, 1));
    GlobalActionMap.unbind(getWord(%this.leftKey, 0), getWord(%this.leftKey, 1));
    GlobalActionMap.unbind(getWord(%this.rightKey, 0), getWord(%this.rightKey, 1));
}

function TankControlsBehavior::stopSounds(%this)
{
   alxStop(%this.movingSound);
   alxStop(%this.idleSound);
}

function TankControlsBehavior::turnLeft(%this)
{
   %this.owner.setAngularVelocity(%this.turnVelocity);
   %this.turnSchedule = %this.schedule(50, turnLeft);
}

function TankControlsBehavior::turnRight(%this)
{
   %this.owner.setAngularVelocity(-%this.turnVelocity);
   %this.turnSchedule = %this.schedule(50, turnRight);
}

function TankControlsBehavior::stopTurn(%this)
{
   cancel(%this.turnSchedule);
   %this.owner.setAngularVelocity(0);
}

function TankControlsBehavior::stopMovement(%this)
{
   cancel(%this.animateSchedule);
   cancel(%this.movementSchedule);
   
   %this.owner.setLinearVelocity(0, 0);
   
   %this.stopSounds();
   %this.idleSound = alxPlay("MyModule:tankIdleSound");
}

function TankControlsBehavior::updateMovement(%this, %speed)
{
   %adjustedAngle = getPositiveAngle(%this.owner);
   
   //Calculate a direction from an Angle and Magnitude
   %movementVector= Vector2Direction(%adjustedAngle,%speed);
   
   %this.owner.setLinearVelocity(%movementVector);
   %this.movementSchedule = %this.schedule(50, updateMovement, %speed);
}

function TankControlsBehavior::moveForward(%this)
{
   %this.stopSounds();
   %this.movingSound = alxPlay("MyModule:tankMovingSound2");
   %this.animate(false);
   %this.updateMovement(%this.forwardSpeed);
}

function TankControlsBehavior::moveBackward(%this)
{
   %this.stopSounds();
   %this.movingSound = alxPlay("MyModule:tankMovingSound2");
   %this.animate(true);
   %this.updateMovement(-%this.backwardSpeed);
}

function TankControlsBehavior::animate(%this, %isBackwards)
{
   if(!%isBackwards) {
      if(%this.owner.Frame == %this.owner.initialFrame + 7) {
         %this.owner.Frame = %this.owner.initialFrame;
      } else {
         %this.owner.Frame += 1;
      }
   } else {
      if(%this.owner.Frame == %this.owner.initialFrame) {
         %this.owner.Frame = %this.owner.initialFrame + 7;      
      } else {
         %this.owner.Frame -= 1;
      }
   }
   
   %this.animateSchedule = %this.schedule(100, animate, %isBackwards);
}