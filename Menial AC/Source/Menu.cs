using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using ClickableTransparentOverlay;
using ImGuiNET;

namespace Menial_AC.Source
{
    public class Menu
    {

        public Vector2 windowLocation = new Vector2(0, 0);
        public Vector2 windowSize = new Vector2(1920, 1080);
        public Vector2 lineOrigin = new Vector2(1920 / 2, 1080);
        public Vector2 windowCenter = new Vector2(1920 / 2, 1080 / 2);

        public void LoadMenuStyle()
        {
            ImGuiIOPtr io = ImGui.GetIO();
            ImGuiStylePtr style = ImGui.GetStyle();
            ImGui.GetStyle().WindowPadding = new Vector2(15, 15);
            style.WindowRounding = 10f;
            style.FrameBorderSize = 2f;
            style.WindowTitleAlign = new Vector2(0.5f, 0.1f);
            style.ScrollbarSize = 20f;
            style.TabBarBorderSize = 2f;
            style.ItemSpacing = new Vector2(7,7);
            style.Colors[(int)ImGuiCol.Separator] = new Vector4(0.00f, 0.00f, 0.00f, 1.00f);
            style.Colors[(int)ImGuiCol.Button] = new Vector4(0.00f, 0.98f, 1.00f, 1.00f);
            style.Colors[(int)ImGuiCol.ButtonActive] = new Vector4(0.00f, 0.98f, 1.00f, 1.00f);
            style.Colors[(int)ImGuiCol.ButtonHovered] = new Vector4(0.00f, 0.74f, 0.76f, 1.00f);
            style.Colors[(int)ImGuiCol.Header] = new Vector4(0.00f, 0.98f, 1.00f, 1.00f);
            style.Colors[(int)ImGuiCol.HeaderActive] = new Vector4(0.00f, 0.98f, 1.00f, 1.00f);
            style.Colors[(int)ImGuiCol.HeaderHovered] = new Vector4(0.00f, 0.74f, 0.76f, 1.00f);
            style.Colors[(int)ImGuiCol.ScrollbarGrabHovered] = new Vector4(0.00f, 0.74f, 0.76f, 1.00f);
            style.Colors[(int)ImGuiCol.ScrollbarGrabActive] = new Vector4(0.00f, 0.98f, 1.00f, 1.00f);
            style.Colors[(int)ImGuiCol.ScrollbarGrab] = new Vector4(0.00f, 0.98f, 1.00f, 1.00f);
            style.Colors[(int)ImGuiCol.ScrollbarBg] = new Vector4(0.5412f, 0.5412f, 0.5412f, 1.0000f);
            style.Colors[(int)ImGuiCol.TitleBgActive] = new Vector4(0.5412f, 0.5412f, 0.5412f, 1.0000f);
            style.Colors[(int)ImGuiCol.TitleBg] = new Vector4(0.5412f, 0.5412f, 0.5412f, 1.0000f);
            style.Colors[(int)ImGuiCol.WindowBg] = new Vector4(0.5412f, 0.5412f, 0.5412f, 1.0000f);
            style.Colors[(int)ImGuiCol.Border] = new Vector4(0.00f, 0.98f, 1.00f, 1.00f);
            style.Colors[(int)ImGuiCol.BorderShadow] = new Vector4(0.00f, 0.74f, 0.76f, 1.00f);
            style.Colors[(int)ImGuiCol.Text] = new Vector4(0.00f, 0.00f, 0.00f, 1.00f);
            style.Colors[(int)ImGuiCol.TextDisabled] = new Vector4(0.24f, 0.23f, 0.29f, 0.50f);
            style.Colors[(int)ImGuiCol.PopupBg] = new Vector4(0.24f, 0.23f, 0.29f, 0.50f); // affects tooltips
            style.Colors[(int)ImGuiCol.FrameBg] = new Vector4(0.00f, 0.98f, 1.00f, 1.00f);
            style.Colors[(int)ImGuiCol.FrameBgHovered] = new Vector4(0.00f, 0.74f, 0.76f, 1.00f);
            style.Colors[(int)ImGuiCol.CheckMark] = new Vector4(0.00f, 0.00f, 0.00f, 1.00f);
            style.Colors[(int)ImGuiCol.TabActive] = new Vector4(0.00f, 0.98f, 1.00f, 1.00f);
            style.Colors[(int)ImGuiCol.Tab] = new Vector4(0.00f, 0.98f, 1.00f, 1.00f);
            style.Colors[(int)ImGuiCol.TabHovered] = new Vector4(0.00f, 0.74f, 0.76f, 1.00f);
            style.Colors[(int)ImGuiCol.MenuBarBg] = new Vector4(1.00f, 1.00f, 1.00f, 1.00f);
        }


    }
}
