using HyperbolicRenderer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameUI
{
    internal class EyeEnemy : EnemyShip
    {
        public static Texture2D[] frames = new Texture2D[10];
        Vector2[] colliderpoints = new Vector2[190] { new Vector2(-17, 170), new Vector2(-24, 162), new Vector2(-34, 153), new Vector2(-55, 145), new Vector2(-76, 134), new Vector2(-118, 127), new Vector2(-146, 126), new Vector2(-160, 118), new Vector2(-170, 109), new Vector2(-171, 96), new Vector2(-171, 82), new Vector2(-170, 70), new Vector2(-162, 62), new Vector2(-156, 52), new Vector2(-162, 42), new Vector2(-167, 32), new Vector2(-172, 23), new Vector2(-175, 14), new Vector2(-184, 5), new Vector2(-192, -8), new Vector2(-199, -18), new Vector2(-193, -30), new Vector2(-186, -41), new Vector2(-178, -50), new Vector2(-175, -55), new Vector2(-170, -65), new Vector2(-165, -74), new Vector2(-159, -85), new Vector2(-157, -95), new Vector2(-168, -104), new Vector2(-171, -115), new Vector2(-171, -129), new Vector2(-171, -143), new Vector2(-168, -152), new Vector2(-156, -158), new Vector2(-147, -163), new Vector2(-138, -171), new Vector2(-123, -164), new Vector2(-112, -165), new Vector2(-99, -166), new Vector2(-81, -170), new Vector2(-71, -174), new Vector2(-59, -181), new Vector2(-48, -186), new Vector2(-37, -189), new Vector2(-27, -193), new Vector2(-23, -203), new Vector2(-16, -208), new Vector2(-6, -211), new Vector2(4, -216), new Vector2(14, -213), new Vector2(27, -208), new Vector2(37, -199), new Vector2(36, -204), new Vector2(27, -209), new Vector2(17, -213), new Vector2(8, -218), new Vector2(-6, -212), new Vector2(-20, -205), new Vector2(-31, -193), new Vector2(-45, -188), new Vector2(-59, -182), new Vector2(-73, -174), new Vector2(-87, -168), new Vector2(-100, -167), new Vector2(-113, -166), new Vector2(-127, -165), new Vector2(-135, -171), new Vector2(-148, -164), new Vector2(-162, -156), new Vector2(-170, -143), new Vector2(-170, -129), new Vector2(-170, -115), new Vector2(-165, -103), new Vector2(-155, -91), new Vector2(-163, -77), new Vector2(-170, -63), new Vector2(-180, -49), new Vector2(-188, -35), new Vector2(-197, -21), new Vector2(-191, -7), new Vector2(-182, 7), new Vector2(-174, 15), new Vector2(-168, 29), new Vector2(-160, 43), new Vector2(-158, 57), new Vector2(-169, 70), new Vector2(-170, 84), new Vector2(-170, 98), new Vector2(-168, 112), new Vector2(-158, 118), new Vector2(-148, 124), new Vector2(-137, 132), new Vector2(-132, 128), new Vector2(-126, 126), new Vector2(-119, 126), new Vector2(-112, 126), new Vector2(-105, 126), new Vector2(-98, 128), new Vector2(-88, 128), new Vector2(-79, 132), new Vector2(-70, 136), new Vector2(-60, 142), new Vector2(-51, 146), new Vector2(-43, 148), new Vector2(-34, 152), new Vector2(-26, 156), new Vector2(-17, 168), new Vector2(-8, 172), new Vector2(0, 174), new Vector2(9, 176), new Vector2(18, 173), new Vector2(28, 170), new Vector2(36, 165), new Vector2(39, 155), new Vector2(49, 152), new Vector2(61, 147), new Vector2(71, 143), new Vector2(83, 137), new Vector2(94, 131), new Vector2(110, 128), new Vector2(122, 127), new Vector2(134, 126), new Vector2(147, 131), new Vector2(157, 128), new Vector2(167, 121), new Vector2(178, 115), new Vector2(183, 107), new Vector2(183, 93), new Vector2(183, 79), new Vector2(182, 67), new Vector2(170, 56), new Vector2(171, 45), new Vector2(177, 35), new Vector2(182, 26), new Vector2(186, 18), new Vector2(190, 11), new Vector2(197, 3), new Vector2(205, -9), new Vector2(211, -20), new Vector2(204, -30), new Vector2(197, -43), new Vector2(188, -53), new Vector2(184, -63), new Vector2(179, -72), new Vector2(173, -81), new Vector2(168, -92), new Vector2(176, -100), new Vector2(182, -110), new Vector2(183, -122), new Vector2(183, -136), new Vector2(183, -148), new Vector2(168, -158), new Vector2(156, -166), new Vector2(154, -169), new Vector2(165, -161), new Vector2(175, -155), new Vector2(184, -149), new Vector2(184, -135), new Vector2(184, -121), new Vector2(183, -107), new Vector2(170, -94), new Vector2(175, -80), new Vector2(183, -66), new Vector2(190, -52), new Vector2(198, -42), new Vector2(207, -28), new Vector2(209, -14), new Vector2(200, 0), new Vector2(189, 14), new Vector2(183, 28), new Vector2(175, 42), new Vector2(171, 56), new Vector2(181, 66), new Vector2(184, 78), new Vector2(184, 92), new Vector2(184, 106), new Vector2(173, 117), new Vector2(159, 125), new Vector2(147, 130), new Vector2(138, 125), new Vector2(124, 126), new Vector2(110, 127), new Vector2(96, 129), new Vector2(82, 137), new Vector2(68, 143), new Vector2(54, 149), new Vector2(40, 156), new Vector2(29, 169), new Vector2(15, 173), };        //MoveableShape graphicalcollider;
        public EyeEnemy(Vector2 pos) : base(frames[0], pos)
        {
            collider = new Collider(colliderpoints, OnCollision, pos, "ENEMY");
            //graphicalcollider = Game1.game.batcher.AddMoveableShape(colliderpoints.Copy().ToArray(), Color.White, Vector2.Zero);
            //graphicalcollider.Move(position);

            startengineemitcolor = Color.DarkOrange;
            endengineemitcolor = Color.LightYellow;
            maxspeed = 300;
            boostable = false;
        }
        public bool disappear;
        double lasttime = Game1.game.totalseconds;
        double timebetweenframes;
        int framedrawidx = 0;
        bool framedirection = true;
        bool blinking = false;
        public override void Draw()
        {
            if (!disappear)
            {
                timebetweenframes += Game1.game.drawlooptime;
                if (timebetweenframes > 1)
                {
                    timebetweenframes -= 1;
                    if (GameManager.RandomDouble() > 0.9) //Blind 10% of the time
                    {
                        blinking = true;
                    }
                }
                if (timebetweenframes > 0.05f && blinking) //Change frames every second
                {
                    if (framedrawidx == 9)
                    {
                        framedirection = !framedirection;
                    }
                    if (framedrawidx == 1 && !framedirection) //Going backwards, at 1
                    {
                        blinking = false;
                    }
                    if (framedirection)
                    {
                        ++framedrawidx;
                    }
                    else
                    {
                        --framedrawidx;
                    }
                    timebetweenframes = 0;
                }
                Game1.game.spriteBatch.Draw(frames[framedrawidx], position, null, Color.White, (float)rotation, origin, 1f, SpriteEffects.None, 1);
            }
            else
            {

            }
        }
    }
}
