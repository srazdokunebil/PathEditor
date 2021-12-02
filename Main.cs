// Decompiled with JetBrains decompiler
// Type: Main
// Assembly: WRadar, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6EAA48DA-C0A7-41DC-829A-7E5014FB3C0C
// Assembly location: C:\wow\trainer\wrobot-3.0-yertle\Plugins\WRadar.dll

using robotManager.Helpful;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using wManager.Plugin;
using wManager.Wow.Enums;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;

public class Main : IPlugin
{
    private static Keys _add = Keys.Insert;
    private static Keys _remove = Keys.Delete;
    private Graphics _g = Graphics.FromHwnd(IntPtr.Zero);
    private int screenWidth = Screen.PrimaryScreen.Bounds.Width;
    private int screenHeight = Screen.PrimaryScreen.Bounds.Height;
    private int _lastCount = 0;
    private bool _isLaunched;
    private bool _inPlaySound;

    public static Vector3 NodeA = new Vector3(5256.173, -717.0386, 343.0444, "None");

    public static List<Vector3> Path = new List<Vector3>();

    public static bool RadFrameInit = false;

    public static bool Combat = false;

    public static bool DrawObjectLines;
    public static bool DrawObjectNames;
    public static bool HideRadarInCombat;
    public static bool PlaySound;
    public static bool ShowEnemyPlayers;

    public static bool EnableRadar;
    public static bool HideInCombat;
    public static bool PlayerDrawUI;
    public static bool PlayerSound;
    public static bool PlayerCorpses;
    public static bool NPCsDrawUI;
    public static bool NPCsSound;
    public static bool ObjectsDrawUI;
    public static bool ObjectsSound;
    public static bool PvPDrawUI;
    public static bool PvPSound;

    public static bool Map3DMe;
    public static bool Map3DTarget;
    public static bool Map3DTargetLine;
    public static bool Map3DPath;
    public static bool Map3DNPCs;
    public static bool Map3DPlayers;
    public static bool Map3DObjects;

    public static Command Cmd = new Command();

    private readonly BackgroundWorker _pulseThread = new BackgroundWorker();

    private bool _isPriest = false;

    public static bool _alarm = false;
    public static DateTime _lastAlarm = DateTime.MinValue;





    public void Initialize()
    {
        try
        {
            Logging.Write("[PathEditor] by Srazdokunebil v1.0");
            Methods.LuaPrint("[PathEditor] by Srazdokunebil v1.0");

            PathSettings.Load();
            Methods.LuaPrint(Methods.FormatLua(@"node count:{0}", Main.Path.Count()));

            UserInterface.InjectLuaFunctions();
            UserInterface.SlashCommands();

            this._isLaunched = true;
            Radar3D.Pulse();

            //Radar3D.OnDrawEvent += new Radar3D.OnDrawHandler(this.DrawThing);

            Radar3D.OnDrawEvent += new Radar3D.OnDrawHandler(this.DrawPath);

            _pulseThread.DoWork += DoBackgroundPulse;
            _pulseThread.RunWorkerAsync();
        }
        catch (Exception e)
        {
            Logging.WriteError("Initialize() Error: " + e.Message);
        }
    }

    private void DoBackgroundPulse(object sender, DoWorkEventArgs args)
    {
        //Thread.Sleep(5000);
        //Thread.Sleep(100);
        while (_isLaunched)
        {
            
            try
            {
                if (Conditions.InGameAndConnectedAndAliveAndProductStartedNotInPause)
                {
                    if (Methods._syncFcom.AddSeconds(0.2) < DateTime.Now)
                    {
                        Main.Cmd.SyncFcom();
                        Methods._syncFcom = DateTime.Now;
                    }
                }
                
            }
            catch (Exception e)
            {
                Logging.WriteError("" + e);
            }

            

            //Thread.Sleep(10);
        }
    }

