function addExplosion(%scene, %position, %scale)
{
   // ParticlePlayer is also derived from SceneObject, we add it just like we've added all the other
   //objects so far
   %explosion = new ParticlePlayer();

   //We load the particle asset from our ToyAssets module
   %explosion.Particle = "MyModule:boomParticle";

   //We set the Particle Player's position to %Sceneobject's position
   %explosion.setPosition(%position);

   //This Scales the particles to twice their original size
   %explosion.setSizeScale(%scale);
   
   %scene.add(%explosion);
}