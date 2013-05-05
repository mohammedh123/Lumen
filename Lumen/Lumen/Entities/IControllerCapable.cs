using Microsoft.Xna.Framework;

namespace Lumen.Entities
{
    internal interface IControllerCapable
    {
        PlayerIndex ControllerIndex { get; set; }

        void ProcessControllerInput(float dt);
    }
}