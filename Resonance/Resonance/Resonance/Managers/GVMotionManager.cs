using System;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using BEPUphysics.Constraints.SingleEntity;
using BEPUphysics.Constraints.TwoEntity.Motors;

namespace Resonance {
    /// <summary>
    /// Handles and provides an interface between button presses and Good Vibe motion.
    /// </summary>
    class GVMotionManager {
        public static readonly float DEFAULT_Z_ACCELERATION          =   150f / ResonanceGame.FPS;
        public static float          DEFAULT_MAX_Z_SPEED             =   1800f / ResonanceGame.FPS;
        public static float          MAX_R_SPEED                     =   4.5f / ResonanceGame.FPS;
        public static float          MAX_X_SPEED                     =   1800f / ResonanceGame.FPS;
        public static float          MAX_Z_SPEED                     =   720f / ResonanceGame.FPS;
        public static float          R_ACCELERATION                  =  0.45f / ResonanceGame.FPS;
        public static float          R_DECELERATION                  =  0.45f / ResonanceGame.FPS;
        public static float          X_ACCELERATION                  =   48f / ResonanceGame.FPS;
        public static float          X_DECELERATION                  =   84f / ResonanceGame.FPS;
        public static float          Z_ACCELERATION                  =   150f / ResonanceGame.FPS;
        public static float          Z_DECELERATION                  =   60f / ResonanceGame.FPS;
        public static float          R_SPEED                         =   0.00f;
        public static float          X_SPEED                         =   0.00f;
        public static float          Z_SPEED                         =   0.00f;
        public static float          BOOST_POWER                     =   2.00f;
        public static bool           BOOSTING                        =   false;

        public static float          BANK_ANGLE                      = 0f;
        public const  float          MAX_BANK_ANGLE                  = (float) (Math.PI / 8d);
        public const  float          MAX_BANK_SPEED                  = MAX_BANK_ANGLE / 8f;

        public static float          PITCH_ANGLE                      = 0f;
        public const  float          MAX_PITCH_ANGLE                  = (float) (Math.PI / 16d);
        public const  float          MAX_PITCH_SPEED                  = MAX_PITCH_ANGLE / 8f;

        public static bool           FLIP_REVERSE_CONTROLS            = false;

        //private static bool MOVING_F         = false;
        //private static bool MOVING_B         = false;


        private static GoodVibe gv;

        private static SingleEntityAngularMotor servo;
        private static LinearAxisMotor           lamZ;
        private static LinearAxisMotor           lamX;

        public static bool initialised = false;

        // On the last iterateion, did direction change?
        private static bool prevRL  = false;
        private static bool prevRR  = false;
        private static bool rChange = false;
        private static bool prevXL  = false;
        private static bool prevXR  = false;
        private static bool xChange = false;

        /// Methods

        public static void init() {
            gv = GameScreen.getGV();

            gv.Body.Material.KineticFriction *= 2f;
            gv.Body.Material.StaticFriction  *= 2f;

            // ADD SERVO TO GV
            servo = new SingleEntityAngularMotor(gv.Body);

            servo.Settings.Mode = MotorMode.Servomechanism;

            servo.Settings.Servo.SpringSettings.DampingConstant   *= 10f;
            servo.Settings.Servo.SpringSettings.StiffnessConstant *= 100f;

            ScreenManager.game.World.addToSpace(servo);


            // ADD LINEARAXISMOTORs TO GV
            lamZ               = new LinearAxisMotor();
            lamZ.Settings.Mode = MotorMode.VelocityMotor;
            lamZ.Settings.VelocityMotor.Softness *= 1000f;
            lamZ.ConnectionA   = null;
            lamZ.ConnectionB   = gv.Body;
            lamZ.Axis          = new Vector3(0f, 0f, 1f);
            lamZ.IsActive      = true;
            ScreenManager.game.World.addToSpace(lamZ);

            lamX               = new LinearAxisMotor();
            lamX.Settings.Mode = MotorMode.VelocityMotor;
            lamX.Settings.VelocityMotor.Softness *= 1000f;
            lamX.ConnectionA   = null;
            lamX.ConnectionB   = gv.Body;
            lamX.Axis          = new Vector3(0f, 0f, 1f);
            lamX.IsActive      = true;
            ScreenManager.game.World.addToSpace(lamX);

            
            switch (GameScreen.DIFFICULTY)
            {
                case GameScreen.BEGINNER:
                    {
                        DEFAULT_MAX_Z_SPEED = 1800f / ResonanceGame.FPS;
                        break;
                    }
                case GameScreen.EASY:
                    {
                        DEFAULT_MAX_Z_SPEED = 2240f / ResonanceGame.FPS;
                        break;
                    }
                case GameScreen.MEDIUM:
                    {
                        DEFAULT_MAX_Z_SPEED = 2680f / ResonanceGame.FPS;
                        break;
                    }
                case GameScreen.HARD:
                    {
                        DEFAULT_MAX_Z_SPEED = 3120f / ResonanceGame.FPS;
                        break;
                    }
                case GameScreen.EXPERT:
                    {
                        DEFAULT_MAX_Z_SPEED = 3560f / ResonanceGame.FPS;
                        break;
                    }
                case GameScreen.INSANE:
                    {
                        DEFAULT_MAX_Z_SPEED = 4000f / ResonanceGame.FPS;
                        break;
                    }
            }

            // INITIALISATION COMPLETE
            initialised = true;
        }

