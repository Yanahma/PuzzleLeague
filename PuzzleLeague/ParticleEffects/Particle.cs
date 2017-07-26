using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PuzzleLeague.Utilities;

namespace PuzzleLeague
{
   public enum ParticleType
   {
      Empty = 0,
      Heart,
      Octagon,
      Star,
      Circle,
      Triangle
   }

   public enum ParticleMovement
   {
      None,
      Constant,
      Track
   }

   public class Particle
   {
      //
      // Private fields
      //
      private ParticleType type;
      private ParticleMovement movement;
      private Vector2 position;
      private Vector2 velocity;
      private Vector2 target;
      private Timer lifespanTimer;

      // Get-only access to the symbol texture (based on type)
      private Texture2D particleTexture
      {
         get
         {
            switch (type)
            {
               case ParticleType.Triangle:
                  return ContentHelper.GetTexture("tileBlue_31");
               case ParticleType.Circle:
                  return ContentHelper.GetTexture("tileGreen_35");
               case ParticleType.Heart:
                  return ContentHelper.GetTexture("tileRed_36");
               case ParticleType.Star:
                  return ContentHelper.GetTexture("tileYellow_33");
               case ParticleType.Octagon:
                  return ContentHelper.GetTexture("tilePink_30");
            }
            return null;
         }
      }

      //
      // Public fields
      //
      public ParticleType Type { get { return type; } }

      // 
      // Constructor
      //
      public Particle(ParticleType type = ParticleType.Empty, ParticleMovement movement = ParticleMovement.None)
      {
         this.type = type;
         this.movement = movement;
         lifespanTimer = new Timer();
         lifespanTimer.OnComplete += StopEmitting;
      }

      // Stop emitting this particle
      public void StopEmitting(object sender, EventArgs e)
      {
         this.type = ParticleType.Empty;
         this.movement = ParticleMovement.None;
         position = Vector2.Zero;
         velocity = Vector2.Zero;
         target = Vector2.Zero;
         lifespanTimer.StopTimer();
      }

      // Set all necessary fields to set a particle going
      public void ResetParticle(ParticleType type, ParticleMovement movement, Vector2 startPosition, Vector2 velocity, Vector2 target, float lifespan)
      {
         this.type = type;
         this.movement = movement;
         this.position = startPosition;
         this.velocity = velocity;
         this.target = target;
         lifespanTimer.SetTimer(lifespan);
         lifespanTimer.StartTimer();
      }

      // Main update method
      public void Update()
      {
         if (!(type == ParticleType.Empty))
         {
            switch (movement)
            {
               case (ParticleMovement.Constant):
                  position += velocity;
                  break;
               case (ParticleMovement.Track):
                  if (Vector2.DistanceSquared(position, target) < 10f)
                  {
                     type = ParticleType.Empty;
                  }
                  else
                  {
                     velocity = Vector2.Normalize(target - position) * 5f;
                     position += velocity;
                  }
                  break;
            }
         }
      }

      // Main draw method
      public void Draw(SpriteBatch spriteBatch)
      {
         if (!(type == ParticleType.Empty))
         {
            var dest = new Rectangle(position.ToPoint(), new Point(50,50));
            spriteBatch.Draw(particleTexture, dest, null, Color.White);
            // spriteBatch.Draw(particleTexture, position, Color.White);
         }
      }
   }
}
