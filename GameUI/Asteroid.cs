using HyperbolicRenderer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct2D1.Effects;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameUI
{
    public enum AsteroidType
    {
        Large,
        Medium,
        Small,
    }
    public class Asteroid : Sprite
    {
        double speed;
        public Vector direction;
        double rotation;
        double rotationspeed;
        public bool disappear;
        public Collider collider;
        ImageDeformer deformer;
        //public MoveableShape graphicalcollider;

        //Centre -83, -75
        public static Vector2[] largecolliderpoints  = new Vector2[55] { new Vector2(-8, 71), new Vector2(2, 68), new Vector2(11, 59), new Vector2(20, 52), new Vector2(30, 45), new Vector2(40, 41), new Vector2(49, 32), new Vector2(58, 23), new Vector2(61, 13), new Vector2(63, 3), new Vector2(72, -7), new Vector2(78, -17), new Vector2(77, -27), new Vector2(72, -37), new Vector2(73, -47), new Vector2(82, -55), new Vector2(75, -65), new Vector2(67, -71), new Vector2(60, -73), new Vector2(53, -75), new Vector2(44, -75), new Vector2(34, -75), new Vector2(26, -74), new Vector2(19, -73), new Vector2(11, -71), new Vector2(3, -68), new Vector2(-5, -65), new Vector2(-12, -61), new Vector2(-19, -57), new Vector2(-25, -54), new Vector2(-31, -49), new Vector2(-38, -44), new Vector2(-43, -40), new Vector2(-48, -34), new Vector2(-53, -29), new Vector2(-56, -22), new Vector2(-61, -16), new Vector2(-69, -10), new Vector2(-72, -5), new Vector2(-76, 3), new Vector2(-77, 10), new Vector2(-78, 15), new Vector2(-78, 22), new Vector2(-79, 30), new Vector2(-81, 37), new Vector2(-83, 43), new Vector2(-82, 52), new Vector2(-78, 59), new Vector2(-73, 66), new Vector2(-68, 72), new Vector2(-58, 72), new Vector2(-48, 67), new Vector2(-38, 65), new Vector2(-30, 67), new Vector2(-23, 70), }.Reverse().ToArray();
        //Centre -32,-25
        public static Vector2[] mediumcolliderpoints = new Vector2[19]{ new Vector2(3,24),new Vector2(-4,20),new Vector2(-10,15),new Vector2(-17,13),new Vector2(-23,10),new Vector2(-30,4),new Vector2(-32,-5),new Vector2(-31,-10),new Vector2(-24,-17),new Vector2(-19,-23),new Vector2(-13,-25),new Vector2(-5,-25),new Vector2(3,-22),new Vector2(10,-20),new Vector2(17,-17),new Vector2(24,-9),new Vector2(30,1),new Vector2(31,11),new Vector2(21,20),};
        //Centre -16,-14
        public static Vector2[] smallcolliderpoints = new Vector2[10] { new Vector2(0, 13), new Vector2(-9, 12), new Vector2(-15, 8), new Vector2(-16, -1), new Vector2(-12, -10), new Vector2(-5, -14), new Vector2(5, -13), new Vector2(12, -9), new Vector2(15, 1), new Vector2(10, 11), };
        public Asteroid(Texture2D tex, Vector2 pos, Vector direction, double speed, AsteroidType asteroidType) : base(tex, pos)
        {
            this.speed = speed;
            this.direction = direction;
            //rotation = GameManager.RandomFloat(0, 50);
            rotationspeed = GameManager.RandomFloat(0, 5);

            Vector2[] colliderpoints = Array.Empty<Vector2>();
            switch (asteroidType)
            {
                case AsteroidType.Large:
                    colliderpoints = largecolliderpoints;
                    break;
                case AsteroidType.Medium:
                    colliderpoints = mediumcolliderpoints;
                    break;
                case AsteroidType.Small:
                    colliderpoints = smallcolliderpoints;
                    break;
            }

            //graphicalcollider = Game1.game.batcher.AddMoveableShape(colliderpoints.Copy().ToArray(), Color.White, Vector2.Zero);
            collider = new Collider(colliderpoints, OnCollision, pos, "ASTEROID");
            //graphicalcollider.Move(position);
        }
        public bool OnCollision(string tag)
        {
            if (tag == "PLAYER")
            {
                GameManager.Stop();
                //graphicalcollider.color = Color.Red;
                return true;
            }
            if (tag.Contains("BULLET"))
            {
                disappear = true;
                return true;
            }
            if (tag == "EYEENEMY")
            {
                disappear = true;
                return true;
            }
            return false;
        }
        public override void Draw()
        {
            //Bend texture
            deformer = new ImageDeformer(texture);
            Texture2D final = new Texture2D(Game1.game.GraphicsDevice, texture.Width*2, texture.Height*2);
            Color[] colors = deformer.DeformImageToPolygon(Game1.AdjustFunc, texture.Width * 2, texture.Height * 2, position, texture.Width/2, texture.Height/2);
            final.SetData(colors);
            Game1.game.spriteBatch.Draw(final, position, null, Color.White, (float)0, origin, 1f, SpriteEffects.None, 1);
        }
        public void Update()
        {
            rotation += (rotationspeed * Game1.game.looptime);
            position = new Vector2 (position.X + (float)(speed*Game1.game.looptime*direction.i), position.Y + (float)(speed * Game1.game.looptime * direction.j));
            if (position.X < -100 || position.Y < -100 || position.X > (Game1.width + 100) || position.Y > (Game1.height+100))
            {
                disappear = true;
            }
            collider.Rotate(rotationspeed * Game1.game.looptime);
            collider.Move(position);
            //graphicalcollider.Move(position);
            //graphicalcollider.Rotate(rotationspeed * Game1.game.looptime);
        }

        public override void Dispose()
        {
            Game1.asteroids.Remove(this);
            collider.Dispose();
            //Game1.game.batcher.shapes.Remove(graphicalcollider);
        }

        internal static void CreateRandomAsteroid()
        {
            //Generate random place on edge
            float edge = GameManager.RandomInt(0, 4); //0:left, 1:top, 2:right, 3:bottom
            Vector2 startposition = new Vector2(0, 0);
            double xdirection = GameManager.RandomDouble() - 0.5;
            double ydirection = GameManager.RandomDouble() - 0.5;


            if (edge == 0)
            {
                startposition.Y = GameManager.RandomFloat(0, Game1.height);
                //Spawn on left, can only move right
                xdirection += 0.5;
            }
            else if (edge == 1)
            {
                startposition.X = GameManager.RandomFloat(0, Game1.width);
                //Spawn on top, can only move down
                ydirection += 0.5;
            }
            if (edge == 2)
            {
                startposition.X = Game1.width;
                startposition.Y = GameManager.RandomFloat(0, Game1.height);
                //Spawn on right, can only move left
                xdirection -= 0.5;
            }
            else if (edge == 3)
            {
                startposition.Y = Game1.height;
                startposition.X = GameManager.RandomFloat(0, Game1.width);
                //Spawn on bottom, can only move up
                ydirection -= 0.5;
            }

            double randomsize = GameManager.RandomDouble();
            if (randomsize > 0.7)
            {
               Game1.asteroids.Add(new Asteroid(Game1.large_asteroidtexture, startposition, new Vector(xdirection, ydirection).GetUnitVector(), GameManager.RandomFloat(200, 500), AsteroidType.Large));
            }
            else if (randomsize > 0.3)
            {
               Game1.asteroids.Add(new Asteroid(Game1.medium_asteroidtexture, startposition, new Vector(xdirection, ydirection).GetUnitVector(), GameManager.RandomFloat(400, 800), AsteroidType.Medium));
            }
            else
            {
               Game1.asteroids.Add(new Asteroid(Game1.small_asteroidtexture, startposition, new Vector(xdirection, ydirection).GetUnitVector(), GameManager.RandomFloat(600, 1000), AsteroidType.Small));
            }
        }
    }
}
