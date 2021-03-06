if (!isObject(SlowOnDamageBehavior))
{
   %template = new BehaviorTemplate(SlowOnDamageBehavior);
   
   %template.friendlyName = "Takes Damage";
   %template.behaviorType = "Damage";
   %template.description  = "Set the object to take damage from DealsDamage objects that collide with it";
   %template.addBehaviorField(tintRedForDamage, "Tint the object red as it takes damage", bool, true);

   %template.addBehaviorField(damageThreshold, "Amount of damage which must be dealt within the damageThresholdTime to disable the owner", int, 25);
   %template.addBehaviorField(damageThresholdTime, "Time (in ms) within which damage can be dealt (for disabling purposes) before resetting", int, 500);
   %template.addBehaviorField(disableTime, "Time (in ms) to disable the owner", int, 200);
   %template.addBehaviorField(disableMultiplier, "Multiplier to apply to owner's speed",float, 0.45); //Blows up with exactly 0
}

function SlowOnDamageBehavior::onBehaviorAdd(%this)
{
   if(%this.disableMultiplier == 0){
      %this.disableMultiplier = 0.01;   
   }
   %this.resetDamageDealt();
}

function SlowOnDamageBehavior::onBehaviorRemove(%this)
{
   alxStop(%this.stunSound);
}

function SlowOnDamageBehavior::resetDamageDealt(%this)
{
   %this.damageDealt = 0;
}

function SlowOnDamageBehavior::onDamage(%this, %damage)
{
   if(!%this.isSlowed)
   {
      if(%this.damageDealt == 0) {
         %this.resetDmgSchedule = %this.schedule(%this.damageThresholdTime, resetDamageDealt);    
      }
      
      %this.damageDealt += %damage;
      if(%this.damageDealt >= %this.damageThreshold)
      {
         cancel(%this.resetDmgSchedule);
         %this.resetDamageDealt();
         %this.onSlow();
         %this.enableSchedule = %this.schedule(%this.disableTime, onReset);
      }
   }
}

function SlowOnDamageBehavior::onSlow(%this)
{
   %movement = %this.owner.getBehavior("TankMovementBehavior");
   if(!isObject(%movement))
      return;
   
   %this.stunSound = alxPlay("MyModule:tankStunSound");
   %this.isSlowed = true;
   %movement.turnSpeedMultiplier *= %this.disableMultiplier;
   %movement.linearSpeedMultiplier *= %this.disableMultiplier;
}

function SlowOnDamageBehavior::onReset(%this)
{
   %movement = %this.owner.getBehavior("TankMovementBehavior");
   if(!isObject(%movement))
      return;
   
   alxStop(%this.stunSound);
   %this.isSlowed = false;
   %movement.turnSpeedMultiplier /= %this.disableMultiplier;
   %movement.linearSpeedMultiplier /= %this.disableMultiplier;
}

