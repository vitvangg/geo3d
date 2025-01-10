using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GD3D.CustomInput;

namespace GD3D.Player
{

    public class PlayerInput : MonoBehaviour
    {
        public const int KEYCODE_LENGTH = 510;

        private static Key[] s_defaultKeys = new Key[]
        {
            //-- The main gameplay button that does everything
            new Key("Click",
                Key.NewKeys(KeyboardKey.UpArrow, KeyboardKey.LeftMouseButton, KeyboardKey.Space),
                Key.NewKeys(GamepadKey.AButton)),
            


            //-- Other
            new Key("Escape",
                Key.NewKeys(KeyboardKey.Escape),
                Key.NewKeys(GamepadKey.Start),
                3),

            //-- Movement keys
            new Key("Left",
                Key.NewKeys(KeyboardKey.A, KeyboardKey.LeftArrow),
                Key.NewKeys(GamepadKey.LeftStickLeft, GamepadKey.DpadLeft),
                3),

            new Key("Right",
                Key.NewKeys(KeyboardKey.D, KeyboardKey.RightArrow),
                Key.NewKeys(GamepadKey.LeftStickRight, GamepadKey.DpadRight),
                3),

            new Key("Up",
                Key.NewKeys(KeyboardKey.W, KeyboardKey.UpArrow),
                Key.NewKeys(GamepadKey.LeftStickUp, GamepadKey.DpadUp),
                3),

            new Key("Down",
                Key.NewKeys(KeyboardKey.S, KeyboardKey.DownArrow),
                Key.NewKeys(GamepadKey.LeftStickDown, GamepadKey.DpadDown),
                3),

            new Key("Submit",
                Key.NewKeys(KeyboardKey.Return),
                Key.NewKeys(GamepadKey.AButton),
                3),

            new Key("Cancel",
                Key.NewKeys(KeyboardKey.Backspace),
                Key.NewKeys(GamepadKey.BButton),
                3),

        };

        public static Key[] DefaultKeys => s_defaultKeys;

        public static Key[] Keys = DefaultKeys;


        private void Start()
        {
        }


        private float LockAxis(float axisValue, float deadZone)
        {
            bool setTo0 = Helpers.ValueWithinRange(axisValue, -deadZone, deadZone);

            float lockedAxisValue = setTo0 ? 0 : axisValue;

            return lockedAxisValue;
        }

        private Vector2 LockVector2Axis(Vector2 axisValue, float deadZone)
        {
            return new Vector2(LockAxis(axisValue.x, deadZone), LockAxis(axisValue.y, deadZone));
        }

        public static Key GetKey(string name, bool caseSensitive = true)
        {
            // Loop through all of the keys
            foreach (Key key in Keys)
            {
                // Check if the names match (Case sensitive)
                if (key.name == name && caseSensitive)
                {
                    // Return the key if the names match
                    return key;
                }

                // Check if the names match (Not case sensitive)
                if (key.name.ToLower() == name.ToLower() && !caseSensitive)
                {
                    // Return the key if the names match
                    return key;
                }
            }

            // If no key was found, return null
            return null;
        }
        public static bool KeyPressed(KeyboardKey key, PressMode mode = PressMode.down)
        {
            // Return false if the key is none
            if (key == KeyboardKey.None)
                return false;

            // If the key is outside the keycode length then return false
            if ((int)key >= KEYCODE_LENGTH - 1)
            {
                return false;
            }

            // Check if the key is pressed but in KeyCode form instead
            return KeyPressed((KeyCode)key, mode);
        }

        public static bool KeyPressed(GamepadKey key, PressMode mode = PressMode.down)
        {
            // Return false if the key is none
            if (key == GamepadKey.None)
                return false;
            // Check if the key is pressed but in KeyCode form instead
            return KeyPressed((KeyCode)key, mode);
        }

        public static bool KeyPressed(KeyCode key, PressMode mode = PressMode.down)
        {
            // Use a switch statement to change the input method to be correct
            switch (mode)
            {
                // Check if the key is held down at all
                case PressMode.hold:
                    return UnityEngine.Input.GetKey(key);

                // Check if the key has just been pressed
                case PressMode.down:
                    return UnityEngine.Input.GetKeyDown(key);

                // Check if the key has just been released
                case PressMode.up:
                    return UnityEngine.Input.GetKeyUp(key);

                // Return false if no valid PressMode was given. (Which shouldn't happen)
                default:
                    return false;
            }
        }

        public static bool ValuePressed(float value, float oldValue, PressMode mode, bool lessThan0)
        {
            // Check the different press modes
            switch (mode)
            {
                // Check if the value is held down at all
                case PressMode.hold:
                    return lessThan0 ? value < 0 : value > 0;

                // Check if the value has just been pressed
                case PressMode.down:
                    return (lessThan0 ? value < 0 : value > 0) && oldValue == 0;

                // Check if the value has just been released
                case PressMode.up:
                    return value == 0 && oldValue == (lessThan0 ? -1 : 1);

                // Return false if no valid PressMode was given. (Which shouldn't happen)
                default:
                    return false;
            }
        }

        public static bool PressedAny(PressMode mode = PressMode.down, bool checkSpecialKeys = true)
        {
            return PressedKeyboard(mode);
        }


        /// <summary>
        public static bool PressedKeyboard(PressMode mode = PressMode.down)
        {
            // Loop through all the KeyboardKey in the KeyboardKey enum
            foreach (KeyboardKey item in Enum.GetValues(typeof(KeyboardKey)))
            {
                // If the key was pressed, then return true
                if (KeyPressed(item, mode))
                {
                    return true;
                }
            }

            // Return false if no KeyboardKey was pressed
            return false;
        }

    }
}
