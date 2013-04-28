function MyModule::create( %this )
{
   //SCENE GROUPS:
   // 0: Background/Walls
   // 1: Tanks
   // 2: weapons/attacks
   // 3: powerups
   // 4: Scenery
   // 5: Mines
   // 6: AI Enemies
   
   // Load GUI profiles.
   exec("./gui/guiProfiles.cs");
   
   exec("./scripts/menu.cs");
   
   exec("./scripts/effects.cs");
   exec("./scripts/misc.cs");
   
   exec("./scripts/scenery/crate.cs");
   exec("./scripts/scenery/rock.cs");
   exec("./scripts/scenery/barrel.cs");
   exec("./scripts/scenery/barrier.cs");

   exec("./scripts/scenewindow.cs");
   exec("./scripts/scene.cs");

   exec("./scripts/background.cs");
   exec("./scripts/tank.cs");
   exec("./scripts/powerup.cs");
   exec("./scripts/powerupSpawner.cs");
   
   exec("./scripts/enemy/turret.cs");
   
   //Behaviors
   
   //Player Controls
   exec("./scripts/behaviors/scene/Pause.cs");
   exec("./scripts/behaviors/controls/TankControls.cs");
   exec("./scripts/behaviors/controls/PrimaryAbility.cs");
   exec("./scripts/behaviors/controls/SecondaryAbility.cs");
   
   //Vehicles
   exec("./scripts/behaviors/vehicles/tanks/TankMovement.cs");

   //Main Weapon
   exec("./scripts/behaviors/combat/ChargeShot.cs");
   //Weapon Powerups
   exec("./scripts/behaviors/combat/powerups/LaserBeam.cs");
   exec("./scripts/behaviors/combat/powerups/SpreadShot.cs");
   exec("./scripts/behaviors/combat/powerups/MineShot.cs");
   
   //Utility Powerups
   exec("./scripts/behaviors/combat/powerups/Regeneration.cs");
   exec("./scripts/behaviors/combat/powerups/Teleport.cs");
   exec("./scripts/behaviors/combat/powerups/SpeedBoost.cs");
   
   exec("./scripts/behaviors/combat/damage/TakesDamage.cs");
   exec("./scripts/behaviors/combat/damage/DealsDamage.cs");
   exec("./scripts/behaviors/combat/damage/SlowOnDamage.cs");
   exec("./scripts/behaviors/combat/damage/FixedHealthBar.cs");
   exec("./scripts/behaviors/spawn/PowerupSpawner.cs");
   
   //AI
   exec("./scripts/behaviors/AI/turret/TurretAbility.cs");
   exec("./scripts/behaviors/AI/turret/Turret.cs");
   exec("./scripts/behaviors/AI/smartMine/SmartMine.cs");
   
   //Teams
   exec("./scripts/behaviors/team/Team.cs");
   
   %id = %this.getId();
   GlobalActionMap.bindCmd(keyboard, "enter", "", %id @ ".resetMap1();");
   GlobalActionMap.bindCmd(keyboard, "escape", "", %id @ ".resetMenu();");
   
   setRandomSeed();
   
   createSceneWindow();
   createScene();
   
   mySceneWindow.setScene(myScene);
   
   //myScene.setDebugOn("collision", "position");
   
   $p1TankFrame = 0;
   $p2TankFrame = 8;
   $maxTankFrame = 64;
   alxPlay("MyModule:introMusic");
   %this.resetMenu();
}

function MyModule::destroy( %this )
{
   destroySceneWindow();
   alxStopAll();
}

function MyModule::resetMenu(%this)
{
      
   %this.unbind();
   
   //alxStopAll();
   myScene.clear();
   
   %id = %this.getId();
   GlobalActionMap.bindCmd(keyboard, "v", "",%id @ ".changeColorP1L();");
   GlobalActionMap.bindCmd(keyboard, "b", "",%id @ ".changeColorP1R();");
   GlobalActionMap.bindCmd(keyboard, ",", "",%id @ ".changeColorP2L();");
   GlobalActionMap.bindCmd(keyboard, ".", "",%id @ ".changeColorP2R();");
   
   %items = createMenuItems(0, 0, 25);
   
   for(%i = 0 ; %i<getWordCount(%items);%i++) {
      myScene.add(getWord(%items, %i));   
   }
   
   %tankP1 = createBasicTank($p1TankFrame, -25, 20);
   %tankP1.Angle = -120;
   %tankP1.Size = "15 16";
   myScene.add(%tankP1);
   %tankP2 = createBasicTank($p2TankFrame, 25, 20);
   %tankP2.Angle = 120;
   %tankP2.Size = "15 16";
   myScene.add(%tankP2);
}

function MyModule::unbind(%this)
{
   GlobalActionMap.unbind(keyboard, "v");
   GlobalActionMap.unbind(keyboard, "b");
   GlobalActionMap.unbind(keyboard, ",");
   GlobalActionMap.unbind(keyboard, ".");
}

function MyModule::changeColorP1R(%this)
{
   $p1TankFrame =  ($p1TankFrame + 8) % $maxTankFrame;
   %this.resetMenu();
}

function MyModule::changeColorP2R(%this)
{
   $p2TankFrame =  ($p2TankFrame + 8) % $maxTankFrame;
   %this.resetMenu();
}

