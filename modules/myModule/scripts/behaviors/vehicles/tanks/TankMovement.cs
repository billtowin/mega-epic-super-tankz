if (!isObject(TankMovementBehavior))
{
   %template = new BehaviorTemplate(TankMovementBehavior);

   %template.friendlyName = "Tank Movement";
   %template.behaviorType = "Movement";
   %template.description  = "Tank style movement behavior";

   %template.addBehaviorField(turnSpeed, "Speed when turning", float, 115);
   %template.addBehaviorField(turnSpeedMultiplier, "Speed multiplier for turning", float, 1.0);      
   
   %template.addBehaviorField(forwardSpeed, "Speed when moving forward", float, 14);
   %template.addBehaviorField(backwardSpeed, "Speed when moving backward", float, 12);
   %template.addBehaviorField(linearSpeedMultiplier, "Speed multiplier for moving backwards or forwards", float, 1.0);
   
   //Damping for making the tank feel heavy when not moving but agile when moving
   %template.addBehaviorField(linearDampingLow, "Low setting for linear damping", float, 0.0);
   %template.addBehaviorField(linearDampingHigh, "High setting for linear damping", float, 30.0);
   %template.addBehaviorField(angularDampingLow, "Low setting for angular damping", float, 0.0);
   %template.addBehaviorField(angularDampingHigh, "High setting for angular damping", float, 30.0);
}

function TankMovementBehavior::onBehaviorAdd(%this)
{   
   %this.idleSound = alxPlay("MyModule:tankIdleSound");
   %this.owner.LinearDamping = %this.linearDampingHigh;
   %this.owner.AngularDamping = %this.angularDampingHigh;
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
   %this.owner.AngularDamping = %this.angularDampingLow;
   %this.owner.setAngularVelocity(%this.turnSpeed * %this.turnSpeedMultiplier);
   %this.turnSchedule = %this.schedule(50, turnLeft);
}

function TankMovementBehavior::turnRight(%this)
{
   %this.owner.AngularDamping = %this.angularDampingLow;
   %this.owner.setAngularVelocity(-%this.turnSpeed * %this.turnSpeedMultiplier);
   %this.turnSchedule = %this.schedule(50, turnRight);
}

function TankMovementBehavior::stopTurn(%this)
{
   %this.owner.AngularDamping = %this.angularDampingHigh;
   cancel(%this.turnSchedule);
   %this.owner.setAngularVelocity(0);
}

function TankMovementBehavior::stopMovement(%this)
{
   %this.owner.LinearDamping = %this.linearDampingHigh;
   cancel(%this.animateSchedule);
   cancel(%this.movementSchedule);
   
   %this.owner.setLinearVelocity(0, 0);
   
   %this.stopSounds();
   %this.idleSound = alxPlay("MyModule:tankIdleSound");
}

function TankMovementBehavior::updateMovement(%this, %speed)
{
   %this.owner.LinearDamping = %this.linearDampingLow;
   %this.owner.setLinearVelocityPolar(%this.owner.Angle - 180, %speed * %this.linearSpeedMultiplier);
   %this.movementSchedule = %this.schedule(50, updateMovement, %speed);
}

function TankMovementBehavior::moveForward(%this)
{
   %this.stopSounds();
   %this.movingSound = alxPlay("MyModule:tankMovingSound");
   %this.animate(false);
   %this.updateMovement(%this.forwardSpeed);
}

function TankMovementBehavior::moveBackward(%this)
{
   %this.stopSounds();
   %this.movingSound = alxPlay("MyModule:tankMovingSound");
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