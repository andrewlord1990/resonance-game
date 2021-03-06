using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Resonance
{
    class CustomFontContentReader : ContentTypeReader<ImportedCustomFont>
    {
        protected override ImportedCustomFont Read(ContentReader input, ImportedCustomFont existingInstance)
        {
            ImportedCustomFont font = new ImportedCustomFont();
            int count = input.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                char character = input.ReadChar();
                Texture2D texture = input.ReadObject<Texture2D>();
                font.add(character, texture);
            }

            return font;
        }
    }
}
