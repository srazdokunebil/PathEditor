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
using wManager.Wow.Forms;
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
    public static int ClosestNode = new int();
    public static int NextClosestNode = new int();

    public static string miniMapLandmark = "MiniMap";
    public static bool MiniMapRefresh = true;

    public static bool TriggerMiniMapShow = false;
    public static bool TriggerMiniMapHide = false;
    public static bool DisplayMiniMap = false;

    public static DateTime _closestNodeScan = DateTime.MinValue;

    public static WoWPlayer Me { get { return ObjectManager.Me; } }

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

            Main.RefreshMiniMap();

            UserInterface.InjectLuaFunctions();
            UserInterface.SlashCommands();

            this._isLaunched = true;
            Radar3D.Pulse();

            

            Radar3D.OnDrawEvent += new Radar3D.OnDrawHandler(this.DrawClosest);
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

                    if (Main._closestNodeScan.AddSeconds(0.2) < DateTime.Now)
                    {
                        Main.ClosestNodeScan();
                        Main._closestNodeScan = DateTime.Now;
                    }
                }
            }
            catch (Exception e)
            {
                Logging.WriteError("" + e);
            }
        }
    }

    public void Dispose()
    {
        if (_isLaunched)
        {
            PathSettings.Save();

            this._isLaunched = false;
            Radar3D.OnDrawEvent -= new Radar3D.OnDrawHandler(this.DrawClosest);
            Radar3D.OnDrawEvent -= new Radar3D.OnDrawHandler(this.DrawPath);
            UserControlMiniMap.LandmarksMiniMap.Remove(miniMapLandmark);
            _pulseThread.DoWork -= DoBackgroundPulse;
            _pulseThread.Dispose();
            Logging.Write("[PathEditor] Stopped.");
        }
    }

    public void Settings()
    {

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

    public static void ClosestNodeScan()
    {
        // if Main.Path list is empty, exit
        if (Main.Path.Count == 0)
            return;

        // iterate through Main.Path list for closest node
        Main.ClosestNode = GetClosestNode();
        //Methods.LuaPrint("closest node: " + i);

        // if there are only two nodes in Main.Path
        if (Main.Path.Count == 2)
        {
            if (Main.ClosestNode == 0)
            {
                Main.NextClosestNode = 1;
            }
            else
            {
                Main.NextClosestNode = 0;
            }
        }

        // if there are more than two nodes in Main.Path
        if (Main.Path.Count > 2)
        {
            if (Main.Path[Main.ClosestNode - 1].DistanceTo(Main.Me.Position) > Main.Path[Main.ClosestNode + 1].DistanceTo(Main.Me.Position))
            {
                Main.NextClosestNode = Main.ClosestNode + 1;
            }
            else
            {
                Main.NextClosestNode = Main.ClosestNode - 1;
            }
        }
    }
    public static int GetClosestNode()
    {
        float closest = 9000;
        int closeidx = 0;
        float distance;
        int i = 0;

        //for (int i = 0; i <= Main.Path.Count; i++)
        //{
        //    distance = Main.Path[i].DistanceTo(Main.Me.Position);
        //    Methods.LuaPrint(Methods.FormatLua(@"Iterating Main.Path[{0}] is {1}yds from your position.", i, distance));
        //    if (distance < closest)
        //    {
        //        closeidx = i;
        //        closest = distance;
        //    }
        //}

        foreach (Vector3 v in Main.Path)
        {
            distance = Main.Path[i].DistanceTo(Main.Me.Position);
            //Methods.LuaPrint(Methods.FormatLua(@"Iterating Main.Path[{0}] is {1}yds from your position.", i, distance));
            if (distance < closest)
            {
                closeidx = i;
                closest = distance;
            }
            i += 1;
        }

        return closeidx;
    }
    public static int GetNextClosestNode()
    {
        float closest = 9000;
        int closeidx = 0;
        float distance;
        int i = 0;

        //for (int i = 0; i <= Main.Path.Count; i++)
        //{
        //    distance = Main.Path[i].DistanceTo(Main.Me.Position);
        //    Methods.LuaPrint(Methods.FormatLua(@"Iterating Main.Path[{0}] is {1}yds from your position.", i, distance));
        //    if (distance < closest)
        //    {
        //        closeidx = i;
        //        closest = distance;
        //    }
        //}

        foreach (Vector3 v in Main.Path)
        {
            distance = Main.Path[i].DistanceTo(Main.Me.Position);
            //Methods.LuaPrint(Methods.FormatLua(@"Iterating Main.Path[{0}] is {1}yds from your position.", i, distance));
            if (distance < closest)
            {
                closeidx = i;
                closest = distance;
            }
            i += 1;
        }

        return closeidx;
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

    private void DrawClosest()
    {
        // exit if Main.Path list empty
        if (Main.Path.Count == 0)
        {
            return;
        }

        // draw one green line to closest node and exit if there is only one node in Main.Path list
        if (Main.Path.Count == 1)
        {
            Radar3D.DrawLine(Main.Path[Main.ClosestNode], Main.Me.Position, Color.Green, (int)byte.MaxValue);

            // draw one white line to first node of path
            Radar3D.DrawLine(Main.Path[0], Main.Me.Position, Color.White, (int)byte.MaxValue);
            return;
        }

        // draw one green line to closest node and one purple line to 2nd closest node
        if (Main.Path.Count >= 2)
        {
            Radar3D.DrawLine(Main.Path[Main.ClosestNode], Main.Me.Position, Color.Green, (int)byte.MaxValue);
            Radar3D.DrawLine(Main.Path[Main.NextClosestNode], Main.Me.Position, Color.Purple, (int)byte.MaxValue);

            // draw one white line to first node of path
            Radar3D.DrawLine(Main.Path[0], Main.Me.Position, Color.White, (int)byte.MaxValue);
            return;
        }

        
    }

    private void DrawPath()
    {
        if (!this._isLaunched || !Conditions.InGameAndConnectedAndAliveAndProductStartedNotInPause)
            return;
        try
        {
            for (int i=0; i <= Main.Path.Count; i++)
            {
                //this.DrawNode(Main.Path[i]);
                //if (Main.MiniMapRefresh)
                //{
                //    //UserControlMiniMap.LandmarksMiniMap.Remove(miniMapLandmark);
                //    UserControlMiniMap.LandmarksMiniMap.Add(Main.Path[i], miniMapLandmark, Color.Orange, 10, "", true);
                //    Main.MiniMapRefresh = false;
                //}

                //UserControlMiniMap.LandmarksMiniMap.Add(Main.Path[i], miniMapLandmark, Color.Orange, 10, "", true);

                if (i != Main.Path.Count && i != 0)
                {
                    Radar3D.DrawLine(Main.Path[i - 1], Main.Path[i], Color.Orange, (int)byte.MaxValue);
                }
            }

            //for (int i = 0; i <= Main.Path.Count; i++)
            //{
            //    UserControlMiniMap.LandmarksMiniMap.Add(Main.Path[i], miniMapLandmark, Color.Orange, 10, "", true);
            //}

            


            
        }
        catch (Exception ex)
        {
            Logging.WriteError("DrawPath() Error: " + Environment.NewLine + (object)ex, true);
        }
    }

    public static void RefreshMiniMap()
    {
        //if (!Main._isLaunched || !Conditions.InGameAndConnectedAndAliveAndProductStartedNotInPause)
        //    return;
        try
        {
            UserControlMiniMap.LandmarksMiniMap.Remove(miniMapLandmark);
            for (int i = 0; i <= Main.Path.Count; i++)
            {
                UserControlMiniMap.LandmarksMiniMap.Add(Main.Path[i], miniMapLandmark, Color.Orange, 10, "", true);
                if (Main.MiniMapRefresh)
                {
                    //UserControlMiniMap.LandmarksMiniMap.Remove(miniMapLandmark);
                    
                    
                }
                //Main.MiniMapRefresh = false;
            }

            //foreach (Vector3 node in Main.Path)
            //{
            //    this.DrawNode(node);
            //}
            //if (Main.TriggerMiniMapShow)
            //{
            //    for (int i = 0; i <= Main.Path.Count; i++)
            //    {
            //        UserControlMiniMap.LandmarksMiniMap.Add(Main.Path[i], miniMapLandmark, Color.Orange, 10, "", true);
            //    }
            //    Main.TriggerMiniMapShow = false;
            //}
            //if (Main.TriggerMiniMapHide)
            //{
            //    UserControlMiniMap.LandmarksMiniMap.Remove(miniMapLandmarkName);
            //    Main.TriggerMiniMapHide = false;
            //}


        }
        catch (Exception ex)
        {
            Logging.WriteError("DrawMiniMap() Error: " + Environment.NewLine + (object)ex, true);
        }
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


