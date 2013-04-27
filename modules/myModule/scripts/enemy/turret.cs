function createTurret(%x_pos, %y_pos, %range, %angle)
{
   // Create the sprite.
   %turret = new Sprite()
   {
      class = Turret;
      Image = "MyModule:turretImage";
      BodyType = dynamic;
      LinearDamping = 100;
      Position = %x_pos SPC %y_pos;
      Angle = %angle;
      Size = 3;
      SceneLayer = 2;
      SceneGroup = 1;
   };
   %turret.createPolygonBoxCollisionShape();
   %turretBehavior = TurretBehavior.createInstance();
   %turretBehavior.rangeRadius = %range;
   %turretBehavior.resetAngle = %angle;
   %turret.addBehavior(%turretBehavior);
   
   //Takes Damage Behavior
   %takedamage = TakesDamageBehavior.createInstance();
   %takedamage.health = 60;
   %takedamage.maxHealth = 60;
   %turret.addBehavior(%takedamage);
   
   return %turret;
}

function createLaserBeamTurret(%x_pos, %y_pos, %range, %angle)
{
   %turret = createTurret(%x_pos, %y_pos, %range, %angle);
   %turret.BlendColor = "Red";
   %laserBeam = LaserBeamBehavior.createInstance();
   %laserBeam.duration = -1;
   %laserBeam.damage = 5;
   %laserBeam.beamWidth = %laserBeam.beamWidth; 
   %laserBeam.beamLength = %range;
   %laserBeam.reloadTime = 500;
   %laserBeam.maxBeamTime = 200;
   %laserBeam.beamRefreshTime = %laserBeam.beamRefreshTime;
   %turret.addBehavior(%laserBeam);
   
   return %turret;
}

function createChargeShotTurret(%x_pos, %y_pos, %range, %angle)
{
   %turret = createTurret(%x_pos, %y_pos, %range, %angle);
   %chargeShot = ChargeShotBehavior.createInstance();
   %chargeShot.duration = -1;
   %chargeShot.damage = 20;
   %chargeShot.minSpeed = 30;
   %chargeShot.maxSpeed = 40;
   %chargeShot.maxChargeTime = 1000;
   %chargeShot.reloadTime = 2000;
   %chargeShot.chargeShotLifespan = 800;
   %turret.addBehavior(%chargeShot);
   
   return %turret;
}

function createSpreadShotTurret(%x_pos, %y_pos, %range, %angle)
{
   %turret = createTurret(%x_pos, %y_pos, %range, %angle);
   %turret.BlendColor = "Yellow";
   %spreadShot = SpreadShotBehavior.createInstance();
   %spreadShot.duration = -1;
   %spreadShot.damage = 7;
   %spreadShot.speed = 30;
   %spreadShot.reloadTime = 1500;
   %spreadShot.spreadShotLifespan = 800;
   %spreadShot.shotAngles = "0 -8 8";
   %turret.addBehavior(%spreadShot);
   
   return %turret;
}

function createRandomTurret(%x_pos, %y_pos, %range, %angle, %choices)
{
   %defaultChoices = "0 1 2";
   if(getWordCount(%choices) == 0) {
      %choices = %defaultChoices;   
   }
   %rand = getWord(%choices, getRandom(0, getWordCount(%choices)-1));
   switch(%rand)
   {
      case 0:
         %turret = createChargeShotTurret(%x_pos, %y_pos, %range, %angle);
      case 1:
         %turret = createLaserBeamTurret(%x_pos, %y_pos, %range, %angle);
      case 2:
         %turret = createSpreadShotTurret(%x_pos, %y_pos, %range, %angle);
      default:
         %turret = createChargeShotTurret(%x_pos, %y_pos, %range, %angle);
   }
   return %turret;
}

function Turret::onDeath(%this)
{
   %this.getScene().add(createExplosion(%this.Position, 4));
   %this.safeDelete();
}