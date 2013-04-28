function createRock(%x_pos, %y_pos)
{
   // Create the sprite.
   %rock = new Sprite()
   {
      class = Rock;
      type = Scenery;
      isBreakable = false;
      Image = "myModule:rocksImage";
      Frame = getRandom(0,2);
      BodyType = dynamic;
      Position = %x_pos SPC %y_pos;
      Size = 3;
      LinearDamping = 1.0;
      AngularDamping = 1.5;
      SceneLayer = 2;
      SceneGroup = 4;
   };
   %rock.createCircleCollisionShape( %rock.Size.x / 2);
   %rock.setCollisionGroups("0 1 2 4");
   
   return %rock;
}