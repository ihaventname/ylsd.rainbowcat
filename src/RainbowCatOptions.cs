using BepInEx;
using Menu.Remix.MixedUI;
using UnityEngine;

namespace RainbowCat
{
    enum ColorMode
    {
        关闭,
        单一颜色变化,
        彩虹身体变化
    }
    //设计配置面板
    internal class RainbowCatOptions : OptionInterface
    {

        public static RainbowCatOptions Instance;

        public readonly Configurable<ColorMode> colorMode;
        public readonly Configurable<float> changeSpeed;
        public readonly Configurable<float> startHue;
        public readonly Configurable<float> saturation;
        public readonly Configurable<float> brightness;
        public readonly Configurable<bool> tintFaceFeatures;

        public RainbowCatOptions(BaseUnityPlugin plugin)
        {
            Instance = this;

            colorMode = config.Bind("ColorMode", defaultValue: ColorMode.单一颜色变化);
            changeSpeed = config.Bind("ChangeSpeed", defaultValue: 0.6f);
            startHue = config.Bind("StartHue", defaultValue: 0f);
            saturation = config.Bind("Saturation", defaultValue: 0.85f);
            brightness = config.Bind("Brightness", defaultValue: 1f);
            tintFaceFeatures = config.Bind("tintFaceFeatures", defaultValue: false);
        }

        public override void Initialize()
        {
            base.Initialize();

            // 创建标签页
            OpTab mainTab = new OpTab(this, "Rainbow Cat");

            Tabs = new OpTab[] { mainTab };

            // 控件数组，一次性添加所有UI元素
            UIelement[] uiElements = new UIelement[]
            {
                new OpLabel(new Vector2(10f, 570f), new Vector2(400f,30f), "设置", bigText:true),

                // 枚举下拉框：色彩模式
                new OpResourceSelector(colorMode, new Vector2(10f, 520f), 220),
                new OpLabel(240f, 521f, "色彩模式"),

                // 滑块：色彩变化速度
                new OpFloatSlider(changeSpeed, new Vector2(10f, 410f), 220, 3),
                new OpLabel(240f,411f,"色彩变化速度"),

                // 滑块：初始色相
                new OpFloatSlider(startHue, new Vector2(10f, 360f), 220, 1),
                new OpLabel(240f,361f,"初始色调"),

                // 滑块：饱和度
                new OpFloatSlider(saturation, new Vector2(10f, 310f), 220, 1),
                new OpLabel(240f,311f,"饱和度"),

                // 滑块：亮度
                new OpFloatSlider(brightness, new Vector2(10f, 260f), 220, 1),
                new OpLabel(240f,261f,"亮度"),

                //开关：眼睛跟随身体颜色
                new OpCheckBox(tintFaceFeatures, 10f, 210f),
                new OpLabel(45f, 211f, "眼睛跟随变化(单色模式下眼睛不可见)"),
            };

            mainTab.AddItems(uiElements);
        }
    }
}

