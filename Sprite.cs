using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// This class is the basis for all sprites in the game, where a sprite
    /// is any graphical object that moves around the screen and collides
    /// with other sprites.
    /// </summary>
    public class Sprite
    {
        /// <summary>
        /// The height of the sprite image (in pixels)
        /// </summary>
        public int Height
        {
            get { return height; }
        }
        private int height;
        /// <summary>
        /// The width of the sprite image (in pixels)
        /// </summary>
        public int Width
        {
            get { return width; }
        }
        private int width;

        /// <summary>
        /// The list of different textures that the sprite can display
        /// </summary>
        public List<Texture2D> Texture
        {
            get { return texture; }
            set { texture = value; }
        }
        private List<Texture2D> texture;

        /// <summary>
        /// The direction that the sprite is currently facing
        /// </summary>
        public SpriteEffects Direction
        {
            get { return direction; }
            set { direction = value; }
        }
        private SpriteEffects direction;

        /// <summary>
        /// The rectangle that the sprite will be drawn into
        /// </summary>
        public Rectangle Rect
        {
            get { return rect; }
            set { rect = value; }
        }
        private Rectangle rect;
        /// <summary>
        /// The sprite's displacement from its starting postion in both x and y directions
        /// </summary>
        public Vector2 Displacement
        {
            get { return displacement; }
            set { displacement = value; }
        }
        private Vector2 displacement;
        /// <summary>
        /// The sprite's velocity in both x and y directions
        /// </summary>
        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }
        private Vector2 velocity;

        /// <summary>
        /// The current animation frame that the sprite is displaying
        /// </summary>
        public int AniFrame
        {
            get { return aniFrame; }
            set { aniFrame = value; }
        }
        private int aniFrame;
        /// <summary>
        /// The time remaining until the animation will change frames
        /// </summary>
        public double AniTime
        {
            get { return aniTime; }
            set { aniTime = value; }
        }
        private double aniTime;

        /// <summary>
        /// The length of time that the sprite has been in the air (when jumping)
        /// </summary>
        public double TimeInAir
        {
            get { return timeInAir; }
            set { timeInAir = value; }
        }
        private double timeInAir;

        /// <summary>
        /// This constructor creates a Sprite with a given height and width.
        /// </summary>
        /// <param name="w">The given width.</param>
        /// <param name="h">The given height.</param>
        public Sprite(int w, int h)
        {
            // Assigning the height and width to the given values.
            width = w;
            height = h;
        }
    }
}
