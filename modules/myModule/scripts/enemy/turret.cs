function createTurret(%x_pos, %y_pos, %range, %angle)
{
   // Create the sprite.
   %turret = new Sprite()
   {
      class = Turret;
      Image = "ToyAssets:HollowArrow";
      BodyType = dynamic;
      BlendColor = "Black";
      LinearDamping = 100;
      Position = %x_pos SPC %y_pos;
      Angle = %angle;
      Size = 3;
      SceneLayer = 2;
      SceneGroup = 1;
   };
   %turret.createCircleCollisionShape(%turret.Size.x / 2);
   %turretBehavior = TurretBehavior.createInstance();
   %turretBehavior.range = %range;
   %turretBehavior.resetAngle = %angle;
   %turret.addBehavior(%turretBehavior);
   //Takes Damage Behavior
   %takedamage = TakesDamageBehavior.createInstance();
   %takedamage.health = 60;
   %takedamage.maxHealth = 60;
   %turret.addBehavior(%takedamage);
   
   return %turret;
}

function Turret::onDeath(%this)
{
   %this.getScene().add(createExplosion(%this.getPositon(), 1));
   %this.safeDelete();
}