    public void Dispose()
    {
        if (_isLaunched)
        {
            //Methods.LuaPrint(Methods.FormatLua(@"disposing"));
            //int nodeindex = 0;

            //Methods.LuaPrint(Methods.FormatLua(@"Main.Path.Count():{0}", Main.Path));

            //foreach (Vector3 v in Main.Path)
            //{

            //    Methods.LuaPrint(Methods.FormatLua(@"node N{0} - X:{1} Y:{2} Z:{3}", nodeindex, v.X, v.Y, v.Z));
            //}
            /*
            //Methods.LuaPrint(Methods.FormatLua(@"DrawObjectLines: {0}", Main.Cmd.DrawObjectLines));
            //Methods.LuaPrint(Methods.FormatLua(@"DrawObjectNames: {0}", Main.Cmd.DrawObjectNames));
            //Methods.LuaPrint(Methods.FormatLua(@"HideRadarInCombat: {0}", Main.Cmd.HideRadarInCombat));
            //Methods.LuaPrint(Methods.FormatLua(@"PlaySound: {0}", Main.Cmd.PlaySound));
            //Methods.LuaPrint(Methods.FormatLua(@"ShowEnemyPlayers: {0}", Main.Cmd.ShowEnemyPlayers));

            //Methods.LuaPrint(Methods.FormatLua(@"EnableRadar: {0}", Main.Cmd.EnableRadar));
            //Methods.LuaPrint(Methods.FormatLua(@"HideInCombat: {0}", Main.Cmd.HideInCombat));
            //Methods.LuaPrint(Methods.FormatLua(@"PlayerDrawUI: {0}", Main.Cmd.PlayerDrawUI));
            //Methods.LuaPrint(Methods.FormatLua(@"PlayerSound: {0}", Main.Cmd.PlayerSound));
            //Methods.LuaPrint(Methods.FormatLua(@"PlayerCorpses: {0}", Main.Cmd.PlayerCorpses));
            //Methods.LuaPrint(Methods.FormatLua(@"NPCsDrawUI: {0}", Main.Cmd.NPCsDrawUI));
            //Methods.LuaPrint(Methods.FormatLua(@"NPCsSound: {0}", Main.Cmd.NPCsSound));
            //Methods.LuaPrint(Methods.FormatLua(@"ObjectsDrawUI: {0}", Main.Cmd.ObjectsDrawUI));
            //Methods.LuaPrint(Methods.FormatLua(@"ObjectsSound: {0}", Main.Cmd.ObjectsSound));
            //Methods.LuaPrint(Methods.FormatLua(@"PvPDrawUI: {0}", Main.Cmd.PvPDrawUI));
            //Methods.LuaPrint(Methods.FormatLua(@"Dispose: PvPSound: {0}", Main.Cmd.PvPSound));

            PluginSettings.CurrentSetting.DrawObjectLines = Main.Cmd.DrawObjectLines;
            PluginSettings.CurrentSetting.DrawObjectNames = Main.Cmd.DrawObjectNames;
            PluginSettings.CurrentSetting.HideRadarInCombat = Main.Cmd.HideRadarInCombat;
            PluginSettings.CurrentSetting.PlaySound = Main.Cmd.PlaySound;
            PluginSettings.CurrentSetting.ShowEnemyPlayers = Main.Cmd.ShowEnemyPlayers;

            PluginSettings.CurrentSetting.EnableRadar = Main.Cmd.EnableRadar;
            PluginSettings.CurrentSetting.HideInCombat = Main.Cmd.HideInCombat;
            PluginSettings.CurrentSetting.PlayerDrawUI = Main.Cmd.PlayerDrawUI;
            PluginSettings.CurrentSetting.PlayerSound = Main.Cmd.PlayerSound;
            PluginSettings.CurrentSetting.PlayerCorpses = Main.Cmd.PlayerCorpses;
            PluginSettings.CurrentSetting.NPCsDrawUI = Main.Cmd.NPCsDrawUI;
            PluginSettings.CurrentSetting.NPCsSound = Main.Cmd.NPCsSound;
            PluginSettings.CurrentSetting.ObjectsDrawUI = Main.Cmd.ObjectsDrawUI;
            PluginSettings.CurrentSetting.ObjectsSound = Main.Cmd.ObjectsSound;
            PluginSettings.CurrentSetting.PvPDrawUI = Main.Cmd.PvPDrawUI;
            PluginSettings.CurrentSetting.PvPSound = Main.Cmd.PvPSound;

            PluginSettings.CurrentSetting.Map3DMe = Main.Cmd.Map3DMe;
            PluginSettings.CurrentSetting.Map3DTarget = Main.Cmd.Map3DTarget;
            PluginSettings.CurrentSetting.Map3DTargetLine = Main.Cmd.Map3DTargetLine;
            PluginSettings.CurrentSetting.Map3DPath = Main.Cmd.Map3DPath;
            PluginSettings.CurrentSetting.Map3DNPCs = Main.Cmd.Map3DNPCs;
            PluginSettings.CurrentSetting.Map3DPlayers = Main.Cmd.Map3DPlayers;
            PluginSettings.CurrentSetting.Map3DObjects = Main.Cmd.Map3DObjects;

            if (Methods.GetLuaBool("wr_running"))
            {

            }
            PluginSettings.CurrentSetting.Save();

            try
            {
                Radar3D.OnDrawEvent -= new Radar3D.OnDrawHandler(this.Radar3DOnDrawEvent);
                Main.KeyBoardHook.OnKeyDown -= new Main.KeyBoardHook.KeyBoardHookEventHandler(this.KeyDown);
                Main.KeyBoardHook.Dispose();
            }
            catch
            {
            }
            */

            PathSettings.Save();

            this._isLaunched = false;
            _pulseThread.DoWork -= DoBackgroundPulse;
            _pulseThread.Dispose();
            Logging.Write("[PathEditor] Stopped.");
        }
    }

    public void Settings()
    {
        //PluginSettings.Load();
        //PluginSettings.CurrentSetting.ToForm();
        //PluginSettings.CurrentSetting.Save();
        //Logging.Write("[WRadar] Settings saved.");
    }

