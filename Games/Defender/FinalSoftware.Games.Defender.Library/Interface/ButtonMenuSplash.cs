﻿using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace FinalSoftware.Games.Defender.Library.Interface {

	public class ButtonMenuSplash : ButtonMenu {

		public ButtonMenuSplash(string label, SpriteFont font, Rectangle area, Texture2D texUp, Texture2D texDown, bool down, string tooltip)
			: base(label, font, area, texUp, texDown, down, tooltip) {

		}

	}

}
