if (!isObject(TankMovementBehavior))
{
   %template = new BehaviorTemplate(TankMovementBehavior);

   %template.friendlyName = "Tank Movement";
   %template.behaviorType = "Movement";
   %template.description  = "Tank style movement behavior";

   %template.addBehaviorField(turnSpeed, "Speed when turning", float, 110);
   %template.addBehaviorField(turnSpeedMultiplier, "Speed multiplier for turning", float, 1.0);      
   
   %template.addBehaviorField(forwardSpeed, "Speed when moving forward", float, 13);
   %template.addBehaviorField(backwardSpeed, "Speed when moving backward", float, 11);
   %template.addBehaviorField(linearSpeedMultiplier, "Speed multiplier for moving backwards or forwards", float, 1.0);   
}

function TankMovementBehavior::onBehaviorAdd(%this)
{   
   %this.idleSound = alxPlay("MyModule:tankIdleSound");
}

function TankMovementBehavior::onBehaviorRemove(%this)
{
   %this.stopSounds();
}

function TankMovementBehavior::stopSounds(%this)
{
   alxStop(%this.movingSound);
   alxStop(%this.idleSound);
}

function TankMovementBehavior::turnLeft(%this)
{
   %this.owner.setAngularVelocity(%this.turnSpeed * %this.turnSpeedMultiplier);
   %this.turnSchedule = %this.schedule(50, turnLeft);
}

function TankMovementBehavior::turnRight(%this)
{
   %this.owner.setAngularVelocity(-%this.turnSpeed * %this.turnSpeedMultiplier);
   %this.turnSchedule = %this.schedule(50, turnRight);
}

function TankMovementBehavior::stopTurn(%this)
{
   cancel(%this.turnSchedule);
   %this.owner.setAngularVelocity(0);
}

function TankMovementBehavior::stopMovement(%this)
{
   cancel(%this.animateSchedule);
   cancel(%this.movementSchedule);
   
   %this.owner.setLinearVelocity(0, 0);
   
   %this.stopSounds();
   %this.idleSound = alxPlay("MyModule:tankIdleSound");
}

function TankMovementBehavior::updateMovement(%this, %speed)
{
   %this.owner.setLinearVelocityPolar(%this.owner.Angle - 180, %speed * %this.linearSpeedMultiplier);
   %this.movementSchedule = %this.schedule(50, updateMovement, %speed);
}

function TankMovementBehavior::moveForward(%this)
{
   %this.stopSounds();
   %this.movingSound = alxPlay("MyModule:tankMovingSound2");
   %this.animate(false);
   %this.updateMovement(%this.forwardSpeed);
}

function TankMovementBehavior::moveBackward(%this)
{
   %this.stopSounds();
   %this.movingSound = alxPlay("MyModule:tankMovingSound2");
   %this.animate(true);
   %this.updateMovement(-%this.backwardSpeed);
}

function TankMovementBehavior::animate(%this, %isBackwards)
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