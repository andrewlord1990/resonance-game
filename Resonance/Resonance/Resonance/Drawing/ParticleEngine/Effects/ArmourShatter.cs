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
        float radius = 2f;
        BadVibe bVRef;

        public ArmourShatter(Vector3 p, Vector3 blast, Color c, BadVibe v) : base(p) {
            pTex = ParticleEmitterManager.Content.Load<Texture2D>("Drawing/Textures/texTriangle");
            bVRef = v;
            blastVec = blast;
            emissionsPerUpdate = 100;
            particlesLeft = 100;
            maxParticleSpd = 1.2f;
            maxParticleLife = 70;
            iPSize = 0.4f;
            iColour = c;
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

                    float iSpd  = maxParticleSpd;//(float) gen.NextDouble() * maxParticleSpd;
                    int   iLife = gen.Next(maxParticleLife);

                    // Add an offset to the position so the particles start off around the BV as opposed to it's centre.
                    float bVRad = 0.1f;//bVRef.calculateMinBBEdge() / 2;
                    Vector3 posOffset = new Vector3((float) gen.NextDouble(), (float) gen.NextDouble(), (float) gen.NextDouble());
                    if (gen.Next() % 2 == 0) posOffset.X *= -1;
                    if (gen.Next() % 2 == 0) posOffset.Y *= -1; 
                    if (gen.Next() % 2 == 0) posOffset.Z *= -1; 

                    posOffset.Normalize();
                    posOffset *= bVRad;
                    pos += posOffset;

                    Vector3 gravity = new Vector3(0f, -0.1f, 0f);

                    if (gen.Next() % 5 == 0) {
                        particles.Add(new Particle(pos, iDir, iSpd, iPSize, iLife, iColour, 1f, gravity));
                    } else {
                        particles.Add(new Particle(pos, iDir, iSpd, iPSize, iLife, Color.Black, 1f, gravity));
                    }
                    particlesLeft--;

                    if (particlesLeft <= 0) break;
                }
            }
        }
    }
}
