using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Resonance {
    class BulletImpact : Emitter {
        // The direction in which the effect is acting, I.E. away from the thing being hit.
        Vector3 blastVec;

        // The 'spread' of the particles in the blast.
        float radius = 300f;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="p">     The position of the emitter (in this case, the bad vibe) </param>
        /// <param name="blast"> The direction of the blast (away from the shockwave.)    </param>
        /// <param name="c">     The initial colour.                                      </param>
        /// <param name="v">     Bad Vibe reference.                                      </param>
        public BulletImpact() : base() {
            particlesLeft = 40;
            particles     = new List<Particle>(particlesLeft);
        }

        // p = position.
        // Blast is the direction of the blast. When initialising, pass bulletPos - targetPos
        public void init(Vector3 p, Vector3 blast) {
            pos                = p;
            pTex               = ParticleEmitterManager.TEX_TRIANGLE;
            blastVec           = blast;
            emissionsPerUpdate = 15; //200
            particlesLeft      = 40; //200

#if XBOX
            particlesLeft      = (int) ((float) particlesLeft      * ParticleEmitterManager.XBOX_PARTICLE_COEFFICIENT);
            emissionsPerUpdate = (int) ((float) emissionsPerUpdate * ParticleEmitterManager.XBOX_PARTICLE_COEFFICIENT);
#endif

            maxParticleSpd     = 2f;
            maxParticleLife    = 20;
            iPSize             = 0.4f;
            iColour            = Color.LightYellow;
            deceleration       = 0.025f;

            ParticleEmitterManager.addEmitter(this);
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
                    //iDir.Y += 3f;
                    iDir.Normalize();

                    float iSpd  = maxParticleSpd;//(float) gen.NextDouble() * maxParticleSpd;
                    int   iLife = gen.Next(maxParticleLife);

                    // Add an offset to the position so the particles start off around the BV as opposed to it's centre.
                    //float bVRad = 0.5f;//bVRef.calculateMinBBEdge() / 2;
                    //Vector3 posOffset = new Vector3((float) gen.NextDouble(), (float) gen.NextDouble(), (float) gen.NextDouble());
                    //if (gen.Next() % 2 == 0) posOffset.X *= -1;
                    //if (gen.Next() % 2 == 0) posOffset.Y *= -1; 
                    //if (gen.Next() % 2 == 0) posOffset.Z *= -1; 

                    //posOffset.Normalize();
                    //posOffset *= bVRad;
                    //pos += posOffset;

                    Vector3 gravity = new Vector3(0f, -0.1f, 0f);

                    Particle p = ParticleEmitterManager.getParticle();
                    p.init(pos, iDir, iSpd, iPSize, iLife, iColour, 1f, gravity, deceleration, true);
                    p.setSpin(new Vector3((float) gen.NextDouble(),(float) gen.NextDouble(),(float) gen.NextDouble()));
                    particles.Add(p);

                    particlesLeft--;

                    if (particlesLeft <= 0) break; 
                }
            }
        }
    }
}