        public static void setGVRef(GoodVibe newGV) {
            gv = newGV;
        }

        private static void rotate(float power) {
            float inc = -power * R_ACCELERATION;

            float posInc = inc;
            float posSpd = R_SPEED;
            if (posInc < 0) posInc *= -1;
            if (posSpd < 0) posSpd *= -1;

            float max = power * MAX_R_SPEED;
            if (max < 0) max *= -1;
            if (max > MAX_R_SPEED) max = MAX_R_SPEED;

            if (posSpd + posInc < max) {
                R_SPEED += inc;
            }

            Quaternion cAng = gv.Body.Orientation;
            Quaternion dAng = Quaternion.CreateFromAxisAngle(Vector3.Up, R_SPEED);
            Quaternion eAng = Quaternion.Concatenate(cAng, dAng);

            servo.Settings.Servo.Goal = eAng;
        }

        public static void boost() {
            Z_ACCELERATION = DEFAULT_Z_ACCELERATION * BOOST_POWER;
            MAX_Z_SPEED    = DEFAULT_MAX_Z_SPEED    * BOOST_POWER;
        }

        public static void resetBoost() {
            Z_ACCELERATION = DEFAULT_Z_ACCELERATION;
            MAX_Z_SPEED    = DEFAULT_MAX_Z_SPEED;
            BOOSTING       = false;
        }

        private static void move(LinearAxisMotor lam, float power) {
            bool zMode = (lam.Equals(lamZ));

            float acc    = 0;
            float spd    = 0;
            float maxSpd = 0;

            if (zMode) { acc = Z_ACCELERATION; spd = Z_SPEED; maxSpd = MAX_Z_SPEED; }
                  else { acc = X_ACCELERATION; spd = X_SPEED; maxSpd = MAX_X_SPEED; }

            float inc = power * acc;

            float posInc = inc;
            float posSpd = spd;
            float max = power * maxSpd;

            if (Math.Abs(PITCH_ANGLE - power * MAX_PITCH_SPEED) <= MAX_PITCH_ANGLE) PITCH_ANGLE -= power * MAX_PITCH_SPEED;

            if (posInc < 0) posInc *= -1;
            if (posSpd < 0) posSpd *= -1;
            if (max < 0) max *= -1;
            if (max > maxSpd) max = maxSpd;
            if (posSpd + posInc < max) {
                spd += inc;
                if (zMode) { Z_SPEED = spd; } else { X_SPEED = spd; }
            }

            Vector3 oVector = Utility.QuaternionToEuler(gv.Body.Orientation);
            Vector3 vel     = gv.Body.LinearVelocity;
            Vector3 d = Vector3.Zero;

            if (zMode) d = gv.Body.OrientationMatrix.Forward; else d = gv.Body.OrientationMatrix.Left;

            lam.Axis = d;
            lam.Settings.VelocityMotor.GoalVelocity = spd;
        }

