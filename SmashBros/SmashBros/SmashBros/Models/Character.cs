using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace SmashBros.Model
{
    public class Character
    {
        /// <summary>
        /// Charactername.
        /// </summary>
        public string name;

        /// <summary>
        /// Source to the picture that contains all frames regarding this character.
        /// </summary>
        public string animations;

        /// <summary>
        /// thumbnail of character.
        /// </summary>
        public string thumbnail;
        /// <summary>
        /// Image of the whole character 
        /// </summary>
        public string image;

        /// <summary>
        /// sounds to be played when (name)
        /// </summary>
        public string sound_selected, sound_won, sound_jump, sound_kill, sound_punch; //, sound_hit, sound_chargingHit....

        /// <summary>
        /// The maximum magnitude of speed in x-direction this character can have.
        /// </summary>
        public int maxSpeed;

        /// <summary>
        /// Acceleration! WTF do you think it means.
        /// </summary>
        public int acceleration;

        /// <summary>
        /// The weight of the character. Determines, along with players damagePoints, how far the character is pushed by some force.
        /// </summary>
        public int weight;

        /// <summary>
        /// The hight of the characters jump.
        /// </summary>
        public int jumpStartVelocity;

        /// <summary>
        /// A force that affects the character at all time. It can be negative or positiv, and it is only to vary the speed of different characters fall.
        /// </summary>
        public int gravity;

        /// <summary>
        /// The size of the characters boundbox.
        /// </summary>
        public Vector2 size;

        /// <summary>
        /// The corresponding moves when the player presses A whilest holding the stick in some direction, and etc.
        /// </summary>
        public Move a, aUp, aDown, aLR,
            x, xUp, xDown, xLR;

        public int ani_noneStart, ani_noneEnd, ani_runStart, ani_runEnd, ani_jumpStart, ani_jumpEnd, ani_fallStart,
            ani_fallEnd, ani_landStart, ani_landEnd, ani_brake, ani_takeHitStart, ani_takeHitEnd;
    }
}
