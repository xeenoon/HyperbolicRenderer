using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameUI
{
    public static class ParticleManager
    {
        public static  List<Particle> particles = new List<Particle>();
        public static  List<ParticleEmitter> particleEmitters = new List<ParticleEmitter>();

        public static void AddParticle(Particle p)
        {
            particles.Add(p);
        }

        public static void AddParticleEmitter(ParticleEmitter e)
        {
            particleEmitters.Add(e);
        }

        public static void UpdateParticles()
        {
            foreach (var particle in particles)
            {
                particle.Update();
            }

            particles.RemoveAll(p => p.isFinished);
        }

        public static void UpdateEmitters()
        {
            foreach (var emitter in particleEmitters)
            {
                emitter.Update();
            }
        }

        public static void Update()
        {
            UpdateParticles();
            UpdateEmitters();
        }

        public static void Draw()
        {
            foreach (var particle in particles)
            {
                particle.Draw();
            }
        }
    }
}
