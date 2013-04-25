using Microsoft.Xna.Framework;

namespace Lumen.Entities
{
    interface IControllerCapable
    {
        PlayerIndex ControllerIndex { get; set; }

        void ProcessControllerInput(float dt);
    }
}
