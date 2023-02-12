﻿using robotManager.Helpful;
using robotManager.Products;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using wManager.Wow;
using wManager.Wow.Class;
using wManager.Wow.Enums;
using wManager.Wow.Forms;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;
using static System.Net.Mime.MediaTypeNames;
using Timer = robotManager.Helpful.Timer;

class Methods
{
    #region  Properties
    public static WoWPlayer Me { get { return ObjectManager.Me; } }
    public static DateTime _mainTankScan = DateTime.Now;
    public static DateTime _syncFcom = DateTime.Now;
    public static List<WoWPlayer> MainTanks = new List<WoWPlayer>();
    #endregion Properties

    #region Commands
    public static void CMD_Add()
    {
        Main.Path.Add(Me.Position);
        Main.RefreshMiniMap();
    }
    public static void CMD_Delete()
    {
        // exit if Main.Path is empty
        if (Main.Path.Count == 0)
            return;

        Main.Path.RemoveAt(Main.ClosestNode);
        Main.RefreshMiniMap();
        //foreach (Vector3 indice in Main.Path.OrderByDescending(v => v))
        //{

        //}
    }
    public static void CMD_New()
    {
        Methods.LuaPrint(Methods.FormatLua(@"CMD_New() invoked."));

        // Clear Path
        Main.Path.Clear();

        // Add 3 nearby nodes starting from my position
        Main.Path.Add(Me.Position);
        Main.RefreshMiniMap();

        //// if Main.Path is empty, exit
        //if (Main.Path.Count == 0)
        //    return;

        //// iterate through Main.Path for closest node
        //Main.ClosestNode = GetClosestNode();
        ////Methods.LuaPrint("closest node: " + i);

        //// if there are only two nodes
        //if (Main.Path.Count == 2)
        //{
        //    if (Main.ClosestNode == 0)
        //    {
        //        Main.NextClosestNode = 1;
        //    }
        //    else
        //    {
        //        Main.NextClosestNode = 0;
        //    }
        //}

        //// if there are more than two nodes
        //if (Main.Path.Count > 2)
        //{
        //    if (Main.Path[Main.ClosestNode - 1].DistanceTo(Main.Me.Position) > Main.Path[Main.ClosestNode + 1].DistanceTo(Main.Me.Position))
        //    {
        //        Main.NextClosestNode = Main.ClosestNode + 1;
        //    }
        //    else
        //    {
        //        Main.NextClosestNode = Main.ClosestNode - 1;
        //    }
        //}

        //for (int i = 0; i <= Main.Path.Count; i++)
        //{
        //    this.DrawNode(Main.Path[i]);
        //    if (i != Main.Path.Count && i != 0)
        //    {
        //        Radar3D.DrawLine(Main.Path[i - 1], Main.Path[i], Color.Orange, (int)byte.MaxValue);
        //    }
        //}
        Methods.LuaPrint(Methods.FormatLua(@"CMD_New() done."));
    }
    public static void CMD_Insert()
    {
        int bisect = new int();

        // exit if Main.Path does not contain at least two nodes
        if (Main.Path.Count < 2)
            return;

        // determine endpoint to bisect line from
        if (Main.ClosestNode > Main.NextClosestNode)
        {
            bisect = Main.NextClosestNode;
        }
        else
        {
            bisect = Main.ClosestNode;
        }

        // adjust coordinates of closest node to match player position
        Main.Path.Insert(Main.ClosestNode + 1, Main.Me.Position);
        Main.RefreshMiniMap();

    }
    public static void CMD_Minimap()
    {
        

    }
    public static void CMD_Reposition()
    {
        // exit if Main.Path is empty
        if (Main.Path.Count == 0)
            return;

        // adjust coordinates of closest node to match player position

        Main.Path[Main.ClosestNode].X = Main.Me.Position.X;
        Main.Path[Main.ClosestNode].Y = Main.Me.Position.Y;
        Main.Path[Main.ClosestNode].Z = Main.Me.Position.Z;
        Main.RefreshMiniMap();

    }
    #endregion Commands

    #region Integrate

