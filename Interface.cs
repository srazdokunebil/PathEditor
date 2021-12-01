using robotManager.Helpful;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using wManager;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;

internal class UserInterface
{
    // Version Header
    //public static string productName = "Radar";
    //public static string productVersion = "0.0";

    public static void InjectLuaFunctions()
    {
        Lua.LuaDoString(string.Format(@"
function radtrigger()
    if rad_hideincombat then
		rad_hideincombat = not rad_hideincombat
			return ""rad_hideincombat""
		end
	if rad_enableradar then
		rad_enableradar = not rad_enableradar
			return ""rad_enableradar""
		end
	if rad_playersdrawui then
		rad_playersdrawui = not rad_playersdrawui
			return ""rad_playersdrawui""
		end
	if rad_playerssound then
		rad_playerssound = not rad_playerssound
			return ""rad_playerssound""
		end
    if rad_playerscorpses then
		rad_playerscorpses = not rad_playerscorpses
			return ""rad_playerscorpses""
		end
	if rad_npcsdrawui then
		rad_npcsdrawui = not rad_npcsdrawui
			return ""rad_npcsdrawui""
		end
	if rad_npcssound then
		rad_npcssound = not rad_npcssound
			return ""rad_npcssound""
		end
	if rad_objectssdrawui then
		rad_objectssdrawui = not rad_objectssdrawui
			return ""rad_objectssdrawui""
		end
	if rad_objectssound then
		rad_objectssound = not rad_objectssound
			return ""rad_objectssound""
		end
	if rad_pvpdrawui then
		rad_pvpdrawui = not rad_pvpdrawui
			return ""rad_pvpdrawui""
		end
	if rad_pvpsound then
		rad_pvpsound = not rad_pvpsound
			return ""rad_pvpsound""
		end
	if rad_map3dme then
		rad_map3dme = not rad_map3dme
			return ""rad_map3dme""
		end
	if rad_map3dtarget then
		rad_map3dtarget = not rad_map3dtarget
			return ""rad_map3dtarget""
		end
	if rad_map3dtargetline then
		rad_map3dtargetline = not rad_map3dtargetline
			return ""rad_map3dtargetline""
		end
	if rad_map3dpath then
		rad_map3dpath = not rad_map3dpath
			return ""rad_map3dpath""
		end
	if rad_map3dnpcs then
		rad_map3dnpcs = not rad_map3dnpcs
			return ""rad_map3dnpcs""
		end
	if rad_map3dplayers then
		rad_map3dplayers = not rad_map3dplayers
			return ""rad_map3dplayers""
		end
	if rad_map3dobjects then
		rad_map3dobjects = not rad_map3dobjects
			return ""rad_map3dobjects""
		end
end
"
        ));
    }
}

public class Command
{
    #region Properties
    public bool DrawObjectLines { get; set; }
    public bool DrawObjectNames { get; set; }
    public bool HideRadarInCombat { get; set; }
    public bool PlaySound { get; set; }
    public bool ShowEnemyPlayers { get; set; }

    public bool EnableRadar { get; set; }
    public bool HideInCombat { get; set; }
    public bool PlayerDrawUI { get; set; }
    public bool PlayerSound { get; set; }
    public bool PlayerCorpses { get; set; }
    public bool NPCsDrawUI { get; set; }
    public bool NPCsSound { get; set; }
    public bool ObjectsDrawUI { get; set; }
    public bool ObjectsSound { get; set; }
    public bool PvPDrawUI { get; set; }
    public bool PvPSound { get; set; }
    public bool Map3DMe { get; set; }
    public bool Map3DTarget { get; set; }
    public bool Map3DTargetLine { get; set; }
    public bool Map3DPath { get; set; }
    public bool Map3DNPCs { get; set; }
    public bool Map3DPlayers { get; set; }
    public bool Map3DObjects { get; set; }

    #endregion Properties

