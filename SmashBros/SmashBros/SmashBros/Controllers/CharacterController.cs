using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmashBros.Model;
using Microsoft.Xna.Framework;
using SmashBros.System;
using Microsoft.Xna.Framework.Content;
using SmashBros.Views;

namespace SmashBros.Controllers
{
    public enum CharacterState { none, jumping, running, attacking }

    class CharacterController : Controller
    {
        /// <summary>
        /// Currently chosen character of this player.
        /// </summary>
        public Sprite character;

        /// <summary>
        /// The state of which the player is currently in. (jumping, running, etc.)
        /// </summary>
        public CharacterState state;

        /// <summary>
        /// Vector that shows the direction and magnitude of the current speed of the player.
        /// </summary>

        /// <summary>
        /// An int that describes how much damage the player has taken in the current game/characterlife.
        /// </summary>
        public int damagePoints;

        /// <summary>
        /// Number of lifes left.
        /// </summary>
        public int lives;

        public Vector2 position;

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

        public GamepadController pad;

        public CharacterController(ScreenController screen, GamepadController pad) 
            : base(screen)
        {
            this.pad = pad;
        }

        public override void Load(ContentManager content)
        {
            character = new Sprite(content, "spiderman", 100, 100, 200, 200);
            character.BoundRect(World, 100, 100);
            AddView(character);

            pad.OnNavigation += OnNavigation;
        }

        public override void Unload()
        {
        }

        public override void Update(GameTime gameTime)
        {
            switch (state)
            {
                case CharacterState.jumping:
                    //Only collid with solide ground while jumping uppwards
                    if (position.Y < character.Position.Y)
                    {

                    }
                    else if (position.Y == character.Position.Y)
                    {
                        state = CharacterState.none;
                    }
                    break;
            }

            position = character.Position;
        }

        public override void OnNext(GameStateManager value)
        {
        }

        public override void Deactivate()
        {
        }

        private void OnNavigation(float directionX, float directionY, int playerIndex)
        {
            if (state == CharacterState.jumping)
            {
                character.ForceX = 20 * directionX;
            }
            else
            {
                character.ForceX = 50 * directionX;

                if (directionY < 0)
                {
                    state = CharacterState.jumping;
                    character.Impulse = new Vector2(0, -30);
                }
            }
        }
    }
}
