using System;
using UnityEngine;
using Verse;
using HarmonyLib;

namespace VanillaRacesExpandedSanguophage
{
    [StaticConstructorOnStartup]
    public static class GraphicsCache
    {
        public static readonly Texture2D BGTex_Red = ContentFinder<Texture2D>.Get("UI/Widgets/AbilityButBG_Red");
        public static readonly Texture2D BGTexShrunk_Red = ContentFinder<Texture2D>.Get("UI/Widgets/AbilityButBGShrunk_Red");
    }
}