    public static WoWPlayer GetPlayerObject(string name)
    {
        WoWPlayer p = (WoWPlayer)null;
        Parallel.ForEach(ObjectManager.GetObjectWoWPlayer(), delegate (WoWPlayer player)
        {
            if (
            //player.IsAlive &&
            player.Name == name)
            {
                p = player;
            }
        });
        if (p != null && p.Guid > 0UL)
        {
            return p;
        }
        return (WoWPlayer)null;
    }
    public static bool PlayerNameExists(string name)
    {
        WoWPlayer player = ObjectManager.GetObjectWoWPlayer().Where(p =>      // find object from player name string
            p != null
            && p.Name == name
            ).FirstOrDefault();

        if (player != null && player.Guid > 0UL && player.IsValid)
        {
            return true;
        }
        return false;
    }
    public static void UpdateQHMTList()
    {
        //List<WoWPlayer> maintanks = new List<WoWPlayer>();
        List<string> s = GetLuaList("QHV.MTList");
        int size = s.Count();
        //Methods.LuaPrint("size of QHV.MTList: " + size);

        for (int i = 0; i < size; i++)
        {
            //if (Main.Cmd.Debug)
            //{
            //    Methods.LuaPrint(Methods.FormatLua(@"QHV.MTList[{0}]: {1}", i, s[i]));
            //}
            if (Methods.PlayerNameExists(s[i]))
            {
                Methods.LuaPrint(Methods.FormatLua(@"Main tank[{0}] {1} is valid.", i, s[i]));
            }
            Methods.MainTanks.Add(Methods.GetPlayerObject(s[i]));
        }
    }
    #endregion Integrate

    #region Sync
    public static void Sync_Combat()
    {
        if (ObjectManager.Me.InCombatFlagOnly
                || ObjectManager.Me.InCombat
                )
        {
            Main.Combat = true;
            Lua.LuaDoString(Methods.FormatLua(@"OptionRadFrame:SetBackdropColor(1,0.2,0.2,0.8)"));
        }
        else
        {
            Main.Combat = false;
            Lua.LuaDoString(Methods.FormatLua(@"OptionRadFrame:SetBackdropColor(0.2,0.2,0.2,0.8)"));
        }
    }
    #endregion Sync

    #region LuaVars
    public static bool GetLuaBool(string LuaVar)
    {
        string s = "return " + LuaVar + ";";
        return Lua.LuaDoString<bool>(s);
    }
    public static void SetLuaBool(string LuaVar, bool BoolVar)
    {
        string s;
        if (BoolVar)
        {
            s = LuaVar + " = true;";
        }
        else
        {
            s = LuaVar + " = false;";
        }
        Lua.LuaDoString(s);
    }
    public static void SetLuaInt(string LuaVar, int IntVar)
    {
        string s;
        s = LuaVar + " = " + IntVar + ";";
        // JGP DEBUG
        //Methods.LuaPrint(Methods.FormatLua(@"setting wr_range to: {0}", IntVar));
        Lua.LuaDoString(s);
    }
    public static List<string> GetLuaList(string LuaList)
    {
        string s = "return unpack(" + LuaList + ");";
        var mylist = Lua.LuaDoString<List<string>>(s);
        return mylist;
    }
    #endregion LuaVars

    #region Logging
    // Logging
    public static string FormatLua(string str, params object[] names)
    {
        return string.Format(str, names.Select(s => s.ToString().Replace("'", "\\'").Replace("\"", "\\\"")).ToArray());
    }
    public static void LuaPrint(string text, string color = "|cffffffff")
    {
        Lua.LuaDoString("print(\"|cff69ccf0[PE] " + color + text + "\")");
        //Lua.LuaDoString("print(\"|cff69ccf0[WR] " + text + "\")");
        //Lua.LuaDoString("/script DEFAULT_CHAT_FRAME: AddMessage(\"|cff69ccf0[WR] |cffffffff\" .. " + Text + "\")");
    }
    public static void LuaPrintWL(string text, string type = "reg")
    {
        string color = "|cffffffff";
        Lua.LuaDoString("ChatFrame4:AddMessage(\"|cff69ccf0[WR] " + color + text + "\");");
        /*
        switch (type)
        {
            case "com":
                Lua.LuaDoString("ChatFrame4:AddMessage(\"|cff00DDDD" + text + "\");");
                break;
            case "exec":
                Lua.LuaDoString("ChatFrame4:AddMessage(\"|cff00DD00" + text + "\");");
                break;
            case "warn":
                Lua.LuaDoString("ChatFrame4:AddMessage(\"|cffDDDD00" + text + "\");");
                break;
            case "reg":
                Lua.LuaDoString("ChatFrame4:AddMessage(\"|cffFFFFFF" + text + "\");");
                break;
            default:
                Lua.LuaDoString("ChatFrame4:AddMessage(\"|cff69ccf0[WR] " + color + text + "\");");
                break;
        }
        */
    }
    #endregion Logging
}
