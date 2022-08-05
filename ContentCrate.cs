using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.GameContent.Shaders;
using Terraria.GameContent.Dyes;
using Terraria.GameContent.UI;
using Terraria.Graphics;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.UI;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;
using Terraria.Utilities;
using Terraria.UI.Chat;
using System.Collections.Concurrent;
using System.Linq;
using static Terraria.ModLoader.Core.TmodFile;
using ContentCrate.Effects;

namespace ContentCrate
{
	public class ContentCrate : Mod
	{
		public static ContentCrate Instance;
		public ContentCrate()
        {
			Instance = this;
        }

        public override void Load()
        {
			ContentCrateShaders.LoadShaders();
        }
        public override void AddRecipeGroups()/* tModPorter Note: Removed. Use ModSystem.AddRecipeGroups */
        {
			RecipeGroup BaseGroup(object GroupName, int[] Items)
			{
				string Name = "";
				switch (GroupName)
				{
					case int i: //modcontent items
						Name += Lang.GetItemNameValue((int)GroupName);
						break;
					case short s: //vanilla item ids
						Name += Lang.GetItemNameValue((short)GroupName);
						break;
					default: //custom group names
						Name += GroupName.ToString();
						break;
				}

				return new RecipeGroup(() => Language.GetTextValue("LegacyMisc.37") + " " + Name, Items);
			}

			RecipeGroup.RegisterGroup("ContentCrate:CopperBars", BaseGroup(ItemID.CopperBar, new int[]
			{
				ItemID.CopperBar,
				ItemID.TinBar
			}));

			RecipeGroup.RegisterGroup("ContentCrate:SilverBars", BaseGroup(ItemID.SilverBar, new int[]
			{
				ItemID.SilverBar,
				ItemID.TungstenBar
			}));

			RecipeGroup.RegisterGroup("ContentCrate:GoldBars", BaseGroup(ItemID.GoldBar, new int[]
			{
				ItemID.GoldBar,
				ItemID.PlatinumBar
			}));
		}
	}
}