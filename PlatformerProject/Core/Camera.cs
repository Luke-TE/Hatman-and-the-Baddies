using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformerProject.Core
{
    /// <summary>
    /// Camera for changing what is drawn in the window and how it is drawn
    /// </summary>
    class Camera
    {
        #region Fields

        static Random random;

        #endregion

        #region Properties

        public Matrix Transform { get; private set; }
        public int Rumbleness { get; set; }

        #endregion

        #region Methods

        static Camera()
        {
            random = new Random();
        }

        public Camera()
        {            
            Rumbleness = 0;
        }

        public void Follow(IGameObject target)
        {
            //Allows camera to follow the target
            var position = Matrix.CreateTranslation(
                -target.Position.X + random.Next(-Rumbleness, Rumbleness + 1),
                -target.Position.Y + random.Next(-Rumbleness, Rumbleness + 1),
                0);

            //Offsets the camera to centre the target on the screen
            var offset = Matrix.CreateTranslation(
                    Stage1.ScreenWidth / 2,
                    Stage1.ScreenHeight / 2,
                    0);

            //Finds the resultant transformation matrix
            Transform = position * offset;
        }

        #endregion
    }
}
