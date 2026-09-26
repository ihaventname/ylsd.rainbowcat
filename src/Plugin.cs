#define V110
#define Menu
#define Orig
#define Main

//-----------------ATTENTION------------------
//本模组在开发过程中借助了国产高级AI模型"豆包"的帮助
//因为过于"高级" 所以可能会有一些意义不明的代码
//如果你发现了构式代码 请不要惊讶 这是正常的
//如果你不是编程大佬 最好不要改动现在的代码结构
//我再也不会让豆包给我写项目了(悲)
//--------------------------------------------


using BepInEx;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using static MachineConnector;
using static On.RainWorld;

namespace RainbowCat
{
    [BepInPlugin("rainbowcat", "Rainbow Cat", "1.1.0")]
    class RainbowCat : BaseUnityPlugin
    {
        //必要组件
        private bool _initialized;
        private RainbowCatOptions rainbowCatOptions;
        private readonly ConditionalWeakTable<Player, HueWrapper> _playerHueTable = new ConditionalWeakTable<Player, HueWrapper>();

#if V100
        private ConfigEntry<int> c_colorState;
        private ConfigEntry<float> c_changeSpeed;
        private ConfigEntry<float> c_startHue;
        private ConfigEntry<float> c_stauration;
        private ConfigEntry<float> c_brightness;
        
        private Configurable<float> mc_startHue;
        private Configurable<float> mc_stauration;
        private Configurable<float> mc_brightness;
        private Configurable<float> mc_changeSpeed;
        private Configurable<RainbowCat.State> c_state;
        private bool _isInit = false;
#endif


        //注册钩子
        private void OnEnable()
        {
            On.RainWorld.OnModsInit += RainWorld_OnModsInit;
            //On.Player.Update += MainColor;
        }
        //注销钩子
        private void OnDestroy()
        {
            if (_initialized)
            {
                On.Player.Update -= ChangeColor;
                On.PlayerGraphics.DrawSprites -= HookPlayerGraphicsDrawSprites;
            }
        }


#if V100
        private float hue = 0f;
        private RoomCamera.SpriteLeaser cachedSLeaser;
        private void OnInitiateSprites(orig_InitiateSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            orig(self, sLeaser, rCam);
            if (!self.player.isNPC) cachedSLeaser = sLeaser;
        }
#endif

#if V100
        [Obsolete()]
        private void SingleColor(On.Player.orig_Update orig, Player self, bool eu)
        {

            orig(self, eu);

            if (c_colorState.Value!=1) return;
            if (self.isNPC || self.graphicsModule == null || cachedSLeaser == null) return;
            if (cachedSLeaser.sprites == null || cachedSLeaser.sprites.Length == 0) return; 
            hue += changeSpeed * Time.deltaTime;
            if (hue >= 1f) hue -= 1f;
            Color catColor = Color.HSVToRGB(hue, saturation, brightness);

            for (int i = 0; i < cachedSLeaser.sprites.Length; i++)
            {
                FSprite sprite = cachedSLeaser.sprites[i];
                if (sprite != null)
                    sprite.color = catColor;
            }
        }

        [Obsolete()]
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
                Color catColor = Color.HSVToRGB(hue, saturation, brightness);
                FSprite sprite = cachedSLeaser.sprites[i];
                if (sprite != null) sprite.color = catColor;
                hue += changeSpeed;
            }
        }
#endif


        //废弃代码
#if OUT
        private void MainColor(On.Player.orig_Update orig, Player self, bool eu)
        {
            orig(self,eu);

            var mode = RainbowCatOptions.Instance.colorMode.Value;

            PlayerGraphics gfx = self.graphicsModule as PlayerGraphics;
            if (gfx == null || gfx.spriteLeasers == null)
                return;


            if (self.isNPC || self.graphicsModule == null || cachedSLeaser == null) return;
            if (cachedSLeaser.sprites == null || cachedSLeaser.sprites.Length == 0) return;
            if (mode == ColorMode.Off) return;

            float speed = RainbowCatOptions.Instance.changeSpeed.Value;
            float sat = RainbowCatOptions.Instance.saturation.Value;
            float bri = RainbowCatOptions.Instance.brightness.Value;

#if Beta

#endif

#if Orig
            if (mode == ColorMode.SingleColor)
            {
                hue += speed * Time.deltaTime;
                if (hue >= 1f) hue -= 1f;

                Color catColor = Color.HSVToRGB(hue, sat, bri);

                for (int i = 0; i < cachedSLeaser.sprites.Length; i++)
                {
                    FSprite sprite = cachedSLeaser.sprites[i];
                    if (sprite != null) sprite.color = catColor;
                }
            }

            else if(mode == ColorMode.RainbowPerSprite)
            {
                hue += speed * Time.deltaTime;
                if (hue >= 1f) hue -= 1f;

                for (int i = 0; i < cachedSLeaser.sprites.Length; i++)
                {
                    Color catColor = Color.HSVToRGB(hue, sat, bri);
                    FSprite sprite = cachedSLeaser.sprites[i];
                    if (sprite != null) sprite.color = catColor;
                    hue += speed;
                }
            }

            else return;
        }
