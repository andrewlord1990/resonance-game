using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Resonance {
    class Rain : Emitter {

        Random gen;

        public Rain() : base() {
            particles = new List<Particle>(1000);
            gen = new Random();
        }

        public override void init(Vector3 p) {
            pos = p;

            pTex = ParticleEmitterManager.TEX_DIST_BV;

            emissionsPerUpdate = 0;
            particlesLeft      = 5000; // This will always be reset anyway as there is (potentially) infinite rain.
            maxParticleSpd     = 2f;
            maxParticleLife    = 1000; // Particles are removed after they hit the ground.
            iPSize             = 0f;
            iColour            = new Color(0.94f, 1f, 1f, 0.3f);
            deceleration       = 0f;

            ParticleEmitterManager.addEmitter(this);
        }

        public void setEmissionsPerUpdate(int ePU) {
            emissionsPerUpdate = ePU;
        }

        public void setRaindropSize(float s) {
            iPSize = s;
        }

        protected override void generateParticles() {
            int emissions = 0;
#if XBOX
            emissions = (int) ((float) emissionsPerUpdate * ParticleEmitterManager.XBOX_PARTICLE_COEFFICIENT);
#endif
#if WINDOWS
            emissions = emissionsPerUpdate;
#endif
            for (int i = 0; i < emissions; ++i) {
                Vector3 iDir = Vector3.Down;

                // These only have to represent the area of screen you can see at one time, as Rain follows the GV
                float mapW = 40f;
                float mapD = 40f;

                float iSpd  = maxParticleSpd;
                int   iLife = maxParticleLife;

                Vector3 posOffset = new Vector3((float) gen.Next((int) (mapW / 2)), 0f, (float) gen.Next((int) (mapD / 2)));
                if (gen.Next() % 2 == 0) posOffset.X *= -1;
                if (gen.Next() % 2 == 0) posOffset.Z *= -1;

                Vector3 gravity = new Vector3(0f, -0.3f, 0f);
                //gravity.X += 0.2f;

                Particle p = ParticleEmitterManager.getParticle();
                p.init(pos + posOffset, gravity, iSpd, iPSize, iLife, iColour, 1f, gravity, deceleration, false);
                p.setDieOnFloor(true);
                p.setSpin(Vector3.Zero);
                particles.Add(p);
            }
        }
    }
}
