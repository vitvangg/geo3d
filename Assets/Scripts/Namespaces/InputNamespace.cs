using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GD3D.Player;

namespace GD3D.CustomInput
{

    [Serializable]
    public enum PressMode
    {

        hold = 0,

        down = 1,

        up = 2,
    }


    [Serializable]
    public class Key
    {

        public string name;


        public KeyboardKey[] keyboardKeys;

        public GamepadKey[] gamepadKeys;


        public int maxKeys = 0;

               public CheckMode checkMode;

        #region New Key() Methods
      
        public Key(string name, int maxKeys = 0, CheckMode checkMode = CheckMode.any)
        {
            // Set the name
            this.name = name;

            // Set the max keys
            this.maxKeys = maxKeys;

            // Set the key arrays
            keyboardKeys = new KeyboardKey[maxKeys];
            gamepadKeys = new GamepadKey[maxKeys];

            // Set the check mode
            this.checkMode = checkMode;

            FixArrays();
        }

        public Key(string name, KeyboardKey[] keyboardKey, GamepadKey[] gamepadKey, int maxKeys = 0, CheckMode checkMode = CheckMode.any)
        {
            // Set the name
            this.name = name;

            // Set the max keys
            this.maxKeys = maxKeys;

            // Set the key arrays
            keyboardKeys = keyboardKey;
            gamepadKeys = gamepadKey;

            // Set the check mode
            this.checkMode = checkMode;

            FixArrays();
        }

        
        public Key(string name, KeyboardKey keyboardKey, GamepadKey gamepadKey, int maxKeys = 0, CheckMode checkMode = CheckMode.any)
        {
            // Set the name
            this.name = name;

            // Set the max keys
            this.maxKeys = maxKeys;

            // Create 2 empty arrays with only one entry in each
            keyboardKeys = NewKeys(keyboardKey);
            gamepadKeys = NewKeys(gamepadKey);

            // Set the check mode
            this.checkMode = checkMode;

            FixArrays();
        }

        
        public Key(string name, KeyboardKey keyboardKey, GamepadKey[] gamepadKey, int maxKeys = 0, CheckMode checkMode = CheckMode.any)
        {
            // Set the name
            this.name = name;

            // Set the max keys
            this.maxKeys = maxKeys;

            // Set one of the key arrays and create a empty array with only one entry in the other
            keyboardKeys = NewKeys(keyboardKey);
            gamepadKeys = gamepadKey;

            // Set the check mode
            this.checkMode = checkMode;

            FixArrays();
        }

        public Key(string name, KeyboardKey[] keyboardKey, GamepadKey gamepadKey, int maxKeys = 0, CheckMode checkMode = CheckMode.any)
        {
            // Set the name
            this.name = name;

            // Set the max keys
            this.maxKeys = maxKeys;

            // Set one of the key arrays and create a empty array with only one entry in the other
            keyboardKeys = keyboardKey;
            gamepadKeys = NewKeys(gamepadKey);

            // Set the check mode
            this.checkMode = checkMode;

            FixArrays();
        }
        #endregion

        public void FixArrays()
        {
            int maxKeys = this.maxKeys;

            // IF the max keys are 0 or below, then return
            if (maxKeys <= 0)
                return;

            // Fix the keyboard keys first
            if (keyboardKeys.Length != maxKeys)
            {
                // Create a new list of KeyboardKey
                List<KeyboardKey> fixedKeyboardKeys = new List<KeyboardKey>();

                // Loop for the amount in maxKeys
                for (int i = 0; i < maxKeys; i++)
                {
                    // Add it to the list if it's in range of the old keyboardKeys array
                    if (Helpers.ValueInRangeOfArray(i, keyboardKeys))
                    {
                        fixedKeyboardKeys.Add(keyboardKeys[i]);
                    }
                    // Else just add a key of none to the list
                    else
                    {
                        fixedKeyboardKeys.Add(KeyboardKey.None);
                    }
                }

                // Set the old array to the new fixed list
                keyboardKeys = fixedKeyboardKeys.ToArray();
            }

        }


        public static KeyboardKey[] NewKeys(params KeyboardKey[] keys) { return keys; }