    private void KeyDown(object sender, Main.KeyBoardHook.KeyArgs keyArgs)
    {
        return;
        if (wManager.Wow.ObjectManager.ObjectManager.Me.Target != 0UL && keyArgs.Key == Main._add)
        {
            WoWUnit targetObject = wManager.Wow.ObjectManager.ObjectManager.Me.TargetObject;
            if (!targetObject.PlayerControlled)
            {
                if (!Main.ContainsLoop(PluginSettings.CurrentSetting.NPCList, targetObject.Name))
                {
                    PluginSettings.CurrentSetting.NPCList.Add(targetObject?.Name);
                    PluginSettings.CurrentSetting.Save();
                    Logging.Write("[WRadar] Added \"" + targetObject.Name + "\" to the NPC list.");
                    return;
                }
                Logging.Write("[WRadar] \"" + targetObject.Name + "\" already exists in the NPC list.");
                return;
            }
        }
        if (keyArgs.Key == Main._remove)
        {
            WoWUnit targetObject = wManager.Wow.ObjectManager.ObjectManager.Me.TargetObject;
            if (!targetObject.PlayerControlled)
            {
                if (Main.ContainsLoop(PluginSettings.CurrentSetting.NPCList, targetObject.Name))
                {
                    PluginSettings.CurrentSetting.NPCList.Remove(targetObject?.Name);
                    PluginSettings.CurrentSetting.Save();
                    Logging.Write("[WRadar] Removed: \"" + targetObject.Name + "\" from the NPC list.");
                    return;
                }
                Logging.Write("[WRadar] \"" + targetObject.Name + "\" does not exist in the NPC list.");
            }
        }
        if (wManager.Wow.ObjectManager.ObjectManager.Me.Target == 0UL && keyArgs.Key == Main._add)
        {
            WoWGameObject woWgameObject = wManager.Wow.ObjectManager.ObjectManager.GetObjectWoWGameObject().OrderBy<WoWGameObject, float>((Func<WoWGameObject, float>) (i => i.GetDistance)).FirstOrDefault<WoWGameObject>();
            if (!Main.ContainsLoop(PluginSettings.CurrentSetting.ObjectsList, woWgameObject.Name))
            {
                PluginSettings.CurrentSetting.ObjectsList.Add(woWgameObject?.Name);
                PluginSettings.CurrentSetting.Save();
                Logging.Write("[WRadar] Added: \"" + woWgameObject.Name + "\" to the Object list.");
            }
            else
                Logging.Write("[WRadar] Object: \"" + woWgameObject.Name + "\" already exists in the Object list.");
        }
        else
        {
            if (wManager.Wow.ObjectManager.ObjectManager.Me.Target != 0UL || keyArgs.Key != Main._remove)
                return;
            WoWGameObject woWgameObject = wManager.Wow.ObjectManager.ObjectManager.GetObjectWoWGameObject().OrderBy<WoWGameObject, float>((Func<WoWGameObject, float>) (i => i.GetDistance)).FirstOrDefault<WoWGameObject>();
            if (Main.ContainsLoop(PluginSettings.CurrentSetting.ObjectsList, woWgameObject.Name))
            {
                PluginSettings.CurrentSetting.ObjectsList.Remove(woWgameObject?.Name);
                PluginSettings.CurrentSetting.Save();
                Logging.Write("[WRadar] Removed: \"" + woWgameObject.Name + "\" from the object list.");
            }
            else
                Logging.Write("[WRadar] Object: \"" + woWgameObject.Name + "\" does not exist in the object list.");
        }
    }

    private FontFamily DefaultFont(params string[] fonts)
    {
        try
        {
            foreach (string font in fonts)
            {
                try
                {
                    return new FontFamily(font);
                }
                catch (ArgumentException ex)
                {
                }
            }
            return new FontFamily(GenericFontFamilies.Serif);
        }
        catch (Exception ex)
        {
            Logging.WriteError("DefaultFont() Error: " + Environment.NewLine + (object) ex, true);
            return (FontFamily) null;
        }
    }

    private static bool ContainsLoop(List<string> list, string value)
    {
        for (int index = 0; index < list.Count; ++index)
        {
            if (list[index] == value)
                return true;
        }
        return false;
    }

