using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CrisisAtSwissStation
{
    public class ParticleGenerator
    {

        Texture2D texture;
        Vector2 origin;
        const int maxParticles = 5;
        const int particleLife = 250;

        public class Particle
        {
            public Vector2 pos;
            public int life;
        }

        Particle[] Particles = new Particle[maxParticles];
        Queue<Particle> inactiveParticles = new Queue<Particle>(maxParticles);

        public ParticleGenerator(Texture2D texture)
        {
            this.texture = texture;
            origin = new Vector2(texture.Width / (2 * CASSWorld.SCALE), texture.Height / (2 * CASSWorld.SCALE));

            for (int i = 0; i < maxParticles; i++)
            {
                Particles[i] = new Particle();
                inactiveParticles.Enqueue(Particles[i]);
            }
        }

        public void addParticle(Box2DX.Common.Vec2 pos)
        {
            if (inactiveParticles.Count != 0)
            {
                Particle p = inactiveParticles.Dequeue();
                p.pos = new Vector2(pos.X, pos.Y);
                p.life = particleLife;
            }
        }

        public void Update()
        {
            for (int i = 0; i < Particles.Length; i++)
            {
                if (Particles[i].life > 0)
                {
                    Particles[i].pos.Y -= 1 / CASSWorld.SCALE;
                    Particles[i].life -= 1;
                    if (Particles[i].life == 0)
                    {
                        inactiveParticles.Enqueue(Particles[i]);
                    }
                }

            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            for (int i = 0; i < Particles.Length; i++)
            {
                if(Particles[i].life > 0)
                    spriteBatch.Draw(texture, Particles[i].pos * CASSWorld.SCALE, null, Color.White, 0f,
                        origin, 1, SpriteEffects.None, 0);
            }
            spriteBatch.End();
        }

    }
}
