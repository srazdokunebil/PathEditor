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
function petrigger()
    if pe_add then
        pe_add = not pe_add
            return ""pe_add""
        end
    if pe_del then
        pe_del = not pe_del
            return ""pe_del""
        end
    if pe_new then
        pe_new = not pe_new
            return ""pe_new""
        end
    if pe_insert then
        pe_insert = not pe_insert
            return ""pe_insert""
        end
    if pe_reposition then
        pe_reposition = not pe_reposition
            return ""pe_reposition""
        end
    if pe_minimap then
        pe_minimap = not pe_minimap
            return ""pe_minimap""
        end
end
"
        ));
    }

    public static void SlashCommands()
    {
        // Universal Hotkey: https://wowwiki.fandom.com/wiki/Creating_a_slash_command

        Lua.LuaDoString(string.Format(@"
SLASH_PATHEDITOR1, SLASH_PATHEDITOR2 = '/pe', '/patheditor';

function SlashCmdList.PATHEDITOR(msg, editbox)
	--print(""Hello, World!"");
end


local function PathEditorCommands(msg, editbox)

    ----------------
    -- /pe <command>
    ----------------

    local _, _, arg1 = string.find(msg, ""%s?(%w+)"")

    -- add

    if arg1 == ""add"" then
        --print(""executing "" .. arg1)
        pe_add = not pe_add
        return
    end

    -- delete

    if arg1 == ""del"" then
        --print(""executing "" .. arg1)
        pe_del = not pe_del
        return
    end

    -- new

    if arg1 == ""new"" then
        --print(""executing "" .. arg1)
        pe_new = not pe_new
        return
    end

    -- insert

    if arg1 == ""insert"" then
        --print(""executing "" .. arg1)
        pe_insert = not pe_insert
        return
    end

    -- reposition

    if arg1 == ""reposition"" then
        --print(""executing "" .. arg1)
        pe_reposition = not pe_reposition
        return
    end

    -- toggle minimap display

    if arg1 == ""minimap"" then
        --print(""executing "" .. arg1)
        pe_minimap = not pe_minimap
        return
    end

end

SLASH_PATHEDITOR1, SLASH_PATHEDITOR2 = '/pe', '/peditor'

SlashCmdList[""PATHEDITOR""] = PathEditorCommands

"
        ));

    }


}

public class Command
{
    #region Properties
    public bool DisplayMiniMap { get; set; }

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
        this.DisplayMiniMap = false;

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

        string trigger = Lua.LuaDoString<string>("return petrigger()");

        if (trigger == "pe_add")
        {
            // add node from endpoint
            Methods.LuaPrint("pe_add invoked");
            Methods.CMD_Add();
        }
        if (trigger == "pe_del")
        {
            // delete closest node
            //Methods.LuaPrint("pe_del invoked");
            Methods.CMD_Delete();
        }
        if (trigger == "pe_new")
        {
            // initialize new path
            //Methods.LuaPrint("pe_new invoked");
            Methods.CMD_New();
        }
        if (trigger == "pe_insert")
        {
            // insert node betwen closest two adjoined nodes
            Methods.LuaPrint("pe_insert invoked");
            Methods.CMD_Insert();
        }
        if (trigger == "pe_reposition")
        {
            // move closest node to your position
            //Methods.LuaPrint("pe_reposition invoked");
            Methods.CMD_Reposition();
        }
        if (trigger == "pe_minimap")
        {
            // move closest node to your position
            //Methods.LuaPrint("pe_reposition invoked");
            this.DisplayMiniMap = !(this.DisplayMiniMap);
            Methods.CMD_Minimap();
        }

    }
    #endregion Methods
}



