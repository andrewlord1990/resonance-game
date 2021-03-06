using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Resonance {
    class Explosion : Emitter {

        public Explosion()
            : base() {
            particlesLeft      = 100;// 500;
            particles = new List<Particle>(particlesLeft);
        }

        public override void init(Vector3 p) {
            pos = p;
            particlesLeft      = 80;// 500;
            emissionsPerUpdate = 80;// 500;

#if XBOX
            particlesLeft      = (int) ((float) particlesLeft      * ParticleEmitterManager.XBOX_PARTICLE_COEFFICIENT);
            emissionsPerUpdate = (int) ((float) emissionsPerUpdate * ParticleEmitterManager.XBOX_PARTICLE_COEFFICIENT);
#endif

            maxParticleSpd     = 0.9f;
            maxParticleLife    = 30;
            iColour            = Color.White;
            iPSize             = 0.1f;

            ParticleEmitterManager.addEmitter(this);
        }

        protected override void generateParticles() {
            if (particlesLeft > 0) {
                for (int i = 0; i < emissionsPerUpdate; ++i) {
                    //Vector3 iDir = new Vector3((float)gen.NextDouble(), (float)gen.NextDouble(), (float)gen.NextDouble());
                    Vector3 iDir = Vector3.Up;
                    Vector3 offset = new Vector3((float) gen.NextDouble() / 2f, (float) gen.NextDouble() / 2f, (float) gen.NextDouble() / 2f);

                    if (gen.Next() % 2 == 0) offset.X *= -1;
                    if (gen.Next() % 2 == 0) offset.Z *= -1;
                    iDir += offset;
                    iDir.Normalize();

                    float iSpd  = (float) gen.NextDouble() * maxParticleSpd;
                    int   iLife = gen.Next(maxParticleLife);

                    Vector3 gravity = new Vector3(0f, -0.05f, 0f);

                    if (gen.Next() % 4 == 0) { 
                        Particle p = ParticleEmitterManager.getParticle();
                        p.init(pos, iDir, iSpd, iPSize, iLife, iColour, 1f, gravity, deceleration, true);
                        particles.Add(p);
                    } else {
                        Particle p = ParticleEmitterManager.getParticle();
                        p.init(pos, iDir, iSpd, iPSize, iLife, Color.Black, 1f, gravity, deceleration, true);
                        particles.Add(p);
                    }
                    particlesLeft--;

                    if (particlesLeft <= 0) break;
                }
            }
        }
    }
}
