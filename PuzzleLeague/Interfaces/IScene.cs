using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PuzzleLeague.Interfaces
{
   public interface IScene
   {
      void Update();
      void Draw(SpriteBatch spriteBatch);

      void OnSceneEnabled();
      void OnSceneDisabled();
   }
}
