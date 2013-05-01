if (!isObject(SpreadShotBehavior))
{
   %template = new BehaviorTemplate(SpreadShotBehavior);

   %template.friendlyName = "Tank Spread Shot";
   %template.behaviorType = "Combat";
   %template.description  = "Tank Shotgun/Spread Shot ability";

   %template.addBehaviorField(duration, "Duration of Powerup/Ability (negative number for infinite) (in ms)", int, 15000);
   %template.addBehaviorField(damage, "Damage per shot", int, 10);

   %template.addBehaviorField(speed, "Projectile Speed", float, 60);
   
   %template.addBehaviorField(reloadTime, "Reload time (in ms)", int, 2000);
   %template.addBehaviorField(spreadShotLifespan, "LifeSpan of Shot (in ms)", int, 400);
   
   %template.addBehaviorField(shotAngles, "Angles of shot from front", string, "0 -8 8 -16 16");
   
}

function SpreadShotBehavior::onBehaviorAdd(%this)
{
   %this.isLoaded = true;
}

function SpreadShotBehavior::onBehaviorRemove(%this)
{
   %this.stopSounds();
}

function SpreadShotBehavior::stopSounds(%this)
{
   alxStop(%this.reloadSound);
   alxStop(%this.shotSound);
}

function SpreadShotBehavior::loadShot(%this)
{
   %this.reloadSound = alxPlay("MyModule:spreadShotReloadSound");
   %this.isLoaded = true;
}

function SpreadShotBehavior::spreadShot(%this)
{
   if(%this.isLoaded)
   {
      %this.createSpreadShot();
      %this.shotSound = alxPlay("MyModule:spreadShotSound");
      %this.isLoaded = false;
      %this.reloadSchedule = %this.schedule(%this.reloadTime, loadShot);
   }
}

function SpreadShotBehavior::createSpreadShot(%this)
{ 
   for(%i = 0; %i < getWordCount(%this.shotAngles); %i++)
   {
      %shot = new Sprite()
      {
         class = SpreadShot;
         Image = "ToyAssets:WhiteSphere";
         BlendColor = "Black";
         BodyType = dynamic;
         Bullet = true;
         Size = 1;
         SceneLayer = 1;
         SceneGroup = 2;
         CollisionCallback = true;
      };
      %shot.Position = %this.owner.getWorldPoint(0 SPC (%this.owner.Size.height * 0.5 + %shot.Size.height * 0.6) );
      %shot.setLinearVelocityPolar(%this.owner.Angle - 180 + getWord(%this.shotAngles, %i),%this.speed);
      %shot.createCircleCollisionShape(%shot.Size.height / 2);
      %shot.setCollisionGroups("0 1 4");
      
      %dealDmgBehavior = DealsDamageBehavior.createInstance();
      %dealDmgBehavior.strength = %this.damage;
      %shot.addBehavior(%dealDmgBehavior);
      
      %shot.dieSchedule = %shot.schedule(%this.spreadShotLifespan,onDeath);
      %this.owner.getScene().add( %shot );
   }
}

function SpreadShot::onCollision(%this, %object, %details)
{
   if(%object.type $= "Scenery" && %object.isBreakable) {
      %object.safeDelete();
   }
   %this.onDeath();
}

function SpreadShot::onDeath(%this)
{
   %this.getScene().add(createExplosion(%this.Position.x SPC (%this.Position.y + 2), 3));
   alxPlay("MyModule:shotExplosionSound");
   %this.safeDelete();
}