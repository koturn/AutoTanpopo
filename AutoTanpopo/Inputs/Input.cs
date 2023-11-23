﻿using System;
using System.Runtime.InteropServices;


namespace AutoTanpopo.Inputs
{
    /// <summary>
    /// Used by <see cref="InputUtil.NativeMethods.SendInput(int, ref Input, int)"/> or <see cref="InputUtil.NativeMethods.SendInput(int, Input[], int)"/>
    /// to store information for synthesizing input events such as keystrokes, mouse movement, and mouse clicks.
    /// </summary>
    /// <remarks><seealso href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-input"/></remarks>
    [StructLayout(LayoutKind.Sequential)]
    public struct Input
    {
        /// <summary>
        /// The type of the input event.
        /// </summary>
        public InputType Type;
        /// <summary>
        /// The information about a simulated mouse, keyboard or hardware event.
        /// </summary>
        public InputUnion Ui;


        /// <summary>
        /// Initialize all members.
        /// </summary>
        /// <param name="type">The type of the input event.</param>
        /// <param name="ui">The information about a simulated mouse, keyboard or hardware event.</param>
        public Input(InputType type, InputUnion ui)
        {
            Type = type;
            Ui = ui;
        }


        /// <summary>
        /// Create <see cref="Input"/> with <see cref="InputUnion"/> which contains <see cref="MouseInput"/>.
        /// </summary>
        /// <param name="flags">A set of bit flags that specify various aspects of mouse motion and button clicks.</param>
        /// <param name="x">The absolute position of the mouse, or the amount of motion since the last mouse event was generated, depending on the value of the <paramref name="flags"/>.</param>
        /// <param name="y">The absolute position of the mouse, or the amount of motion since the last mouse event was generated, depending on the value of the <paramref name="flags"/>.</param>
        /// <param name="data">A mouse data depends on <paramref name="flags"/>.</param>
        /// <param name="time">The time stamp for the event, in milliseconds.</param>
        /// <param name="extraInfo">An additional value associated with the mouse event.</param>
        /// <returns><see cref="Input"/> associated with mouse event.</returns>
        public static Input CreateMouseInput(MouseEventFlags flags, int x = 0, int y = 0, int data = 0, int time = 0, IntPtr extraInfo = default)
        {
            return new Input(InputType.Mouse, InputUnion.CreateMouseInput(flags, x, y, data, time, extraInfo));
        }

        /// <summary>
        /// Create <see cref="Input"/> with <see cref="InputUnion"/> which contains <see cref="KeyboardInput"/>.
        /// </summary>
        /// <param name="flags"></param>
        /// <param name="virtualKey"></param>
        /// <param name="scanCode"></param>
        /// <param name="time"></param>
        /// <param name="extraInfo"></param>
        /// <returns><see cref="Input"/> associated with keyboard event.</returns>
        public static Input CreateKeyboardInput(KeyEventFlags flags, short virtualKey = 0, short scanCode = 0, int time = 0, IntPtr extraInfo = default)
        {
            return new Input(InputType.Keyboard, InputUnion.CreateKeyboardInput(flags, virtualKey, scanCode, time, extraInfo));
        }

        /// <summary>
        /// Create <see cref="Input"/> with <see cref="InputUnion"/> which contains <see cref="HardwareInput"/>.
        /// </summary>
        /// <param name="message">The message generated by the input hardware.</param>
        /// <param name="paramL">The low-order word of the lParam parameter for <see cref="Message"/>.</param>
        /// <param name="paramH">The high-order word of the lParam parameter for <see cref="Message"/>.</param>
        /// <returns><see cref="Input"/> associated with hardware event.</returns>
        public static Input CreateHardwareInput(int message, short paramL, short paramH)
        {
            return new Input(InputType.Hardware, InputUnion.CreateHardwareInput(message, paramL, paramH));
        }
    }
}