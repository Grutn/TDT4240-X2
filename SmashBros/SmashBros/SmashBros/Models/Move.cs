﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SmashBros.Controllers;

namespace SmashBros.Model
{
    public enum MoveType { Regular, Charge, Body, BodyCharge, Range }

    public class Move
    {
        /// <summary>
        /// What type of move this is: range charge etc...
        /// </summary>
        public MoveType Type = MoveType.Regular;

        /// <summary>
        /// Whether the bullets movementdirection is adjustable after it is fired.
        /// If true, the character will freeze until the bullet has hit something, or he or she exits the adjustmode.
        /// </summary>
        public bool Adjustable = false;

        /// <summary>
        /// The increase in damagepoints to any charachters being hit by the movebox.
        /// </summary>
        public int Damage;

        /// <summary>
        /// Total duration of move. (millisec)
        /// </summary>
        public int Duration;

        /// <summary>
        /// How much time in millisec before movebox appears and starts/ends moving.
        /// </summary>
        public int Start, End;

        /// <summary>
        /// From what frame to what frame the animation of the move is.
        /// </summary>
        public int AniFrom, AniTo;

        /// <summary>
        /// From and to what frame the animation of the effect of a hit is.
        /// </summary>
        public int AniEffectFrom, AniEffectTo;

        /// <summary>
        /// The startingpoint of the movebox.
        /// </summary>
        public Vector2 SqFrom, SqTo;

        /// <summary>
        /// Determines the velocity set to any charachters being hit by the movebox.
        /// </summary>
        public Vector2 Power;

        /// <summary>
        /// Bredde/høyde til angrepsfirkant
        /// </summary>
        public Vector2 SqSize;

        /// <summary>
        /// Which gamesoundtype to play when hit occurs.
        /// </summary>
        public GameSoundType hitSound = GameSoundType.hit;

        //public float RotateBodyTo;

        public Move(int damage, int duration, int sqStart, int aniFrom, int aniTo, Vector2 sqFrom, Vector2 sqTo, Vector2 power, Vector2 sqSize)
        {
            Damage = damage;
            Power = power;
            Duration = duration;
            Start = sqStart;
            AniFrom = aniFrom;
            AniTo = aniTo;
            SqFrom = sqFrom;
            SqTo = sqTo;
            SqSize = sqSize;
        }
    }

    public class ChargeMove : Move
    {
        /// <summary>
        /// Minimum/Maximum ventetid før angrepsfirkant opprettes og angrepet utføres.
        /// </summary>
        public int MinWait, MaxWait;

        /// <summary>
        /// Animation for the entering of the chargeAnimationLoop.
        /// </summary>
        public int AniStartChargeFrom, AniStartChargeTo;

        /// <summary>
        /// Animation of the chargeAnimationloop.
        /// </summary>
        public int AniChargeLoopFrom, AniChargeLoopTo;

        public ChargeMove(int damage, int duration, int sqStart, int aniFrom, int aniTo, Vector2 sqFrom, Vector2 sqTo, Vector2 power,Vector2 sqSize,
            int minWait, int maxWait, int aniStartChargeFrom, int aniStartChargeTo, int aniChargeLoopFrom, int aniChargeLoopTo)
            : base(damage, duration, sqStart, aniFrom, aniTo, sqFrom, sqTo, sqSize, power)
        {
            Type = MoveType.Charge;
            
            MinWait = minWait;
            MaxWait = maxWait;
            AniStartChargeFrom = aniStartChargeFrom;
            AniStartChargeTo = aniStartChargeTo;
            AniChargeLoopFrom = aniChargeLoopFrom;
            AniChargeLoopTo = aniChargeLoopTo;
        }
    }

    public abstract class AdjustableMove : Move
    {
        /// <summary>
        /// The acceleration in change of direction, IF ADJUSTABLE.
        /// If RangeMove, the character is inactive while adjusting.
        /// </summary>
        public float AdjustAcc;

        /// <summary>
        /// Whether the move stops when it hits a character.
        /// </summary>
        public bool StopAtHit;

        public AdjustableMove(int damage, int duration, int sqStart, int aniFrom, int aniTo, Vector2 sqFrom, Vector2 sqTo, Vector2 power, Vector2 sqSize,
            float adjustAcc)
            : base(damage, duration, sqStart, aniFrom, aniTo, sqFrom, sqTo, sqSize, power)
        {
            Adjustable = adjustAcc != 0;
            AdjustAcc = adjustAcc;
        }
    }

    public class BodyMove : AdjustableMove
    {
        /// <summary>
        /// The Speed of the characterbody.
        /// </summary>
        public Vector2 BodySpeed;

        /// <summary>
        /// Whether the bodyspeed should remain the same during the attack.
        /// </summary>
        public bool ConstantSpeed;

        public BodyMove(int damage, int duration, int sqStart, int aniFrom, int aniTo, Vector2 sqFrom, Vector2 sqTo, Vector2 power, Vector2 sqSize,
            bool stopAtHit, Vector2 bodySpeed, bool constantSpeed, float adjustAcc = 0)
            : base(damage, duration, sqStart, aniFrom, aniTo, sqFrom, sqTo, sqSize, power, adjustAcc)
        {
            Type = MoveType.Body;
            
            StopAtHit = stopAtHit;
            BodySpeed = bodySpeed;
            ConstantSpeed = constantSpeed;
        }
    }

    public class RangeMove : AdjustableMove
    {
        /// <summary>
        /// Animationframes for the bullet.
        /// </summary>
        public int AniBulletFrom, AniBulletTo;

        /// <summary>
        /// Animationframes for when character is in adjustmode.
        /// </summary>
        public int AniAdjustmodeFrom, AniAdjustmodeTo;

        public RangeMove(int damage, int duration, int sqStart, int aniFrom, int aniTo, Vector2 sqFrom, Vector2 sqTo, Vector2 power, Vector2 sqSize,
            int aniBulletFrom, int aniBulletTo, int aniAdjustmodeFrom, int aniAdjustmodeTo, float adjustAcc = 0) 
            : base(damage, duration, sqStart, aniFrom, aniTo, sqFrom, sqTo, sqSize, power, adjustAcc)
        {
            Type = MoveType.Range;
            
            AniBulletFrom = aniBulletFrom;
            AniBulletTo = aniBulletTo;
            AniAdjustmodeFrom = aniAdjustmodeFrom;
            AniAdjustmodeTo = aniAdjustmodeTo;
            StopAtHit = true;
        }
    }
}