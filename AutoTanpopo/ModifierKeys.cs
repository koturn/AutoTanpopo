using System;


namespace AutoTanpopo
{
    /// <summary>
    /// Modifier Key falgs.
    /// </summary>
    /// <remarks>
    /// <seealso href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-registerhotkey"/>
    /// </remarks>
    [Flags]
    public enum ModifierKeys : int
    {
        /// <summary>
        /// No modifier key.
        /// </summary>
        None = 0x0000,
        /// <summary>
        /// Either ALT key must be held down.
        /// </summary>
        Alt = 0x0001,
        /// <summary>
        /// Either CTRL key must be held down.
        /// </summary>
        Control = 0x0002,
        /// <summary>
        /// Either SHIFT key must be held down.
        /// </summary>
        Shift = 0x0004,
        /// <summary>
        /// Either WINDOWS key was held down.
        /// These keys are labeled with the Windows logo.
        /// Keyboard shortcuts that involve the WINDOWS key are reserved for use by the operating system.
        /// </summary>
        Windows = 0x0008,
        /// <summary>
        /// <para>Changes the hotkey behavior so that the keyboard auto-repeat does not yield multiple hotkey notifications.</para>
        /// <para>Windows Vista: This flag is not supported.</para>
        /// </summary>
        NoRepeat = 0x4000
    }
}
