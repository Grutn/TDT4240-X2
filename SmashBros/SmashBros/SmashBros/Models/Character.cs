using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

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
        /// The corresponding moves when the player presses A whilest holding the stick in some direction, and etc.
        /// </summary>
        public Move a, aUp, aDown, aLR,
            x, xUp, xDown, xLR;
    }
}
