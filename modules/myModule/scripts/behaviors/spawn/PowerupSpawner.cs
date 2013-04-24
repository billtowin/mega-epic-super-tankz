if (!isObject(PowerupSpawnerBehavior))
{
   %template = new BehaviorTemplate(PowerupSpawnerBehavior);

   %template.friendlyName = "Powerup Spawn";
   %template.behaviorType = "Spawn";
   %template.description  = "Powerup spawns if ";

   %template.addBehaviorField(spawnRate, "Spawn Rate", int, 20000);
   %template.addBehaviorField(spawnAtStart, "Spawn at start?", bool, true);
   %template.addBehaviorField(choices, "Indices of possible powerups", string, "0 1 2 3 4");
}

function PowerupSpawnerBehavior::onBehaviorAdd(%this)
{
}

function PowerupSpawnerBehavior::onBehaviorRemove(%this)
{
}


function PowerupSpawnerBehavior::onAddToScene(%this)
{
   if(%this.spawnAtStart)
   {
      %this.spawn(false);
   }
}

function PowerupSpawnerBehavior::onPickup(%this)
{
   %this.spawnSchedule = %this.schedule(%this.spawnRate, spawn, true);
}

function PowerupSpawnerBehavior::spawn(%this, %shouldPlaySound)
{
   if(%shouldPlaySound){
      alxPlay("MyModule:powerupSpawnSound");
   }
   %powerup = createRandomPowerup(%this.owner.Position.x, %this.owner.Position.y, %this.choices);
   %powerup.powerupSpawner = %this;
   %this.owner.getScene().add(%powerup);
}