        /// <summary>
        /// Takes the state of GV motion input devices and moves the GV based on these.
        /// </summary>
        /// <param name="kbd"> Current keyboard state. </param>
        /// <param name="pad"> Current game pad state. </param>
        public static void input(KeyboardState kbd, GamePadState pad)
        {
            if (!initialised) {
                init();
            }

            // Analogue stick positions
            float leftX = pad.ThumbSticks.Left.X;
            float leftY = pad.ThumbSticks.Left.Y;
            float rightX = pad.ThumbSticks.Right.X;
            float rightY = pad.ThumbSticks.Right.Y;
            float leftL = (float)Math.Sqrt(Math.Pow(leftX, 2) + Math.Pow(leftX, 2));
            float rightL = (float)Math.Sqrt(Math.Pow(rightX, 2) + Math.Pow(rightX, 2));

            bool  forward  = kbd.IsKeyDown(Keys.Up)    || (pad.DPad.Up    == ButtonState.Pressed) || leftY  > 0 || BOOSTING || ((ScreenManager.game.getMode().MODE == GameMode.OBJECTIVES) && (ObjectiveManager.currentObjective() == ObjectiveManager.TERRITORIES));
            bool  backward = ((kbd.IsKeyDown(Keys.Down)  || (pad.DPad.Down  == ButtonState.Pressed) || leftY  < 0) && (!(ScreenManager.game.getMode().MODE == GameMode.OBJECTIVES) || !(ObjectiveManager.currentObjective() == ObjectiveManager.TERRITORIES)));
            bool  rotateL  = kbd.IsKeyDown(Keys.Left)  || (pad.DPad.Left  == ButtonState.Pressed) || rightX < 0;
            bool  rotateR  = kbd.IsKeyDown(Keys.Right) || (pad.DPad.Right == ButtonState.Pressed) || rightX > 0;

            bool  strafeL  = kbd.IsKeyDown(Keys.OemComma)  || leftX < 0;
            bool  strafeR  = kbd.IsKeyDown(Keys.OemPeriod) || leftX > 0;

            bool chargeNitro   = kbd.IsKeyDown(Keys.D1); //TODO: change to non combat drum pattern
            bool nitro         = kbd.IsKeyDown(Keys.D2);
            bool chargeShield  = kbd.IsKeyDown(Keys.D3); //TODO: change to non combat drum pattern
            bool shield        = kbd.IsKeyDown(Keys.D4);
            bool chargeFreeze  = kbd.IsKeyDown(Keys.D5); //TODO: change to non combat drum pattern
            bool freeze        = kbd.IsKeyDown(Keys.D6);
            bool chargeDeflect = kbd.IsKeyDown(Keys.D7);

            // Trigger positions
            float lTrig = pad.Triggers.Left;
            float rTrig = pad.Triggers.Right; //TODO: use boost with right trigger

            bool posR = false;

            // Rotate GV based on keyboard / dPad
            if (rotateL ^ rotateR && !rChange) {
                float power;
                if (rightX != 0) power = rightX; else power = 1f;
                if (power < 0) power *= -1;

                //power = (float) Math.Sin(power * (Math.PI / 2));

                if (FLIP_REVERSE_CONTROLS) posR = (rotateR ^ backward); /*^ !FLIP_REVERSE_CONTROLS;*/ else posR = rotateR;

                if (posR) {
                    if (prevRR) rChange = true; else rChange = false;
                    rotate(power);
                    prevRR = false;
                    prevRL = true;
                } else {
                    if (prevRL) rChange = true; else rChange = false;
                    rotate(-power);
                    prevRR = true;
                    prevRL = false;
                }
            } else {
                rChange = false;
                if (R_SPEED > 0) if (R_DECELERATION > R_SPEED)  R_SPEED = 0f; else R_SPEED -= R_DECELERATION;
                if (R_SPEED < 0) if (R_DECELERATION > -R_SPEED) R_SPEED = 0f; else R_SPEED += R_DECELERATION;
                rotate(0f);
            }

            if ((rotateL ^ rotateR && !rChange) || (strafeL ^ strafeR && !xChange)) {
                float power;
                if (rightX != 0) power = rightX; else power = 1f;
                if (power < 0) power *= -1;
                if (!rotateR) power *= -1;

                if (!(rotateL ^ rotateR)) {
                    if (leftX != 0) power = leftX; else power = 1f;
                    if (power < 0) power *= -1;
                    if (strafeL) power *= -1;
                }
                
                if (Math.Abs(BANK_ANGLE - power * MAX_BANK_SPEED) <= MAX_BANK_ANGLE) BANK_ANGLE -= power * MAX_BANK_SPEED;
            }

            if (!(rotateL ^ rotateR) && !(strafeL ^ strafeR) && BANK_ANGLE != 0) {
                if (Math.Abs(BANK_ANGLE) - MAX_BANK_SPEED < 0) {
                    BANK_ANGLE = 0f;
                } else {
                    if (BANK_ANGLE < 0) {
                        BANK_ANGLE += MAX_BANK_SPEED;
                    } else {
                        BANK_ANGLE -= MAX_BANK_SPEED;
                    }
                }
            }

            if (BOOSTING && gv.Nitro > 0) {
                boost();
            } else {
                resetBoost();
            }

            // Move forward / backward based on keyboard / dPad
            if (forward ^ backward) {
                float power;
                if (leftY != 0) power = leftY; else power = 1f;
                if (power < 0) power *= -1;

                //power = (float) Math.Sin(power * (Math.PI / 2));

                if (forward) {
                    move(lamZ,  power);
                } else {
                    move(lamZ, -power);
                }
            }

            if (!(forward ^ backward) || !BOOSTING) {
                if (Z_SPEED > 0) if (Z_DECELERATION > Z_SPEED)  Z_SPEED = 0f; else Z_SPEED -= Z_DECELERATION;
                if (Z_SPEED < 0) if (Z_DECELERATION > -Z_SPEED) Z_SPEED = 0f; else Z_SPEED += Z_DECELERATION;
                move(lamZ, 0f);
            }

            if (!(forward ^ backward) && !BOOSTING && (PITCH_ANGLE != 0)) {
                if (Math.Abs(PITCH_ANGLE) - MAX_PITCH_SPEED < 0) {
                    PITCH_ANGLE = 0f;
                } else {
                    if (PITCH_ANGLE < 0) {
                        PITCH_ANGLE += MAX_PITCH_SPEED;
                    } else {
                        PITCH_ANGLE -= MAX_PITCH_SPEED;
                    }
                }
            }

            // Strafe based on keyboard.
            if (strafeL ^ strafeR && !xChange) {
                float power;
                if (leftX != 0) power = leftX; else power = 1f;
                if (power < 0) power *= -1;

                //power = (float) Math.Sin(power * (Math.PI / 2));

                if (strafeL) {
                    if (prevXR) xChange = true; else xChange = false;
                    move(lamX,  power);
                    prevXL = true;
                    prevXR = false;
                } else {
                    if (prevXL) xChange = true; else xChange = false;
                    move(lamX, -power);
                    prevXL = false;
                    prevXR = true;
                }
            } else {
                xChange = false;
                if (X_SPEED > 0) if (X_DECELERATION > X_SPEED)  X_SPEED = 0f; else X_SPEED -= X_DECELERATION;
                if (X_SPEED < 0) if (X_DECELERATION > -X_SPEED) X_SPEED = 0f; else X_SPEED += X_DECELERATION;
                move(lamX, 0f);
            }

            //Charge speed boost when not in combat
            if (!gv.InCombat && chargeNitro) {
                gv.adjustNitro(1);
            }

            //Charge shield when not in combat
            if (!gv.InCombat && chargeShield) {
                gv.adjustShield(1);
            }

            //Use shield
            if (shield && (gv.Shield > 0)) {
                gv.adjustShield(-1);
            }

            //Charge freeze when not in combat
            if (!gv.InCombat && chargeFreeze) {
                gv.adjustFreeze(1);
            }

            //Charge deflect power
            if (chargeDeflect) {
                gv.adjustDeflectShield(1);
            }
        }
    }
}
