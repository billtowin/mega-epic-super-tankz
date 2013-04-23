if (!isObject(DealsDamageBehavior))
{
   %template = new BehaviorTemplate(DealsDamageBehavior);
   
   %template.friendlyName = "Deals Damage";
   %template.behaviorType = "Damage";
   %template.description  = "Set the object to deal damage to TakesDamage objects it collides with";

   %template.addBehaviorField(strength, "The amount of damage the object deals", int, 25);
   %template.addBehaviorField(destroyOnHit, "Destroy the object when it collides", bool, true);
}

function DealsDamageBehavior::onBehaviorAdd(%this)
{
   %this.owner.setCollisionCallback(true);
}

function DealsDamageBehavior::dealDamage(%this, %amount, %victim)
{
   %takesDamage = %victim.getBehavior("TakesDamageBehavior");
   if (!isObject(%takesDamage))
      return;
   
   %takesDamage.takeDamage(%amount);
}

function DealsDamageBehavior::onCollision(%this, %dstObj, %srcRef, %dstRef, %time, %normal, %contactCount, %contacts)
{
   %this.dealDamage(%this.strength, %dstObj);
   
   if (%this.destroyOnHit && %dstObj $= "Tank")
   {
      %this.owner.destroy();
   }
}
