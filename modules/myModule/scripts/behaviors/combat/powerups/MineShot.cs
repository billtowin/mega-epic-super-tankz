if (!isObject(MineShotBehavior))
{
   %template = new BehaviorTemplate(MineShotBehavior);

   %template.friendlyName = "Tank Mine Shot";
   %template.behaviorType = "Combat";
   %template.description  = "Tank Mine laying ability";

   %template.addBehaviorField(duration, "Duration of Powerup/Ability (negative number for infinite) (in ms)", int, 15000);
   %template.addBehaviorField(damage, "Damage per mine", int, 60);

   %template.addBehaviorField(speed, "Mine Speed", float, 3);
   
   %template.addBehaviorField(reloadTime, "Reload time (in ms)", int, 1500);
   %template.addBehaviorField(ammoCount, "Amount of Ammo", int, 5);
   %template.addBehaviorField(mineLifespan, "LifeSpan of Mine (in ms)", int, 20000);
}

function MineShotBehavior::onBehaviorAdd(%this)
{
   %this.ammoSpent = 0;
   %this.isLoaded = true;
}

function MineShotBehavior::onBehaviorRemove(%this)
{
   %this.stopSounds();
}

function MineShotBehavior::loadMine(%this)
{
   %this.reloadSound = alxPlay("MyModule:spreadShotReloadSound");
   %this.isLoaded = true;
}
function MineShotBehavior::mineShot(%this)
{
   if(%this.ammoSpent < %this.ammoCount && %this.isLoaded)
   {
      %this.createMineShot();
      %this.shotSound = alxPlay("MyModule:spreadShotSound");
      %this.ammoSpent += 1;
      %this.isLoaded = false;
      if(%this.ammoSpent < %this.ammoCount) {
         %this.reloadSchedule = %this.schedule(%this.reloadTime, loadMine);
      } else {
         //Out of Ammo
         %this.outOfAmmoSound = alxPlay("MyModule:mineShotOutOfAmmoSound");
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
   %mine = new Sprite()
   {
      class = MineShot;
      Image = "MyModule:landmineImage";
      Frame = 0;
      BodyType = dynamic;
      Size = 3;
      Position = %this.owner.getWorldPoint(0 SPC (-%this.owner.Size.height * 0.75) );
      LinearDamping = 2.0;
      AngularDamping = 1.0;
      SceneLayer = 1;
      SceneGroup = 5;
      CollisionCallback = true;
      
      isHidden = false;
      isAnimating = false;
   };
   %mine.setLinearVelocityPolar(%this.owner.Angle, %this.speed);
   //Sets the collision shape to a circle
   %mine.createCircleCollisionShape(%mine.Size.x / 2);
   %mine.setCollisionGroups("0 1 4 5");
   
   %proximitySensor = ProximitySensorBehavior.createInstance();
   %mine.addBehavior(%proximitySensor);
   
   %dealDmgBehavior = DealsDamageBehavior.createInstance();
   %dealDmgBehavior.strength = %this.damage;
   %mine.addBehavior(%dealDmgBehavior);
   
   %mine.dieSchedule = %mine.schedule(%this.mineLifespan,onDeath);
   
   // Add the sprite to the scene.
   %ownerScene = %this.owner.getScene();   
   %ownerScene.add( %mine );
}

function MineShot::setIsNotAnimating(%this)
{
   %this.isAnimating = false;
}
function MineShot::onProximitySensorOn(%this)
{
   if(%this.isHidden && !%this.isAnimating) {
      %this.isAnimating = true;
      %this.isHidden = false;
      %this.Animation = "MyModule:landmineEmergeAnim";
      %this.animatingSchedule = %this.schedule(1500, setIsNotAnimating);
   }
}

function MineShot::onProximitySensorOff(%this)
{
   if(!%this.isHidden && !%this.isAnimating) {
      %this.isAnimating = true;
      %this.isHidden = true;
      %this.Animation = "MyModule:landmineSubmergeAnim";
      %this.animatingSchedule = %this.schedule(1500, setIsNotAnimating);
   }
}

function MineShot::onCollision(%this, %object, %details)
{
   if(%object.type $= "Vehicle") {
      %this.onDeath();
   }
}

function MineShot::onDeath(%this)
{
   %this.getScene().add(createExplosion(%this.Position.x SPC (%this.Position.y + 2), 3));
   alxPlay("MyModule:mineShotExplosionSound");
   %this.safeDelete();
}