    private void DrawToEnemyPlayers()
    {
        int num = this.screenWidth / 2;
        List<WoWPlayer> source = new List<WoWPlayer>();
        source.AddRange((IEnumerable<WoWPlayer>) wManager.Wow.ObjectManager.ObjectManager.GetObjectWoWPlayer().Where<WoWPlayer>((Func<WoWPlayer, bool>) (p => p != null && p.IsValid && p.IsAlive && wManager.Wow.ObjectManager.ObjectManager.Me.PlayerFaction != p.PlayerFaction && p.IsAttackable)).OrderBy<WoWPlayer, float>((Func<WoWPlayer, float>) (u => wManager.Wow.ObjectManager.ObjectManager.Me.Position.DistanceTo(u.Position))));
        if (source.Count<WoWPlayer>() <= 0)
            return;
        try
        {
            if (source == null || (!Main.Cmd.PvPDrawUI && Main.Cmd.PvPSound))
                return;
            foreach (WoWPlayer woWplayer in source)
            {
                if (woWplayer != null && (woWplayer.IsValid && woWplayer.IsAlive) && !woWplayer.IsOnTaxi && Main.Cmd.PvPDrawUI)
                    Radar3D.DrawLine(wManager.Wow.ObjectManager.ObjectManager.Me.Position, woWplayer.Position, Color.Red, (int) byte.MaxValue);
            }
            if (source.Count >= 1)
            {
                
                for (int index = 0; index < source.Count; ++index)
                {

                    if (source[index] != null && source[index].IsValid && (source[index].IsAlive && !source[index].IsOnTaxi) && Main.Cmd.PvPDrawUI)
                    {
                        //Methods.LuaPrint("found " + source[index].Name);
                        string text = source[index].Level.ToString() + " " + (object) source[index].WowClass + " " + (object)source[index].Name + " (" + (object) System.Math.Round((double) source[index].GetDistance, 0) + "yd)";
                        int width = (int) this._g.MeasureString(text, SystemFonts.DefaultFont).Width;
                        Radar3D.DrawString(text, new Vector3((float) (num - width), (float) (this.screenHeight / 2 + -200 + index * 18), 0.0f, "None"), 14f, Color.Red, (int) byte.MaxValue, this.DefaultFont());
                        if (Main.Cmd.PvPSound && this._lastCount <= 0 && source.Count > 0)
                        {
                            //LuaPrint("BING");
                            new Thread(new ThreadStart(this.ThreadSound)).Start();
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Logging.WriteError("DrawToEnemyPlayers() Error: " + Environment.NewLine + (object) ex, true);
        }
    }



    private void DrawToPlayers()
    {
        int num = this.screenWidth / 2;
        List<WoWPlayer> source = new List<WoWPlayer>();
        source.AddRange((IEnumerable<WoWPlayer>)wManager.Wow.ObjectManager.ObjectManager.GetObjectWoWPlayer().Where<WoWPlayer>((Func<WoWPlayer, bool>)(p => p != null && p.IsValid && p.IsAlive && !PluginSettings.CurrentSetting.FriendsList.Contains(p.Name))).OrderBy<WoWPlayer, float>((Func<WoWPlayer, float>)(u => wManager.Wow.ObjectManager.ObjectManager.Me.Position.DistanceTo(u.Position))));
        if (source.Count<WoWPlayer>() <= 0)
            return;
        try
        {
            if (source == null || (!Main.Cmd.PlayerDrawUI && !Main.Cmd.PlayerSound))
                return;
            foreach (WoWPlayer woWplayer in source)
            {
                if (woWplayer != null && (woWplayer.IsValid && woWplayer.IsAlive) && !woWplayer.IsOnTaxi && Main.Cmd.PlayerDrawUI)
                    Radar3D.DrawLine(wManager.Wow.ObjectManager.ObjectManager.Me.Position, woWplayer.Position, Color.ForestGreen, (int)byte.MaxValue);
            }
            if (source.Count >= 1)
            {

                for (int index = 0; index < source.Count; ++index)
                {

                    if (source[index] != null && source[index].IsValid && (source[index].IsAlive && !source[index].IsOnTaxi) && Main.Cmd.PlayerDrawUI)
                    {
                        //Methods.LuaPrint("found " + source[index].Name);
                        string text = source[index].Level.ToString() + " " + (object)source[index].WowClass + " " + (object)source[index].Name + " (" + (object)System.Math.Round((double)source[index].GetDistance, 0) + "yd)";
                        int width = (int)this._g.MeasureString(text, SystemFonts.DefaultFont).Width;
                        Radar3D.DrawString(text, new Vector3((float)(num - width), (float)(this.screenHeight / 2 + -200 + index * 18), 0.0f, "None"), 14f, Color.ForestGreen, (int)byte.MaxValue, this.DefaultFont());
                        if (Main.Cmd.PlayerSound && this._lastCount <= 0 && source.Count > 0)
                        {
                            //LuaPrint("BING");
                            new Thread(new ThreadStart(this.ThreadSound)).Start();
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Logging.WriteError("DrawToEnemyPlayers() Error: " + Environment.NewLine + (object)ex, true);
        }
    }

    private void DrawToTarget()
    {
        try
        {
            if (ObjectManager.Target != null && ObjectManager.Target.Guid > 0UL)
            {
                if (ObjectManager.Target.Reaction <= Reaction.Neutral)
                {
                    Radar3D.DrawLine(wManager.Wow.ObjectManager.ObjectManager.Me.Position, ObjectManager.Target.Position, Color.Red, (int)byte.MaxValue);
                }
                if (ObjectManager.Target.Reaction == Reaction.Neutral)
                {
                    Radar3D.DrawLine(wManager.Wow.ObjectManager.ObjectManager.Me.Position, ObjectManager.Target.Position, Color.Yellow, (int)byte.MaxValue);
                }
                if (ObjectManager.Target.Reaction >= Reaction.Neutral)
                {
                    Radar3D.DrawLine(wManager.Wow.ObjectManager.ObjectManager.Me.Position, ObjectManager.Target.Position, Color.Green, (int)byte.MaxValue);
                }
            }
        }
        catch (Exception ex)
        {
            Logging.WriteError("DrawToTarget() Error: " + Environment.NewLine + (object)ex, true);
        }
    }


    private void DrawToResurrectableCorpses()
    {
        int num = this.screenWidth / 2;
        List<WoWPlayer> source = new List<WoWPlayer>();
        source.AddRange((IEnumerable<WoWPlayer>)wManager.Wow.ObjectManager.ObjectManager.GetObjectWoWPlayer().Where<WoWPlayer>((Func<WoWPlayer, bool>)(p => p != null && p.IsValid && !p.IsAlive)).OrderBy<WoWPlayer, float>((Func<WoWPlayer, float>)(u => wManager.Wow.ObjectManager.ObjectManager.Me.Position.DistanceTo(u.Position))));
        if (source.Count<WoWPlayer>() <= 0)
            return;
        try
        {
            if (source == null)
                return;
            foreach (WoWPlayer woWplayer in source)
            {
                if (woWplayer != null && (woWplayer.IsValid) && !woWplayer.IsOnTaxi && PluginSettings.CurrentSetting.DrawObjectLines)
                    Radar3D.DrawLine(wManager.Wow.ObjectManager.ObjectManager.Me.Position, woWplayer.Position, Color.CadetBlue, (int)byte.MaxValue);
            }
            if (source.Count >= 1)
            {
                for (int index = 0; index < source.Count; ++index)
                {
                    if (source[index] != null && source[index].IsValid && (source[index].IsAlive && !source[index].IsOnTaxi) && PluginSettings.CurrentSetting.DrawObjectNames)
                    {
                        string text = source[index].Level.ToString() + " " + (object)source[index].WowClass + " (" + (object)System.Math.Round((double)source[index].GetDistance, 0) + "yd)";
                        int width = (int)this._g.MeasureString(text, SystemFonts.DefaultFont).Width;
                        Radar3D.DrawString(text, new Vector3((float)(num - width), (float)(this.screenHeight / 2 + 150 + index * 18), 0.0f, "None"), 10f, Color.Red, (int)byte.MaxValue, this.DefaultFont());
                        //if (PluginSettings.CurrentSetting.PlaySound && this._lastCount <= 0 && source.Count > 0)
                        //{
                        //    //LuaPrint("BING");
                        //    new Thread(new ThreadStart(this.ThreadSound)).Start();
                        //}
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Logging.WriteError("DrawToEnemyPlayers() Error: " + Environment.NewLine + (object)ex, true);
        }
    }

    private void DrawToGameObjects()
    {
        int num = this.screenWidth / 2;
        List<WoWGameObject> source = new List<WoWGameObject>();
        source.AddRange((IEnumerable<WoWGameObject>) wManager.Wow.ObjectManager.ObjectManager.GetWoWGameObjectByName(PluginSettings.CurrentSetting.ObjectsList).OrderByDescending<WoWGameObject, float>((Func<WoWGameObject, float>) (o => o.GetDistance)));
        if (source.Count<WoWGameObject>() <= 0)
            return;
        try
        {
            if (source == null)
            {
                return;
            }

            //            LuaPrint("ThreadSound, analysing..." +
            //" /PluginSettings.CurrentSetting.PlaySound:" + PluginSettings.CurrentSetting.PlaySound +
            //" /this._lastCount:" + this._lastCount +
            //" /source.Count:" + source.Count
            //);

            if (Main.Cmd.ObjectsSound && this._lastCount <= 0 && source.Count > 0)
            {
                //LuaPrint("BING");
                new Thread(new ThreadStart(this.ThreadSound)).Start();
            }

            //if (PluginSettings.CurrentSetting.PlaySound && this._lastCount <= 0 && source.Count > 0)
            //{
            //    //LuaPrint("BING");
            //    new Thread(new ThreadStart(this.MySound)).Start();
            //}


            WoWGameObject woWgameObject1 = source.Where<WoWGameObject>((Func<WoWGameObject, bool>) (o => o != null && o.IsValid)).OrderBy<WoWGameObject, float>((Func<WoWGameObject, float>) (o => wManager.Wow.ObjectManager.ObjectManager.Me.Position.DistanceTo(o.Position))).FirstOrDefault<WoWGameObject>();
            if (woWgameObject1 != null)
            {
                Vector3 position = woWgameObject1.Position;
                string name = source.Where<WoWGameObject>((Func<WoWGameObject, bool>) (o => o != null && o.IsValid)).OrderBy<WoWGameObject, float>((Func<WoWGameObject, float>) (o => wManager.Wow.ObjectManager.ObjectManager.Me.Position.DistanceTo(o.Position))).FirstOrDefault<WoWGameObject>().Name;
                if (position != (Vector3) null)
                {
                    // render flightline overlay
                    if (Main.Cmd.ObjectsDrawUI)
                        Radar3D.DrawLine(wManager.Wow.ObjectManager.ObjectManager.Me.Position, position, Color.CornflowerBlue, 100);
                    // render text overlay
                    if (Main.Cmd.ObjectsDrawUI && name != null)
                    {
                        int width = (int) this._g.MeasureString(name + " (" + (object) System.Math.Round((double) wManager.Wow.ObjectManager.ObjectManager.Me.Position.DistanceTo(position), 0) + "yd)", SystemFonts.DefaultFont).Width;
                        Radar3D.DrawString(name + " (" + (object) System.Math.Round((double) wManager.Wow.ObjectManager.ObjectManager.Me.Position.DistanceTo(position), 0) + "yd)", new Vector3((float) (num + 75), (float) (this.screenHeight / 2 + 100), 0.0f, "None"), 10f, Color.CornflowerBlue, 200, this.DefaultFont());
                    }
                }
            }
            foreach (WoWGameObject woWgameObject2 in source)
            {
                // render circle reticles overlay
                if (woWgameObject2 != null && woWgameObject2.IsValid && Main.Cmd.ObjectsDrawUI)
                    Radar3D.DrawCircle(woWgameObject2.Position, 1f, Color.Magenta, false, 150);
            }
        }
        catch (Exception ex)
        {
            Logging.WriteError("DrawToGameObjects() Error: " + Environment.NewLine + (object) ex, true);
        }
    }

    private void DrawNode(Vector3 node)
    {
        int num = this.screenWidth / 2;

        try
        {
            Radar3D.DrawCircle(node, 1f, Color.Orange, true, 150);
        }
        catch (Exception ex)
        {
            Logging.WriteError("DrawToGameObjects() Error: " + Environment.NewLine + (object)ex, true);
        }
    }


    private void DrawToNPCs()
    {
        int num1 = this.screenWidth / 2;
        int num2 = 0;
        Random random = new Random();
        List<WoWUnit> source = new List<WoWUnit>();
        source.AddRange((IEnumerable<WoWUnit>) wManager.Wow.ObjectManager.ObjectManager.GetObjectWoWUnit().Where<WoWUnit>((Func<WoWUnit, bool>) (u => u != null && u.IsValid && (u.IsAlive && !u.IsPet) && PluginSettings.CurrentSetting.NPCList.Contains(u.Name))).OrderBy<WoWUnit, float>((Func<WoWUnit, float>) (n => wManager.Wow.ObjectManager.ObjectManager.Me.Position.DistanceTo(n.Position))));
        if (source.Count<WoWUnit>() <= 0)
            return;
        try
        {
            if (source == null || (!Main.Cmd.NPCsDrawUI && !Main.Cmd.NPCsSound))
                return;

            if (Main.Cmd.NPCsSound && this._lastCount <= 0 && source.Count > 0)
            {
                //LuaPrint("BING");
                new Thread(new ThreadStart(this.ThreadSound)).Start();
            }

            int num3 = Convert.ToInt32(System.Math.Floor((double) source.Count / 3.0));
            if (num3 == 0)
                num3 = 1;
            if (num3 > 4)
                num3 = 4;
            foreach (WoWUnit woWunit in source)
            {
                if (num2 < num3 && (woWunit != null && woWunit.IsValid && woWunit.IsAlive))
                {
                    Vector3 position = woWunit.Position;
                    if (Main.Cmd.NPCsDrawUI)
                    {
                        Radar3D.DrawLine(wManager.Wow.ObjectManager.ObjectManager.Me.Position, position, Color.LightGoldenrodYellow, 150);
                        ++num2;
                    }
                }
            }
            if (source.Count >= 1)
            {
                for (int index = 0; index < num3; ++index)
                {
                    string text = source[index].Level.ToString() + " " + source[index].Name + " (" + (object) System.Math.Round((double) source[index].GetDistance, 0) + "yd)";
                    int width = (int) this._g.MeasureString(text, SystemFonts.DefaultFont).Width;
                    if (Main.Cmd.NPCsDrawUI)
                        Radar3D.DrawString(text.ToString(), new Vector3((float) (num1 + 75), (float) (this.screenHeight / 2 + 150 + index * 18), 0.0f, "None"), 10f, Color.LightGoldenrodYellow, 200, this.DefaultFont());
                }
            }
        }
        catch (Exception ex)
        {
            Logging.WriteError("DrawToNPCs() Error!" + Environment.NewLine + (object) ex, true);
        }
    }

    private void DrawToRareSpawns()
    {
        int num1 = this.screenWidth / 2;
        int num2 = 0;
        Random random = new Random();
        List<WoWUnit> source = new List<WoWUnit>();
        source.AddRange((IEnumerable<WoWUnit>) wManager.Wow.ObjectManager.ObjectManager.GetObjectWoWUnit().Where<WoWUnit>((Func<WoWUnit, bool>) (u => u != null && u.IsValid && (u.IsAlive && !u.IsPet) && PluginSettings.CurrentSetting.RareSpawnList.Contains(u.Name))).OrderBy<WoWUnit, float>((Func<WoWUnit, float>) (n => wManager.Wow.ObjectManager.ObjectManager.Me.Position.DistanceTo(n.Position))));
        if (source.Count<WoWUnit>() <= 0)
        {
            source.RemoveRange(0, source.Count<WoWUnit>());
            this._lastCount = 0;
        }
        else
        {
            try
            {
                if (source == null || source.Count <= 0)
                    return;
                if (PluginSettings.CurrentSetting.PlaySound && this._lastCount <= 0 && source.Count > 0)
                {
                    //LuaPrint("BING");
                    new Thread(new ThreadStart(this.ThreadSound)).Start();
                }



                this._lastCount = source.Count;
                int num3 = Convert.ToInt32(System.Math.Floor((double) source.Count / 3.0));
                if (num3 == 0)
                    num3 = 1;
                if (num3 > 4)
                    num3 = 4;
                foreach (WoWUnit woWunit in source)
                {
                    if (num2 < num3 && (woWunit != null && woWunit.IsValid && woWunit.IsAlive))
                    {
                        Vector3 position = woWunit.Position;
                        if (PluginSettings.CurrentSetting.DrawObjectLines)
                        {
                            Radar3D.DrawLine(wManager.Wow.ObjectManager.ObjectManager.Me.Position, position, Color.LightGoldenrodYellow, 150);
                            ++num2;
                        }
                    }
                }
                if (source.Count >= 1)
                {
                    for (int index = 0; index < num3; ++index)
                    {
                        string text = source[index].Level.ToString() + " " + source[index].Name + " (" + (object) System.Math.Round((double) source[index].GetDistance, 0) + "yd)";
                        int width = (int) this._g.MeasureString(text, SystemFonts.DefaultFont).Width;
                        if (PluginSettings.CurrentSetting.DrawObjectNames)
                            Radar3D.DrawString(text.ToString(), new Vector3((float) (num1 + 75), (float) (this.screenHeight / 2 + 150 + index * 18), 0.0f, "None"), 10f, Color.LightGoldenrodYellow, 200, this.DefaultFont());
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.WriteError("DrawToRareSpawns() Error!" + Environment.NewLine + (object) ex, true);
            }
        }
    }


    private void DrawPath()
    {
        if (!this._isLaunched || !Conditions.InGameAndConnectedAndAliveAndProductStartedNotInPause)
            return;
        try
        {
            //foreach (Vector3 node in Main.Path)
            //{
            //    this.DrawNode(node);
            //}

            for (int i=0; i <= Main.Path.Count; i++)
            {
                this.DrawNode(Main.Path[i]);
                if (i != Main.Path.Count && i != 0)
                {
                    Radar3D.DrawLine(Main.Path[i - 1], Main.Path[i], Color.Orange, (int)byte.MaxValue);
                }
            }
            
        }
        catch (Exception ex)
        {
            Logging.WriteError("DrawPath() Error: " + Environment.NewLine + (object)ex, true);
        }
    }

    private void DrawThing()
    {
        if (!this._isLaunched || !Conditions.InGameAndConnectedAndAliveAndProductStartedNotInPause)
            return;
        try
        {
            this.DrawNode(NodeA);

        }
        catch (Exception ex)
        {
            Logging.WriteError("DrawPath() Error: " + Environment.NewLine + (object)ex, true);
        }
    }

    private void Radar3DOnDrawEvent()
    {
        if (!this._isLaunched || !Conditions.InGameAndConnectedAndAliveAndProductStartedNotInPause)
            return;
        try
        {
            // always draw to current target
            this.DrawToTarget();

            if (!Main.Cmd.EnableRadar)
                return;

            if (Main.Combat && Main.Cmd.HideInCombat)
                return;
            if (Main.Combat && !Main.Cmd.HideInCombat)
            {
                this.DrawToEnemyPlayers();
                this.DrawToPlayers();

                if (PluginSettings.CurrentSetting.ObjectsList.Count > 0)
                    this.DrawToGameObjects();
                if (PluginSettings.CurrentSetting.NPCList.Count > 0)
                    this.DrawToNPCs();
                if (PluginSettings.CurrentSetting.RareSpawnList.Count > 0)
                    this.DrawToRareSpawns();
            }
            else if (!Main.Combat)
            {
                this.DrawToEnemyPlayers();
                this.DrawToPlayers();

                if (PluginSettings.CurrentSetting.ObjectsList.Count > 0)
                    this.DrawToGameObjects();
                if (PluginSettings.CurrentSetting.NPCList.Count > 0)
                    this.DrawToNPCs();
                if (PluginSettings.CurrentSetting.RareSpawnList.Count > 0)
                    this.DrawToRareSpawns();
                if (_isPriest)
                {
                    this.DrawToResurrectableCorpses();
                }
            }
        }
        catch (Exception ex)
        {
            Logging.WriteError("Radar3DOnDrawEvent() Error: " + Environment.NewLine + (object) ex, true);
        }
    }

    private void MySound()
    {
        try
        {
            SoundPlayer soundPlayer = new SoundPlayer()
            {
                SoundLocation = Application.StartupPath + "\\Plugins\\alert.wav"
                //SoundLocation = Application.StartupPath + "\\Plugins\\iasip.wav"
            };
            soundPlayer.PlaySync();
            soundPlayer.Stop();
        }
        catch
        {

        }
    }

    private void ThreadSound()
    {
        if (this._inPlaySound)
            return;
        this._inPlaySound = true;

        //try
        //{
        //    LuaPrint("There sound be a sound notification here.");
        //    Notify("successful_interrupt");
        //}
        //catch
        //{

        //}

        try
        {
            SoundPlayer soundPlayer = new SoundPlayer()
            {
                //SoundLocation = Application.StartupPath + "\\Plugins\\alert.wav"
                SoundLocation = Application.StartupPath + "\\Plugins\\sonar_long.wav"
            };
            soundPlayer.PlaySync();
            soundPlayer.Stop();
        }
        catch
        {
        }
        this._inPlaySound = false;
    }

    private class KeyBoardHook
    {
        private static readonly Main.KeyBoardHook.LowLevelKeyboardProc Proc = new Main.KeyBoardHook.LowLevelKeyboardProc(Main.KeyBoardHook.HookCallback);
        private static IntPtr _hookId = IntPtr.Zero;
        private static Keys _lastKeyDown = Keys.None;
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 256;
        private const int WM_KEYUP = 257;

        public static event Main.KeyBoardHook.KeyBoardHookEventHandler OnKeyDown = (_param1, _param2) => {};

        public static event Main.KeyBoardHook.KeyBoardHookEventHandler OnKeyUp = (_param1, _param2) => {};

        internal static void Initialize()
        {
            Task.Factory.StartNew((Action) (() =>
            {
                Main.KeyBoardHook._hookId = Main.KeyBoardHook.SetHook(Main.KeyBoardHook.Proc);
                Application.Run();
            }));
        }

        internal static void Dispose()
        {
            Main.KeyBoardHook.UnhookWindowsHookEx(Main.KeyBoardHook._hookId);
            Application.Exit();
        }

        private static IntPtr SetHook(Main.KeyBoardHook.LowLevelKeyboardProc proc)
        {
            using (Process currentProcess = Process.GetCurrentProcess())
            {
                using (ProcessModule mainModule = currentProcess.MainModule)
                    return Main.KeyBoardHook.SetWindowsHookEx(13, proc, Main.KeyBoardHook.GetModuleHandle(mainModule.ModuleName), 0U);
            }
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode < 0)
                return Main.KeyBoardHook.CallNextHookEx(Main.KeyBoardHook._hookId, nCode, wParam, lParam);
            if (wParam == (IntPtr) 256)
            {
                Keys key = (Keys) Marshal.ReadInt32(lParam);
                if (Main.KeyBoardHook._lastKeyDown == key)
                    return Main.KeyBoardHook.CallNextHookEx(Main.KeyBoardHook._hookId, nCode, wParam, lParam);
                Main.KeyBoardHook.OnKeyDown((object) null, new Main.KeyBoardHook.KeyArgs(key));
                Main.KeyBoardHook._lastKeyDown = key;
            }
            else if (wParam == (IntPtr) 257)
            {
                Keys key = (Keys) Marshal.ReadInt32(lParam);
                Main.KeyBoardHook.OnKeyUp((object) null, new Main.KeyBoardHook.KeyArgs(key));
                if (Main.KeyBoardHook._lastKeyDown == key)
                    Main.KeyBoardHook._lastKeyDown = Keys.None;
            }
            return Main.KeyBoardHook.CallNextHookEx(Main.KeyBoardHook._hookId, nCode, wParam, lParam);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(
            int idHook,
            Main.KeyBoardHook.LowLevelKeyboardProc lpfn,
            IntPtr hMod,
            uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(
            IntPtr hhk,
            int nCode,
            IntPtr wParam,
            IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        public delegate void KeyBoardHookEventHandler(object sender, Main.KeyBoardHook.KeyArgs e);

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        public class KeyArgs : EventArgs
        {
            public Keys Key { get; private set; }

            public KeyArgs(Keys key)
            {
                this.Key = key;
            }
        }
    }
}
