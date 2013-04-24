function MyModule::create( %this )
{
   //SCENE GROUPS:
   // 0: Background/Walls
   // 1: Tanks
   // 2: weapons/attacks
   // 3: powerups
   // 4: Scenery
   // 5: Mines
   
   // Load GUI profiles.
   exec("./gui/guiProfiles.cs");
   
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
      
   exec("./scripts/behaviors/movement/TankControls.cs");
   exec("./scripts/behaviors/scene/Pause.cs");
   
   //Main Weapon
   exec("./scripts/behaviors/combat/ChargeShot.cs");
   //Secondary Weapons
   exec("./scripts/behaviors/combat/powerups/LaserBeam.cs");
   exec("./scripts/behaviors/combat/powerups/SpreadShot.cs");
   exec("./scripts/behaviors/combat/powerups/Teleport.cs");
   exec("./scripts/behaviors/combat/powerups/MineShot.cs");
   
   //Boosts
   exec("./scripts/behaviors/combat/powerups/Regeneration.cs");
   
   exec("./scripts/behaviors/combat/damage/TakesDamage.cs");
   exec("./scripts/behaviors/combat/damage/DealsDamage.cs");
   exec("./scripts/behaviors/combat/damage/FixedHealthBar.cs");
   exec("./scripts/behaviors/spawn/PowerupSpawner.cs");
   
   GlobalActionMap.bindObj(keyboard, "enter", "resetMap1", %this);
   
   setRandomSeed();
   
   createSceneWindow();
   createScene();
   
   mySceneWindow.setScene(myScene);
   
   //myScene.setDebugOn("collision", "position");
   
   %this.resetMap1();
}

function MyModule::destroy( %this )
{
   destroySceneWindow();
   alxStopAll();
}

function MyModule::resetMap1(%this)
{
   alxStopAll();
   myScene.clear();
   myScene.add(createBackground());
   addPlayer1Tank();
   
   addPlayer2Tank();
   
   //Middle Area
   //myScene.add(createBarrier(-10,20, 5, 20, -30));
   //myScene.add(createBarrier(10,20, 5, 20, 30));
   //myScene.add(createBarrier(-10,-20, 5, 20, 30));
   //myScene.add(createBarrier(10,-20, 5, 20, -30));
   
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
   
   myScene.add(createPowerupSpawner(0, 35, "0 3"));
   myScene.add(createPowerupSpawner(0, -35, "1 2 4"));
}

function addPlayer1Tank()
{
   %p1Tank = createTank(0, -45, 0);
   %p1Tank.Angle = -90;
   %p1Tank.mainKey1 = "keyboard z";
   %p1Tank.mainKey2 = "keyboard x";
   //Controls
   %controlsP1 = TankControlsBehavior.createInstance();
   %controlsP1.forwardKey = "keyboard up";
   %controlsP1.backwardKey = "keyboard down";
   %controlsP1.leftKey = "keyboard left";
   %controlsP1.rightKey = "keyboard right";
   %p1Tank.addBehavior(%controlsP1);
   //Weapons
   %chargeShotP1 = ChargeShotBehavior.createInstance();
   %chargeShotP1.powerKey = %p1Tank.mainKey1;
   %p1Tank.addBehavior(%chargeShotP1);
   myScene.add(%p1Tank.healthBar);
   myScene.add(%p1Tank);
}

function addPlayer2Tank()
{
   %p2Tank = createTank(24, 45, 0);
   %p2Tank.Angle = 90;
   %p2Tank.mainKey1 = "keyboard o";
   %p2Tank.mainKey2 = "keyboard p";
   %controlsP2 = TankControlsBehavior.createInstance();
   %controlsP2.forwardKey = "keyboard w";
   %controlsP2.backwardKey = "keyboard s";
   %controlsP2.leftKey = "keyboard a";
   %controlsP2.rightKey = "keyboard d";
   %p2Tank.addBehavior(%controlsP2);
   %chargeShotP2 = ChargeShotBehavior.createInstance();
   %chargeShotP2.powerKey = %p2Tank.mainKey1;
   %p2Tank.addBehavior(%chargeShotP2);
   myScene.add(%p2Tank.healthBar);
   myScene.add(%p2Tank);
}