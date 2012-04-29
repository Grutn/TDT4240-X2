using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmashBros.Model;
using FarseerPhysics.Dynamics;

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
        public ImageModel Box;

        /// <summary>
        /// Time left to when move is over.
        /// </summary>
        public float attackTimeLeft = 0;

        /// <summary>
        /// How long the character has charged the attack.
        /// </summary>
        public float chargeTime = 0;

        public MoveModel(MoveStats stats)
        {
            Stats = stats;
            attackTimeLeft = stats.Duration;
        }
    }
}