function MyModule::changeColorP1L(%this)
{
   $p1TankFrame = $p1TankFrame <= 0 ? $maxTankFrame - 8 : ($p1TankFrame - 8) % $maxTankFrame;
   %this.resetMenu();
}

function MyModule::changeColorP2L(%this)
{
   $p2TankFrame = $p2TankFrame <= 0 ? $maxTankFrame - 8 : ($p2TankFrame - 8) % $maxTankFrame;
   %this.resetMenu();
}

function MyModule::resetMap1(%this)
{
      
   %this.unbind();
   
   alxStopAll();
   myScene.clear();
   
   myScene.add(createBackground());
   
   %tankP1 = createPlayer1Tank($p1TankFrame, -45, 0, -90);
   myScene.add(%tankP1.healthBar);
   myScene.add(%tankP1);
   %tankP2 = createPlayer2Tank($p2TankFrame, 45, 0, 90);
   myScene.add(%tankP2.healthBar);
   myScene.add(%tankP2);
   
   //Top Powerup Area
   myScene.add(createBarrier(0,45, 20, 5));
   myScene.add(createBarrier(-16,40, 20, 5, 35));
   myScene.add(createBarrier(16,40, 20, 5, -35));
   
   myScene.add(createBarrier(0,25, 15, 5));

   //Bottom Powerup Area
   myScene.add(createBarrier(0,-45, 20, 5));
   myScene.add(createBarrier(-16,-40, 20, 5, -35));
   myScene.add(createBarrier(16,-40, 20, 5, 35));
   
   myScene.add(createBarrier(0,-25, 15, 5));
   
   //Left Area
   myScene.add(createBarrier(-28,-12, 5, 15, -25));
   myScene.add(createBarrier(-28,12, 5, 15, 25));
   
   //Right Area
   myScene.add(createBarrier(28,-12, 5, 15, 25));
   myScene.add(createBarrier(28,12, 5, 15, -25));
   
   //Middle Area
   myScene.add(createBarrier(0,0, 5, 15));
   
   //Powerups
   myScene.add(createPowerupSpawner(0, 35, "0 3 5"));
   myScene.add(createPowerupSpawner(0, -35, "1 2 4"));
   
   // Corner Turrets
   myScene.add(createRandomTurret(47, 47, 16, 130));
   myScene.add(createRandomTurret(-47, 47, 16, -130));
   myScene.add(createRandomTurret(47, -47, 16, 50));
   myScene.add(createRandomTurret(-47, -47, 16,-50));
   
   // Powerup Turrets
   myScene.add(createRandomTurret(22, 47, 13, -130));
   myScene.add(createRandomTurret(-22, 47, 13, 130));
   myScene.add(createRandomTurret(22, -47, 13, -50));
   myScene.add(createRandomTurret(-22, -47, 13,50));
}

function createPlayer1Tank(%initialFrame, %x_pos, %y_pos, %angle)
{
   %p1Tank = createTank(%initialFrame, %x_pos, %y_pos);
   %p1Tank.Angle = %angle;
   
   //Controls
   %primary = PrimaryAbilityBehavior.createInstance();
   %primary.key = "keyboard v";
   %secondary = SecondaryAbilityBehavior.createInstance();
   %secondary.key = "keyboard b";
   %p1Tank.addBehavior(%primary);
   %p1Tank.addBehavior(%secondary);
   
   %controlsP1 = TankControlsBehavior.createInstance();
   %controlsP1.forwardKey = "keyboard w";
   %controlsP1.backwardKey = "keyboard s";
   %controlsP1.leftKey = "keyboard a";
   %controlsP1.rightKey = "keyboard d";
   %p1Tank.addBehavior(%controlsP1);
   //Weapons
   %chargeShotP1 = ChargeShotBehavior.createInstance();
   %p1Tank.addBehavior(%chargeShotP1);
   %team1 = TeamBehavior.createInstance();
   %team1.teamID = 1;
   %p1Tank.addBehavior(%team1);
   return %p1Tank;
   
}

function createPlayer2Tank(%initialFrame, %x_pos, %y_pos, %angle)
{
   %p2Tank = createTank(%initialFrame, %x_pos, %y_pos);
   %p2Tank.Angle = %angle;
   
   %primary = PrimaryAbilityBehavior.createInstance();
   %primary.key = "keyboard ,";
   %secondary = SecondaryAbilityBehavior.createInstance();
   %secondary.key = "keyboard .";
   %p2Tank.addBehavior(%primary);
   %p2Tank.addBehavior(%secondary);
   
   %controlsP2 = TankControlsBehavior.createInstance();
   %controlsP2.forwardKey = "keyboard up";
   %controlsP2.backwardKey = "keyboard down";
   %controlsP2.leftKey = "keyboard left";
   %controlsP2.rightKey = "keyboard right";
   %p2Tank.addBehavior(%controlsP2);
   %chargeShotP2 = ChargeShotBehavior.createInstance();
   %p2Tank.addBehavior(%chargeShotP2);
   %team2 = TeamBehavior.createInstance();
   %team2.teamID = 2;
   %p2Tank.addBehavior(%team2);
   return %p2Tank;
}