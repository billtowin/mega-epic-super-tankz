if (!isObject(RegenerationBehavior))
{
   %template = new BehaviorTemplate(RegenerationBehavior);

   %template.friendlyName = "Tank Life Regeneration";
   %template.behaviorType = "Combat";
   %template.description  = "Tank Regeneration Ability";

   %template.addBehaviorField(duration, "Duration of Powerup/Ability (negative number for infinite) (in ms)", int, 15000);
   %template.addBehaviorField(maxNumberOfTicks, "Number of Ticks", int, 5);
   %template.addBehaviorField(timeBetweenTicks, "Time between ticks", int, 1000);
   %template.addBehaviorField(lifeGainedPerTick, "Life Gained per tick", int, 20);
   
   %template.addBehaviorField(ticksOnAdd, "Does the owner gain life immediately?", bool, true);
}

function RegenerationBehavior::onBehaviorAdd(%this)
{
   alxPlay("MyModule:powerupSound");
   
   %this.currentTicks = 0;
   %takesDamage = %this.owner.getBehavior("TakesDamageBehavior");
   if (!isObject(%takesDamage))
      return;
   if(%this.ticksOnAdd){
      %this.gainLife();
   } else {
      %this.gainLifeSchedule = %this.schedule(%this.timeBetweenTicks, gainLife);
   }
}

function RegenerationBehavior::onBehaviorRemove(%this)
{
   %this.stopSounds();
}

function RegenerationBehavior::stopSounds(%this)
{
   alxStop(%this.regenSound);
}

function RegenerationBehavior::gainLife(%this)
{
   if(%this.currentTicks <= %this.maxNumberOfTicks)
   {
      %this.stopSounds();
      %this.regenSound = alxPlay("MyModule:regenSound");
      %takesDamage = %this.owner.getBehavior("TakesDamageBehavior");
      if (!isObject(%takesDamage))
         return;
      %takesDamage.takeDamage(-%this.lifeGainedPerTick);
      %this.currentTicks += 1;
      %this.gainLifeSchedule = %this.schedule(%this.timeBetweenTicks, gainLife);
   }
}