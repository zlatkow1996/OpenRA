#region Copyright & License Information
/*
 * Copyright 2007,2009,2010 Chris Forbes, Robert Pepperell, Matthew Bowra-Dean, Paul Chote, Alli Witheford.
 * This file is part of OpenRA.
 * 
 *  OpenRA is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 * 
 *  OpenRA is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 * 
 *  You should have received a copy of the GNU General Public License
 *  along with OpenRA.  If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System;
using System.Drawing;
using System.Linq;
using OpenRA.Graphics;
using OpenRA.Traits;

namespace OpenRA.Widgets
{
	public class WorldTooltipWidget : Widget
	{
		const int worldTooltipDelay = 10;		/* ticks */

		public WorldTooltipWidget() : base() { }
		
		public override void DrawInner(World world)
		{
			if (Game.chrome.ticksSinceLastMove < worldTooltipDelay || world == null || world.LocalPlayer == null)
				return;

			var actor = world.FindUnitsAtMouse(Game.chrome.lastMousePos).FirstOrDefault();
			if (actor == null) return;

			var text = actor.Info.Traits.Contains<ValuedInfo>()
				? actor.Info.Traits.Get<ValuedInfo>().Description
				: actor.Info.Name;
			var text2 = (actor.Owner.NonCombatant)
				? "" : "{0}".F(actor.Owner.PlayerName);
			var text3 = (actor.Owner == world.LocalPlayer || actor.Owner.NonCombatant)
				? "" : " ({0})".F(world.LocalPlayer.Stances[actor.Owner]);
			var renderer = Game.chrome.renderer;

			var sz = renderer.BoldFont.Measure(text);
			var sz2 = renderer.RegularFont.Measure(text2);
			var sz3 = renderer.RegularFont.Measure(text3);
						
			sz.X = Math.Max(sz.X, sz2.X + sz3.X + 35);

			if (text2 != "") sz.Y += sz2.Y + 2;

			sz.X += 20;
			sz.Y += 24;

			WidgetUtils.DrawPanel("dialog4", Rectangle.FromLTRB(
				Game.chrome.lastMousePos.X + 20, Game.chrome.lastMousePos.Y + 20,
				Game.chrome.lastMousePos.X + sz.X + 20, Game.chrome.lastMousePos.Y + sz.Y + 20));

			renderer.BoldFont.DrawText(text,
				new float2(Game.chrome.lastMousePos.X + 30, Game.chrome.lastMousePos.Y + 30), Color.White);
			
			if (text2 != "")
			{
				renderer.RegularFont.DrawText(text2,
					new float2(Game.chrome.lastMousePos.X + 65, Game.chrome.lastMousePos.Y + 50), actor.Owner.Color);
				
				renderer.RegularFont.DrawText(text3,
					new float2(Game.chrome.lastMousePos.X + 65 + sz2.X, Game.chrome.lastMousePos.Y + 50), Color.White);

				WidgetUtils.DrawRGBA(
					ChromeProvider.GetImage(Game.chrome.renderer, "flags", actor.Owner.Country.Race),
					new float2(Game.chrome.lastMousePos.X + 30, Game.chrome.lastMousePos.Y + 50));
			}
			
			renderer.RgbaSpriteRenderer.Flush();
		}
	}
}
