using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
        public const double _rotationSpeed = 3f;
        public double maxspeed = 600;
        public double speed;
        public float acceleration = 150f;
        public bool forcestop;
        public bool wasmoving = false;
        public Collider collider;
        
        public Ship(Texture2D tex, Vector2 pos) : base(tex, pos)
        {
            engineEmitter = new EngineEmitter(this);
        }

        public bool OnCollision(string tag)
        {
            return false;
        }

        public StaticEmitter _staticEmitter2 = new StaticEmitter(new Vector2(500, 500));
        public Vector2 backend;
        EngineEmitter engineEmitter;
        public ParticleEmitter enginehandler;
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
                ParticleManager.particleEmitters.Remove(enginehandler);
                wasmoving = true;
                forcestop = true;
            }
            else if (distancetoend >= 5) //Dont restart if we just finished stopping
            {
                wasmoving = false;
                forcestop = false;
            }
        }
        protected Color startengineemitcolor;
        protected Color endengineemitcolor;
        protected void UpdateParticles()
        {
            if (!wasmoving && !ParticleManager.particleEmitters.Contains(enginehandler))
            {
                int count = 6;
                float variance = 0.3f;
                float sizemultipler = 1;
                if (InputManager.boosting)
                {
                    count = 20;
                    sizemultipler = 2;
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
                        colorStart = startengineemitcolor,
                        colorEnd = endengineemitcolor,
                        sizeStart = 8f * sizemultipler,
                        sizeEnd = 4f * sizemultipler,
                    },
                };

                enginehandler = new ParticleEmitter(engineEmitter, ped2);
                ParticleManager.AddParticleEmitter(enginehandler);
            }
            wasmoving = true;
        }
        public void UpdateBoostParticles()
        {
            ParticleManager.particleEmitters.Remove(enginehandler);
            wasmoving = false;
            UpdateParticles();
        }

        public override void Draw()
        {
            Game1.game.spriteBatch.Draw(texture, position, null, Color.White, (float)rotation, origin, 1f, SpriteEffects.None, 1);
        }
    }
}
