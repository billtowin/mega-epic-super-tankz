if (!isObject(TakesDamageBehavior))
{
   %template = new BehaviorTemplate(TakesDamageBehavior);
   
   %template.friendlyName = "Takes Damage";
   %template.behaviorType = "Damage";
   %template.description  = "Set the object to take damage from DealsDamage objects that collide with it";
   %template.addBehaviorField(tintRedForDamage, "Tint the object red as it takes damage", bool, true);

   %template.addBehaviorField(health, "The amount of health the object has", int, 120);
   %template.addBehaviorField(maxHealth, "The maximum amount of health the object has", int, 120);
   
   %template.addBehaviorField(disableOnDamage, "Disable when damaged?", bool, true);
   %template.addBehaviorField(disableTime, "Time to be disabled after being hit (in ms)", int, 250);
}

function TakesDamageBehavior::onBehaviorAdd(%this)
{
   %this.startHealth = %this.health;
}

function TakesDamageBehavior::onBehaviorRemove(%this)
{
}

function TakesDamageBehavior::takeDamage(%this, %amount)
{
   if(%this.disableOnDamage && %amount > 0 )
   {
      %this.owner.onDisable();
      %this.owner.reEnableSchedule = %this.owner.schedule(%this.disableTime, onEnable);
   }
   
   %newHealth = %this.health - %amount;
   
   if(%newHealth >= %this.maxHealth){
      %newHealth = %this.maxHealth;   
   }
   %this.health = %newHealth;
   
   if (%this.health <= 0)
   {
      %this.owner.onDeath();
      return;
   }
   
   if(%this.tintRedForDamage)
   {
      %tint = %this.health / %this.startHealth;
      %this.owner.setBlendColor(1, %tint, %tint, 1);
   }
}
