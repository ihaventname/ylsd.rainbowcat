using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using static On.PlayerGraphics;

namespace RainbowCat
{
    [BepInPlugin(MOD_ID, "Rainbow Cat", "1.0.0")]
    class RainbowCat : BaseUnityPlugin
    {
        private const string MOD_ID = "rainbowcat";

        private float hue = 0f;
        public float changeSpeed = 0.6f;
        private RoomCamera.SpriteLeaser cachedSLeaser;

        private const float _G = 0.85f;
        private const float _B = 1f;

        private ConfigEntry<int> c_colorState;
        private ConfigEntry<float> c_changeSpeed;
        private ConfigEntry<float> c_startColor;


        // Add hooks
        private void Awake()
        {
            c_colorState = Config.Bind(
                "General",
                "State",
                1,
                "色彩状态\n数值为1: 蛞蝓猫每游戏时刻身体呈现单一色彩\n数值为2: 蛞蝓猫每游戏时刻身体不同部位呈现不同色彩\n其他数值: 关闭色彩变化，蛞蝓猫呈现原本的颜色"
            );
            c_changeSpeed = Config.Bind(
                "General",
                "Speed",
                0.6f,
                "色彩改变速度，数值越大改变越快，为0时颜色不变");
            c_startColor = Config.Bind(
                "General",
                "Start",
                0f,
                "初始颜色，0~1之间的小数");
            hue = c_startColor.Value;
            changeSpeed = c_changeSpeed.Value;
            On.PlayerGraphics.InitiateSprites += OnInitiateSprites;
            On.Player.Update += SingleColor;
            On.Player.Update += RainbowColor;

        }
        private void OnDestroy()
        {
            On.Player.Update -= SingleColor;
            On.Player.Update -= RainbowColor;
            On.PlayerGraphics.InitiateSprites -= OnInitiateSprites;


        }
        private void OnInitiateSprites(orig_InitiateSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            orig(self, sLeaser, rCam);
            if (!self.player.isNPC) cachedSLeaser = sLeaser;
        }
        private void SingleColor(On.Player.orig_Update orig, Player self, bool eu)
        {

            orig(self, eu);

            if (c_colorState.Value!=1) return;
            if (self.isNPC || self.graphicsModule == null || cachedSLeaser == null) return;
            if (cachedSLeaser.sprites == null || cachedSLeaser.sprites.Length == 0) return; 
            hue += changeSpeed * Time.deltaTime;
            if (hue >= 1f) hue -= 1f;
            Color catColor = Color.HSVToRGB(hue, _G, _B);

            for (int i = 0; i < cachedSLeaser.sprites.Length; i++)
            {
                FSprite sprite = cachedSLeaser.sprites[i];
                if (sprite != null)
                    sprite.color = catColor;
            }
        }

        private void RainbowColor(On.Player.orig_Update orig, Player self, bool eu)
        {
            orig(self, eu);

            if (c_colorState.Value != 2) return;
            if (self.isNPC || self.graphicsModule == null || cachedSLeaser == null) return;
            if (cachedSLeaser.sprites == null || cachedSLeaser.sprites.Length == 0) return;

            hue += changeSpeed * Time.deltaTime;  

            for(int i = 0;i < cachedSLeaser.sprites.Length;i++)
            {
                if (hue >= 1f) hue -= 1f;
                Color catColor = Color.HSVToRGB(hue, _G, _B);
                FSprite sprite = cachedSLeaser.sprites[i];
                if (sprite != null) sprite.color = catColor;
                hue += changeSpeed;
            }
        }



    }
}