        public static GamepadKey[] NewKeys(params GamepadKey[] keys) { return keys; }


        public bool Pressed(PressMode mode = PressMode.down)
        {
            // Change depending on the checkMode
            switch (checkMode)
            {
                // Check if any key in either keyboardKeys or gamepadKeys were pressed
                case CheckMode.any:

                    // Loop through all keyboard keys first
                    foreach (KeyboardKey key in keyboardKeys)
                    {
                        // Skip empty keys
                        if (key == KeyboardKey.None)
                            continue;

                        // Return true if one of the keyboard keys were pressed
                        if (PlayerInput.KeyPressed(key, mode))
                            return true;
                    }


                    // Return false if no key was pressed
                    return false;

                // Check if all the keys in either keyboardKeys or gamepadKeys were pressed
                case CheckMode.all:

                    bool checkGamepad = false;

                    // Loop through all keyboard keys first
                    foreach (KeyboardKey key in keyboardKeys)
                    {
                        // Skip empty keys
                        if (key == KeyboardKey.None)
                            continue;

                    }

                    // Return true if all the keys in either keyboardKeys or gamepadKeys were pressed
                    return true;

                // Check if all the keys in BOTH keyboardKeys and gamepadKeys were pressed
                case CheckMode.everything:

                    // Loop through all keyboard keys first
                    foreach (KeyboardKey key in keyboardKeys)
                    {
                        // Skip empty keys
                        if (key == KeyboardKey.None)
                            continue;

                        // Return false if one of the keyboard keys weren't pressed
                        if (!PlayerInput.KeyPressed(key, mode))
                            return false;
                    }

                    // Return true if all the keys in BOTH keyboardKeys and gamepadKeys were pressed
                    return true;

                // Return false if no valid CheckMode was given. (Which shouldn't happen)
                default:
                    return false;
            }
        }


        [Serializable]
        public enum CheckMode
        {

            any = 0,

            all = 1,

            everything = 2,
        }

        public static float GetAxis(Key negativeKey, Key positiveKey)
        {
            // Create bools to use instead of the function
            bool negativeHeld = negativeKey.Pressed(PressMode.hold);
            bool positiveHeld = positiveKey.Pressed(PressMode.hold);

            // Return 1 if the positive key is held down but the negative one is not
            if (positiveHeld && !negativeHeld)
            {
                return 1;
            }
            // Return -1 if the negative key is held down but the positive one is not
            else if (negativeHeld && !positiveHeld)
            {
                return -1;
            }

            // Return 0 by default
            return 0;
        }
    }


    [Serializable]
    public enum KeyboardKey
    {
        None = 0,
        Backspace = 8,
        Tab = 9,
        Clear = 12,
        Return = 13,
        Pause = 19,
        Escape = 27,
        Space = 32,
        A = 97,
        D = 100,
        S = 115,
        W = 119,
        KeypadEnter = 271,
        KeypadEquals = 272,
        UpArrow = 273,
        DownArrow = 274,
        RightArrow = 275,
        LeftArrow = 276,
        LeftMouseButton = 323,
    }


    [Serializable]
    public enum GamepadKey
    {
        None = 0,                                                                                                                                                                                                                                                          AButton = 330,BButton = 331,XButton = 332,YButton = 333,LeftBumper = 334,RightBumper = 335,Select = 336,        Start = 337,LeftStickPress = 338,RightStickPress = 339,JoystickButton10 = 340,JoystickButton11 = 341,JoystickButton12 = 342,JoystickButton13 = 343,JoystickButton14 = 344,JoystickButton15 = 345,JoystickButton16 = 346,JoystickButton17 = 347,JoystickButton18 = 348,JoystickButton19 = 349,LeftTrigger = 510, RightTrigger = 511,LeftStickUp = 512,LeftStickDown = 513,LeftStickLeft = 514,LeftStickRight = 515,RightStickUp = 516,RightStickDown = 517,RightStickLeft = 518,RightStickRight = 519,DpadUp = 520,DpadDown = 521,DpadLeft = 522,DpadRight = 523,
}
}