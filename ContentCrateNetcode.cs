using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using ContentCrate.Items;

namespace ContentCrate
{
    //Mostly from Calamity so yeah
    public class ContentCrateNetcode
    {
        public static void HandlePacket(Mod mod, BinaryReader reader, int whoAmI)
        {
            try
            {
                ContentCrateMessageType msgType = (ContentCrateMessageType)reader.ReadByte();
                switch (msgType)
                {
                    case ContentCrateMessageType.SyncNPCMotionDataToServer:
                        int npcIndex = reader.ReadInt32();
                        Vector2 center = reader.ReadVector2();
                        Vector2 velocity = reader.ReadVector2();
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Main.npc[npcIndex].Center = center;
                            Main.npc[npcIndex].velocity = velocity;
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npcIndex);
                        }
                        break;
                    default:
                        ContentCrate.Instance.Logger.Error($"Content Crate packet ID {msgType} doesn't exist.");
                        throw new Exception("Content Crate packet ID doesn't exist.");
                        //will add more as I go on
                }
            }
            catch(Exception e)
            {
                if (e is EndOfStreamException eose)
                    ContentCrate.Instance.Logger.Error("Failed to parse ContentCrate packet: Packet was too short, missing data, or otherwise corrupt.", eose);
                else if (e is ObjectDisposedException ode)
                    ContentCrate.Instance.Logger.Error("Failed to parse ContentCrate packet: Packet reader disposed or destroyed.", ode);
                else if (e is IOException ioe)
                    ContentCrate.Instance.Logger.Error("Failed to parse ContentCrate packet: An unknown I/O error occurred.", ioe);
                else
                    throw e; // this either will crash the game or be caught by TML's packet policing
            }
        }
    }

    public enum ContentCrateMessageType : byte
    {
        SyncNPCMotionDataToServer
    }
}
