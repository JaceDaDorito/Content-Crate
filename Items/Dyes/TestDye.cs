using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using ContentCrate.Utils;

namespace ContentCrate.Items.Dyes
{
    public class TestDye : BaseDye
    {
        public override ArmorShaderData ShaderDataToBind => new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/DyeOneShader", AssetRequestMode.ImmediateLoad).Value), "DyePass").
            UseColor(new Color(199, 56, 185)).UseSecondaryColor(new Color(242, 121, 176)).SetShaderTextureArmor(ModContent.Request<Texture2D>("ContentCrate/Effects/ShaderTextures/EternityStreak", AssetRequestMode.ImmediateLoad));
        public override void SafeSetStaticDefaults()
        {
            SacrificeTotal = 3;
            DisplayName.SetDefault("Test Dye");
            Tooltip.SetDefault("You shouldn't have this");
        }

        public override void SafeSetDefaults()
        {
            Item.rare = ItemRarityID.Purple;
            Item.value = Item.sellPrice(0, 0, 50, 0);
        }

        /*
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.BottledWater).
                AddTile(TileID.DyeVat).
                Register();
        }*/
    }
}
