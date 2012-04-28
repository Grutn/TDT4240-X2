using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using SmashBros.Models;
using Microsoft.Xna.Framework.Content;
using SmashBros.Model;
using SmashBros.MySystem;

namespace SmashBros.Views
{
    public class CharacterView : Sprite
    {
        private CharacterStats stats;
        
        public CharacterView(ContentManager content, string assetName, int frameWidth, int frameHeight, float xPos, float yPos, CharacterStats stats)
            : base(content, assetName, frameWidth, frameHeight, xPos, yPos)
        {
            this.stats = stats;
        }

        public void StateChanged(CharacterState oldState, CharacterState newState, MoveStats move)
        {
            switch (newState)
            {
                case CharacterState.none:
                    if (oldState == CharacterState.running) AddAnimation(0, 2, true);
                    else if (oldState == CharacterState.falling || oldState == CharacterState.jumping)
                    {
                        StartAnimation(stats.ani_landStart, stats.ani_landEnd, false);
                        AddAnimation(stats.ani_noneStart, stats.ani_noneEnd, true);
                    }
                    else StartAnimation(stats.ani_noneStart, stats.ani_noneEnd, true);
                    break;
                case CharacterState.running:
                    if(oldState == CharacterState.falling || oldState == CharacterState.jumping)
                    {
                        StartAnimation(stats.ani_landStart, stats.ani_landEnd, false);
                        AddAnimation(stats.ani_runStart, stats.ani_runEnd, true);
                    }
                    else StartAnimation(stats.ani_runStart, stats.ani_runEnd, true);
                    break;
                case CharacterState.braking:
                    if(oldState == CharacterState.falling || oldState == CharacterState.jumping)
                    {
                        StartAnimation(stats.ani_landStart, stats.ani_landEnd, false);
                        AddAnimation(stats.ani_brake, stats.ani_brake, true);
                    }
                    else StartAnimation(stats.ani_brake, stats.ani_brake, true);
                    break;
                case CharacterState.jumping:
                    StartAnimation(stats.ani_jumpStart, stats.ani_jumpEnd, false);
                    AddAnimation(stats.ani_fallStart, stats.ani_fallEnd, true);
                    break;
                case CharacterState.falling:
                    StartAnimation(stats.ani_fallStart, stats.ani_fallEnd, true);
                    break;
                case CharacterState.takingHit:
                    StartAnimation(stats.ani_takeHitStart, stats.ani_takeHitEnd, true);
                    break;
                case CharacterState.attacking:
                    StartAnimation(move.AniFrom, move.AniTo, false);
                    if (move.Type == MoveType.Body) Velocity = ((BodyMove)move).BodySpeed;
                    break;
                case CharacterState.shielding:
                    
                    break;
                case CharacterState.chargingHit:
                    break;
                case CharacterState.chargingSuper:
                    break;
                default:
                    break;
            }

            if (newState != CharacterState.running) fps = Constants.FPS;
        }
    }

}
