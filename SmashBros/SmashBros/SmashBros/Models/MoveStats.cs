using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SmashBros.Controllers;
using SmashBros.Models;

namespace SmashBros.Model
{
    public enum MoveType { Regular, Charge, Body, BodyCharge, Range }

    public class MoveStats
    {
        /// <summary>
        /// What type of move this is: range charge etc...
        /// </summary>
        public MoveType Type;

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
        public int EffectFrame;

        /// <summary>
        /// Determines the velocity set to any charachters being hit by the movebox.
        /// </summary>
        public Vector2 Power;

        /// <summary>
        /// The startingpoint of the movebox.
        /// </summary>
        public Vector2 SqFrom, SqTo;

        /// <summary>
        /// Bredde/høyde til angrepsfirkant
        /// </summary>
        public Vector2 SqSize;

        /// <summary>
        /// Which gamesoundtype to play when hit occurs.
        /// </summary>
        public GameSoundType hitSound = GameSoundType.hit;

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

        /// <summary>
        /// The acceleration in change of direction, IF ADJUSTABLE.
        /// If RangeMove, the character is inactive while adjusting.
        /// </summary>
        public float AdjustAcc;

        /// <summary>
        /// The angle the adjustable move starts out from.
        /// </summary>
        public double StartAngle;

        /// <summary>
        /// Whether the move stops when it hits a character.
        /// </summary>
        public bool StopAtHit;

        /// <summary>
        /// Animationframes for when character is in adjustmode.
        /// </summary>
        public int AniAdjustmodeFrom, AniAdjustmodeTo;

        /// <summary>
        /// The Speed of the characterbody.
        /// </summary>
        public Vector2 BodySpeed;

        /// <summary>
        /// When the body starts and ends moving.
        /// </summary>
        public int BodyStart, BodyEnd;

        /// <summary>
        /// The velocity of the bullet leaving the characters body.
        /// </summary>
        public Vector2 BulletVelocity;

        /// <summary>
        /// Animationframes for the bullet.
        /// </summary>
        public int AniBulletFrom, AniBulletTo;

        /// <summary>
        /// Whether the bullet is affected by gravity. Bad match with adjustable...
        /// </summary>
        public bool Gravity;

        /// <summary>
        /// Whether the bullet is heatseaking.
        /// </summary>
        public bool Heatseaking;

        /// <summary>
        /// if != null, and explotion, that takes the moves sq stats as parameters, will occur on hit or after "StopAfter" or both.
        /// </summary>
        public Explotion Explotion;

        /// <summary>
        /// if != -1 the bullet will either explode or disappear after this amount of millisec. Or when character does next move.
        /// </summary>
        public int StopAfter;

        //public float RotateBodyTo;

        public MoveStats(int damage, int duration, int start, int end, int aniFrom, int aniTo, int effectFrame, Vector2 power, Vector2 sqFrom, Vector2 sqTo, Vector2 sqSize)
        {
            Damage = damage;
            Duration = duration;
            Start = start;
            End = end;
            AniFrom = aniFrom;
            AniTo = aniTo;
            EffectFrame = effectFrame;
            Power = power;
            SqFrom = sqFrom;
            SqTo = sqTo;
            SqSize = sqSize;

            Type = MoveType.Regular;
        }
    }

    public class ChargeMove : MoveStats
    {
        /*
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
        */
        public ChargeMove(int damage, int duration, int start, int end, int aniFrom, int aniTo, int effectFrame, Vector2 power, Vector2 sqFrom, Vector2 sqTo, Vector2 sqSize,
            int minWait, int maxWait, int aniStartChargeFrom, int aniStartChargeTo, int aniChargeLoopFrom, int aniChargeLoopTo)
            : base(damage, duration, start, end, aniFrom, aniTo, effectFrame, power, sqFrom, sqTo, sqSize)
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

