if (!isObject(SpeedBoostBehavior))
{
   %template = new BehaviorTemplate(SpeedBoostBehavior);

   %template.friendlyName = "Tank Speed Boost";
   %template.behaviorType = "Combat";
   %template.description  = "Tank Speed Boost";

   %template.addBehaviorField(duration, "Duration of Powerup/Ability (negative number for infinite) (in ms)", int, 15000);
   %template.addBehaviorField(turnSpeedMultiplier, "Turning Speed Multiplier", float, 1.3);
   %template.addBehaviorField(linearSpeedMultiplier, "Linear Speed Multiplier", float, 1.4);
}

function SpeedBoostBehavior::onBehaviorAdd(%this)
{
   %movement = %this.owner.getBehavior("TankMovementBehavior");
   if (!isObject(%movement))
      return;
      
   %movement.turnSpeedMultiplier *= %this.turnSpeedMultiplier;
   %movement.linearSpeedMultiplier *= %this.linearSpeedMultiplier;
}

function SpeedBoostBehavior::onBehaviorRemove(%this)
{
   %movement = %this.owner.getBehavior("TankMovementBehavior");
   if (!isObject(%movement))
      return;
      
   %movement.turnSpeedMultiplier /= %this.turnSpeedMultiplier;
   %movement.linearSpeedMultiplier /= %this.linearSpeedMultiplier;
}