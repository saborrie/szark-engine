/*
	OpacityMode.cs
        By: Jakub P. Szarkowicz / JakubSzark
*/

namespace PGE
{
    /// <summary>
    /// The Alpha 'Blending' Modes
    /// Normal - Alpha has no Affect
    /// Mask - Any Alpha below 255 doesn't get rendered
    /// Alpha - 'Proper' Alpha Blending
    /// </summary>
    public enum OpacityMode
    {
        NORMAL,
        MASK,
        ALPHA
    }
}