    public abstract class AdjustableMove : MoveStats
    {
        /*
        /// <summary>
        /// The acceleration in change of direction, IF ADJUSTABLE.
        /// If RangeMove, the character is inactive while adjusting.
        /// </summary>
        public float AdjustAcc;

        /// <summary>
        /// Whether the move stops when it hits a character.
        /// </summary>
        public bool StopAtHit;

        /// <summary>
        /// Animationframes for when character is in adjustmode.
        /// </summary>
        public int AniAdjustmodeFrom, AniAdjustmodeTo;
        */
        public AdjustableMove(int damage, int duration, int start, int end, int aniFrom, int aniTo, int effectFrame, Vector2 power, Vector2 sqFrom, Vector2 sqTo, Vector2 sqSize,
            float adjustAcc, double startAngle, int aniAdjustmodeFrom, int aniAdjustmodeTo, bool stopAtHit)
            : base(damage, duration, start, end, aniFrom, aniTo, effectFrame, power, sqFrom, sqTo, sqSize)
        {
            Adjustable = adjustAcc != 0;
            AdjustAcc = adjustAcc;
            startAngle = startAngle;
            AniAdjustmodeFrom = aniAdjustmodeFrom;
            AniAdjustmodeTo = aniAdjustmodeTo;
            StopAtHit = stopAtHit;
        }
    }

    public class BodyMove : AdjustableMove
    {
        /*
        /// <summary>
        /// The Speed of the characterbody.
        /// </summary>
        public Vector2 BodySpeed;

        /// <summary>
        /// When the body starts and ends moving.
        /// </summary>
        public int BodyStart, BodyEnd;
        */
        public BodyMove(int damage, int duration, int start, int end, int aniFrom, int aniTo, int effectFrame, Vector2 power, Vector2 sqFrom, Vector2 sqTo, Vector2 sqSize,
            Vector2 bodySpeed, int bodyStart = -1, int bodyEnd = -1, float adjustAcc = 0, double startAngle = 0, int aniAdjustmodeFrom = 0, int aniAdjustmodeTo = 0, bool stopAtHit = false)
            : base(damage, duration, start, end, aniFrom, aniTo, effectFrame, power, sqFrom, sqTo, sqSize, adjustAcc, startAngle, aniAdjustmodeFrom, aniAdjustmodeTo, stopAtHit)
        {
            Type = MoveType.Body;
            
            BodySpeed = bodySpeed;
            BodyStart = bodyStart == -1? Start : bodyStart;
            BodyEnd = bodyEnd == -1 ? End : bodyEnd;
        }
    }

    public class RangeMove : AdjustableMove
    {
        /*
        /// <summary>
        /// The velocity of the bullet leaving the characters body.
        /// </summary>
        public Vector2 BulletVelocity;
        
        /// <summary>
        /// Animationframes for the bullet.
        /// </summary>
        public int AniBulletFrom, AniBulletTo;

        /// <summary>
        /// Whether the bullet is affected by gravity. Bad match with adjustable...
        /// </summary>
        public bool Gravity;

        /// <summary>
        /// Whether the bullet is heatseaking.
        /// </summary>
        public bool Heatseaking;

        /// <summary>
        /// if != null, and explotion, that takes the moves sq stats as parameters, will occur on hit or after "StopAfter" or both.
        /// </summary>
        public MoveStats Explotion;

        /// <summary>
        /// if != -1 the bullet will either explode or disappear after this amount of millisec. Or when character does next move.
        /// </summary>
        public int StopAfter;
        */
        public RangeMove(int damage, int duration, int start, int aniFrom, int aniTo, int effectFrame, Vector2 power, Vector2 sqFrom, Vector2 sqSize,
            Vector2 bulletVelocity, int aniBulletFrom, int aniBulletTo, bool gravity = false, bool heatSeaking = false, Explotion explotion = null, bool stopAtHit = true, int stopAfter = -1, float adjustAcc = 0, double startAngle = 0, int aniAdjustmodeFrom = 0, int aniAdjustmodeTo = 0)
            : base(damage, duration, start, 5000, aniFrom, aniTo, effectFrame, power, sqFrom, new Vector2(0,0), sqSize, adjustAcc, startAngle, aniAdjustmodeFrom, aniAdjustmodeTo, stopAtHit)
        {
            Type = MoveType.Range;

            BulletVelocity = bulletVelocity;
            AniBulletFrom = aniBulletFrom;
            AniBulletTo = aniBulletTo;
            Gravity = gravity;
            Heatseaking = heatSeaking;
            Explotion = explotion;
            StopAfter = stopAfter;
        }
    }

    public class Explotion
    {
        public Vector2 Size;
        public int Duration;
        public int TimeLeft;
        public int AniFrom, AniTo;
        public ImageModel Img;
        public int PlayerIndex;

        public Explotion(Vector2 size, int duration, int aniFrom, int aniTo)
        {
            Size = size;
            Duration = duration;
            TimeLeft = duration;
            AniFrom = aniFrom;
            AniTo = aniTo;
        }
    }
}