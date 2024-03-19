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


        public static readonly Texture2D AnimalHemogenBarTex = SolidColorMaterials.NewSolidColorTexture(new ColorInt(166, 90, 82).ToColor);
        public static readonly Texture2D AnimalHemogenBarTexHighlight = SolidColorMaterials.NewSolidColorTexture(new ColorInt(186, 117, 109).ToColor);

        public static readonly Texture2D CorpseHemogenBarTex = SolidColorMaterials.NewSolidColorTexture(new ColorInt(51, 47, 47).ToColor);
        public static readonly Texture2D CorpseHemogenBarTexHighlight = SolidColorMaterials.NewSolidColorTexture(new ColorInt(81, 75, 172).ToColor);

        public static readonly Texture2D SanguophageHemogenBarTex = SolidColorMaterials.NewSolidColorTexture(new ColorInt(124, 61, 91).ToColor);
        public static readonly Texture2D SanguophageHemogenBarTexHighlight = SolidColorMaterials.NewSolidColorTexture(new ColorInt(218, 151, 211).ToColor);

        public static readonly Texture2D DefaultHemogenBarTex = SolidColorMaterials.NewSolidColorTexture(new ColorInt(138, 3, 3).ToColor);
        public static readonly Texture2D DefaultHemogenBarTexHighlight = SolidColorMaterials.NewSolidColorTexture(new ColorInt(145, 42, 42).ToColor);

    }
}
