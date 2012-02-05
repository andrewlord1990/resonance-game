using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Resonance {
    class ArmourShatter : Emitter {

        // The direction in which the armour layer is being blasted.
        Vector3 blastVec;
        float radius = 3f;

        public ArmourShatter(Vector3 p, Vector3 blast) : base(p) {
            pTex = ParticleEmitterManager.Content.Load<Texture2D>("Drawing/Textures/texTriangle");
            blastVec = blast;
            emissionsPerUpdate = 300;
            particlesLeft = 300;
            maxParticleSpd = 0.6f;
            maxParticleLife = 8;
            iPSize = 0.5f;
        }

        /// <summary>
        /// Override generateParticles as the blast vector will have to come into play.
        /// </summary>
        protected override void generateParticles() {
            if (particlesLeft > 0) {
                for (int i = 0; i < emissionsPerUpdate; ++i) {
                    Vector3 iDir = blastVec;
                    if (gen.Next() % 2 == 0) iDir.X += (float) (gen.NextDouble() * radius); else iDir.X += (float) (gen.NextDouble() * radius);
                    if (gen.Next() % 2 == 0) iDir.Y += (float) (gen.NextDouble() * radius); else iDir.Y += (float) (gen.NextDouble() * radius);
                    if (gen.Next() % 2 == 0) iDir.Z += (float) (gen.NextDouble() * radius); else iDir.Z += (float) (gen.NextDouble() * radius);

                    iDir.Normalize();

                    float iSpd  = (float) gen.NextDouble() * maxParticleSpd;
                    int   iLife = gen.Next(maxParticleLife);
                    particles.Add(new Particle(pos, iDir, iSpd, iPSize, iLife, Color.White));
                    particlesLeft--;

                    if (particlesLeft <= 0) break;
                }
            }
        }
    }
}