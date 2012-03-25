﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SmashBros.Objects
{
    public enum PlayerState { jumping, running, attacking }

    class Player
    {
        /// <summary>
        /// Currently chosen character of this player.
        /// </summary>
        public Character character;

        /// <summary>
        /// The state of which the player is currently in. (jumping, running, etc.)
        /// </summary>
        public PlayerState state;

        /// <summary>
        /// The players current position on the map, as a vector.
        /// </summary>
        public Vector2 position;

        /// <summary>
        /// Vector that shows the direction and magnitude of the current speed of the player.
        /// </summary>
        public Vector2 speed;

        /// <summary>
        /// An int that describes how much damage the player has taken in the current game/characterlife.
        /// </summary>
        public int damagePoints;

        /// <summary>
        /// Number of lifes left.
        /// </summary>
        public int lives;

        /// <summary>
        /// A powerUp the player currently is in posetion of.
        /// </summary>
        public PowerUp powerUp;

        /// <summary>
        /// Time left before current powerup expires.
        /// </summary>
        public int powerUpTimeLeft;

        /// <summary>
        /// Currently posessed weapon.
        /// </summary>
        public Weapon weapon;

        public Player()
        {
        }
    }
}
