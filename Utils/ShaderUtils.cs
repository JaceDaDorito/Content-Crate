using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ReLogic.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using System.Reflection;

namespace ContentCrate.Utils
{
    public static class ShaderUtils
    {
        internal static readonly FieldInfo UImageFieldMisc = typeof(MiscShaderData).GetField("_uImage1", BindingFlags.NonPublic | BindingFlags.Instance);
        internal static readonly FieldInfo UImageFieldArmor = typeof(ArmorShaderData).GetField("_uImage", BindingFlags.NonPublic | BindingFlags.Instance);
        /// <summary>
        /// Manually sets the texture of a <see cref="MiscShaderData"/> instance, since vanilla's implementation only supports strings that access vanilla textures.
        /// </summary>
        /// <param name="shader">The shader to bind the texture to.</param>
        /// <param name="texture">The texture to bind.</param>
        public static MiscShaderData SetShaderTexture(this MiscShaderData shader, Asset<Texture2D> texture)
        {
            UImageFieldMisc.SetValue(shader, texture);
            return shader;
        }

        /// <summary>
        /// Manually sets the texture of a <see cref="ArmorShaderData"/> instance, since vanilla's implementation only supports strings that access vanilla textures.
        /// </summary>
        /// <param name="shader">The shader to bind the texture to.</param>
        /// <param name="texture">The texture to bind.</param>
        public static ArmorShaderData SetShaderTextureArmor(this ArmorShaderData shader, Asset<Texture2D> texture)
        {
            UImageFieldArmor.SetValue(shader, texture);
            return shader;
        }

        public static void EnterShaderRegion(this SpriteBatch spriteBatch, BlendState newBlendState = null)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, newBlendState ?? BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        }

        public static void ExitShaderRegion(this SpriteBatch spriteBatch)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }
}