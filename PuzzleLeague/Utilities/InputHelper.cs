using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace PuzzleLeague.Utilities
{
   /// <summary>
   /// Public enum defining the available buttons to control the game
   /// (Mapping available in "InputHelper" class)
   /// </summary>
   public enum Buttons
   {
      Up,
      Down,
      Left,
      Right,
      Swap,
      AddRow,
      Pause,
      Escape
   }

   /// <summary>
   /// Public enum defining the available states of GUI Buttons
   /// </summary>
   public enum ButtonStates
   {
      None,
      Hover,
      Click
   }

   /// <summary>
   /// Event handler for a button being pushed
   /// </summary>
   /// <param name="buttonPressed"></param>
   public delegate void ButtonPushHandler(Buttons buttonPressed);

   /// <summary>
   /// Event handler for the mouse moving
   /// </summary>
   /// <param name="mousePosition"></param>
   public delegate void MouseMoveHandler(Point mousePosition);

   /// <summary>
   /// Static class to make available changes in input state/allow entities to subscribe to events
   /// and allow for custom keybindings
   /// </summary>
   static class InputHelper
   {
      /// <summary>
      /// The state of the keyboard on the current frame
      /// </summary>
      private static KeyboardState currentKeyboardState;

      /// <summary>
      /// The state of the keyboard on the previous frame
      /// </summary>
      private static KeyboardState previousKeyboardState;

      /// <summary>
      /// The state of the mouse on the current frame
      /// </summary>
      private static MouseState currentMouseState;

      /// <summary>
      /// The state of the mouse on the previous frame
      /// </summary>
      private static MouseState previousMouseState;

      /// <summary>
      /// Whether or not to check for mouse updates
      /// </summary>
      public static bool MouseEnabled { get; private set; }

      /// <summary>
      /// An array to hold the enum values of the "Buttons" enum
      /// </summary>
      private static Array buttonEnums;

      /// <summary>
      /// A mapping of keyboard keys to action buttons (private, use "MapButton" function to update)
      /// </summary>
      private static Dictionary<Buttons,Keys> KeyMappings;

      /// <summary>
      /// Public event for when an action button is pressed
      /// </summary>
      public static event ButtonPushHandler ButtonPressed;

      /// <summary>
      /// Public event for when the mouse moves
      /// </summary>
      public static event MouseMoveHandler MouseMoved;

      /// <summary>
      /// Static constructor to init values
      /// </summary>
      static InputHelper()
      {
         // Initalize the "KeyMappings" dictionary and add default values
         // TODO: Read from a .ini mapping file for these values on launch
         KeyMappings = new Dictionary<Buttons, Keys>();
         KeyMappings.Add(Buttons.Left, Keys.Left);
         KeyMappings.Add(Buttons.Right, Keys.Right);
         KeyMappings.Add(Buttons.Up, Keys.Up);
         KeyMappings.Add(Buttons.Down, Keys.Down);

         KeyMappings.Add(Buttons.Swap, Keys.A);
         KeyMappings.Add(Buttons.AddRow, Keys.Space);

         KeyMappings.Add(Buttons.Pause, Keys.P);
         KeyMappings.Add(Buttons.Escape, Keys.Escape);

         // Initalize the "buttonEnums" array with the values of all types of "Buttons" 
         buttonEnums = Enum.GetValues(typeof(Buttons));

         //
         MouseEnabled = true;
      }

      /// <summary>
      /// Public access to modifying the keymappings input scheme
      /// </summary>
      /// <param name="button">The button value to change</param>
      /// <param name="key">The keyboard key to use for the button</param>
      public static void MapButton(Buttons button, Keys key)
      {
         // If keyMappings already contains this button, alter the key value
         if (KeyMappings.ContainsKey(button))
         {
            KeyMappings[button] = key;
         }
         // Otherwise, we need to add the button to the dictionary
         else
         {
            KeyMappings.Add(button, key);
         }
      }

      public static void EnableMouse() => MouseEnabled = true;

      public static void DisableMouse() => MouseEnabled = false;

      /// <summary>
      /// Update the input details for this frame (should be called every frame)
      /// </summary>
      public static void Update()
      {
         // Update keyboard states
         previousKeyboardState = currentKeyboardState;
         currentKeyboardState = Keyboard.GetState();

         // If keyboard state has changed, loop around the button enums & see if the related
         // value in "KeyMappings" has been changed; if it has, use the "ButtonPressed" event to 
         // notify subscribers
         if (previousKeyboardState != currentKeyboardState)
         {
            foreach (Buttons button in buttonEnums)
            {
               if (KeyMappings.ContainsKey(button))
               {
                  Keys key = KeyMappings[button];
                  if (currentKeyboardState.IsKeyDown(key) &&
                     !(previousKeyboardState.IsKeyDown(key)))
                  {
                     ButtonPressed?.Invoke(button);
                  }
               }
            }
         }

         if (MouseEnabled)
         {
            // Update mouse states
            previousMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();

            // Update if the mouse has moved
            if (currentMouseState.X != previousMouseState.X
               || currentMouseState.Y != previousMouseState.Y)
            {
               MouseMoved?.Invoke(currentMouseState.Position);
            }

            // If lmb pressed, act as if confirm button pressed
            if (currentMouseState.LeftButton == ButtonState.Pressed
               && previousMouseState.LeftButton != ButtonState.Pressed)
            {
               ButtonPressed?.Invoke(Buttons.Swap);
            }
         }
      }

      /// <summary>
      /// Static function to determine if the current mouse position overlaps a rectange area
      /// </summary>
      /// <param name="checkArea">The rectange to check</param>
      /// <returns>True if the mouse is over the passed area, else False</returns>
      public static bool MouseIsOver(Rectangle checkArea)
      {
         if ((currentMouseState.X > checkArea.Left)
            && (currentMouseState.X < checkArea.Right)
            && (currentMouseState.Y > checkArea.Top)
            && (currentMouseState.Y < checkArea.Bottom))
         {
            return true; // Mouse exists within the rectange
         }
         return false; // Otherwise, it doesn't
      }
   }
}
