﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformerProject.Core
{
    interface IInteractable : IGameObject
    {
        Rectangle InteractionArea { get; set; }
        void InteractedWith(ISprite sprite);
    }
}
