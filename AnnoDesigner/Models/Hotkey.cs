﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AnnoDesigner.Core.Models;

namespace AnnoDesigner.Models
{
    /// <summary>
    /// Acts as a wrapper for a named <see cref="InputBinding"/>, and allows updating if the currently wrapped <see cref="InputBinding"/> 
    /// is replaced with a fresh reference, whilst still maintaining the same <see cref="HotkeyId"/>.
    /// </summary>
    public class Hotkey : Notify
    {
        private Hotkey() { }
        public Hotkey(string hotkeyId, InputBinding binding):this(hotkeyId, binding, null) { }
        public Hotkey(string hotkeyId, InputBinding binding, string description)
        {
            HotkeyId = hotkeyId;
            Binding = binding;
            Description = description;

            if (binding.Gesture is PolyGesture gesture)
            {
                defaultKey = gesture.Key;
                defaultModifiers = gesture.ModifierKeys;
                defaultMouseAction = gesture.MouseAction;
                defaultType = gesture.Type;
            }
            else
            {
                throw new ArgumentException($"{nameof(binding)} must use a {nameof(PolyGesture)}");
            }
        }

        private InputBinding _binding;
        public InputBinding Binding
        {
            get => _binding;
            set
            {
                UpdateProperty(ref _binding, value);
                //Check that a PolyGesture is still being used
                _ = GetGestureOrThrow();
            }
        }

        private string _name;
        /// <summary>
        /// An identifier for the <see cref="Hotkey"/>, usually required to be unique.
        /// </summary>
        public string HotkeyId
        {
            get => _name;
            set => UpdateProperty(ref _name, value);
        }

        private string _description;
        public string Description
        {
            get => _description;
            set => UpdateProperty(ref _description, value);
        }

        /// <summary>
        /// Resets a hotkey to its defaults.
        /// </summary>
        public void Reset()
        {
            SynchronizeProperties(defaultKey, defaultMouseAction, defaultModifiers, defaultType);
        }

        private void SynchronizeProperties(Key key, ExtendedMouseAction mouseAction, ModifierKeys modifiers, GestureType type)
        {
            var gesture = GetGestureOrThrow();
            gesture.Key = key;
            gesture.MouseAction = mouseAction;
            gesture.ModifierKeys = modifiers;
            gesture.Type = type;
        }

        /// <summary>
        /// Returns <see langword="true"/> if the current mappings for this <see cref="Hotkey"/> 
        /// do not match the default mappings it was created with.
        /// </summary>
        /// <returns></returns>
        public bool IsRemapped()
        {
            var gesture = GetGestureOrThrow(); 
            return !(gesture.Type == defaultType && gesture.Key == defaultKey && gesture.ModifierKeys == defaultModifiers && gesture.MouseAction == defaultMouseAction);
        }

        public HotkeyInformation GetHotkeyInformation()
        {
            var gesture = GetGestureOrThrow();
            return new HotkeyInformation()
            {
                Key = gesture.Key,
                Modifiers = gesture.ModifierKeys,
                MouseAction = gesture.MouseAction,
                Type = gesture.Type
            };
        }

        /// <summary>
        /// Updates a hotkey and based on the given HotkeyInformation
        /// </summary>
        /// <param name="information"></param>
        public void UpdateHotkey(HotkeyInformation information)
        {
            SynchronizeProperties(information.Key, information.MouseAction, information.Modifiers, information.Type);
        }
        
        /// <summary>
        /// Updates a hotkey and based on the given information
        /// </summary>
        /// <param name="information"></param>
        public void UpdateHotkey(Key key, ExtendedMouseAction mouseAction, ModifierKeys modifiers, GestureType type)
        {
            SynchronizeProperties(key, mouseAction, modifiers, type);
        }

        /// <summary>
        /// Checks that the Hotkey is using a PolyGesture
        /// </summary>
        /// <returns></returns>
        private PolyGesture GetGestureOrThrow()
        {
            return Binding.Gesture as PolyGesture ?? throw new InvalidOperationException($"{nameof(Hotkey)} must use a {nameof(PolyGesture)}");
        }

        private readonly Key defaultKey = default;
        private readonly ExtendedMouseAction defaultMouseAction = default;
        private readonly ModifierKeys defaultModifiers = default;
        private readonly GestureType defaultType = default;
    }
}
