using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using SharpDX.X3DAudio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameUI
{
    public class Particle
    {
        private readonly ParticleData _data;
        private Vector2 _position;
        private float _lifespanLeft;
        private float _lifespanAmount;
        private Color _color;
        private float _opacity;
        public bool isFinished = false;
        private float _scale;
        private Vector2 _origin;
        private Vector2 _direction;

        public Particle(Vector2 pos, ParticleData data)
        {
            _data = data;
            _lifespanLeft = data.lifespan;
            _lifespanAmount = 1f;
            _position = pos;
            _color = data.colorStart;
            _opacity = data.opacityStart;
            _origin = new(_data.texture.Width / 2, _data.texture.Height / 2);

            if (data.speed != 0)
            {
                _direction = new Vector2((float)Math.Sin(_data.angle), -(float)Math.Cos(_data.angle));
            }
            else
            {
                _direction = Vector2.Zero;
            }
        }

        public void Update()
        {
            _lifespanLeft -= (float)Game1.game.looptime;
            if (_lifespanLeft <= 0f)
            {
                isFinished = true;
                return;
            }

            _lifespanAmount = MathHelper.Clamp(_lifespanLeft / _data.lifespan, 0, 1);
            _color = Color.Lerp(_data.colorEnd, _data.colorStart, _lifespanAmount);
            _opacity = MathHelper.Clamp(MathHelper.Lerp(_data.opacityEnd, _data.opacityStart, _lifespanAmount), 0, 1);
            _scale = MathHelper.Lerp(_data.sizeEnd, _data.sizeStart, _lifespanAmount) / _data.texture.Width;
            _position += _direction * _data.speed * (float)Game1.game.looptime;
        }

        public void Draw()
        {
            Game1.game.spriteBatch.Draw(_data.texture, _position, null, _color * _opacity, 0f, _origin, _scale, SpriteEffects.None, 1f);
        }
    }
    public class ParticleEmitter
    {
        private readonly ParticleEmitterData _data;
        private float _intervalLeft;
        private readonly IEmitter _emitter;

        public ParticleEmitter(IEmitter emitter, ParticleEmitterData data)
        {
            _emitter = emitter;
            _data = data;
            _intervalLeft = data.interval;
        }

        private void Emit(Vector2 pos)
        {
            ParticleData d = _data.particleData;
            d.lifespan = GameManager.RandomFloat(_data.lifespanMin, _data.lifespanMax);
            d.speed = GameManager.RandomFloat(_data.speedMin, _data.speedMax);
            d.angle = GameManager.RandomFloat(_data.angle - _data.angleVariance, _data.angle + _data.angleVariance);

            Particle p = new(pos, d);
            ParticleManager.AddParticle(p);
        }
        public void Update()
        {
            _intervalLeft -= (float)Game1.game.looptime;
            while (_intervalLeft <= 0f)
            {
                _intervalLeft += _data.interval;
                var pos = _emitter.EmitPosition;
                for (int i = 0; i < _data.emitCount; i++)
                {
                    Emit(pos);
                }
            }
        }
    }
    public class StaticEmitter : IEmitter
    {
        public Vector2 EmitPosition { get; }

        public StaticEmitter(Vector2 pos)
        {
            EmitPosition = pos;
        }
    }
    public struct ParticleEmitterData
    {
        public ParticleData particleData = new ParticleData();
        public float angle = 0f;
        public float angleVariance = 0.3f;
        public float lifespanMin = 0.1f;
        public float lifespanMax = 2f;
        public float speedMin = 10f;
        public float speedMax = 100f;
        public float interval = 1f;
        public int emitCount = 1;

        public ParticleEmitterData()
        {
        }
    }

    public struct ParticleData
    {
        private static Texture2D _defaultTexture;
        public Texture2D texture = _defaultTexture ??= Game1.game.Content.Load<Texture2D>("particle");
        public float lifespan = 2f;
        public Color colorStart = Color.White;
        public Color colorEnd = Color.DarkBlue;
        public float opacityStart = 1f;
        public float opacityEnd = 0f;
        public float sizeStart = 32f;
        public float sizeEnd = 4f;
        public float speed = 100f;
        public float angle = 0f;

        public ParticleData()
        {
        }
    }

    public class MouseEmitter : IEmitter
    {
        public Vector2 EmitPosition => InputManager.MousePosition;
    }
    public class EngineEmitter : IEmitter
    {
        public Vector2 EmitPosition => Ship.backend;
    }
    public interface IEmitter
    {
        Vector2 EmitPosition { get; }
    }
}