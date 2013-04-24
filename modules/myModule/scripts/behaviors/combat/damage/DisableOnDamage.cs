if (!isObject(DisableOnDamageBehavior))
{
   %template = new BehaviorTemplate(DisableOnDamageBehavior);
   
   %template.friendlyName = "Takes Damage";
   %template.behaviorType = "Damage";
   %template.description  = "Set the object to take damage from DealsDamage objects that collide with it";
   %template.addBehaviorField(tintRedForDamage, "Tint the object red as it takes damage", bool, true);

   %template.addBehaviorField(damageThreshold, "Amount of damage which must be dealt within the damageThresholdTime to disable the owner", int, 30);
   %template.addBehaviorField(damageThresholdTime, "Time (in ms) within which damage can be dealt (for disabling purposes) before resetting", int, 1000);
   %template.addBehaviorField(disableTime, "Time (in ms) to disable the owner", int, 300);
   %template.addBehaviorField(disableMultiplier, "Multiplier to apply to owner's speed",float, 0.4);
}

function DisableOnDamageBehavior::onBehaviorAdd(%this)
{
   %this.resetDamageDealt();
}

function DisableOnDamageBehavior::onBehaviorRemove(%this)
{
}

function DisableOnDamageBehavior::resetDamageDealt(%this)
{
   %this.damageDealt = 0;
}

function DisableOnDamageBehavior::onDamage(%this, %damage)
{
   if(!%this.isDisabled)
   {
      if(%this.damageDealt == 0) {
         %this.resetDmgSchedule = %this.schedule(%this.damageThresholdTime, resetDamageDealt);    
      }
      
      %this.damageDealt += %damage;
      if(%this.damageDealt >= %this.damageThreshold)
      {
         cancel(%this.resetDmgSchedule);
         %this.resetDamageDealt();
         %this.onDisable();
         %this.enableSchedule = %this.schedule(%this.disableTime, onEnable);
      }
   }
}

function DisableOnDamageBehavior::onDisable(%this)
{
   %movement = %this.owner.getBehavior("TankMovementBehavior");
   if(!isObject(%movement))
      return;
      
   %this.isDisabled = true;
   %movement.turnSpeedMultiplier *= %this.disableMultiplier;
   %movement.linearSpeedMultiplier *= %this.disableMultiplier;
}

function DisableOnDamageBehavior::onEnable(%this)
{
   %movement = %this.owner.getBehavior("TankMovementBehavior");
   if(!isObject(%movement))
      return;
      
   %this.isDisabled = false;
   %movement.turnSpeedMultiplier /= %this.disableMultiplier;
   %movement.linearSpeedMultiplier /= %this.disableMultiplier;
}

