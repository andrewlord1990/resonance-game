using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Resonance {

    /// <summary>
    /// Class which represents a single particle of a particle system.
    /// </summary>
    class Particle {

        /// Fields
        Vector3 pos;
        float speed;
        Vector3 direction; // Normalised direction of movement vector.
        int lifespan; // Number of frames for which this particle is alive.
        Color colour;
        float size;
        Vector3 rotation;
        Vector3 spin;
        Vector3 force;
        float deceleration;
        bool bounce;

        float dAlpha;


        /// Properties

        public bool isDead() {
            return (lifespan <= 0);
        }

        public float getSize() {
            return size;
        }

        public Vector3 getPos() {
            return pos;
        }

        public Vector3 getRotation() {
            return rotation;
        }

        public Color getColour() {
            return colour;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="p">          Initial particle position.               </param>
        /// <param name="iDirection"> Initial direction of travel.             </param>
        /// <param name="iSpeed">     Initial speed.                           </param>
        /// <param name="s">          Initial size.                            </param>
        /// <param name="iLife">      Lifespan (in frames).                    </param>
        /// <param name="c">          Initial colour.                          </param>
        /// <param name="iAlpha">     Initial alpha transparency.              </param>
        /// <param name="f">          Constantly applied force (e.g. gravity). </param>
        public Particle(Vector3 p, Vector3 iDirection, float iSpeed, float s, int iLife, Color c, float iAlpha, Vector3 f, float d, bool b) {
            speed        = iSpeed;
            pos          = p;
            direction    = iDirection;
            size         = s;
            lifespan     = iLife;
            colour       = c;
            dAlpha       = iAlpha / iLife;
            rotation     = Vector3.Zero;
            spin         = new Vector3(1f, 1f, 1f);
            force        = f;
            deceleration = d;
            bounce       = b;
        }

        public Particle() {
        }

        public void init(Vector3 p, Vector3 iDirection, float iSpeed, float s, int iLife, Color c, float iAlpha, Vector3 f, float d, bool b) {
            speed        = iSpeed;
            pos          = p;
            direction    = iDirection;
            size         = s;
            lifespan     = iLife;
            colour       = c;
            dAlpha       = iAlpha / iLife;
            rotation     = Vector3.Zero;
            spin         = new Vector3(1f, 1f, 1f);
            force        = f;
            deceleration = d;
            bounce       = b;
        }


        // s only used for testing
        public void update() {
            pos += (direction * speed);
            rotation += spin;
            colour = new Color(colour.R / 255f, colour.G / 255f, colour.B / 255f, (colour.A / 255f) - dAlpha);
            lifespan--;
            
            if (force != Vector3.Zero) {
                direction += force;
            }

            if ((deceleration != 0) && (speed > deceleration)) {
                speed -= deceleration;
            }

            if (bounce && (pos.Y <= 0)) direction.Y *= -1;
        }
    }
}