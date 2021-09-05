using DigBuild.Engine.Render;

namespace DigBuild.Engine.Ui
{
    /// <summary>
    /// A user interface text renderer.
    /// </summary>
    public interface ITextRenderer
    {
        /// <summary>
        /// Gets the width of a character
        /// </summary>
        /// <param name="c">The character</param>
        /// <returns>The character's width</returns>
        uint GetWidth(char c);

        /// <summary>
        /// Draws a line of text.
        /// </summary>
        /// <param name="buffer">The geometry buffer</param>
        /// <param name="text">The text</param>
        /// <param name="scale">The scaling factor</param>
        /// <param name="yellow">Whether the text should be yellow</param>
        /// <returns>The length of the line</returns>
        uint DrawLine(IGeometryBuffer buffer, string text, uint scale = 1, bool yellow = false);
    }
}