using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Graphics;

namespace ContentCrate.Items.Armor.Myrmeleon
{
    [AutoloadEquip(EquipType.Head)]
    public class MyrmeleonCrown : ModItem, IExtendedHat
    {
        //public Player Player => Main.player[];
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Dune Myrmeleon Crown");
            Tooltip.SetDefault(
                "Increases minion damage by 6%\n" +
                "Increases whip range by 20% speed by 15%");
            if (Main.netMode == NetmodeID.Server)
                return;
            int equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head);
            ArmorIDs.Head.Sets.DrawFullHair[equipSlot] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.defense = 4;
            Item.value = Item.sellPrice(0, 1, 0, 0); ;
            Item.rare = ItemRarityID.Blue;
        }
        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
            player.armorEffectDrawOutlines = true;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<SummonDamageClass>() += 0.06f;
            player.whipRangeMultiplier += 0.2f;
            player.GetAttackSpeed<SummonMeleeSpeedDamageClass>() += 0.15f;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe(1);
            recipe.AddIngredient(ItemID.FossilOre, 15);
            recipe.AddIngredient(ItemID.Ruby, 2);
            recipe.AddIngredient(ItemID.Amber, 2);
            recipe.AddIngredient(ItemID.AntlionMandible, 5);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
        public string ExtensionTexture => "ContentCrate/Items/Armor/Myrmeleon/MyrmeleonCrown_HeadExtension";
        public Vector2 ExtensionSpriteOffset(PlayerDrawSet drawInfo) => new Vector2(0, drawInfo.drawPlayer.gravDir <= 0 ? 54f: -54);
    }
}
