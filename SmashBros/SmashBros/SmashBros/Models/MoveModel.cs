using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace SmashBros.Models
{
    public class MoveModel
    {
        /// <summary>
        /// The move that has either begun, or that is in charging face.
        /// </summary>
        public MoveStats Stats;

        /// <summary>
        /// The box that punches people on collision.
        /// </summary>
        public ImageModel Img;

        /// <summary>
        /// Time left to when move is over.
        /// </summary>
        public float attackTimeLeft;

        /// <summary>
        /// How long the character has charged the attack.
        /// </summary>
        public float chargeTime;

        /// <summary>
        /// Whether the move is in positive or negative X direction. (either (1,1) or (-1,1)).
        /// </summary>
        public Vector2 Xdirection;

        /// <summary>
        /// The list of playerIndexen that have bin hit by this move.
        /// </summary>
        public List<int> PlayerIndexes;

        /// <summary>
        /// Whether the movebox have bin created etc.
        /// </summary>
        public bool moveStarted;

        /// <summary>
        /// Whether the move have ended. Is currently needed by adjustable rangeattacks.
        /// </summary>
        public bool Ended;

        public MoveModel(MoveStats stats, bool right, int playerIndex)
        {
            Stats = stats;
            attackTimeLeft = stats.Duration;
            chargeTime = 0;
            moveStarted = false;
            Ended = false;
            Xdirection = right ? new Vector2(1, 1) : new Vector2(-1, 1);
            PlayerIndexes = new List<int>();
            PlayerIndexes.Add(playerIndex);
        }
    }
}
