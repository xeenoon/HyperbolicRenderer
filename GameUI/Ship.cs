using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.MediaFoundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameUI
{
    public class Ship : Sprite
    {
        public double rotation = 0;
        public double _rotationSpeed = 3f;
        public double maxspeed = 600;
        public double speed;
        public float acceleration = 150f;
        public bool forcestop;
        public bool wasmoving = false;
        public Collider collider;
        public bool boostable = true;
        public bool disappear;
        public Ship(Texture2D tex, Vector2 pos) : base(tex, pos)
        {
        }

        public virtual bool OnCollision(string tag)
        {
            return false;
        }

        public virtual void Update()
        {
            
        }

        protected void AutoDecelerate(double distancetoend)
        {
            //v^2 = u^2 + 2as
            //0 = speed^2 + 2*-150*4*distance
            //distance = ((speed^2))/1200
            if (distancetoend <= speed * speed / 1200)
            {
                if (engineEmitData != null)
                {
                    ParticleManager.particleEmitters.RemoveRange(engineEmitData.enginehandlers);
                }
                wasmoving = true;
                forcestop = true;
            }
            else if (distancetoend >= 5) //Dont restart if we just finished stopping
            {
                wasmoving = false;
                forcestop = false;
            }
        }
        public EngineEmitData engineEmitData;
        public List<Vector2> emitpositions = new List<Vector2>() { new Vector2(0,0)};
        protected void UpdateParticles()
        {
            if (engineEmitData == null)
            {
                return;
            }
            for (int i = 0; i < engineEmitData.engineEmitters.Count; i++)
            {
                EngineEmitter engineEmitter = engineEmitData.engineEmitters[i];

                if (!ParticleManager.particleEmitters.Contains(engineEmitData.enginehandlers[i]))
                {
                    int count = engineEmitData.count;
                    float variance = engineEmitData.variance;
                    float sizemultiplier = engineEmitData.sizemultipler;

                    if (InputManager.boosting && boostable)
                    {
                        count = 20;
                        sizemultiplier = 2;
                    }
                    ParticleEmitterData ped2 = new()
                    {
                        interval = 0.01f,
                        emitCount = count,
                        lifespanMax = 0.6f,
                        angleVariance = variance,
                        angle = (float)(rotation - Math.PI),
                        particleData = new ParticleData()
                        {
                            colorStart = engineEmitData.startcolor,
                            colorEnd = engineEmitData.endcolor,
                            sizeStart = 8f * sizemultiplier,
                            sizeEnd = 4f * sizemultiplier,
                        },
                    };

                    engineEmitData.enginehandlers[i] = new ParticleEmitter(engineEmitter, ped2);
                    ParticleManager.AddParticleEmitter(engineEmitData.enginehandlers[i]);
                }
            }
            wasmoving = true;
        }
        public void UpdateBoostParticles()
        {
            ParticleManager.particleEmitters.RemoveRange(engineEmitData.enginehandlers);
            wasmoving = false;
            UpdateParticles();
        }

        public override void Draw()
        {
            //Bend texture
            var deformer = new ImageDeformer(texture);
            Texture2D final = new Texture2D(Game1.game.GraphicsDevice, texture.Width * 4, texture.Height * 4);
            Color[] colors = deformer.DeformImageToPolygon(Game1.AdjustFunc, texture.Width * 4, texture.Height * 4, position);
            final.SetData(colors);
            Game1.game.spriteBatch.Draw(final, position, null, Color.White, (float)rotation, new Vector2(final.Width/2, final.Width/2), 1f, SpriteEffects.None, 1);

            //Game1.game.spriteBatch.Draw(texture, position, null, Color.White, (float)rotation, origin, 1f, SpriteEffects.None, 1);
        }
    }
    public class EngineEmitData
    {
        public StaticEmitter _staticEmitter2 = new StaticEmitter(new Vector2(500, 500));
        public List<EngineEmitter> engineEmitters;

        public ParticleEmitter[] enginehandlers;

        public Color startcolor;
        public Color endcolor;

        public int count = 6;
        public float variance = 0.3f;
        public float sizemultipler = 1;

        public EngineEmitData(Ship sender, Color startcolor, Color endcolor, int count, float variance, float sizemultipler)
        {
            this.engineEmitters = new List<EngineEmitter>();

            for (int i = 0; i < sender.emitpositions.Count; ++i)
            {
                engineEmitters.Add(new EngineEmitter(sender, i));
            }
            enginehandlers = new ParticleEmitter[sender.emitpositions.Count];

            this.startcolor = startcolor;
            this.endcolor = endcolor;
            this.count = count;
            this.variance = variance;
            this.sizemultipler = sizemultipler;
        }
    }
}
