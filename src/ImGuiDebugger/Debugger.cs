namespace ImGuiDebugger;

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

using Divine.Service;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.Renderer;
using Divine.SourceGenerator;
using Divine.Update;

using ImGuiDebugger.Enum;
using ImGuiDebugger.JsonTypeInfoResolvers;

using ImGuiNET;

[Export]
internal sealed partial class Debugger : IService
{
    private bool DebuggerEnabled;
    private bool initDone;
    private MenuSettings Menu;
    private JsonSerializerOptions op;
    private JsonSerializerOptions op1;

    private Dictionary<string, bool> WidgetData;
    private Dictionary<string, (TabFlag, Action)> DebuggerTabs = new();

    public Debugger(MenuSettings Menu)
    {
        LoadCFG();

        DebuggerTabs.Add("Callback Inspector 2", (TabFlag.CallbackInspector2, CallbackInspector2));
        DebuggerTabs.Add("On Add/Remove", (TabFlag.OnAddRemove, OnAddRemove));
        DebuggerTabs.Add("Information", (TabFlag.Information, Information));
        DebuggerTabs.Add("GC Message Sender", (TabFlag.GCMessageSender, GCMessageSender));

        op = new JsonSerializerOptions()
        {
            TypeInfoResolver = new PropertyContractResolver(),
            DefaultIgnoreCondition = JsonIgnoreCondition.Never,
            NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
            ReferenceHandler = ReferenceHandler.Preserve,
            MaxDepth = 64,
            Converters = { new JsonStringEnumConverter() }
        };

        op1 = new JsonSerializerOptions()
        {
            TypeInfoResolver = new InformationTypeContractResolver(),
            DefaultIgnoreCondition = JsonIgnoreCondition.Never,
            NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
            ReferenceHandler = ReferenceHandler.Preserve,
            MaxDepth = 64,
            Converters = { new JsonStringEnumConverter() }
        };

        //var fontsFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);
        //var fontConfig = new ImFontConfig { MergeMode = 1 };
        //Console.WriteLine(Path.Combine(fontsFolderPath, "calibri.ttf"));
        //defaultFontPtr = ImGui.GetIO().Fonts.AddFontDefault();
        //fontPtr = ImGui.GetIO().Fonts.AddFontFromFileTTF(Path.Combine(fontsFolderPath, "verdana.ttf"), 30f);
        //ImGui.GetIO().Fonts.Build();

        //var allCallbacksEnabledFlag = 0;
        //var num = 0;
        //CallbackData2.ForEach((data) =>
        //{
        //    var flag = 1 << num;
        //    //CallbackFlags[data.Key] = flag;
        //    allCallbacksEnabledFlag += flag;
        //    num++;
        //});
        //Console.WriteLine(allCallbacksEnabledFlag);
        ////CallbackFlags["AllCallbacksEnabled"] = allCallbacksEnabledFlag;

        CallbackData2[CallbackFlag.GameEvent].callbacks_funcs.New(game_event, true);
        CallbackData2[CallbackFlag.NetMessageReceived].callbacks_funcs.New(callback_example, true);

        //GetMessageDesciptors();

        this.Menu = Menu;
        Menu.ImGuiDebuggerSwitcher.ValueChanged += ImGuiDebuggerSwitcher_ValueChanged;
        RendererManager.Draw += RendererManager_Draw;
    }

    private void ImGuiDebuggerSwitcher_ValueChanged(MenuToggleKey toggleKey, ToggleKeyChangedEventArgs e)
    {
        if (e.Value)
        {
            Activate();

            UpdateManager.BeginInvoke(1000, () =>
            {
                if (!toggleKey)
                {
                    return;
                }

                Deactivate();
                Activate();
            });
        }
        else
        {
            Deactivate();
        }
        DebuggerEnabled = e.Value;
    }

    private void Activate()
    {
        CallbackInspector2Activate();
        OnAddRemoveActivate();
    }

    private void Deactivate()
    {
        CallbackInspector2Deactivate();
        OnAddRemoveDeactivate();
    }

    private void RendererManager_Draw()
    {
        if (!DebuggerEnabled)
            return;


        //ImGui.PushFont(defaultFontPtr);
        //ImGui.ShowDemoWindow();
        //ImGui.PopFont();

        //ImGui.PushFont(fontPtr);
        //ImGui.PopFont()
        //DivineMenu();


        if (ImGui.Begin("Debugger", ref DebuggerEnabled, ImGuiWindowFlags.None))
        {
            if (ImGui.BeginTabBar("Tabs"))
            {
                foreach (var (Name, (Flag, Tab)) in DebuggerTabs)
                {
                    if (ImGui.BeginTabItem(Name))
                    {
                        Tab();

                        ImGui.EndTabItem();
                    }
                }
                ImGui.EndTabBar();
            }
            ImGui.End();
        }

        if (Menu.ImGuiDebuggerSwitcher.Value != DebuggerEnabled)
        {
            Menu.ImGuiDebuggerSwitcher.Value = DebuggerEnabled;
            SaveCFG();
        }
    }
}