#endif
#endif
        //加载模组时干什么
        private void RainWorld_OnModsInit(orig_OnModsInit orig, RainWorld self)
        {
            orig(self);
            if (_initialized) return;
            _initialized = true;
            rainbowCatOptions = new RainbowCatOptions(this);

            //加载配置界面
            SetRegisteredOI("rainbowcat", rainbowCatOptions);
            
            //必须在此方法内注册这两个钩子
            On.Player.Update += ChangeColor;
            On.PlayerGraphics.DrawSprites += HookPlayerGraphicsDrawSprites;
        }
        

#if Main
        //改变颜色
        private void ChangeColor(On.Player.orig_Update orig, Player self, bool eu)
        {
            orig(self, eu);

            var mode = RainbowCatOptions.Instance.colorMode.Value;
            if (mode == ColorMode.关闭) return;

            HueWrapper wrapper;
            if (!_playerHueTable.TryGetValue(self, out wrapper))
            {
                float initHue = RainbowCatOptions.Instance.startHue.Value;
                wrapper = new HueWrapper(initHue);
                _playerHueTable.Add(self, wrapper);
            }

            float speed = RainbowCatOptions.Instance.changeSpeed.Value;
            wrapper.Value += speed * Time.deltaTime;
            if (wrapper.Value > 1f) wrapper.Value -= 1f;

        }

        //主逻辑
        private void HookPlayerGraphicsDrawSprites(On.PlayerGraphics.orig_DrawSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            orig(self, sLeaser, rCam, timeStacker, camPos);
            Player player = self.player;

            //自检环节
            if (player == null || player.isNPC) return;
            var mode = RainbowCatOptions.Instance.colorMode.Value;
            if (mode == ColorMode.关闭) return;
            if (!_playerHueTable.TryGetValue(player, out HueWrapper wrapper)) return;

            //获取配置
            float currentHue = wrapper.Value;
            float sat = RainbowCatOptions.Instance.saturation.Value;
            float bri = RainbowCatOptions.Instance.brightness.Value;
            bool tintFace = RainbowCatOptions.Instance.tintFaceFeatures.Value;

            //我不知道为什么AI把这个放到这里 保险起见还是不要动了
            if (sLeaser.sprites == null) return;

            //获取初始眼睛颜色
            Dictionary<int, Color> faceSpriteOriginalColor = null;
            if (!tintFace)
            {
                faceSpriteOriginalColor = new Dictionary<int, Color>();
                for (int i = 0; i < sLeaser.sprites.Length; i++)
                {
                    FSprite spr = sLeaser.sprites[i];
                    if (spr == null || spr.element == null || string.IsNullOrEmpty(spr.element.name))
                        continue;
                    if (spr.element.name.StartsWith("Face"))
                    {
                        faceSpriteOriginalColor[i] = spr.color;
                    }
                }
            }



            //单一颜色变化处理
            if (mode == ColorMode.单一颜色变化)
            {
                Color col = Color.HSVToRGB(currentHue, sat, bri);
                for (int i = 0; i < sLeaser.sprites.Length; i++)
                {
                    FSprite spr = sLeaser.sprites[i];
                    if (spr == null) continue;
                    spr.color = col;
                }
            }
            //多颜色变化处理
            else if (mode == ColorMode.彩虹身体变化)
            {
                float localHue = currentHue;
                float speed = RainbowCatOptions.Instance.changeSpeed.Value;
                for (int i = 0; i < sLeaser.sprites.Length; i++)
                {
                    FSprite spr = sLeaser.sprites[i];
                    if (spr == null) continue;
                    Color col = Color.HSVToRGB(localHue, sat, bri);
                    spr.color = col;
                    localHue += speed * 0.25f;
                    if (localHue > 1f) localHue -= 1f;
                }
            }


            //渲染眼部颜色
            if (!tintFace && faceSpriteOriginalColor != null)
            {
                foreach (var kvp in faceSpriteOriginalColor)
                {
                    int idx = kvp.Key;
                    if (idx >= sLeaser.sprites.Length) continue;
                    FSprite spr = sLeaser.sprites[idx];
                    if (spr != null)
                    {
                        spr.color = kvp.Value;
                    }
                }
            }
        }
#endif

    }

}

