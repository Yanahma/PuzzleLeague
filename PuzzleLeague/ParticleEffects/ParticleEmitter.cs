using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PuzzleLeague
{
   public static class ParticleEmitter
   {
      //
      // Private fields
      //
      private static Particle[] particles = new Particle[100];

      //
      // Constructor
      //
      static ParticleEmitter()
      {
         for (var i = 0; i < 100; i++)
            particles[i] = new Particle();
      }

      // Create a certain amount of particles
      public static void CreateParticles(int number, ParticleType type, ParticleMovement movement, Vector2 position, Vector2 velocity, Vector2 target, float lifespan = 200f)
      {
         for(var i = 0; i < 100 && number > 0; i++)
         {
            if (particles[i].Type == ParticleType.Empty)
            {
               particles[i].ResetParticle(type, movement, position, velocity, target, lifespan);
               number--;
            }
         }
      }

      // Main update method
      public static void Update()
      {
         foreach (Particle p in particles)
            p.Update();
      }

      public static void Draw(SpriteBatch spriteBatch)
      {
         foreach (Particle p in particles)
            p.Draw(spriteBatch);
      }
   }
}
