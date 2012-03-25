﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmashBros.Objects
{
    class PowerUp
    {
        /// <summary>
        /// The duration of the powerup. Countdown starts when some player is equipped with the powerup.
        /// </summary>
        public int duration;
        
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
        public int jumpHeight;
    }
}