    #region Lua
    public bool GetLuaBool(string LuaVar)
    {
        string s = "return " + LuaVar + ";";
        return Lua.LuaDoString<bool>(s);
    }
    public int GetLuaInt(string LuaVar)
    {
        string s = "return " + LuaVar + ";";
        return Lua.LuaDoString<int>(s);
    }
    public string GetLuaString(string LuaVar)
    {
        string s = "return " + LuaVar + ";";
        return Lua.LuaDoString<string>(s);
    }
    public void SetLuaBool(string LuaVar, bool BoolVar)
    {
        string s = "";
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
    public void SetLuaInt(string LuaVar, int IntVar)
    {
        string s = LuaVar + " = " + IntVar;
        Lua.LuaDoString(s);
    }
    #endregion Lua

    #region Methods
    public void InitLuaVars()
    {
        //if (!Methods.GetLuaBool("wr_running"))
        //{
        //    return;
        //}

        this.DrawObjectLines = Main.DrawObjectLines;
        this.DrawObjectNames = Main.DrawObjectNames;
        this.HideRadarInCombat = Main.HideRadarInCombat;
        this.PlaySound = Main.PlaySound;
        this.ShowEnemyPlayers = Main.ShowEnemyPlayers;
        if (Main.EnableRadar)
        {
            Lua.LuaDoString<bool>("return OptionRadFrame.enableradar_check:SetChecked(true)");
        }
        else
        {
            Lua.LuaDoString<bool>("return OptionRadFrame.enableradar_check:SetChecked(false)");
        }
        this.EnableRadar = Main.EnableRadar;
        if (Main.HideInCombat)
        {
            Lua.LuaDoString<bool>("return OptionRadFrame.hideincombat_check:SetChecked(true)");
        }
        else
        {
            Lua.LuaDoString<bool>("return OptionRadFrame.hideincombat_check:SetChecked(false)");
        }
        this.HideInCombat = Main.HideInCombat;
        if (Main.PlayerDrawUI)
        {
            Lua.LuaDoString<bool>("return OptionRadFrame.playersdrawui_check:SetChecked(true)");
        }
        else
        {
            Lua.LuaDoString<bool>("return OptionRadFrame.playersdrawui_check:SetChecked(false)");
        }
        this.PlayerDrawUI = Main.PlayerDrawUI;
        if (Main.PlayerSound)
        {
            Lua.LuaDoString<bool>("return OptionRadFrame.playerssound_check:SetChecked(true)");
        }
        else
        {
            Lua.LuaDoString<bool>("return OptionRadFrame.playerssound_check:SetChecked(false)");
        }
        this.PlayerSound = Main.PlayerSound;
        if (Main.PlayerCorpses)
        {
            Lua.LuaDoString<bool>("return OptionRadFrame.playerscorpses_check:SetChecked(true)");
        }
        else
        {
            Lua.LuaDoString<bool>("return OptionRadFrame.playerscorpses_check:SetChecked(false)");
        }
        this.PlayerCorpses = Main.PlayerCorpses;
        if (Main.NPCsDrawUI)
        {
            Lua.LuaDoString<bool>("return OptionRadFrame.npcsdrawui_check:SetChecked(true)");
        }
        else
        {
            Lua.LuaDoString<bool>("return OptionRadFrame.npcsdrawui_check:SetChecked(false)");
        }
        this.NPCsDrawUI = Main.NPCsDrawUI;
        if (Main.NPCsSound)
        {
            Lua.LuaDoString<bool>("return OptionRadFrame.npcssound_check:SetChecked(true)");
        }
        else
        {
            Lua.LuaDoString<bool>("return OptionRadFrame.npcssound_check:SetChecked(false)");
        }
        this.NPCsSound = Main.NPCsSound;
        if (Main.ObjectsDrawUI)
        {
            Lua.LuaDoString<bool>("return OptionRadFrame.objectssdrawui_check:SetChecked(true)");
        }
        else
        {
            Lua.LuaDoString<bool>("return OptionRadFrame.objectssdrawui_check:SetChecked(false)");
        }
        this.ObjectsDrawUI = Main.ObjectsDrawUI;
        if (Main.ObjectsSound)
        {
            Lua.LuaDoString<bool>("return OptionRadFrame.objectssound_check:SetChecked(true)");
        }
        else
        {
            Lua.LuaDoString<bool>("return OptionRadFrame.objectssound_check:SetChecked(false)");
        }
        this.ObjectsSound = Main.ObjectsSound;
        if (Main.PvPDrawUI)
        {
            Lua.LuaDoString<bool>("return OptionRadFrame.pvpdrawui_check:SetChecked(true)");
        }
        else
        {
            Lua.LuaDoString<bool>("return OptionRadFrame.pvpdrawui_check:SetChecked(false)");
        }
        this.PvPDrawUI = Main.PvPDrawUI;
        if (Main.PvPSound)
        {
            Lua.LuaDoString<bool>("return OptionRadFrame.pvpsound_check:SetChecked(true)");
        }
        else
        {
            Lua.LuaDoString<bool>("return OptionRadFrame.pvpsound_check:SetChecked(false)");
        }
        this.PvPSound = Main.PvPSound;

        if (Main.Map3DMe)
        {
            Lua.LuaDoString<bool>("return OptionRadFrame.map3dme_check:SetChecked(true)");
            wManagerGlobalSetting.CurrentSetting.MeShow = true;
        }
        else
        {
            Lua.LuaDoString<bool>("return OptionRadFrame.map3dme_check:SetChecked(false)");
            wManagerGlobalSetting.CurrentSetting.MeShow = false;
        }
        this.Map3DMe = Main.Map3DMe;
        if (Main.Map3DTarget)
        {
            Lua.LuaDoString<bool>("return OptionRadFrame.map3dtarget_check:SetChecked(true)");
            wManagerGlobalSetting.CurrentSetting.TargetShow = true;
        }
        else
        {
            Lua.LuaDoString<bool>("return OptionRadFrame.map3dtarget_check:SetChecked(false)");
            wManagerGlobalSetting.CurrentSetting.TargetShow = false;
        }
        this.Map3DTarget = Main.Map3DTarget;
        if (Main.Map3DTargetLine)
        {
            Lua.LuaDoString<bool>("return OptionRadFrame.map3dtargetline_check:SetChecked(true)");
        }
        else
        {
            Lua.LuaDoString<bool>("return OptionRadFrame.map3dtargetline_check:SetChecked(false)");
        }
        this.Map3DTargetLine = Main.Map3DTargetLine;
        if (Main.Map3DPath)
        {
            Lua.LuaDoString<bool>("return OptionRadFrame.map3dpath_check:SetChecked(true)");
            wManagerGlobalSetting.CurrentSetting.PathShow = true;
        }
        else
        {
            Lua.LuaDoString<bool>("return OptionRadFrame.map3dpath_check:SetChecked(false)");
            wManagerGlobalSetting.CurrentSetting.PathShow = false;
        }
        this.Map3DPath = Main.Map3DPath;
        if (Main.Map3DNPCs)
        {
            Lua.LuaDoString<bool>("return OptionRadFrame.map3dnpcs_check:SetChecked(true)");
            wManagerGlobalSetting.CurrentSetting.NpcShow = true;
        }
        else
        {
            Lua.LuaDoString<bool>("return OptionRadFrame.map3dnpcs_check:SetChecked(false)");
            wManagerGlobalSetting.CurrentSetting.NpcShow = false;
        }
        this.Map3DNPCs = Main.Map3DNPCs;
        if (Main.Map3DPlayers)
        {
            Lua.LuaDoString<bool>("return OptionRadFrame.map3dplayers_check:SetChecked(true)");
            wManagerGlobalSetting.CurrentSetting.PlayersShow = true;
        }
        else
        {
            Lua.LuaDoString<bool>("return OptionRadFrame.map3dplayers_check:SetChecked(false)");
            wManagerGlobalSetting.CurrentSetting.PlayersShow = false;
        }
        this.Map3DPlayers = Main.Map3DPlayers;
        if (Main.Map3DObjects)
        {
            Lua.LuaDoString<bool>("return OptionRadFrame.map3dobjects_check:SetChecked(true)");
            wManagerGlobalSetting.CurrentSetting.ObjectsShow = true;
        }
        else
        {
            Lua.LuaDoString<bool>("return OptionRadFrame.map3dobjects_check:SetChecked(false)");
            wManagerGlobalSetting.CurrentSetting.ObjectsShow = false;
        }
        this.Map3DObjects = Main.Map3DObjects;
    }

    public void SyncFcom()
    {
        //Methods.LuaPrint("SyncFcom executing..");

        string trigger = Lua.LuaDoString<string>("return radtrigger()");


        if (trigger == "rad_hideincombat")
        {
            //Methods.LuaPrint(Methods.FormatLua(@"rad_hideincombat triggered."));
            if (Lua.LuaDoString<bool>("return OptionRadFrame.hideincombat_check:GetChecked()"))
            {
                this.HideInCombat = true;
            }
            else
            {
                this.HideInCombat = false;
            }
        }
        if (trigger == "rad_enableradar")
        {
            //Methods.LuaPrint(Methods.FormatLua(@"rad_enableradar triggered."));
            if (Lua.LuaDoString<bool>("return OptionRadFrame.enableradar_check:GetChecked()"))
            {
                this.EnableRadar = true;
            }
            else
            {
                this.EnableRadar = false;
            }
        }
        if (trigger == "rad_playersdrawui")
        {
            //Methods.LuaPrint(Methods.FormatLua(@"rad_playersdrawui triggered."));
            if (Lua.LuaDoString<bool>("return OptionRadFrame.playersdrawui_check:GetChecked()"))
            {
                this.PlayerDrawUI = true;
            }
            else
            {
                this.PlayerDrawUI = false;
            }
        }
        if (trigger == "rad_playerssound")
        {
            //Methods.LuaPrint(Methods.FormatLua(@"rad_playerssound triggered."));
            if (Lua.LuaDoString<bool>("return OptionRadFrame.playerssound_check:GetChecked()"))
            {
                this.PlayerSound = true;
            }
            else
            {
                this.PlayerSound = false;
            }
        }
        if (trigger == "rad_playerscorpses")
        {
            //Methods.LuaPrint(Methods.FormatLua(@"rad_playerscorpses triggered."));
            if (Lua.LuaDoString<bool>("return OptionRadFrame.playerscorpses_check:GetChecked()"))
            {
                this.PlayerCorpses = true;
            }
            else
            {
                this.PlayerCorpses = false;
            }
        }
        if (trigger == "rad_npcsdrawui")
        {
            //Methods.LuaPrint(Methods.FormatLua(@"rad_npcsdrawui triggered."));
            if (Lua.LuaDoString<bool>("return OptionRadFrame.npcsdrawui_check:GetChecked()"))
            {
                this.NPCsDrawUI = true;
            }
            else
            {
                this.NPCsDrawUI = false;
            }
        }
        if (trigger == "rad_npcssound")
        {
            //Methods.LuaPrint(Methods.FormatLua(@"rad_npcssound triggered."));
            if (Lua.LuaDoString<bool>("return OptionRadFrame.npcssound_check:GetChecked()"))
            {
                this.NPCsSound = true;
            }
            else
            {
                this.NPCsSound = false;
            }
        }
        if (trigger == "rad_objectssdrawui")
        {
            //Methods.LuaPrint(Methods.FormatLua(@"rad_objectssdrawui triggered."));
            if (Lua.LuaDoString<bool>("return OptionRadFrame.objectssdrawui_check:GetChecked()"))
            {
                this.ObjectsDrawUI = true;
            }
            else
            {
                this.ObjectsDrawUI = false;
            }
        }
        if (trigger == "rad_objectssound")
        {
            //Methods.LuaPrint(Methods.FormatLua(@"rad_objectssound triggered."));
            if (Lua.LuaDoString<bool>("return OptionRadFrame.objectssound_check:GetChecked()"))
            {
                this.ObjectsSound = true;
            }
            else
            {
                this.ObjectsSound = false;
            }
        }
        if (trigger == "rad_pvpdrawui")
        {
            //Methods.LuaPrint(Methods.FormatLua(@"rad_pvpdrawui triggered."));
            if (Lua.LuaDoString<bool>("return OptionRadFrame.pvpdrawui_check:GetChecked()"))
            {
                this.PvPDrawUI = true;
            }
            else
            {
                this.PvPDrawUI = false;
            }
        }
        if (trigger == "rad_pvpsound")
        {
            //Methods.LuaPrint(Methods.FormatLua(@"rad_pvpsound triggered."));
            if (Lua.LuaDoString<bool>("return OptionRadFrame.pvpsound_check:GetChecked()"))
            {
                this.PvPSound = true;
                //Methods.LuaPrint(Methods.FormatLua(@"Main.Cmd.PvPSound: {0}", Main.Cmd.PvPSound));
            }
            else
            {
                this.PvPSound = false;
                //Methods.LuaPrint(Methods.FormatLua(@"Main.Cmd.PvPSound: {0}", Main.Cmd.PvPSound));
            }
        }
        if (trigger == "rad_map3dme")
        {
            //Methods.LuaPrint(Methods.FormatLua(@"rad_map3dme triggered."));
            if (Lua.LuaDoString<bool>("return OptionRadFrame.map3dme_check:GetChecked()"))
            {
                wManagerGlobalSetting.CurrentSetting.MeShow = true;
                this.Map3DMe = true;
            }
            else
            {
                wManagerGlobalSetting.CurrentSetting.MeShow = false;
                this.Map3DMe = false;
            }
        }
        if (trigger == "rad_map3dtarget")
        {
            //Methods.LuaPrint(Methods.FormatLua(@"rad_map3dme triggered."));
            if (Lua.LuaDoString<bool>("return OptionRadFrame.map3dtarget_check:GetChecked()"))
            {
                wManagerGlobalSetting.CurrentSetting.TargetShow = true;
                this.Map3DTarget = true;
            }
            else
            {
                wManagerGlobalSetting.CurrentSetting.TargetShow = false;
                this.Map3DTarget = false;
            }
        }
        if (trigger == "rad_map3dtargetline")
        {
            //Methods.LuaPrint(Methods.FormatLua(@"rad_map3dme triggered."));
            if (Lua.LuaDoString<bool>("return OptionRadFrame.map3dtargetline_check:GetChecked()"))
            {
                this.Map3DTargetLine = true;
            }
            else
            {
                this.Map3DTargetLine = false;
            }
        }
        if (trigger == "rad_map3dpath")
        {
            //Methods.LuaPrint(Methods.FormatLua(@"rad_map3dme triggered."));
            if (Lua.LuaDoString<bool>("return OptionRadFrame.map3dpath_check:GetChecked()"))
            {
                wManagerGlobalSetting.CurrentSetting.PathShow = true;
                this.Map3DPath = true;
            }
            else
            {
                wManagerGlobalSetting.CurrentSetting.PathShow = false;
                this.Map3DPath = false;
            }
        }
        if (trigger == "rad_map3dnpcs")
        {
            //Methods.LuaPrint(Methods.FormatLua(@"rad_map3dnpcs triggered."));
            if (Lua.LuaDoString<bool>("return OptionRadFrame.map3dnpcs_check:GetChecked()"))
            {
                wManagerGlobalSetting.CurrentSetting.NpcShow = true;
                this.Map3DNPCs = true;
            }
            else
            {
                wManagerGlobalSetting.CurrentSetting.NpcShow = false;
                this.Map3DNPCs = false;
            }
        }
        if (trigger == "rad_map3dplayers")
        {
            //Methods.LuaPrint(Methods.FormatLua(@"rad_map3dme triggered."));
            if (Lua.LuaDoString<bool>("return OptionRadFrame.map3dplayers_check:GetChecked()"))
            {
                wManagerGlobalSetting.CurrentSetting.PlayersShow = true;
                this.Map3DPlayers = true;
            }
            else
            {
                wManagerGlobalSetting.CurrentSetting.PlayersShow = false;
                this.Map3DPlayers = false;
            }
        }
        if (trigger == "rad_map3dobjects")
        {
            //Methods.LuaPrint(Methods.FormatLua(@"rad_map3dme triggered."));
            if (Lua.LuaDoString<bool>("return OptionRadFrame.map3dobjects_check:GetChecked()"))
            {
                wManagerGlobalSetting.CurrentSetting.ObjectsShow = true;
                this.Map3DObjects = true;
            }
            else
            {
                wManagerGlobalSetting.CurrentSetting.ObjectsShow = false;
                this.Map3DObjects = false;
            }
        }
    }
    #endregion Methods


    #region Methods

    #endregion Methods


}



