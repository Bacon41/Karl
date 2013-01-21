using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace InClassDemo
{
    /// <summary>
    /// This is the main class that contains most of the bulk of the code.
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        // These are the default objects created
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // Setting the size of the screen
        const int SCREEN_WIDTH = 800;
        const int SCREEN_HEIGHT = 600;

        // Declaring an object to create randomness
        Random rand;

        // Two objects to hold the current keyboard and the old keyboard, to check for just pressed
        KeyboardState currentKeyboard;
        KeyboardState oldKeyboard;
        // The same for the gamepad
        GamePadState currentGamepad;
        GamePadState oldGamepad;

        // The level that the player is currently on
        int level;

        // Values to store if the player is currently moving side-to-side, or up-and-down, or is dead, or if the game is paused
        bool keyPressed;
        bool jumping;
        bool dead;
        bool paused;

        // An object to hold the font for writing to the screen
        SpriteFont font;

        // Some objects to hold the background images
        Texture2D background;
        Texture2D gameoverBackground;

        // Objects for all of the moving sprites that will be on the screen
        Sprite karl;
        List<Sprite> enemies;
        List<Sprite> bullets;

        /// <summary>
        /// This is the default created constructor, simply setting a couple of values.
        /// </summary>
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// This initializes all of the objects in the game that are not created from loading content,
        /// such as KeyboardState objects or Sprites.
        /// </summary>
        protected override void Initialize()
        {
            // Setting the window to the sizes that we defined earlier
            graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
            graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;
            graphics.ApplyChanges();

            // Setting the current level
            level = 1;

            // Initializng the Random object
            rand = new Random();

            // Initializing the two KeyboardState objects to be the same
            currentKeyboard = Keyboard.GetState();
            oldKeyboard = Keyboard.GetState();

            // Telling the game that the player is not moving right now and is alive, and the game is not paused
            keyPressed = false;
            jumping = false;
            dead = false;
            paused = false;

            // Initializing the player's object with its dimensions
            karl = new Sprite(80, 100);
            // Creating the list of Sprite objects for the enemies and bullets so their size can vary
            enemies = new List<Sprite>();
            bullets = new List<Sprite>();

            // Calling the method to enumerate components and initialize them
            base.Initialize();
        }

        /// <summary>
        /// This is where all of the graphical content is loaded, and where the player's object's
        /// specific values are initialized.
        /// </summary>
        protected override void LoadContent()
        {
            // Initialize the SpriteBatch, which is used to draw textures
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Loading what font we want our object to use
            font = Content.Load<SpriteFont>("Font");

            // Loading in the background textures from our content
            background = Content.Load<Texture2D>("gameBackground");
            gameoverBackground = Content.Load<Texture2D>("gameoverBackground");

            // Initializing the texture list and loading in all of the required textures
            karl.Texture = new List<Texture2D>();
            karl.Texture.Add(Content.Load<Texture2D>("karlStand"));
            karl.Texture.Add(Content.Load<Texture2D>("karlWalk1"));
            karl.Texture.Add(Content.Load<Texture2D>("karlWalk2"));
            // Setting the starting location of the player, as well as the player's size that we gave it earlier
            karl.Rect = new Rectangle((SCREEN_WIDTH - karl.Width) / 2, SCREEN_HEIGHT - karl.Height, karl.Width, karl.Height);
            // Setting the intitial displacement to zero
            karl.Displacement = Vector2.Zero;
            // Setting the velocity in both the x and y directons, as well as the initial way the player is facing
            karl.Velocity = new Vector2(3, 700);
            karl.Direction = SpriteEffects.None;
            // Setting the starting image to be the standing image
            karl.AniFrame = 0;
            // Setting the two counters to be zero
            karl.AniTime = 0;
            karl.TimeInAir = 0;

            // Creating one enemy
            addEnemy();
        }

        /// <summary>
        /// This builds an enemy Sprite object, and initializes all of its values to be correct.
        /// </summary>
        protected void addEnemy()
        {
            // Adding the enenmy Sprite to the list with the correct dimensions
            enemies.Add(new Sprite(80, 80));

            // Finding the index of the current object that was just created
            int x = enemies.Count - 1;
            // Setting the starting angle that the enemy will move on
            double theta = rand.NextDouble() * Math.PI / 2 + Math.PI / 4;

            // Initializing the texture list and loading in all of the required textures
            enemies[x].Texture = new List<Texture2D>();
            enemies[x].Texture.Add(Content.Load<Texture2D>("BadBox0000"));
            enemies[x].Texture.Add(Content.Load<Texture2D>("BadBox0001"));
            // Setting the starting location of the enemy, as well as the enemy's size that we gave it earlier
            enemies[x].Rect = new Rectangle((SCREEN_WIDTH - enemies[x].Width) / 2, 50, enemies[x].Width, enemies[x].Height);
            // Setting the intitial displacement to zero
            enemies[x].Displacement = Vector2.Zero;
            // Setting the velocity in both the x and y directons, as well as the initial way the enemy is facing
            enemies[x].Velocity = new Vector2((float)(4 * Math.Cos(theta)), (float)(4 * Math.Sin(theta)));
            enemies[x].Direction = SpriteEffects.None;
            // Setting the starting image to be the first image
            enemies[x].AniFrame = 0;
            // Setting the initial time to the next animation switch to be at its maximum
            enemies[x].AniTime = 0;
        }

        /// <summary>
        /// This builds an bullet Sprite object, and initializes all of its values to be correct.
        /// </summary>
        protected void addBullet()
        {
            // Adding the bullet Sprite to the list with the correct dimensions
            bullets.Add(new Sprite(20, 20));

            // Finding the index of the current object that was just created
            int x = bullets.Count - 1;

            // Initializing the texture list and loading in all of the required textures
            bullets[x].Texture = new List<Texture2D>();
            bullets[x].Texture.Add(Content.Load<Texture2D>("Fireball0000"));
            bullets[x].Texture.Add(Content.Load<Texture2D>("Fireball0001"));
            bullets[x].Texture.Add(Content.Load<Texture2D>("Fireball0002"));
            bullets[x].Texture.Add(Content.Load<Texture2D>("Fireball0003"));

            // If the player is facing to the right, create the bullet on the sprite's right with a rightward velocity
            if (karl.Direction == SpriteEffects.FlipHorizontally)
            {
                bullets[x].Rect = new Rectangle(karl.Rect.X + karl.Width, karl.Rect.Y + 40, bullets[x].Width, bullets[x].Height);
                bullets[x].Velocity = new Vector2(5, 0);
            }
            // If the player is facing to the left, create the bullet on the sprite's left with a leftward velocity
            else
            {
                bullets[x].Rect = new Rectangle(karl.Rect.X - bullets[x].Width, karl.Rect.Y + 40, bullets[x].Width, bullets[x].Height);
                bullets[x].Velocity = new Vector2(-5, 0);
            }
            // Setting the intitial displacement to zero and direction to be normal
            bullets[x].Displacement = Vector2.Zero;
            bullets[x].Direction = SpriteEffects.None;
            // Setting the starting image to be the first image
            bullets[x].AniFrame = 0;
            // Setting the initial time to the next animation switch to be at its maximum
            bullets[x].AniTime = 0;
        }

        /// <summary>
        /// This is used to unload any loaded content (in the case of a graphics reload such as
        /// a resolution change).
        /// </summary>
        protected override void UnloadContent()
        {
            // This will unload all of the content loaded by the Content object
            Content.Unload();
        }

        /// <summary>
        /// This is the method that is called as the main part of the game's loop.
        /// All work or changes will be routed through here.
        /// </summary>
        /// <param name="gameTime">Contains the timeing values (such as time since last update).</param>
        protected override void Update(GameTime gameTime)
        {
            // Setting the current KeyboardState and GamepadState to be right now
            currentKeyboard = Keyboard.GetState();
            currentGamepad = GamePad.GetState(PlayerIndex.One);

            // How to pause (when the Back button or Esc key are pressed)
            if ((currentKeyboard.IsKeyDown(Keys.Escape) && !oldKeyboard.IsKeyDown(Keys.Escape))
                || (currentGamepad.IsButtonDown(Buttons.Start) && !oldGamepad.IsButtonDown(Buttons.Start)))
            {
                if (!dead)
                {
                    paused = !paused;
                }
            }

            // If the player is still alive
            if (!dead && !paused)
            {
                // If the Left key is pressed, move the player to the left
                if (currentKeyboard.IsKeyDown(Keys.Left)
                    || currentGamepad.ThumbSticks.Left.X < -.1)
                {
                    // Move the x coordinate of the sprite to the left
                    Rectangle temp = karl.Rect;
                    temp.X -= (int)karl.Velocity.X;
                    karl.Rect = temp;

                    // Set the sprite to face to the left
                    karl.Direction = SpriteEffects.None;

                    // If this is the first call to update that the key is pressed, mark that the player is moving
                    if (!oldKeyboard.IsKeyDown(Keys.Left)
                        && !(oldGamepad.ThumbSticks.Left.X < -.1))
                    {
                        keyPressed = true;
                        karl.AniFrame = 1;
                    }
                }
                // If the Right key is pressed, move the player to the right
                else if (currentKeyboard.IsKeyDown(Keys.Right)
                    || currentGamepad.ThumbSticks.Left.X > .1)
                {
                    // Move the x coordinate of the sprite to the right
                    Rectangle temp = karl.Rect;
                    temp.X += (int)karl.Velocity.X;
                    karl.Rect = temp;

                    // Set the sprite to face to the right
                    karl.Direction = SpriteEffects.FlipHorizontally;

                    // If this is the first call to update that the key is pressed, mark that the player is moving
                    if (!oldKeyboard.IsKeyDown(Keys.Right)
                        && !(oldGamepad.ThumbSticks.Left.X > .1))
                    {
                        keyPressed = true;
                        karl.AniFrame = 1;
                    }
                }
                // If the player is not moving left or right, mark that down
                else
                {
                    keyPressed = false;
                }

                // If the Up key is pressed and the player is on the ground, mark that the player is jumping
                if ((currentKeyboard.IsKeyDown(Keys.Up) || currentGamepad.IsButtonDown(Buttons.A))
                    && karl.Rect.Y + karl.Height == SCREEN_HEIGHT)
                {
                    jumping = true;
                }

                // If the player is jumping, track how long the player has been in the air and use that to find its current height
                if (jumping)
                {
                    karl.TimeInAir += gameTime.ElapsedGameTime.TotalSeconds;
                    Rectangle temp = karl.Rect;
                    temp.Y = (int)((SCREEN_HEIGHT - karl.Height) - karl.Velocity.Y * karl.TimeInAir + 490 * karl.TimeInAir * karl.TimeInAir);
                    karl.Rect = temp;
                }

                // If the player is moving
                if (keyPressed)
                {
                    // Track how long the player has had the same animation image
                    karl.AniTime += gameTime.ElapsedGameTime.TotalSeconds;
                    // If it has had the same animation for long enough, switch it for the next one are reset the counter
                    if (karl.AniTime > .1)
                    {
                        karl.AniFrame = (karl.AniFrame % (karl.Texture.Count - 1)) + 1;
                        karl.AniTime = 0;
                    }
                }
                // If the player isn't moving, set its animation to be the standing image
                else
                {
                    karl.AniFrame = 0;
                    karl.AniTime = 0;
                }

                // Check to see that the player isn't out of bounds
                checkBoundries(karl, false);

                // If the Space key is pressed for the first time, create a bullet
                if ((currentKeyboard.IsKeyDown(Keys.Space) && !oldKeyboard.IsKeyDown(Keys.Space))
                    || (currentGamepad.IsButtonDown(Buttons.X) && !oldGamepad.IsButtonDown(Buttons.X)))
                {
                    addBullet();
                }

                // Look at all of the bullets
                for (int x = 0; x < bullets.Count; x++)
                {
                    // Move the x coordinate of the sprite in its current direction
                    Rectangle temp = bullets[x].Rect;
                    temp.X += (int)bullets[x].Velocity.X;
                    bullets[x].Rect = temp;

                    // Track how long the bullet has had the same animation image
                    bullets[x].AniTime += gameTime.ElapsedGameTime.TotalSeconds;
                    // If it has had the same animation for long enough, switch it for the next one are reset the counter
                    if (bullets[x].AniTime > .2)
                    {
                        bullets[x].AniFrame++;
                        bullets[x].AniFrame = (bullets[x].AniFrame % (bullets[x].Texture.Count - 1));
                        bullets[x].AniTime = 0;
                    }

                    // Check to see that the bullet isn't out of bounds
                    checkBoundries(bullets[x], true);
                }

                // Look at all of the enemies
                for (int x = 0; x < enemies.Count; x++)
                {
                    // Change the enemy's displacement based on its velocity components
                    Vector2 tempV = enemies[x].Displacement;
                    tempV.X += enemies[x].Velocity.X;
                    tempV.Y += enemies[x].Velocity.Y;
                    enemies[x].Displacement = tempV;

                    // Using its displacement and starting positions, move the enemy's sprite
                    Rectangle tempR = enemies[x].Rect;
                    tempR.X = (SCREEN_WIDTH - enemies[x].Width) / 2 + (int)enemies[x].Displacement.X;
                    tempR.Y = 50 + (int)enemies[x].Displacement.Y;
                    enemies[x].Rect = tempR;

                    // Track how long the bullet has had the same animation image
                    enemies[x].AniTime += gameTime.ElapsedGameTime.TotalSeconds;
                    // If it has had the same animation for long enough, switch it for the next one are reset the counter
                    if (enemies[x].AniTime > .2)
                    {
                        enemies[x].AniFrame++;
                        enemies[x].AniFrame = (enemies[x].AniFrame % enemies[x].Texture.Count);
                        enemies[x].AniTime = 0;
                    }

                    // Check to see that the enemy isn't out of bounds
                    checkBoundries(enemies[x], false);

                    // If an enemy collides with the player, exit the game
                    if (enemies[x].Rect.Intersects(karl.Rect))
                    {
                        dead = true;
                    }

                    // Look at all of the bullets
                    for (int y = 0; y < bullets.Count; y++)
                    {
                        // If an enemy collides with a bullet, destroy both of them
                        if (enemies[x].Rect.Intersects(bullets[y].Rect))
                        {
                            enemies.RemoveAt(x);
                            bullets.RemoveAt(y);
                            break;
                        }
                    }
                }

                // If all enemies are dead, go to the next level
                if (enemies.Count == 0)
                {
                    increaseLevel();
                }
            }
            // If the player is dead, exit the game when the user gives input
            else if (dead)
            {
                if (currentKeyboard.IsKeyDown(Keys.Escape) || currentGamepad.IsButtonDown(Buttons.Back))
                {
                    this.Exit();
                }
                if (currentKeyboard.IsKeyDown(Keys.Space) || currentGamepad.IsButtonDown(Buttons.Start))
                {
                    startOver();
                }
            }

            // Set the previous KeyboardState to be the current one
            oldKeyboard = currentKeyboard;
            oldGamepad = currentGamepad;

            // Update again
            base.Update(gameTime);
        }

        /// <summary>
        /// This checks to see if the given sprite is outside of the screen, and if it is,
        /// it moves it back inside.
        /// </summary>
        /// <param name="sprite">This is the sprite that will be tested.</param>
        /// <param name="bullet">This lets the method know if the sprite is for a bullet.</param>
        protected void checkBoundries(Sprite sprite, bool bullet)
        {
            // If the sprite is too far to the left
            if (sprite.Rect.X < 0)
            {
                // Move the x coordinate of the sprite back inside
                Rectangle tempR = sprite.Rect;
                tempR.X = 0;
                sprite.Rect = tempR;

                // If the sprite isn't karl
                if (sprite != karl)
                {
                    // Change the x velocity to the opposite direction
                    Vector2 tempV = sprite.Velocity;
                    tempV.X *= -1;
                    sprite.Velocity = tempV;
                }

                // If the sprite is a bullet, destory it
                if (bullet)
                {
                    bullets.Remove(sprite);
                }
            }
            // If the sprite is too far to the right
            if (sprite.Rect.X + sprite.Width > SCREEN_WIDTH)
            {
                // Move the x coordinate of the sprite back inside
                Rectangle temp = sprite.Rect;
                temp.X = SCREEN_WIDTH - sprite.Width;
                sprite.Rect = temp;

                // If the sprite isn't karl
                if (sprite != karl)
                {
                    // Change the x velocity to the opposite direction
                    Vector2 tempV = sprite.Velocity;
                    tempV.X *= -1;
                    sprite.Velocity = tempV;
                }

                // If the sprite is a bullet, destory it
                if (bullet)
                {
                    bullets.Remove(sprite);
                }
            }
            // If the sprite is too far over the top of the screen
            if (sprite.Rect.Y < 0)
            {
                // Move the y coordinate of the sprite back inside
                Rectangle temp = sprite.Rect;
                temp.Y = 0;
                sprite.Rect = temp;

                // If the sprite isn't karl
                if (sprite != karl)
                {
                    // Change the y velocity to the opposite direction
                    Vector2 tempV = sprite.Velocity;
                    tempV.Y *= -1;
                    sprite.Velocity = tempV;
                }
            }
            // If the sprite is too far under the bottom of the screen
            if (sprite.Rect.Y + sprite.Height > SCREEN_HEIGHT)
            {
                // Move the y coordinate of the sprite back inside
                Rectangle temp = sprite.Rect;
                temp.Y = SCREEN_HEIGHT - sprite.Height;
                sprite.Rect = temp;

                // If the sprite isn't karl
                if (sprite != karl)
                {
                    // Change the y velocity to the opposite direction
                    Vector2 tempV = sprite.Velocity;
                    tempV.Y *= -1;
                    sprite.Velocity = tempV;
                }

                // If the sprite is the player, tell the game that it is not jumping currently
                if (sprite.Equals(karl))
                {
                    jumping = false;
                    sprite.TimeInAir = 0;
                }
            }
        }

        /// <summary>
        /// This changes the level number to one higher and adds the appropriate
        /// number of enemies.
        /// </summary>
        protected void increaseLevel()
        {
            // Increment the level
            level++;

            // Add the same number of enemies as the level number
            for (int x = 0; x < level; x++)
            {
                addEnemy();
            }
        }

        /// <summary>
        /// This is the method that will set the level back to it's beginning if people want to start over.
        /// </summary>
        protected void startOver()
        {
            // Recreating the list of Sprite objects for the enemies and bullets so they are empty
            enemies = new List<Sprite>();
            bullets = new List<Sprite>();

            // Setting the starting location of the player, as well as the player's size that we gave it earlier
            karl.Rect = new Rectangle((SCREEN_WIDTH - karl.Width) / 2, SCREEN_HEIGHT - karl.Height, karl.Width, karl.Height);
            // Setting the intitial displacement to zero
            karl.Displacement = Vector2.Zero;
            // Setting the velocity in both the x and y directons, as well as the initial way the player is facing
            karl.Velocity = new Vector2(3, 700);
            karl.Direction = SpriteEffects.None;
            // Setting the starting image to be the standing image
            karl.AniFrame = 0;
            // Setting the two counters to be zero
            karl.AniTime = 0;
            karl.TimeInAir = 0;

            // Setting the game variables to be back to their default position
            jumping = false;
            level = 0;
            dead = false;
        }

        /// <summary>
        /// This is the method that contains all of the drawing code.
        /// </summary>
        /// <param name="gameTime">Contains the timeing values (such as time since last update).</param>
        protected override void Draw(GameTime gameTime)
        {
            // Clears the drawing screen and colors it
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Starting the ability to draw to the screen
            spriteBatch.Begin();

            // If the player is still alive
            if (!dead)
            {
                // Draw the background image at the back
                spriteBatch.Draw(background, new Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT), Color.White);
                // Draw karl in the correct location ontop of the background
                spriteBatch.Draw(karl.Texture[karl.AniFrame], karl.Rect, new Rectangle(0, 0, karl.Width, karl.Height), Color.White, 0f, new Vector2(0, 0), karl.Direction, 0f);
                // Draw all of the enemies in the correct location ontop of the background
                foreach (Sprite enemy in enemies)
                {
                    // Draw(Texture2D, destination Rectangle, source Rectangle, tinting Color,
                    // rotation Float, origin Vector2, direction SpriteEffects, layer Float)
                    spriteBatch.Draw(enemy.Texture[enemy.AniFrame], enemy.Rect, null, Color.White, 0f, new Vector2(0, 0), enemy.Direction, 0f);
                }
                // Draw all of the bullets in the correct location ontop of the background
                foreach (Sprite bullet in bullets)
                {
                    spriteBatch.Draw(bullet.Texture[bullet.AniFrame], bullet.Rect, null, Color.White, 0f, new Vector2(0, 0), bullet.Direction, 0f);
                }
                // Draw the level counter in the top left corner ontop of everything else, using the font object
                spriteBatch.DrawString(font, "Level: " + level, new Vector2(10, 10), Color.Black);

                if (paused)
                {
                    // Draw the paused in the center ontop of everything else, using the font object
                    spriteBatch.DrawString(font, "PAUSED", new Vector2(SCREEN_WIDTH / 2 - 45, SCREEN_HEIGHT / 2 + 20), Color.Black);
                }
            }
            // If the player is dead
            else
            {
                // Draw the background image at the back
                spriteBatch.Draw(gameoverBackground, new Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT), Color.White);

                // Draw the game over and level number in the center ontop of everything else, using the font object
                spriteBatch.DrawString(font, "Game Over!", new Vector2(SCREEN_WIDTH / 2 - 55, SCREEN_HEIGHT / 2), Color.Black);
                spriteBatch.DrawString(font, "Level: " + level, new Vector2(SCREEN_WIDTH / 2 - 45, SCREEN_HEIGHT / 2 + 20), Color.Black);
                spriteBatch.DrawString(font, "Press Esc or Back to Exit", new Vector2(SCREEN_WIDTH / 2 - 135, SCREEN_HEIGHT / 2 + 40), Color.Black);
                spriteBatch.DrawString(font, "Press Space or Start to Restart", new Vector2(SCREEN_WIDTH / 2 - 165, SCREEN_HEIGHT / 2 + 60), Color.Black);
            }

            // Ending the ability to draw to the screen
            spriteBatch.End();

            // Draw again
            base.Draw(gameTime);
        }
    }
}
