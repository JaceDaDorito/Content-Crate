using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using System;
using ContentCrate.Utils;
using static ContentCrate.Utils.EaseFunction;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using ReLogic.Content;
using Terraria.Graphics.Shaders;
using Terraria.GameContent.Creative;
using Terraria.Graphics.Effects;
using static Terraria.ModLoader.ModContent;


namespace ContentCrate.Items.Weapons.Swung
{
    public class Halberd : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Diamond Tipped Halberd"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            Tooltip.SetDefault("First spear.");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 13;
            Item.DamageType = DamageClass.Melee;
            Item.width = 52;
            Item.height = 52;
            Item.useTime = 1;
            Item.useAnimation = 12;
            Item.reuseDelay = 1;
            Item.channel = true;
            Item.useStyle = ItemUseStyleID.HiddenAnimation;
            Item.knockBack = 0;
            Item.value = Item.sellPrice(0, 0, 50, 0);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.DD2_MonkStaffSwing;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<HalberdProj>();
            Item.shootSpeed = 1;
            Item.autoReuse = true;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe(1);
            recipe.AddRecipeGroup("ContentCrate:SilverBars", 10);
            recipe.AddIngredient(ItemID.Diamond, 3);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.projectile.Any(n => n.active && n.owner == player.whoAmI && n.type == ModContent.ProjectileType<HalberdProj>()))
                return false;
            return true;

        }
    }

    enum CurrentAttack : int
    {
        Down = 0,
        Up = 1,
        Thrust = 2,
    }


    internal class HalberdProj : ModProjectile
    {


        //public float SwingRadians = MathHelper.Pi * 1.35f;
        private CurrentAttack currentAttack
        {
            get {
                return (CurrentAttack)Projectile.ai[0];
            }

            set {
                Projectile.ai[0] = (int)value;
            }
        }
        private bool FirstTickOfSwing
        {
            get => Projectile.ai[1] == 0;
        }
        //public ref int comboState => ref Projectile.ai[0];

        public Player Player => Main.player[Projectile.owner];
        float attackSpeed => Player.GetTotalAttackSpeed(DamageClass.Melee);
        private float SwingTime
        {
            get
            {
                if (currentAttack != CurrentAttack.Thrust)
                    return 40f * (1 / attackSpeed);
                return 60f;
            }
        }

        private bool buffer;

        private bool initialized = false;
        //Vector2 direction = Vector2.Zero;
        float collisionPoint = 0;
        public Vector2 direction => Projectile.velocity.RotatedBy(AngleFromBase(progress) * Player.direction);
        public int mirror => Math.Sign(Projectile.velocity.X) <= 0 ? -1 : 1;
        public float baseRotation => Projectile.velocity.ToRotation(); 
        private float progress => (SwingTime - Projectile.timeLeft) / (float)SwingTime;

        public CurveSegment SwingAnti = new(SineOutEasing, 0f, -0.7f, -0.4f, 0);
        public CurveSegment SwingAcce = new(PolyInOutEasing, 0.1f, -1.2f, 2f, 3);

        public float SwingDisplace(float prog) => PiecewiseAnimation(prog, new CurveSegment[] { SwingAnti, SwingAcce });

        //stab
        public CurveSegment StabAnti = new(SineBumpEasing, 0f, 0f, -0.1f, 0);
        public CurveSegment StabThrust = new(PolyOutEasing, 0.1f, 0f, 1f, 6);
        public CurveSegment StabRetract = new(PolyInEasing, 0.6f, 1f, -0.5f, 3);

        public float StabDisplace(float prog) => PiecewiseAnimation(prog, new CurveSegment[] { StabAnti, StabThrust, StabRetract });


        Dust thrustGlow = new Dust();


        public float DistanceFromPlayer
        {
            get
            {
                if (currentAttack != CurrentAttack.Thrust)
                    return 30;
                return 60 * StabDisplace(progress);
            }
        }

        public float AngleFromBase(float prog)
        {
            switch (currentAttack)
            {
                case CurrentAttack.Down:
                    return SwingDisplace(prog) * MathHelper.PiOver2 * Player.gravDir;
                case CurrentAttack.Up:
                    return -SwingDisplace(prog) * MathHelper.PiOver2 * Player.gravDir;
                default:
                    return 0;
            }
        }

        public float sweetSpot = 70f;

        internal PrimitiveTrail TrailDrawer;
        const float BladeLength = 140;
        public float TrailEndProgression
        {
            get
            {
                float endProgression;
                if (progress < 0.75f)
                    endProgression = progress - 0.5f + 0.1f * (progress / 0.75f);

                else
                    endProgression = progress - 0.4f * (1 - (progress - 0.75f) / 0.75f);

                return Math.Clamp(endProgression, 0, 1);
            }
        }

        public Vector2 DirectionAtProgressScuffed(float progress)
        {
            float angleShift = AngleFromBase(progress);

            //Get the coordinates of the angle shift.
            Vector2 anglePoint = angleShift.ToRotationVector2();

            //And back into an angle
            angleShift = anglePoint.ToRotation();

            return (baseRotation + angleShift * mirror).ToRotationVector2();
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Diamond Tipped Halberd");
            // Main.projFrames[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.extraUpdates = 1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.Size = new Vector2(114, 114);
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 16;
            Projectile.ownerHitCheck = true;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Player.MountedCenter, Projectile.Center + direction * 80.7f, 10, ref collisionPoint))
            {
                if (collisionPoint >= sweetSpot && !FirstTickOfSwing)
                {
                    SoundEngine.PlaySound(SoundID.Research);
                }
                return true;
            }
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Player Player = Main.player[Projectile.owner];
            if (target.knockBackResist != 0)
            {
                float adjustedKBRes = (float)Math.Pow(target.knockBackResist, 0.5);
                Vector2 coolKnockback;
                switch (currentAttack)
                {

                    case CurrentAttack.Down:
                        coolKnockback = Vector2.UnitX * (21f - (collisionPoint / 5f)) * adjustedKBRes;
                        if (Math.Sign(Projectile.velocity.X) < 0)
                            coolKnockback *= -1;
                        break;
                    case CurrentAttack.Up:
                        coolKnockback.X = 0f;
                        coolKnockback.Y = -9f * adjustedKBRes;
                        break;
                    default:
                        coolKnockback = Projectile.velocity * 10f/*(27f - (collisionPoint / 4.8f))*/ * adjustedKBRes;
                        break;
                }
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.position, coolKnockback, ModContent.ProjectileType<HalberdCoolKB>(), 0, 0, Main.myPlayer, target.whoAmI);
            }
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            Rectangle targetHitbox = target.Hitbox;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Player.MountedCenter, Projectile.Center + direction * 80.7f, 10, ref collisionPoint))
            {
                if (collisionPoint >= sweetSpot && !FirstTickOfSwing)
                {
                    damage = damage * 2;
                }
            }
        }

        public override void ModifyDamageScaling(ref float damageScale)
        {
            base.ModifyDamageScaling(ref damageScale);
        }

        public override bool? CanDamage() => buffer ? false : null;

        public override bool? CanHitNPC(NPC target)
        {
            return base.CanHitNPC(target);
        }

        public override void AI()
        {
            if (buffer)
            {
                if(Player.channel && Projectile.timeLeft < 110)
                    InitializationEffects();
                return;
            }

            Player.itemTime = Player.itemAnimation = 5;
            Player.heldProj = Projectile.whoAmI;

            if (FirstTickOfSwing)
            {
                Projectile.ai[1] = 1;
                InitializationEffects();
            }
            Projectile.rotation = direction.ToRotation() + MathHelper.PiOver4;

            //switch()
            Projectile.Center = Player.MountedCenter + (DistanceFromPlayer * direction);

            Projectile.spriteDirection = 1;
            if (Math.Sign(Projectile.velocity.X) * ((int)currentAttack == 1 ? -1 : 1) < 0)
            {
                Projectile.rotation += MathHelper.PiOver2;
                Projectile.spriteDirection *= -1;
            }
            //Projectile.spriteDirection *= (int)Player.gravDir;



            if (Projectile.timeLeft == 1 && !buffer){
                buffer = true;
                Projectile.timeLeft = 120;
            }


            Player.direction = Math.Sign(Projectile.velocity.X);
            Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, direction.ToRotation() * Player.gravDir - MathHelper.PiOver2 );
        }

        public void InitializationEffects()
        {
            buffer = false;
            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.velocity = Player.MountedCenter.DirectionTo(Main.MouseWorld);
                Projectile.netUpdate = true;
            }

            if(!initialized)
                {

                initialized = true;

                Projectile.velocity.Normalize();
            }
            else
            {
                currentAttack = (CurrentAttack)((int)currentAttack + 1);
                if ((int)currentAttack > 2)
                    currentAttack = CurrentAttack.Down;
            }
            if ((int)currentAttack < 2)
                Projectile.localNPCHitCooldown = 16;
            else
                Projectile.localNPCHitCooldown = 60;

            Projectile.timeLeft = (int)SwingTime;

            Projectile.netUpdate = true;

        }

        public IEnumerable<Vector2> GenerateSlashPoints()
        {
            List<Vector2> result = new();

            for (int i = 0; i < 40; i++)
            {
                float prog = MathHelper.Lerp(progress, TrailEndProgression, i / 40f);

                result.Add(DirectionAtProgressScuffed(prog) * (BladeLength-80) * Projectile.scale);
            }

            return result;
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.CadetBlue * Terraria.Utils.GetLerpValue(1f, 0.2f, completionRatio, true) * Projectile.Opacity * 0.5f;

        }
        public float WidthFunction(float completionRatio) => Projectile.scale * 20f;

        public override bool PreDraw(ref Color lightColor)
        {
            if (buffer)
                return false;

            if (currentAttack != CurrentAttack.Thrust)
            {
                TrailDrawer ??= new(WidthFunction, ColorFunction, PrimitiveTrail.RigidPointRetreivalFunction, GameShaders.Misc["ContentCrate:ExobladeSlash"]);
                DrawSlash();
            }

            return true;
        }

        public void DrawSlash()
        {
            int condition = 1;
            if (currentAttack == CurrentAttack.Up)
                condition = -1;
            
            Main.spriteBatch.EnterShaderRegion();
            GameShaders.Misc["ContentCrate:ExobladeSlash"].SetShaderTexture(ModContent.Request<Texture2D>("ContentCrate/Effects/ShaderTextures/EternityStreak"));
            GameShaders.Misc["ContentCrate:ExobladeSlash"].UseColor(new Color(44, 141, 171));
            GameShaders.Misc["ContentCrate:ExobladeSlash"].UseSecondaryColor(new Color(30, 43, 43));
            GameShaders.Misc["ContentCrate:ExobladeSlash"].Shader.Parameters["fireColor"].SetValue(new Color(163, 36, 110).ToVector3());
            GameShaders.Misc["ContentCrate:ExobladeSlash"].Shader.Parameters["flipped"].SetValue((mirror * condition == 1));
            GameShaders.Misc["ContentCrate:ExobladeSlash"].Apply();

            TrailDrawer.Draw(GenerateSlashPoints(), Projectile.Center - Main.screenPosition, 25);
            Main.spriteBatch.ExitShaderRegion();
        }

    }


    internal class HalberdCoolKB : ModProjectile
    {
        public override string Texture => "ContentCrate/BlankSprite";

        private float enemyWhoAmI
        {
            get => Projectile.ai[0];
        }

        private NPC target => Main.npc[(int)enemyWhoAmI];

        public override void AI()
        {
            if (Main.myPlayer == Projectile.owner)
            {
                target.velocity += Projectile.velocity;
                target.netUpdate = true;
            }

            Projectile.Kill();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }

    /*public class HalberdPacket : Module
    {

    }*/


}