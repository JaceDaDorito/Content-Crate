using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;

namespace ContentCrate.Effects
{
    public class ContentCrateShaders
    {
        public static Effect ExobladeSlashShader;
        public static Effect LightningShader;
        public static Effect DyeOne;

        public static void LoadShaders()
        {
            if (Main.dedServ)
                return;

            ExobladeSlashShader = ContentCrate.Instance.Assets.Request<Effect>("Effects/ExobladeSlashShader", AssetRequestMode.ImmediateLoad).Value;
            LightningShader = ContentCrate.Instance.Assets.Request<Effect>("Effects/LightningShader", AssetRequestMode.ImmediateLoad).Value;

            DyeOne = ContentCrate.Instance.Assets.Request<Effect>("Effects/DyeOne", AssetRequestMode.ImmediateLoad).Value;

            GameShaders.Misc["ContentCrate:ExobladeSlash"] = new MiscShaderData(new Ref<Effect>(ExobladeSlashShader), "TrailPass");
            GameShaders.Misc["ContentCrate:LightningShader"] = new MiscShaderData(new Ref<Effect>(LightningShader), "TrailPass");

            GameShaders.Misc["ContentCrate:DyeOne"] = new MiscShaderData(new Ref<Effect>(LightningShader), "TrailPass");
        }
    }
}
