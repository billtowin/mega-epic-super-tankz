if (!isObject(MineShotBehavior))
{
   %template = new BehaviorTemplate(MineShotBehavior);

   %template.friendlyName = "Tank Mine Shot";
   %template.behaviorType = "Combat";
   %template.description  = "Tank Mine laying ability";

   %template.addBehaviorField(powerKey, "Key to bind to Shoot", keybind, "keyboard j");
   %template.addBehaviorField(duration, "Duration of Powerup/Ability (negative number for infinite) (in ms)", int, 15000);
   %template.addBehaviorField(damage, "Damage per mine", int, 60);

   %template.addBehaviorField(speed, "Mine Speed", float, 3);
   
   %template.addBehaviorField(reloadTime, "Reload time (in ms)", int, 1500);
   %template.addBehaviorField(ammoCount, "Amount of Ammo", int, 5);
   %template.addBehaviorField(mineLifespan, "LifeSpan of Mine (in ms)", int, 20000);
}

function MineShotBehavior::onBehaviorAdd(%this)
{
   if (!isObject(GlobalActionMap))
      return;
   GlobalActionMap.bindObj(getWord(%this.powerKey, 0), getWord(%this.powerKey, 1), "mineShot", %this);
   
   %this.ammoSpent = 0;
   %this.isLoaded = true;
}

function MineShotBehavior::onBehaviorRemove(%this)
{
   if (!isObject(GlobalActionMap))
      return;
   
   %this.stopSounds();
   GlobalActionMap.unbind(getWord(%this.powerKey, 0), getWord(%this.powerKey, 1));
}

function MineShotBehavior::loadMine(%this)
{
   %this.reloadSound = alxPlay("MyModule:tankSpreadShotReloadSound");
   %this.isLoaded = true;
}
function MineShotBehavior::mineShot(%this)
{
   if(%this.ammoSpent < %this.ammoCount && %this.isLoaded)
   {
      %this.createMineShot();
      %this.shotSound = alxPlay("MyModule:tankSpreadShotSound");
      %this.ammoSpent += 1;
      %this.isLoaded = false;
      if(%this.ammoSpent < %this.ammoCount) {
         %this.reloadSchedule = %this.schedule(%this.reloadTime, loadMine);
      } else {
         //Out of Ammo
         %this.outOfAmmoSound = alxPlay("MyModule:tankMineShotOutOfAmmoSound");
      }
   }
}

function MineShotBehavior::stopSounds(%this)
{
   alxStop(%this.shotSound);
   alxStop(%this.outOfAmmoSound);
   alxStop(%this.reloadSound);
}

function MineShotBehavior::createMineShot(%this)
{
   %adjustedAngle = getPositiveAngle(%this.owner);
   
   //Calculate a direction from an Angle and Magnitude
   %mineOffset= Vector2Direction(%adjustedAngle-180,%this.owner.Size.height * 0.75);
   
   // Create the sprite.
   %mine = new Sprite()
   {
      class = MineShot;
      Image = "MyModule:woodenBarrelImage";
      BlendColor = "Black";
      BodyType = dynamic;
      Size = 3;
      Position = (%this.owner.Position.x + %mineOffset.x) SPC (%this.owner.Position.y + %mineOffset.y);
      LinearDamping = 2.0;
      AngularDamping = 1.0;
      SceneLayer = 1;
      SceneGroup = 5;
      CollisionCallback = true;
   };
   %mine.setLinearVelocityPolar(%this.owner.Angle, %this.speed);
   //Sets the collision shape to a circle
   %mine.createCircleCollisionShape(%mine.Size.x / 2);
   %mine.setCollisionGroups("0 1 4 5");
   
   %dealDmgBehavior = DealsDamageBehavior.createInstance();
   %dealDmgBehavior.strength = %this.damage;
   %mine.addBehavior(%dealDmgBehavior);
   
   %mine.dieSchedule = %mine.schedule(%this.mineLifespan,onDeath);
   
   // Add the sprite to the scene.
   %ownerScene = %this.owner.getScene();   
   %ownerScene.add( %mine );
}

function MineShot::onCollision(%this, %object, %details)
{
   if(%object.class $= "Tank"){
      %this.onDeath();
   }
}

function MineShot::onDeath(%this)
{
   %currentScene = %this.getScene();
   addExplosion(%currentScene, %this.Position.x SPC (%this.Position.y + 2), 3);
   alxPlay("MyModule:tankMineShotExplosionSound");
   %this.safeDelete();
}