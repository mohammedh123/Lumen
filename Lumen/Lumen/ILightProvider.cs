using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Lumen
{
    interface ILightProvider
    {
        Vector2 Position { get; set; }
        
        Color LightColor { get; set; }

        float LightRadius { get; set; }
    }
}
