using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Resonance
{
    class ParticleShader : Shader
    {
        public ParticleShader(string file) : base(file, false) { }
        private Color currentColour;

        public void setColour(Color colour)
        {
            if (currentColour != colour)
            {
                currentColour = colour;
                Vector4 vc = colour.ToVector4();
                float[] c = { vc.X * vc.W, vc.Y * vc.W, vc.Z * vc.W, vc.W };
                Effect.Parameters["Colour"].SetValue(c);
            }
        }
    }
}
