/*
 *  From: http://www.craftworkgames.com/blog/tutorial-bmfont-rendering-with-monogame/
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace SSD
{
    public class FontRenderer
    {
        public FontRenderer(FontFile fontFile, Texture2D fontTexture)
        {
            _fontFile = fontFile;
            _texture = fontTexture;
            _characterMap = new Dictionary<char, FontChar>();

            foreach (var fontCharacter in _fontFile.Chars)
            {
                char c = (char)fontCharacter.ID;
                _characterMap.Add(c, fontCharacter);
            }
        }

        private Dictionary<char, FontChar> _characterMap;
        private FontFile _fontFile;
        private Texture2D _texture;
        public void DrawText(SpriteBatch spriteBatch, Vector2 textPosition, string text, float scale = 1.0f, Color? textColor = null, bool drawRightToLeft = false)
        {
            int dx = (int)textPosition.X;
            int dy = (int)textPosition.Y;
            //text = text.ToUpper(); //Use if font has same chars for upper+lower
            Color fontColour = (Color)(textColor.HasValue ? textColor : Color.White);

            if (drawRightToLeft)
            {
                text = Reverse(text);
            }

            foreach (char c in text)
            {
                FontChar fc;
                if (_characterMap.TryGetValue(c, out fc))
                {
                    if (drawRightToLeft)
                    {
                        dx -= (int)(fc.XAdvance * scale);
                    }

                    var sourceRectangle = new Rectangle(fc.X, fc.Y, fc.Width, fc.Height);
                    var position = new Vector2(dx + fc.XOffset, dy + fc.YOffset);

                    //Allows text to be scaled
                    var destinationRectangle = new Rectangle(dx, dy, (int)(fc.Width * scale), (int)(fc.Height * scale));

                    spriteBatch.Draw(_texture, destinationRectangle, sourceRectangle, fontColour);
                    if (!drawRightToLeft)
                    {
                        dx += (int)(fc.XAdvance * scale);
                    }
                }
            }
        }

        //Modification of above to simply return text width
        public int TextWidth(string text, float scale = 1.0f)
        {
            int totalWidth = 0;

            foreach (char c in text)
            {
                FontChar fc;
                if (_characterMap.TryGetValue(c, out fc))
                {
                    totalWidth += (int)(fc.XAdvance * scale);
                }
            }

            return totalWidth;
        }

        //From: http://stackoverflow.com/questions/228038/best-way-to-reverse-a-string
        public static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
    }
}
