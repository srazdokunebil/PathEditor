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



