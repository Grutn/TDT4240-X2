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
        public MoveStats move;

        /// <summary>
        /// The box that punches people on collision.
        /// </summary>
        public Body moveBox;

        /// <summary>
        /// Time left to when attack is over.
        /// </summary>
        public float attackTimeLeft = 0;

        /// <summary>
        /// How long the character has charged the attack.
        /// </summary>
        public float chargeTime = 0;

        /// <summary>
        /// Is set to true if the character releases attackbutton before minimun chargetime has passed.
        /// </summary>
        public bool startMoveWhenReady = false;

        /// <summary>
        /// Whether the movebox have bin created etc.
        /// </summary>
        public bool moveStarted = false;
    }
}
